using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Search.Models;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Core.Search;
using MSCorp.AdventureWorks.Web.Utilities;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    /// <summary>
    /// API controller which allows us to wrap up search service calls with the correct auth information attached.
    /// </summary>
    public class ApiSearchController : ApiController
    {
        private readonly AzureSearchClient<PrimaryIndexEntry> _searchClient;
        private readonly AzureSearchClient<ReviewIndexEntry> _reviewSearchClient;
        private readonly IProductRepository _productRepository;
        private readonly ICurrencyRepository _currencyRepository;

        public ApiSearchController(
            AzureSearchClient<PrimaryIndexEntry> searchClient,
            AzureSearchClient<ReviewIndexEntry> reviewSearchClient,
            IProductRepository productRepository,
            ICurrencyRepository currencyRepository)
        {
            Argument.CheckIfNull(searchClient, "searchClient");
            Argument.CheckIfNull(reviewSearchClient, "reviewSearchClient");
            Argument.CheckIfNull(productRepository, "productRepository");
            Argument.CheckIfNull(currencyRepository, "currencyRepository");
            _searchClient = searchClient;
            _reviewSearchClient = reviewSearchClient;
            _productRepository = productRepository;
            _currencyRepository = currencyRepository;
        }


        [Route("Api/Search/Suggest/{searchText}")]
        [HttpGet]
        public async Task<HttpResponseMessage> Suggest(string searchText)
        {
            var result = await _searchClient.ExecuteSuggest(searchText, new SuggestParameters()
            {
                UseFuzzyMatching = true,
                Top = 6,
                Select = new[] { "ProductCode" }
            });
            return WebApiHelper.ReturnRawJson(JsonConvert.SerializeObject(result));
        }

        [Route("Api/Search/Comments/{searchText}")]
        [HttpGet]
        public async Task<HttpResponseMessage> Comments(string searchText)
        {
            try
            {
                var result = await _reviewSearchClient.ExecuteSearch(searchText, new SearchParameters() { SearchMode = SearchMode.Any, Top = 5, HighlightFields = new[] { "ProductName", "Text" } });
                return WebApiHelper.ReturnRawJson(JsonConvert.SerializeObject(result));
            }
            catch (Exception)
            {
                return null;
            }

        }

        [Route("Api/Search/Products")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetProducts()
        {
            var result = await _searchClient.ExecuteSearch("*", new SearchParameters() { SearchMode = SearchMode.Any, Top = 20, Facets = new List<string> { "ProductCategory" }, Select = new[] { "Name", "Price", "ProductCode", "ProductCategory", "ThumbImageUrl" } });
            return WebApiHelper.ReturnRawJson(JsonConvert.SerializeObject(result));
        }

        [Route("Api/Search/Products/{searchText}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetProducts(string searchText)
        {
            return await GetProducts(searchText, 1000);
        }

        [Route("Api/Search/Products/{searchText}/{count}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetProducts(string searchText, int count)
        {
            KeyValuePair<string, string> orderByQuery = Request.GetQueryNameValuePairs().FirstOrDefault(p => p.Key.Equals("orderby", StringComparison.OrdinalIgnoreCase));

            var searchParam = new SearchParameters()
            {
                SearchMode = SearchMode.Any,
                Top = count,
                HighlightFields = new[] { "Description" },
                Facets = new List<string> { "SizeFacet", "Manufacturer", "Color" }
            };

            if (!string.IsNullOrWhiteSpace(orderByQuery.Value))
            {
                searchParam.OrderBy = new[] { orderByQuery.Value };
            }
            var result = await _searchClient.ExecuteSearch(searchText, searchParam);
            return WebApiHelper.ReturnRawJson(JsonConvert.SerializeObject(result));
        }

        [Route("Api/Search/Products/{searchText}/Drilldown/")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetProductDrilldown(string searchText)
        {
            var drillDownQuery = Request.GetQueryNameValuePairs().FirstOrDefault(p => p.Key.Equals("query", StringComparison.OrdinalIgnoreCase));
            var orderByQuery = Request.GetQueryNameValuePairs().FirstOrDefault(p => p.Key.Equals("orderby", StringComparison.OrdinalIgnoreCase));
            string query = "{0}*".FormatWith(searchText);

            var searchParam = new SearchParameters()
            {
                SearchMode = SearchMode.Any,
                Top = 1000,
                HighlightFields = new[] { "Description" },
                Facets = new List<string> { "SizeFacet", "Manufacturer", "Color" },
                Filter = drillDownQuery.Value
            };


            if (!string.IsNullOrWhiteSpace(orderByQuery.Value))
            {
                searchParam.OrderBy = new[] { orderByQuery.Value };
            }

            var result = await _searchClient.ExecuteSearch(query, searchParam);
            return WebApiHelper.ReturnRawJson(JsonConvert.SerializeObject(result));
        }

        [Route("Api/Search/RelatedProducts/{searchAttribute}/{searchText}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetRelatedProducts(string searchAttribute, string searchText)
        {
            // Hit document DB for this;
            string result = await _productRepository.LoadRelatedProductsByAttributeJsonAsync(searchAttribute, searchText);
            string response = "{{\"value\":{0}, \"@search.facets\":[]}}".FormatWith(result);
            return WebApiHelper.ReturnRawJson(response);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate"), Route("Api/Search/AllProductAttributes")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetAllProductAttributes()
        {
            // Hit document DB for this;
            string result = await _productRepository.LoadAllProductAttributesJsonAsync();
            return WebApiHelper.ReturnRawJson(result);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [Route("Api/Search/Currencies")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetCurrencies()
        {
            string result = await _currencyRepository.LoadAllExchangeRatesJsonAsync();
            return WebApiHelper.ReturnRawJson(result);
        }

        [Route("Api/Search/ProductsByCategory/{categoryName}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetProductsByCategory(string categoryName)
        {
            return await GetProductsByCategory(categoryName, 50);
        }

        [Route("Api/Search/ProductsByCategory/{categoryName}/{count}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetProductsByCategory(string categoryName, int count)
        {
            var result = await _searchClient.ExecuteSearch("", new SearchParameters()
            {
                Top = count,
                Facets = new[] { "ProductCategory" },
                Filter = "ProductCategory eq '{0}'".FormatWith(categoryName)
            });
            return WebApiHelper.ReturnRawJson(JsonConvert.SerializeObject(result));
        }

        [Route("Api/Search/Product/{id}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetProduct(string id)
        {
            string productJson = await _productRepository.LoadProductByCodeViaStoredProcedure(id);
            return WebApiHelper.ReturnRawJson(productJson);
        }

        [Route("Api/Search/Product/{id}/Images")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetProductImages(string id)
        {
            Product product = await _productRepository.LoadProduct(id);
            IEnumerable<Attachment> attachments = await _productRepository.LoadAttachments(product.SelfLink);
            JArray results = attachments.ToJArray();
            return WebApiHelper.ReturnRawJson(results.ToString());
        }
    }
}