using System.Web.Mvc;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Repository;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    public class OrderHistoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderHistoryController"/> class.
        /// </summary>
        public OrderHistoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: Search
        public ActionResult Index()
        {
            ViewBag.BannerTitle = "Order History";
            return View();
        }
        
        [Route("OrderHistory/Detail/{orderId}")]
        public ActionResult Detail(int orderId)
        {
            Order order = _unitOfWork.Orders.Load(orderId);
            return View(order);
        }
    }
}