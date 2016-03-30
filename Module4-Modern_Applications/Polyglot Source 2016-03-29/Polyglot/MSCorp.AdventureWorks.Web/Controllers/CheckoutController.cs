using System.Web.Mvc;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: Search
        public ActionResult Index()
        {
            ViewBag.BannerTitle = "Cart";
            return View();
        }
    }
}