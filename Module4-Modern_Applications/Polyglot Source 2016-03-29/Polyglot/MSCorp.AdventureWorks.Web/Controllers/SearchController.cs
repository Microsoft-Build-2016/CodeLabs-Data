using System.Web.Mvc;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    public class SearchController : Controller
    {
        [Route("Search/{searchText}")]
        public ActionResult Index(string searchText)
        {
            ViewBag.SearchText = searchText;
            ViewBag.BannerTitle = "Search";
            ViewBag.IsRelatedProductSearch = false;
            return View();
        }

        [Route("Search/{attribute}/{searchText}")]
        public ActionResult Index(string attribute, string searchText)
        {
            ViewBag.SearchText = searchText;
            ViewBag.SearchAttribute = attribute;
            ViewBag.BannerTitle = "Search";
            ViewBag.IsRelatedProductSearch = true;
            return View();
        }

    }
}