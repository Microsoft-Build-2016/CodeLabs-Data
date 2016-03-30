using System.Web.Mvc;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        [Route("Product/{productId}")]
        public ActionResult Index(string productId)
        {
            ViewBag.ProductId = productId;
            ViewBag.BannerTitle = "Product";
            return View();
        }
    }
}