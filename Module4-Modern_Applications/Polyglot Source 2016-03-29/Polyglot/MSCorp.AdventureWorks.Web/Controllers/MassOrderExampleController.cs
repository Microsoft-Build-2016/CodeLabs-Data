using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Common;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Web.Utilities;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    public class MassOrderExampleController : Controller
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;

        private const int numberOfThreads = 25;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public MassOrderExampleController(IUnitOfWorkFactory unitOfWorkFactory, 
            ICustomerRepository customerRepository, IProductRepository productRepository)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
        }

        [Route("MassOrdering")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("MassOrdering/MassOrdering")]
        public async Task<ActionResult> MassOrdering(int numberOfOrders)
        {
            var startTime = DateTime.Now;

            var customersGlobal = _customerRepository.LoadAllCustomers().ToArray();
            var productsEnum = await _productRepository.LoadProducts();
            var productsGlobal = productsEnum.ToArray();

            var threads = new List<Thread>();

            var ordersPerThread = numberOfOrders/ numberOfThreads;
            
            for (int t = 0; t < numberOfThreads; t++)
            {
                var t1 = t;
                Thread thread = new Thread(async o =>
                {
                    var gst = new Gst(15);
                    var random = new Random();
                    //To avoid thread lock waits (i.e. hopefully lets the thread run faster!)
                    var customers = customersGlobal.Clone() as Customer[];
                    var products = productsGlobal.Clone() as Product[];
                    using (var unitofWork = _unitOfWorkFactory.GenerateUnitOfWork())
                    {
                        for (var i = 0; i < ordersPerThread; i++)
                        {
                            var customer = customers[random.Next(0, customers.Length - 1)];
                            var orderLines = new List<OrderLine>();

                            for (var y = 0; y < 5; y++)
                            {
                                var product = products[random.Next(0, products.Length - 1)];

                                var count = random.Next(1, 3);

                                var orderLine = new OrderLine((double)product.Price.Amount * count, (double)product.DiscountedPrice.Amount * count, (double)product.DiscountedPrice.GstToPay(gst).Amount * count, (double)(product.Price.Amount - product.DiscountedPrice.Amount) * count, (double)product.Price.Amount, (double)product.Price.Amount, product.ProductCode, product.ProductDescription, product.ProductSizes?.FirstOrDefault()?.Name, count);
                                orderLines.Add(orderLine);
                            }

                            Order order = new Order(DateTime.Now, customer.CustomerKey.ToString(), customer.PreferredCurrency.Code, orderLines);
                            unitofWork.Orders.Save(order);
                            await unitofWork.Commit();
                        }
                    }
                });
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            var endTime = DateTime.Now;
            var timeDifference = endTime - startTime;
            return View(timeDifference.TotalSeconds);
        }
    }
}
