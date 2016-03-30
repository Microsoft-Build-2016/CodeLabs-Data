using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Import;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Core.Search;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    public class DemoSetupController : Controller
    {
        private readonly AzureSearchClient<PrimaryIndexEntry> _searchClient;
        private readonly AzureSearchClient<ReviewIndexEntry> _reviewSearchClient;
        private readonly IProductRepository _productRepository;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification="Work in progress")]
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrencyRepository _currencyRepository;
        private readonly DbConfig _dbConfig;

        public DemoSetupController(AzureSearchClient<PrimaryIndexEntry> searchClient, AzureSearchClient<ReviewIndexEntry> reviewSearchClient, IProductRepository productRepository, 
            IProductReviewRepository productReviewRepository, ICustomerRepository customerRepository, ICurrencyRepository currencyRepository,
            DbConfig dbConfig)
        {
            Argument.CheckIfNull(searchClient, "searchClient");
            Argument.CheckIfNull(reviewSearchClient, "reviewSearchClient");
            Argument.CheckIfNull(productRepository, "productRepository");
            Argument.CheckIfNull(productReviewRepository, "productReviewRepository");
            Argument.CheckIfNull(customerRepository, "customerRepository");
            Argument.CheckIfNull(currencyRepository, "currencyRepository");
            Argument.CheckIfNull(dbConfig, "dbConfig");
            _searchClient = searchClient;
            _reviewSearchClient = reviewSearchClient;
            _productRepository = productRepository;
            _productReviewRepository = productReviewRepository;
            _customerRepository = customerRepository;
            _currencyRepository = currencyRepository;
            _dbConfig = dbConfig;
        }

        // GET: Search
        [Route("Admin/DemoSetup")]
        [Route("DemoSetup")]
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> CreateIndex()
        {
            // Create main search index
            await _searchClient.DeleteIndex();

            MagnitudeScoringFunction priorityFunction = new MagnitudeScoringFunction(new MagnitudeScoringParameters(1, 10000) , "Priority", 10);
            FreshnessScoringFunction lastPurchasedDateFunction = new FreshnessScoringFunction(new FreshnessScoringParameters(TimeSpan.FromDays(1)), "LastPurchasedDate", 2);

            var profileScoringProfile = new ScoringProfile("priority") { Functions = new List<ScoringFunction>() { priorityFunction } };
            var productWeightingProfile = new ScoringProfile("productWeightings") ;
            var lastPurchasedProfile = new ScoringProfile("lastPurchased") { Functions = new List<ScoringFunction>() { lastPurchasedDateFunction } };
            await _searchClient.CreateIndex(profileScoringProfile, productWeightingProfile, lastPurchasedProfile);
            
            // Create review search index
            await _reviewSearchClient.DeleteIndex();
            await _reviewSearchClient.CreateIndex();
            
            return RedirectToAction("Index");
        }

        [Route("DemoSetup/SimpleSetup")]
        public async Task<ActionResult> SimpleSetup()
        {
            await DeleteData();

            Assembly assembly = Assembly.GetExecutingAssembly();
            var resourceName = "MSCorp.AdventureWorks.Web.Data.import.zip";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                HttpStatusCodeResult codeResult = await PerformImport(stream);
                if (codeResult.StatusCode == (int) HttpStatusCode.OK)
                {
                    return RedirectToAction("Index");
                }

                return codeResult;
            }
        }

        /// <summary>
        /// Deletes the data.
        /// </summary>
        [Route("DemoSetup/DeleteData")]
        public async Task<ActionResult> DeleteData()
        {
            await CreateIndex();
            await ResetDatabase();

            return RedirectToAction("Index");
        }

        private async Task ResetDatabase()
        {
            await _dbConfig.Delete();
            await _dbConfig.Initialise();
        }

        [HttpPost]
        public async Task<HttpStatusCodeResult> ImportData()
        {
            HttpFileCollectionBase files = Request.Files;
            HttpPostedFileBase file = files[0];
            if (file == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Could not find a file in post data.");
            }
            Stream inputStream = file.InputStream;

            return await PerformImport(inputStream);
        }

        private async Task<HttpStatusCodeResult> PerformImport(Stream inputStream)
        {
            int customerCount = 0;
            int currencyCount = 0;
            int productCount = 0;
            try
            {
                ImportScenario scenario = new ScenarioImporter().ImportFromZip(inputStream);

                // Save customers
                foreach (Customer customer in scenario.Customers)
                {
                    await customer.Save(_customerRepository);
                    customerCount++;
                }

                // Save currency exchange rates
                foreach (ExchangeRate exchangeRate in scenario.ExchangeRates)
                {
                    await exchangeRate.Save(_currencyRepository);
                    currencyCount++;
                }

                // Save products.
                ProductSaveTask saveProductTask = new ProductSaveTask(_productRepository);
                foreach (Product product in scenario.Products)
                {
                    ProductImage[] relatedImages = scenario.ProductImagesFor(product.ProductCode).ToArray();
                    await saveProductTask.SaveAsync(product, relatedImages);
                    productCount++;
                }

                foreach (ProductReview review in scenario.ProductReviews)
                {
                    await review.Save(_productReviewRepository);
                }
            }
            catch (Exception e)
            {
                string message = "Something went wrong.  Processed {0} customers, {1} currencies, and {2} products.  Error was {3}"
                    .FormatWith(customerCount, currencyCount, productCount, e.Message);
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, message);
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
    }
}