using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;
using Common;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Search.Models;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Core.Search;
using MSCorp.AdventureWorks.Web.Models;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    public class DiscoverController : Controller
    {
        private readonly DocumentDbDiscoveryRepository _discoveryRepository;
        private readonly AzureSearchClient<PrimaryIndexEntry> _azureSearchClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoverController"/> class.
        /// </summary>
        public DiscoverController(DocumentDbDiscoveryRepository discoveryRepository, AzureSearchClient<PrimaryIndexEntry> azureSearchClient)
        {
            _discoveryRepository = discoveryRepository;
            _azureSearchClient = azureSearchClient;
        }

        public ActionResult Index(ItemSet set)
        {
            return View("Index", set);
        }

        [HttpGet]
        [Route("Api/Discover/Triggers/{collectionName}")]
        public ActionResult GetTriggers(string collectionName)
        {
            IEnumerable<Trigger> triggers = _discoveryRepository.TriggersFor(collectionName);
            ItemSet set = new ItemSet();
            ViewBag.Title = "Triggers for collection {0}".FormatWith(collectionName);

            foreach (Trigger trigger in triggers)
            {
                set.Add(new ItemModel
                {
                    Name = trigger.Id,
                    Subtitle1 = "Trigger Operation : {0}".FormatWith(trigger.TriggerOperation.ToString()),
                    Subtitle2 = "Trigger Type : {0}".FormatWith(trigger.TriggerType),
                    Body = trigger.Body
                });
            }
            return Index(set);

        }

        [HttpGet]
        [Route("Api/Discover/StoredProcedures/{collectionName}")]
        public ActionResult GetProcedures(string collectionName)
        {
            IEnumerable<StoredProcedure> procedures = _discoveryRepository.StoredProceduresFor(collectionName);
            ItemSet set = new ItemSet();
            ViewBag.Title = "Procedures for collection {0}".FormatWith(collectionName);

            foreach (var proc in procedures)
            {
                set.Add(new ItemModel
                {
                    Name = proc.Id,
                    Body = proc.Body
                });
            }
            return Index(set);
        }

        [HttpGet]
        [Route("Api/Discover/UserDefinedFunctions/{collectionName}")]
        public ActionResult GetUserDefinedFunctions(string collectionName)
        {
            IEnumerable<UserDefinedFunction> functions = _discoveryRepository.UserDefinedFunctionsFor(collectionName);
            ItemSet set = new ItemSet();
            ViewBag.Title = "UDF for collection {0}".FormatWith(collectionName);

            foreach (var function in functions)
            {
                set.Add(new ItemModel
                {
                    Name = function.Id,
                    Body = function.Body
                });
            }
            return Index(set);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [HttpGet]
        [Route("Api/Discover/Indexer/")]
        public ActionResult GetIndexer()
        {
            ItemSet set = new ItemSet();
            ViewBag.Title = "Indexer for Products";

            set.Add(new ItemModel
            {
                Name = "DataSource",
                Body = JsonConvert.SerializeObject(_azureSearchClient.BuildDataSource(), Formatting.Indented)
            });

            set.Add(new ItemModel
            {
                Name = "Indexer",
                Body = JsonConvert.SerializeObject(_azureSearchClient.BuildIndexer(), Formatting.Indented)
            });

            return Index(set);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        [HttpGet]
        [Route("Api/Discover/Index/")]
        public ActionResult GetIndex()
        {
            ItemSet set = new ItemSet();
            ViewBag.Title = "Index for Products";

            MagnitudeScoringFunction priorityFunction = new MagnitudeScoringFunction(new MagnitudeScoringParameters(1, 10000), "Priority", 10);
            FreshnessScoringFunction lastPurchasedDateFunction = new FreshnessScoringFunction(new FreshnessScoringParameters(TimeSpan.FromDays(1)), "LastPurchasedDate", 2);

            var profileScoringProfile = new ScoringProfile("priority") { Functions = new List<ScoringFunction>() { priorityFunction } };
            var productWeightingProfile = new ScoringProfile("productWeightings");
            var lastPurchasedProfile = new ScoringProfile("lastPurchased") { Functions = new List<ScoringFunction>() { lastPurchasedDateFunction } };
            var index = _azureSearchClient.BuildIndex(profileScoringProfile, productWeightingProfile, lastPurchasedProfile);

            set.Add(new ItemModel
            {
                Name = "Index",
                Body = JsonConvert.SerializeObject(index, Formatting.Indented)
            });

            return Index(set);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Configs")]
        [HttpGet]
        [Route("Api/Discover/Configs/")]
        public ActionResult Configs()
        {
            ItemSet set = new ItemSet();
            ViewBag.Title = "Configuration";

            foreach (FieldInfo info in typeof(SettingKeys).GetFields())
            {
                if (info.IsLiteral)
                {
                    var key = info.GetRawConstantValue().ToString();
                    set.Add(new ItemModel
                    {
                        Name = key,
                        Body = SettingLoader.Load(key).Value
                    });
                }
            }

            return Index(set);
        }
    }
}