using System.Web.Mvc;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        [Route("Category/{categoryName}")]
        public ActionResult Index(string categoryName)
        {
            ViewBag.CategoryName = categoryName;
            ViewBag.BannerTitle = "Search";
            return View();
        }
    }
}