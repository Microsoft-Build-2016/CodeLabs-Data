using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common;
using MSCorp.AdventureWorks.Core.Search;
using MSCorp.AdventureWorks.Web.Utilities;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    /// <summary>
    /// API controller which allows us to view the Doc DB code.
    /// </summary>
    public class ApiDiscoverController : ApiController
    {
        private readonly AzureSearchClient<PrimaryIndexEntry> _searchClient;
        private readonly AzureSearchClient<ReviewIndexEntry> _reviewSearchClient;

        public ApiDiscoverController(AzureSearchClient<PrimaryIndexEntry> searchClient,
            AzureSearchClient<ReviewIndexEntry> reviewSearchClient)
        {
            _searchClient = searchClient;
            _reviewSearchClient = reviewSearchClient;
        }

        [HttpGet]
        [Route("Api/Discover/AzureSearchSchema/{indexName}")]
        public async Task<HttpResponseMessage> GetSearchIndexSchema(string indexName)
        {
            if (_searchClient.IndexName.Equals(indexName, StringComparison.OrdinalIgnoreCase))
            {
                string schema = await _searchClient.LoadSchema();
                return WebApiHelper.ReturnRawJson(JObject.Parse(schema).ToString());
            }
            if (_reviewSearchClient.IndexName.Equals(indexName, StringComparison.OrdinalIgnoreCase))
            {
                string schema = await _reviewSearchClient.LoadSchema();
                return WebApiHelper.ReturnRawJson(JObject.Parse(schema).ToString());
            }

            string message = "Unable to find Azure Search index with name {0}".FormatWith(indexName);
            return WebApiHelper.ReturnRawJson(message);
        }
    }
}