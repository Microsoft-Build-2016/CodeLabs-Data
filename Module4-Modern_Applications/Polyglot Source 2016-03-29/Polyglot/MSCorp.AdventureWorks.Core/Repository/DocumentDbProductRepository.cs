using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Import;
using MSCorp.AdventureWorks.Core.Initialiser;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using AccessCondition = Microsoft.Azure.Documents.Client.AccessCondition;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Repository to save and load products from the Document Database
    /// </summary>
    public class DocumentDbProductRepository : DocumentDbRepository, IProductRepository
    {
        private readonly CloudCredentials _blobCredentials;
        private readonly CloudContainerName _containerName;
        private readonly Dictionary<string, string> _storedProcLinks;

        private const string LoadProductDetailsProc = "LoadProductDetails";
        private const string SearchForProductByAttributeProc = "SearchForProductByAttribute";
        private const string ApplyDiscountToProductsProc = "ApplyDiscountToProducts";
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDbProductRepository"/> class.
        /// </summary>
        public DocumentDbProductRepository(DocumentDbCredentials credentials, string database, string collection, 
            CloudCredentials blobCredentials, CloudContainerName containerName) :base(credentials, database, collection) 
        {
            _blobCredentials = blobCredentials;
            _containerName = containerName;
            _storedProcLinks = new Dictionary<string, string>();
        }

        public override async Task Initialise(DocumentDbInitialiser initialiser)
        {
            await base.Initialise(initialiser);

            List<StoredProcedure> storedProcs = Client.CreateStoredProcedureQuery(CollectionUri).ToList();

            List<UserDefinedFunction> functions = Client.CreateUserDefinedFunctionQuery(CollectionUri).ToList();
            await CreateRelatedProductsProc(storedProcs);
            await CreateLoadProductDetailsProc(storedProcs);
            await CreateApplyDiscountToProductsProc(storedProcs);
            await CreateForeignCurrencyCalcFunction(functions);
            await CreateFormatProductFunction(functions);
        }

        private async Task CreatStoredProc(IEnumerable<StoredProcedure> storedProcedures, string procName, string storedProcBody)
        {
            StoredProcedure existingProc = storedProcedures.SingleOrDefault(proc => proc.Id == procName);
            if (existingProc != null)
            {
                _storedProcLinks.Add(procName, existingProc.SelfLink);
            }
            else
            {
                var newProcedure = new StoredProcedure
                {
                    Id = procName,
                    Body = storedProcBody
                };

                ResourceResponse<StoredProcedure> createResponse = await Client.CreateStoredProcedureAsync(CollectionUri, newProcedure);
                _storedProcLinks.Add(procName, createResponse.Resource.SelfLink);
            }
        }

        private async Task CreateRelatedProductsProc(IEnumerable<StoredProcedure> storedProcs)
        {
            const string storedProcBody = @"
function loadRelatedProducts(searchParams) {

    var collection = getContext().getCollection();
    var products = [];  
    
    function productCallback(err, doc, options) {
        products = doc;
        for (i = 0; i < doc.length; i++) {        
            collection.readAttachments(doc[i]._self + doc[i].attachments, {}, attachmentCallback);
        }
        getContext().getResponse().setBody(products);
    }

    function attachmentCallback(err, doc, options) {
        if (doc.length > 0) {
            for (i = 0; i < products.length; i++) {
                if (products[i].ProductCode === doc[0].productCode) {                  
                    products[i].attachmentCollection = doc;
                    return;
                }
            }
        }
    }

    var query = 'SELECT * FROM ProductCollection p WHERE p.ProductName';
    if (searchParams && searchParams != null) {
        query = 'SELECT * FROM ProductCollection p WHERE p.ProductAttributes[""' + searchParams.AttributeName + '""]=""' + searchParams.Value + '""'
    }

    // Begin query
    collection.queryDocuments(collection.getSelfLink(), 
        query, 
        {}, 
        productCallback);
}";
            await CreatStoredProc(storedProcs, SearchForProductByAttributeProc, storedProcBody);
        }

        private async Task CreateLoadProductDetailsProc(IEnumerable<StoredProcedure> storedProcs)
        {
            const string storedProcBody = @"
function loadProductDetails(productCode) {

    var context = getContext();
    var collection = context.getCollection();
    var products = null;
    function rateCallback(err, doc, options) {
        var rates = doc;
        if(err) throw 'Error while querying documents for exchange rates: ' + err;
        generateResponse(rates);
    }

    function attachmentCallback(err, doc, options) {
        if(err) throw 'Error while querying documents for attachments: ' + err;
        if (products && doc) {
            products[0].attachmentCollection = doc;
        }
    }

    function productCallback(err, doc, options) {
        products = doc;
        if(err) throw 'Error while querying documents for exchange rates: ' + err;
        
        collection.readAttachments(products[0]._self + products[0].attachments, {}, attachmentCallback);        
        collection.queryDocuments(collection.getSelfLink(), 'SELECT r.Rates from Rates r WHERE r.Currency.Code=""' + products[0].Price.Currency.Code + '""', {}, rateCallback);
   }

    function generateResponse(rates){
        if (products != null) {
            var response = products[0];
            response.Prices = [response.Price];
            response.DiscountedPrices = [response.DiscountedPrice];
            
            // Add foreign currencies into the price collection
            if (rates != null && rates.length > 0) {
                for (i = 0; i < rates[0].Rates.length; i++) {
                    var foreignPrice = calculateForeignCurrencyPrices(rates[0].Rates[i], response.Price.Amount);
                    var foreignDiscountedPrice = calculateForeignCurrencyPrices(rates[0].Rates[i], response.Price.Amount, response.Discount.PercentageValue);
                    response.Prices.push(foreignPrice);
                    response.DiscountedPrices.push(foreignDiscountedPrice);
                };
            }
    
            context.getResponse().setBody(response); 
        }
    }

    function calculateForeignCurrencyPrices(foreignRate, price, discount) {
        var foreignPrice = price * foreignRate.Rate;
        if (discount) {
            foreignPrice = (+foreignPrice.toFixed(2) * (100-discount))/100;
        }

        return {Currency: foreignRate.Currency, Amount: +foreignPrice.toFixed(2) };
    }

    // Begin query
    collection.queryDocuments(collection.getSelfLink(), 'SELECT * FROM Products p WHERE p.ProductCode=""' + productCode + '""', {}, productCallback);
}";
            await CreatStoredProc(storedProcs, LoadProductDetailsProc, storedProcBody);
        }

        private async Task CreateApplyDiscountToProductsProc(IEnumerable<StoredProcedure> storedProcs)
        {
            const string storedProcBody = @"
function applyDiscountToProducts(discountData) {

    var context = getContext();
    var collection = context.getCollection();
    var discount = discountData.discount;    
    var products = discountData.products;
    
    for (i = 0; i < products.length; i++) {
        var query = 'SELECT * FROM Products p WHERE p.ProductCode=\'' + products[i].productCode + '\' AND p._etag=\'' + products[i].etag + '\'';
        collection.queryDocuments(collection.getSelfLink(), query, {}, saveCallback);
    };

    function saveCallback(err, doc, options) {
        if(err) throw 'Error while applying discounts: ' + err;
        if(doc && doc.length === 0) throw 'One or more products have been modified or cannot be found.';

        // perform save.
        doc[0].Discount.PercentageValue = discount; 
        doc[0].Discount.IsZero = discount === 0; 
        doc[0].DiscountedPrice.Amount = (+doc[0].Price.Amount.toFixed(2) * (100-discount))/100;
        collection.replaceDocument(doc[0]._self, doc[0], {}, 
            function(saveError, savedDoc, responseOptions) {
                if (saveError) throw 'Error while updating document' + saveError;
            });
   }
}";

            await CreatStoredProc(storedProcs, ApplyDiscountToProductsProc, storedProcBody);
        }

        private async Task CreateUserDefinedFunction(IEnumerable<UserDefinedFunction> functions, string name, string body)
        {
            UserDefinedFunction existingFunction = functions.SingleOrDefault(proc => proc.Id == name);
            if (existingFunction != null)
            {
            }
            else
            {
                var newFunction = new UserDefinedFunction
                {
                    Id = name,
                    Body = body
                };

                await Client.CreateUserDefinedFunctionAsync(CollectionUri, newFunction);
            }
        }
        
        private async Task CreateForeignCurrencyCalcFunction(IEnumerable<UserDefinedFunction> functions)
        {
            const string body = @"
function calculateForeignCurrencyPrices(foreignRate, price, discount) {
    var foreignPrice = price * foreignRate.Rate;
    if (discount) {
        foreignPrice = (+foreignPrice.toFixed(2) * (100-discount))/100;
    }

    return {Currency: foreignRate.Currency, Amount: +foreignPrice.toFixed(2) };
}";
            await CreateUserDefinedFunction(functions, "calculatePrice", body);
        }

        private async Task CreateFormatProductFunction(IEnumerable<UserDefinedFunction> functions)
        {
            const string body = @"
function formatProduct(product) { 
	return {
		Key : product.ProductCode,
        ProductCode : product.ProductCode,
		Name: product.ProductName,
		Description : product.ProductDescription,
		Manufacturer : product.Manufacturer,
		Color : product.Color,
		Size : product.ProductSizes.map(function(v) { return v.Name }),
		SizeFacet : product.ProductSizes.map(function(v) { return '{\""name\"":\""' + v.Name + '\"",\""sortValue\"":' + v.SortValue + '}' }),
        CurrencyCode: product.Price.Currency.Code,
		Price: parseFloat(product.Price.Amount).toFixed(2),
		Discount: parseFloat(product.Price.Amount - product.DiscountedPrice.Amount).toFixed(2),
		ProductType: product.ProductType,
		ProductCategory: product.ProductCategory,
		Priority: product.Priority,
		ThumbImageUrl: product.ThumbnailUrl,
		_ts: product._ts
    };
}";
            await CreateUserDefinedFunction(functions, "formatProduct", body);
        }

        public override async Task Uninitialise(DocumentDbInitialiser initialiser)
        {
            await base.Uninitialise(initialiser);
            _storedProcLinks.Clear();
        }

        /// <summary>
        /// Saves the specified product.
        /// </summary>
        public async Task<SaveResponse> SaveAsync(Product product)
        {
            Argument.CheckIfNull(product, "product");

            Document persistedProduct;
            if (product.IsDeleted)
            {
                ResourceResponse<Document> response = await Client.DeleteDocumentAsync(product.SelfLink).ConfigureAwait(false);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return new SaveResponse(new Identifier(), product);
                }
                throw new InvalidOperationException("Unable to delete product with code {0}".FormatWith(product.ProductCode));
            }
            
            if (product.IsNew)
            {
                ResourceResponse<Document> newCreation = await Client.CreateDocumentAsync(CollectionUri, product).ConfigureAwait(false);
                persistedProduct = newCreation.Resource;
            }
            else
            {
                try
                {
                    var accessCondition = new AccessCondition { Condition = product.ETag, Type = AccessConditionType.IfMatch };
                    var requestOptions = new RequestOptions { AccessCondition = accessCondition };
                    ResourceResponse<Document> updateResponse = await Client.ReplaceDocumentAsync(product, requestOptions).ConfigureAwait(false);
                    persistedProduct = updateResponse.Resource;
                }
                catch (Exception e)
                {
                    throw new EntityNotFoundException("Error saving the document.  It may have been out of date.", e);
                }    
            }
            var identifier = new Identifier(persistedProduct.Id, new VersionNumber(persistedProduct.ETag));
            return new SaveResponse(identifier, persistedProduct);
        }

        /// <summary>
        /// Saves the specified product.
        /// </summary>
        public SaveResponse Save(Product product)
        {
            return SaveAsync(product).Result;
        }

        /// <summary>
        /// Adds the attachment.
        /// </summary>
        public async Task<IEnumerable<Attachment>> AddAttachmentsAsync(Product product, params ProductImage[] images)
        {
            Argument.CheckIfNull(product, "product");
            Argument.CheckIfNull(images, "images");

            var attachments = new List<Attachment>();

            CloudBlobContainer blobContainer = GetBlobContainer();
            foreach (ProductImage image in images.Where(i => i.Image != null))
            {
                // Send full resolution asset
                Attachment fullResImage = await SendImageAttachmentAsync(blobContainer, product, image.Image, product.ProductCode, "full resolution image", false);
                attachments.Add(fullResImage);

                // Send thumbnail asset

                Image thumbnail = new Bitmap(250, 165);
                Graphics graphics = Graphics.FromImage(thumbnail);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image.Image, new Rectangle(0, 0, 250, 165));
                Attachment thumb = await SendImageAttachmentAsync(blobContainer, product, thumbnail, product.ProductCode + "_thumb", "thumbnail", true);
                product.ThumbnailUrl = thumb.MediaLink;
                attachments.Add(thumb);
            }

            return attachments;
        }

        /// <summary>
        /// Adds the attachment.
        /// </summary>
        public IEnumerable<Attachment> AddAttachments(Product product, params ProductImage[] images)
        {
            return AddAttachmentsAsync(product, images).Result;
        }

        private async Task<Attachment> SendImageAttachmentAsync(CloudBlobContainer container, Product product, Image image, string id, string description, bool isThumbnail)
        {
            var imageStream = new MemoryStream();
            image.Save(imageStream, ImageFormat.Jpeg);
            imageStream.Seek(0, SeekOrigin.Begin);
            id = id.TrimEnd(".jpg") + ".jpg";

            //Get reference to the picture blob or create if not exists. 
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(id);

            //Save to storage
            byte[] buffer = imageStream.ToArray();
            blockBlob.UploadFromByteArray(buffer, 0, buffer.Length);
            blockBlob.Properties.ContentType = "image/jpeg";
            blockBlob.SetProperties();

            var metadata = new { id, contentType = "image/jpeg", media = blockBlob.Uri.ToString(), productCode = product.ProductCode, description, isThumbnail };
            ResourceResponse<Attachment> response = await Client.CreateAttachmentAsync(product.AttachmentsLink, metadata).ConfigureAwait(false);
            return response.Resource;
        }

        private CloudBlobContainer GetBlobContainer()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_blobCredentials.ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_containerName.Text);
            return container;
        }

        /// <summary>
        /// Loads the product with the given product code.
        /// </summary>
        public async Task<Product> LoadProduct(string productCode)
        {
            Argument.CheckIfNullOrEmpty(productCode, "productCode");
            string value = await LoadProductByCodeViaStoredProcedureInternal(productCode);
            return JsonConvert.DeserializeObject<Product>(value);
        }

        /// <summary>
        /// Loads the product in raw JSON with the given product code.
        /// </summary>
        public Task<string> LoadProductJson(string productCode)
        {
            Argument.CheckIfNullOrEmpty(productCode, "productCode");
            return Task.FromResult(LoadProductJsonByCode(productCode));
        }

        private string LoadProductJsonByCode(string productCode)
        {
            string queryText = "SELECT * FROM {0} p WHERE p.ProductCode=\"{1}\"".FormatWith(Collection, productCode);
            IQueryable<dynamic> query = Client.CreateDocumentQuery(CollectionUri, queryText);

            try
            {
                List<dynamic> results = query.ToList();

                if (results.Any())
                {
                    dynamic result = results.First();
                    return result.ToString();
                }
            }
            catch (AggregateException aggregateException)
            {
                const string message = "Unable to find document with product code {0}.  Document either does not exist or an unexpected error occurred on the server";
                throw new EntityNotFoundException(message.FormatWith(productCode), aggregateException);
            }

            const string notExistMessage = "Unable to find document with product code {0}.";
            throw new EntityNotFoundException(notExistMessage);
        }

        /// <summary>
        /// Loads all the products.
        /// </summary>
        public async Task<IEnumerable<Product>> LoadProducts()
        {
            IQueryable<Product> query =
                from product in Client.CreateDocumentQuery<Product>(CollectionUri)
                select product;

            return await Task.Run(() => query.AsEnumerable().Where(item => item.ProductCode != null).ToList());
        }
        
        /// <summary>
        /// Loads all the products for a product category.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public async Task<IEnumerable<Product>> LoadProductsByCategory(string category)
        {
            IQueryable<Product> query =
                from product in Client.CreateDocumentQuery<Product>(CollectionUri)
                where product.ProductCategory == category
                select product;

            return await Task.Run(() => query.ToList());
        }


        /// <summary>
        /// Loads all the available Product Category Sets.
        /// </summary>
        public Task<IEnumerable<ProductCategorySet>> LoadAllCategorySets()
        {
            return Task.Run(() => LoadAllProductCategorySets());
        }

        private IEnumerable<ProductCategorySet> LoadAllProductCategorySets()
        {
            var query =
                from product in Client.CreateDocumentQuery<Product>(CollectionUri)
                select product;

            try
            {
                var results = query.ToList();
                IEnumerable<ProductCategorySet> categorySets = results
                    .Where(r => r.ProductCategory!= null)
                    .GroupBy(r => r.ProductCategory)
                    .Select(r => new ProductCategorySet(r.Key, r.Select(item => item.ProductType)));

                return categorySets.ToList();
            }
            catch (AggregateException aggregateException)
            {
                const string message = "Document either does not exist or an unexpected error occurred on the server";
                throw new EntityNotFoundException(message, aggregateException);
            }
        }

        /// <summary>
        /// Loads the product with the given Identifier.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Proc")]
        public Task<string> LoadProductByCodeViaStoredProcedure(string productCode)
        {
            Argument.CheckIfNullOrEmpty(productCode, "productCode");
            return LoadProductByCodeViaStoredProcedureInternal(productCode);
        }

        private async Task<string> LoadProductByCodeViaStoredProcedureInternal(string productCode)
        {
            try
            {
                string storedProcSelfLink = _storedProcLinks[LoadProductDetailsProc];
                StoredProcedureResponse<dynamic> storedProcedureResponse = await Client.ExecuteStoredProcedureAsync<dynamic>(storedProcSelfLink, productCode).ConfigureAwait(false);
                string result = storedProcedureResponse.Response.ToString();

                return result;
            }
            catch (AggregateException aggregateException)
            {
                const string message = "Unable to find product with product code {0}.";
                throw new EntityNotFoundException(message.FormatWith(productCode), aggregateException);
            }
        }

    /// <summary>
        /// Loads the attachments.
    /// </summary>
        public async Task<IEnumerable<Attachment>> LoadAttachments(string documentLink)
        {
            IOrderedQueryable<Attachment> query = Client.CreateAttachmentQuery(documentLink);
            try
    {
                return await Task.Run(() => query.ToList());
            }
            catch (AggregateException aggregateException)
        {
                string message = 
                    "Unable to find attachments belonging to document at {0}.  Document either does not exist or an unexpected error occurred on the server"
                    .FormatWith(documentLink);
                throw new EntityNotFoundException(message.FormatWith(documentLink), aggregateException);
            }
        }

        /// <summary>
        /// Deletes the attachment.
        /// </summary>
        public async Task DeleteAttachment(Attachment attachment)
        {
            CloudBlobContainer container = GetBlobContainer();
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(attachment.Id);
            blockBlob.DeleteIfExists();
            await Client.DeleteAttachmentAsync(attachment.SelfLink).ConfigureAwait(false);
        }

        /// <summary>
        /// Loads the related products by attribute, returning the raw JSON string result. Asynchronous.
        /// </summary>
        /// <param name="searchAttribute"></param>
        /// <param name="searchText"></param>
        public async Task<string> LoadRelatedProductsByAttributeJsonAsync(string searchAttribute, string searchText)
        {
            Argument.CheckIfNullOrEmpty(searchAttribute, "searchAttribute");
            Argument.CheckIfNullOrEmpty(searchText, "searchText");

            try
            {
                var procedureParams = new {AttributeName = searchAttribute, Value = searchText};
                string storedProcSelfLink = _storedProcLinks[SearchForProductByAttributeProc];
                StoredProcedureResponse<dynamic> storedProcedureResponse = await Client.ExecuteStoredProcedureAsync<dynamic>(storedProcSelfLink, procedureParams);
                string result = storedProcedureResponse.Response.ToString();

                return result;
            }
            catch (AggregateException aggregateException)
            {
                const string message = "Unable to find related documents with {0} matching {1}.";
                throw new EntityNotFoundException(message.FormatWith(searchAttribute, searchText), aggregateException);
            }
        }

        /// <summary>
        /// Loads a list of all product attributes as a single formatted JSON string.
        /// </summary>
        public Task<string> LoadAllProductAttributesJsonAsync()
        {
            string queryText = "SELECT p.ProductAttributes FROM {0} p WHERE p.ProductAttributes != {1}".FormatWith(Collection, "{}");
            IQueryable<dynamic> query = Client.CreateDocumentQuery(CollectionUri, queryText);

            try
            {
                List<dynamic> results = query.ToList();

                if (results.Any())
                {
                    var result = new JArray(results.Select(r => JObject.Parse(r.ToString())).ToArray());
                    return Task.FromResult(result.ToString());
                }
            }
            catch (AggregateException aggregateException)
            {
                const string message = "Unable to retrieve product attributes.";
                throw new EntityNotFoundException(message, aggregateException);
            }

            return Task.FromResult(string.Empty);
        }

        /// <summary>
        /// Applies the discount to products.
        /// </summary>
        /// <param name="requestData">The request data as a JSON formatted string.</param>
        public void ApplyDiscountToProducts(string requestData)
        {
            try
            {
                string storedProcSelfLink = _storedProcLinks[ApplyDiscountToProductsProc];
                JObject requestDataObject = JObject.Parse(requestData);
                Client.ExecuteStoredProcedureAsync<dynamic>(storedProcSelfLink, requestDataObject).Wait();
            }
            catch (Exception exception)
            {
                const string message = "Unable to apply discounts to the selected products.";
                throw new EntityNotFoundException(message, exception);
            }
        }
    }
}