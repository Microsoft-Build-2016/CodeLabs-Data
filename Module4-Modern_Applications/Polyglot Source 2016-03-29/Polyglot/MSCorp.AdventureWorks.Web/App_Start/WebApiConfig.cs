using System.Web.Http;
using Common;
using MSCorp.AdventureWorks.Web.Utilities;

namespace MSCorp.AdventureWorks.Web
{
    public static class WebApiConfig
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public static void Register(HttpConfiguration configuration)
        {
            Argument.CheckIfNull(configuration, "configuration");
            // Web API routes
            configuration.MapHttpAttributeRoutes();
            //configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}",
            //    new { id = RouteParameter.Optional });

            configuration.DependencyResolver = _resolver;
        }


        private static WebApiDependencyResolver _resolver;
        /// <summary>
        /// must be set before calling Register
        /// </summary>
        /// <param name="resolver"></param>
        public static void SetResolver(WebApiDependencyResolver resolver)
        {
            _resolver = resolver;
        }
    }

}