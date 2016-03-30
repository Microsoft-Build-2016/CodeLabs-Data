using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.Azure.Search.Models;
using MSCorp.AdventureWorks.Core.Search;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    public class IndexerController : Controller
    {
        private readonly AzureSearchClient<PrimaryIndexEntry> _productSearchClient;

        public IndexerController(AzureSearchClient<PrimaryIndexEntry> productSearchClient)
        {
            _productSearchClient = productSearchClient;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Route("Indexer/Create")]
        public async Task<ActionResult> Create()
        {
            await _productSearchClient.CreateIndexer();
            return RedirectToAction("Index");
        }

        [Route("Indexer/Run")]
        public async Task<ActionResult> Run()
        {
            await _productSearchClient.RunIndexer();
            return RedirectToAction("Index");
        }
    }
}
