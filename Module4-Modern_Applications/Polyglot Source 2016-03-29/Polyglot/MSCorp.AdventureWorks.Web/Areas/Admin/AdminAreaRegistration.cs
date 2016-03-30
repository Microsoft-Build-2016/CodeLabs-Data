using System.Web.Mvc;
using Common;

namespace MSCorp.AdventureWorks.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public override void RegisterArea(AreaRegistrationContext context)
        {
            Argument.CheckIfNull(context, "context");

            context.Routes.MapMvcAttributeRoutes();

            context.MapRoute(
                "Admin_withkey",
                "Admin/{controller}/{action}/{key}",
                new { action = "Index", guid = "key" },
                new[] { "MSCorp.AdventureWorks.Web.Areas.Admin.Controllers" }
                );

            
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "MSCorp.AdventureWorks.Web.Areas.Admin.Controllers" }
                );

        }
    }
}