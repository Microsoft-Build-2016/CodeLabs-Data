using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Web.Utilities;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Web.Controllers
{

    /// <summary>
    /// API controller which allows us to wrap up customer service calls with the correct auth information attached.
    /// </summary>
    public class ApiCustomerController : ApiController
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IOrderSummaryRepository _orderSummaryRepository;

        public ApiCustomerController(ICustomerRepository customerRepository, IOrderSummaryRepository orderSummaryRepository)
        {
            Argument.CheckIfNull(customerRepository, "customerRepository");
            Argument.CheckIfNull(orderSummaryRepository, "orderSummaryRepository");
            _customerRepository = customerRepository;
            _orderSummaryRepository = orderSummaryRepository;
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not appropriate")]
        [HttpGet]
        [Route("Api/Customer/GetAllCustomers")]
        public HttpResponseMessage GetAllCustomers()
        {
            IEnumerable<Customer> customers = _customerRepository.LoadAllCustomers();
            //  turn the customers list back into a JSON array
            JObject[] results = customers.Select(d => JObject.Parse(d.ToString())).ToArray();
            JArray array = new JArray(results);
            return WebApiHelper.ReturnRawJson(array.ToString());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Not appropriate")]
        [HttpGet]
        [Route("Api/Customer/GetFirstCustomer/")]
        public HttpResponseMessage GetFirstCustomer()
        {
            Customer customer = _customerRepository.LoadAllCustomers().FirstOrDefault();
            if (customer == null)
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            //  turn the customers list back into a JSON object
            JObject customerObject = JObject.Parse(customer.ToString());
            return WebApiHelper.ReturnRawJson(customerObject.ToString());
        }

        [HttpGet]
        [Route("Api/Customer/Get/{customerId}")]
        public async Task<HttpResponseMessage> GetCustomerById(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                //  default to the first available user for demo purposes
                return GetFirstCustomer();
            }

            try
            {
                Customer customer =  await _customerRepository.LoadCustomer(new Identifier(customerId, new VersionNumber()));
                if (customer == null)
                {
                    //return WebApiHelper.ReturnRawJson("Stupid");
                    return new HttpResponseMessage(HttpStatusCode.BadRequest);
                }
                //  turn the customers list back into a JSON object
                JObject customerObject = JObject.Parse(customer.ToString());
                return WebApiHelper.ReturnRawJson(customerObject.ToString());
            }
            catch (Exception)
            {
                // ignore for demo purposes and get the first available
                return GetFirstCustomer();
            }
        }

        [HttpGet]
        [Route("Api/Customer/Orders/{customerId}")]
        public async Task<HttpResponseMessage> GetOrdersByCustomerId(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                return WebApiHelper.ReturnRawJson(String.Empty);
            }

            string returnJson = await _orderSummaryRepository.LoadSummariesByCustomerJson(customerId);
            return WebApiHelper.ReturnRawJson(returnJson);
        }
    }
}