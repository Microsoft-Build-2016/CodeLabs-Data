using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using Common;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Core.Search;
using MSCorp.AdventureWorks.Web.Models.ShoppingCart;
using MSCorp.AdventureWorks.Web.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Order = MSCorp.AdventureWorks.Core.Domain.Order;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    /// <summary>
    /// API controller which allows us to wrap up shopping cart calls.
    /// </summary>
    public class ApiCartController : ApiController
    {
        private readonly ICustomerCartRepository _customerCartRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderSummaryRepository _orderSummaryRepository;
        private readonly AzureSearchClient<PrimaryIndexEntry> _search;

        public ApiCartController(ICustomerCartRepository customerCartRepository, 
            IOrderSummaryRepository orderSummaryRepository, IUnitOfWork unitOfWork, AzureSearchClient<PrimaryIndexEntry> search)
        {
            Argument.CheckIfNull(customerCartRepository, "customerCartRepository");
            Argument.CheckIfNull(orderSummaryRepository, "orderSummaryRepository");
            Argument.CheckIfNull(unitOfWork, "unitOfWork");
            Argument.CheckIfNull(search, "search");

            _customerCartRepository = customerCartRepository;
            _orderSummaryRepository = orderSummaryRepository;
            _unitOfWork = unitOfWork;
            _search = search;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "customerCode"), System.Web.Http.Route("Api/Cart/{customerCode}")]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("Api/Cart/{customerKey}")]
        public async Task<HttpResponseMessage> GetCart(int customerKey)
        {
            try
            {
                string storedOrder = await _customerCartRepository.Load(customerKey);
                return WebApiHelper.ReturnRawJson(storedOrder);
            }
            catch (Exception)
            {
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }

        [System.Web.Http.Route("Api/Cart/Add")]
        [System.Web.Http.HttpPost]
        public async Task<HttpStatusCodeResult> Add()
        {
            try
            {
                string result = await Request.Content.ReadAsStringAsync();
                JObject request = JObject.Parse(result);
                int customerKey = request["customer"]["CustomerKey"].Value<int>();

                string storedOrder = await _customerCartRepository.Load(customerKey);
                
                JObject cart = new JObject();
                if (string.IsNullOrWhiteSpace(storedOrder))
                {
                    cart["customer"] = request["customer"];
                    cart["orderLines"] = new JArray();
                }
                else
                {
                    // If exists, load cart from customerCartRepository.
                    cart = JObject.Parse(storedOrder);
                }

                JToken newLine = request["orderLine"];

                JToken matchingLine = cart["orderLines"].SingleOrDefault(line => line["productDetails"]["productCode"].Value<string>().Equals(newLine["productDetails"]["productCode"].Value<string>()));
                if (matchingLine == null)
                {
                    ((JArray)cart["orderLines"]).Add(newLine);
                }
                else
                {
                    matchingLine["productDetails"]["quantity"] =
                        matchingLine["productDetails"]["quantity"].ToObject<int>() +
                        newLine["productDetails"]["quantity"].ToObject<int>();
                }

                await _customerCartRepository.Save(customerKey, cart.ToString());
                return new HttpStatusCodeResult(HttpStatusCode.Accepted);
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "Unable to save cart");
            }
        }

        [System.Web.Http.Route("Api/Cart/PlaceOrder")]
        [System.Web.Http.HttpPost]
        public async Task<HttpStatusCodeResult> PlaceOrder()
        {
            string result = await Request.Content.ReadAsStringAsync();
            Cart cart = JsonConvert.DeserializeObject<Cart>(result);
            Order order = OrderBuilder.Build(cart);
            
            // After saving, clear out the table entry.
            _unitOfWork.Orders.Save(order);
            await _unitOfWork.Commit();
            await _customerCartRepository.Delete(cart.Customer.CustomerKey);

            OrderSummary orderSummary = new OrderSummary(order);
            await orderSummary.Save(_orderSummaryRepository);

            //update the last purchased date
            foreach (var code in order.Lines.Select(l => l.ProductCode).Distinct())
            {
                var update = PrimarySearchIndexBuilder.BuildForLastPurchasedDate(code, Clock.Now);
                await _search.DeleteDocuments(new PrimaryIndexEntry[] { new PrimaryIndexEntry("HL-U509-R") });
                await _search.MergeDocumentAsync(update);
            }

            return new HttpStatusCodeResult(HttpStatusCode.Accepted, "Your order has been placed");
        }
    }
}