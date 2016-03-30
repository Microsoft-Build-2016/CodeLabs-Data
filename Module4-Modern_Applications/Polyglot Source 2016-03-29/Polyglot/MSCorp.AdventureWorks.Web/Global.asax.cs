using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MSCorp.AdventureWorks.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "dbConfig persists (in IoC) after this method")]
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            DbConfig dbConfig = new DbConfig();
            dbConfig.Initialise().Wait();

            UnityConfig.RegisterComponents(dbConfig); 
            GlobalConfiguration.Configure(WebApiConfig.Register); 
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
