using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Initialiser;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Repository to save and load product reviews from the Document Database
    /// </summary>
    public class DocumentDbProductReviewRepository : DocumentDbRepository, IProductReviewRepository
    {
        private readonly Dictionary<string, string> _storedProcLinks;
        private const string ResponseTriggerName = "CreateSnippetFromResponsev2";
        private const string ReviewTriggerName = "CreateSnippetFromReview";
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDbProductReviewRepository"/> class.
        /// </summary>
        public DocumentDbProductReviewRepository(DocumentDbCredentials credentials, string database, string collection) : base(credentials, database, collection)
        {
            _storedProcLinks = new Dictionary<string, string>();
        }

        public override async Task Initialise(DocumentDbInitialiser initialiser)
        {
            await base.Initialise(initialiser);

            IOrderedQueryable<Trigger> triggers = Client.CreateTriggerQuery(CollectionUri);
            List<StoredProcedure> storedProcs = Client.CreateStoredProcedureQuery(CollectionUri).ToList();
            List<Trigger> triggersList = triggers.ToList();

            await CreateSnippetFromReviewTrigger(triggersList);
            await CreateSnippetFromResponseTrigger(triggersList);
            await CreateLoadAllReviewsStoredProcedure(storedProcs);
        }
        
        public override async Task Uninitialise(DocumentDbInitialiser initialiser)
        {
            await base.Uninitialise(initialiser);
            _storedProcLinks.Clear();
        }

        private Task CreateLoadAllReviewsStoredProcedure(List<StoredProcedure> storedProcs)
        {
            const string storedProc = @"
function loadAllReviews(productCodeToFilterOn) {

    var context = getContext();
    var collection = context.getCollection();
    function callback(err, doc, options) {{
        if(err) throw 'Error while querying documents for exchange rates: ' + err;
        context.getResponse().setBody(doc);
    }}

    // Begin query
    collection.queryDocuments(collection.getSelfLink(), 'SELECT * FROM Reviews r WHERE r.ProductCode = ""' + productCodeToFilterOn + '"" AND NOT r.IsForIndex', {}, callback);
}";

            return CreatStoredProc(storedProcs, "loadAllReviews", storedProc);
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
                StoredProcedure newProcedure = new StoredProcedure
                {
                    Id = procName,
                    Body = storedProcBody
                };

                ResourceResponse<StoredProcedure> createResponse = await Client.CreateStoredProcedureAsync(CollectionUri, newProcedure);
                _storedProcLinks.Add(procName, createResponse.Resource.SelfLink);
            }
        }

        private async Task CreateSnippetFromReviewTrigger(IEnumerable<Trigger> triggersList)
        {
            Trigger createSnippetFromReview = triggersList.SingleOrDefault(trig => trig.Id == ReviewTriggerName);
            if (createSnippetFromReview == null)
            {
                Trigger trigger = new Trigger
                {
                    Id = ReviewTriggerName,
                    TriggerOperation = TriggerOperation.Create,
                    TriggerType = TriggerType.Post,
                    Body = @"
                    function createSnippetFromReview() {
                        function resultCallback(err, doc, responseOptions) {
                            if (err) throw 'Error while creating snippet from review: ' + err;
                        }

                        var collection = getContext().getCollection();
                        var insertedReview = getContext().getRequest().getBody();
                        var snippet = {
                            id : Math.random().toString(16).slice(2),
                            CustomerName: insertedReview.ReviewingCustomer.Name,
                            CustomerKey: insertedReview.ReviewingCustomer.CustomerKey,
                            Text: insertedReview.ReviewText,
                            Rating: insertedReview.Rating,
                            ProductCode: insertedReview.ProductCode,
                            Date: insertedReview[""Date""],
                            IsForIndex: true
                        };
                        collection.createDocument(collection.getSelfLink(), snippet, {}, resultCallback);
                    }"
                };

                ResourceResponse<Trigger> createResponse = await Client.CreateTriggerAsync(CollectionUri, trigger);
            }
        }

        private async Task CreateSnippetFromResponseTrigger(IEnumerable<Trigger> triggersList)
        {
            Trigger createSnippetFromResponse = triggersList.SingleOrDefault(trig => trig.Id == ResponseTriggerName);
            if (createSnippetFromResponse == null)
            {
                Trigger trigger = new Trigger
                {
                    Id = ResponseTriggerName,
                    TriggerOperation = TriggerOperation.Replace,
                    TriggerType = TriggerType.Post,
                    Body = @"
function createSnippetFromResponse() {
    function resultCallback(err, doc, responseOptions) {
        if (err) throw 'Error while creating snippet from response: ' + err;
    }

    var collection = getContext().getCollection();
    var updatedReview = getContext().getRequest().getBody();
    var lastResponse = updatedReview.Responses[updatedReview.Responses.length - 1];
    var snippet = {
        id : Math.random().toString(16).slice(2),
        CustomerName: lastResponse.CustomerResponding.Name,
        CustomerKey: lastResponse.CustomerResponding.CustomerKey,
        Text: lastResponse.ResponseText,
        Rating: -1,
        ProductCode: updatedReview.ProductCode,
        Date: updatedReview[""Date""],
        IsForIndex: true
    };
    collection.createDocument(collection.getSelfLink(), snippet, {}, resultCallback);
}"
                };

                ResourceResponse<Trigger> createResponse = await Client.CreateTriggerAsync(CollectionUri, trigger);
            }
        }

        /// <summary>
        /// Saves the specified review.
        /// </summary>
        public async Task<SaveResponse> Save(ProductReview review)
        {
            Argument.CheckIfNull(review, "review");

            Document persistedDocument;
            if (review.IsNew)
            {
                RequestOptions requestOptions = new RequestOptions
                {
                    PostTriggerInclude = new List<string> { ReviewTriggerName },
                    ConsistencyLevel = ConsistencyLevel.Eventual
                };

                ResourceResponse<Document> newCreation = await Client.CreateDocumentAsync(CollectionUri, review, requestOptions);
                persistedDocument = newCreation.Resource;
            }
            else
            {
                RequestOptions requestOptions = new RequestOptions
                {
                    PostTriggerInclude = new List<string> { ResponseTriggerName },
                    ConsistencyLevel = ConsistencyLevel.Eventual
                };

                ResourceResponse<Document> updateResponse = await Client.ReplaceDocumentAsync(review, requestOptions);
                persistedDocument = updateResponse.Resource;
            }
            Identifier identifier = new Identifier(persistedDocument.Id, new VersionNumber(persistedDocument.ETag));
            return new SaveResponse(identifier, persistedDocument);
        }

        /// <summary>
        /// Loads the reviews.
        /// </summary>
        public async Task<IEnumerable<ProductReview>> LoadReviews(string productCode)
        {
            Argument.CheckIfNull(productCode, "productCode");

            string value = await LoadReviewsJson(productCode);
            ProductReview[] productReviews = JArray.Parse(value).Select(item => item.ToObject<ProductReview>()).ToArray();
            return productReviews;
        }

        /// <summary>
        /// Loads the reviews for a given product and returns them as a JSON string
        /// </summary>
        public async Task<string> LoadReviewsJson(string productCode)
        {
            Argument.CheckIfNull(productCode, "productCode");
            return await Task.Run(() => LoadReviewJsonByProductId(productCode));
        }

        private async Task<string> LoadReviewJsonByProductId(string productCode)
        {
            try
            {
                string loadAllReviewsReference = _storedProcLinks["loadAllReviews"];
                StoredProcedureResponse<dynamic> response = await Client.ExecuteStoredProcedureAsync<dynamic>(loadAllReviewsReference, productCode);
                return response.Response.ToString();
            }
            catch (AggregateException aggregateException)
            {
                const string message = "Unable to find reviews.";
                throw new EntityNotFoundException(message, aggregateException);
            }
        }

        /// <summary>
        /// Adds the review response.
        /// </summary>
        /// <param name="reviewId">The review identifier.</param>
        /// <param name="reviewResponse">The review response.</param>
        public async Task<string> AddReviewResponse(string reviewId, ProductReviewResponse reviewResponse)
        {
            Argument.CheckIfNullOrEmpty(reviewId, "reviewId");
            Argument.CheckIfNull(reviewResponse, "reviewResponse");

            string query = "SELECT * FROM Reviews r WHERE r.id = \"{0}\"".FormatWith(reviewId);
            IQueryable<dynamic> reviews = Client.CreateDocumentQuery<ProductReview>(CollectionUri, query);

            List<dynamic> results = reviews.ToList();
            if (!results.Any())
            {
                throw new EntityNotFoundException("Unable to load review with id {0}".FormatWith(reviewId));
            }

            JObject result = JObject.Parse(results[0].ToString());
            if (result["Responses"] == null)
            {
                result["Responses"] = new JArray();
            }

            JArray responseArray = (JArray) result["Responses"];
            responseArray.Add(JObject.Parse(JsonConvert.SerializeObject(reviewResponse)));
            ProductReview review = result.ToObject<ProductReview>();

            await review.Save(this);
            return JsonConvert.SerializeObject(review);
        }

        /// <summary>
        /// Loads the reviews.
        /// </summary>
        public async Task<IEnumerable<ProductReviewEntry>> LoadReviewsForIndexing()
        {
            IQueryable<ProductReviewEntry> query =
                from review in Client.CreateDocumentQuery<ProductReviewEntry>(CollectionUri)
                where review.IsForIndex
                select review;

            List<ProductReviewEntry> results = query.ToList();

            return await Task.Run(() => results.ToList());
        }

        /// <summary>
        /// Deletes the specified entry.
        /// </summary>
        public async Task Delete(ProductReviewEntry entry)
        {
            await Client.DeleteDocumentAsync(entry.SelfLink);
        }
    }
}