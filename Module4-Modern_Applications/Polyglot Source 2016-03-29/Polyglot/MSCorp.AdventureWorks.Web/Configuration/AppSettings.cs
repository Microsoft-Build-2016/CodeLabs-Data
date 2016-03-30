using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MSCorp.AdventureWorks.Core.Configuration;

namespace MSCorp.AdventureWorks.Web.Configuration
{
    public class AppSettings : IAppSettings
    {
        public AppSettings()
        {
            JsonPath = HttpContext.Current.Server.MapPath("~/Data/Json");
        }
        public string DocumentDbConnectionString { get; set; }
        public string JsonPath { get; set; }
    }
}
