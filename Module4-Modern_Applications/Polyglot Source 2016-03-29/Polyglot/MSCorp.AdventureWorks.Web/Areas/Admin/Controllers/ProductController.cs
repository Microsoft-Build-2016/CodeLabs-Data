using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common;
using Microsoft.Azure.Search.Models;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Core.Search;
using MSCorp.AdventureWorks.Web.Areas.Admin.Models;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    public class ProductController : Controller
    {
        private readonly AzureSearchClient<PrimaryIndexEntry> _searchClient;
        private readonly IProductRepository _productRepository;

        public ProductController(AzureSearchClient<PrimaryIndexEntry> searchClient, IProductRepository productRepository)
        {
            _searchClient = searchClient;
            _productRepository = productRepository;
        }

        public async Task<ActionResult> Index()
        {
            var products = await _searchClient.ExecuteSearch("*", null);

            // check that we actualy have the "Documents" property
            if (products != null)
            {
                return View(products);
            }

            return View();
        }


        // GET: Admin/Product/Create
        [System.Web.Mvc.Route("Product/Create/{categoryName}")]
        public ActionResult Create(string categoryName)
        {
            ViewBag.CategoryName = categoryName;
            return View();
        }

        // POST: Admin/Product/Create
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Temp stub method only")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "collection", Justification = "Temp stub method only")]
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Error = "Unable to create product";
                return View();
            }
        }

        [Route("Product/Edit/{productCode}")]
        public async Task<ActionResult> Edit(string productCode, string success, string error)
        {
            ViewBag.BannerTitle = "Edit";
            EditProductModel model = new EditProductModel();
            model.Product = await _productRepository.LoadProduct(productCode);
            
            //  grab the list of valid products
            var categories = await _productRepository.LoadAllCategorySets();
            model.CategoriesList = categories.Select(i => new SelectListItem{Text = i.Category, Value = i.Category});
            model.ColorsList = GetValidColors();

            ViewBag.Success = success;
            ViewBag.Error = error;

            return View(model);
        }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Web.Mvc.SelectListItem.set_Text(System.String)")]
        private static IEnumerable<SelectListItem> GetValidColors()
        {
            return new List<SelectListItem>()
            {
                new SelectListItem() { Text = "" },
                new SelectListItem() { Text = "Black" },
                new SelectListItem() { Text = "White" },
                new SelectListItem() { Text = "Silver" }, 
                new SelectListItem() { Text = "Red" },
                new SelectListItem() { Text = "Green" },
                new SelectListItem() { Text = "Blue" },
                new SelectListItem() { Text = "Yellow" },
            };
            
        }
            
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Temp stub method only")]
        [HttpPost]
        [Route("Product/Edit/{productCode}")]
        public async Task<ActionResult> Edit(string productCode, FormCollection collection)
        {
            try
            {
                //  grab the existing product from the repo, and update it.
                Product product = await _productRepository.LoadProduct(productCode);
                product.SetPropertyValue(DocumentIdentity.Etag, collection["Product.ETag"]);

                product.ProductCode = HttpUtility.HtmlDecode(collection["Product.ProductCode"]);
                product.ProductCategory = HttpUtility.HtmlDecode(collection["Product.ProductCategory"]);
                product.ProductName = HttpUtility.HtmlDecode(collection["Product.ProductName"]);
                product.ProductType = HttpUtility.HtmlDecode(collection["Product.ProductType"]);
                product.ProductDescription = HttpUtility.HtmlDecode(collection["Product.ProductDescription"]);

                decimal price;
                //update the price with the existing currency
                if (decimal.TryParse(collection["Price"], out price))
                {
                    product.Price = new Money(price, product.Price.Currency);
                }

                if (decimal.TryParse(collection["DiscountedPrice"], out price))
                {
                    product.DiscountedPrice = new Money(price, product.DiscountedPrice.Currency);
                    product.Discount = new Percentage(100 - (100 * product.DiscountedPrice.Amount/product.Price.Amount));
                }

                int priority;
                if (int.TryParse(collection["Product.Priority"], out priority))
                {
                    product.Priority = priority;
                }

                string productSizes = HttpUtility.HtmlDecode(collection["ProductSizes"]);
                product.ProductSizes = productSizes.Split('|').
                    Where(i=> !string.IsNullOrWhiteSpace(i)).
                    Select(i => new ProductSize(i, 0));

                string colorName = collection["ColorName"];
                if (!string.IsNullOrWhiteSpace(colorName))
                {
                    product.Color = Color.FromName(colorName);
                }

                ProductSaveTask productSaveTask = new ProductSaveTask(_productRepository);
                await productSaveTask.SaveAsync(product);
                return RedirectToAction("Edit",new {productCode = product.ProductCode, success = "Save successful!"});
            }
            catch (EntityNotFoundException)
            {
                string error = "Unable to save product.  This product may been concurrently modified by someone else.  Please reapply your changes.";
                return RedirectToAction("Edit", new { productCode, error });
            }
        }

        // GET: Admin/Product/Delete/5
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "productCode", Justification = "Temp stub method only")]
        [Route("Product/Delete/{productCode}")]
        public ActionResult Delete(string productCode)
        {
            throw  new NotImplementedException("Product Deletions not yet implemented");
        }

        // GET: Admin/Product/Discount
        [Route("Product/Discount")]
        public ActionResult Discount()
        {
            return View();
        }

        // GET: Admin/Product/Discount
        [Route("Product/Discount")]
        [HttpPost]
        public async Task<HttpStatusCodeResult> ApplyDiscount()
        {
            try
            {
                string requestData = await new StreamReader(Request.InputStream).ReadToEndAsync();
                UpdateDiscountInDocumentCollection(requestData);
                await UpdateDiscountInSearchIndex(requestData);
            }
            catch (Exception exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Error while applying discounts.  Error was {0}".FormatWith(exception.Message));
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        private void UpdateDiscountInDocumentCollection(string requestData)
        {
            _productRepository.ApplyDiscountToProducts(requestData);
        }

        private async Task UpdateDiscountInSearchIndex(string requestData)
        {
            JObject request = JObject.Parse(requestData);
            int newDiscount = request["discount"].ToObject<int>();
            JArray products = request["products"].ToJArray();

            foreach (JToken product in products)
            {
                string key = product["productCode"].ToString();
                var updateRequest = new Document();
                updateRequest.Add("Discount", newDiscount);
                updateRequest.Add("Key",key);
                await _searchClient.MergeDocumentAsync(updateRequest);
            }
        }

    }
}
