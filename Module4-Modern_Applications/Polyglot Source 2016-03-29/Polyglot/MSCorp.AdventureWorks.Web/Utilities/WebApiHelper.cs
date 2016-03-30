using System.Net;
using System.Net.Http;
using System.Text;

namespace MSCorp.AdventureWorks.Web.Utilities
{
    public static class WebApiHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static HttpResponseMessage ReturnRawJson(string json)
        {
            HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            return message;
        }
    }
}