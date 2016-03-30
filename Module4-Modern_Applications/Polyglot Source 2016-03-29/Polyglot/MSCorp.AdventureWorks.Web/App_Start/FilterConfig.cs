using System.Web.Mvc;
using Common;

namespace MSCorp.AdventureWorks.Web
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            Argument.CheckIfNull(filters, "filters");
            
            filters.Add(new HandleErrorAttribute());
        }
    }
}
