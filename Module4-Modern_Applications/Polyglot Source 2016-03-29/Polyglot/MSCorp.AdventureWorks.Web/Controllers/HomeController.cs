using System.Web.Mvc;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.BannerTitle = "Smooth Performance.";
            return View();
        }
    }
}