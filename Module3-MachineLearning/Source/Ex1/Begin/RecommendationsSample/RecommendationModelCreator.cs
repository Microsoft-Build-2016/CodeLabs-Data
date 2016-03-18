namespace RecommendationsSampleApp
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Xml;

    public class RecommendationModelCreator
    {
        private HttpClient _httpClient;
        private const string RootUri = "https://api.datamarket.azure.com/amla/recommendations/v3/";

        public RecommendationModelCreator(string email, string accountKey)
        {
            _httpClient = new HttpClient();
            var pass = GeneratePass(email, accountKey);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", pass);
            _httpClient.BaseAddress = new Uri(RootUri);
        }

        /// <summary>
        /// create the model with the given name.
        /// </summary>
        /// <returns>The model id</returns>
        public string CreateModel(string modelName)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, String.Format(Uris.CreateModelUrl, modelName));
            var response = _httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(String.Format("Error {0}: Failed to create model {1}, \n reason {2}",
                    response.StatusCode, modelName, ExtractErrorInfo(response)));
            }

            // process response if success
            var modelId = string.Empty;

            var node = XmlUtils.ExtractXmlElement(response.Content.ReadAsStreamAsync().Result, "//a:entry/a:content/m:properties/d:Id");
            if (node != null)
                modelId = node.InnerText;

            return modelId;
        }

        public void UploadCatalog(string modelId, string catalogFilePath)
        {
            ImportFile(modelId, catalogFilePath, Uris.ImportCatalog);
        }

        public void UploadUsage(string modelId, string usageFilePath)
        {
            ImportFile(modelId, usageFilePath, Uris.ImportUsage);
        }

        /// <summary>
        /// Trigger a recommendation build for the given model.
        /// Note: unless configured otherwise the u2i (user to item/user based) recommendations are enabled too.
        /// </summary>
        /// <param name="modelId">the model id</param>
        /// <param name="buildDescription">a description for the build</param>
        /// <returns>the id of the triggered build</returns>
        public string Build(string modelId, string buildDescription)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, String.Format(Uris.BuildModel, modelId, buildDescription));

            // setup the build parameters here we use a simple build without feature usage, for a complete list and 
            // explanation check the API document AT
            // http://azure.microsoft.com/en-us/documentation/articles/machine-learning-recommendation-api-documentation/#1113-recommendation-build-parameters
            request.Content = new StringContent("<BuildParametersList>" +
                                                "<NumberOfModelIterations>10</NumberOfModelIterations>" +
                                                "<NumberOfModelDimensions>20</NumberOfModelDimensions>" +
                                                "<ItemCutOffLowerBound>1</ItemCutOffLowerBound>" +
                                                "<EnableModelingInsights>false</EnableModelingInsights>" +
                                                "<EnableU2I>false</EnableU2I>" +
                                                "<UseFeaturesInModel>false</UseFeaturesInModel>" +
                                                "<ModelingFeatureList></ModelingFeatureList>" +
                                                "<AllowColdItemPlacement>true</AllowColdItemPlacement>" +
                                                "<EnableFeatureCorrelation>false</EnableFeatureCorrelation>" +
                                                "<ReasoningFeatureList></ReasoningFeatureList>" +
                                                "</BuildParametersList>", Encoding.UTF8, "Application/xml");
            var response = _httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(String.Format("Error {0}: Failed to start build for model {1}, \n reason {2}",
                    response.StatusCode, modelId, ExtractErrorInfo(response)));
            }

            // process response if success
            var buildId = string.Empty;
            var node = XmlUtils.ExtractXmlElement(response.Content.ReadAsStreamAsync().Result, "//a:entry/a:content/m:properties/d:Id");
            if (node != null)
                buildId = node.InnerText;

            return buildId;
        }

        /// <summary>
        /// Retrieve the build status for the given build
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="buildId"></param>
        /// <returns></returns>
        public BuildStatus GetBuildStatus(string modelId, string buildId)
        {

            var request = new HttpRequestMessage(HttpMethod.Get, String.Format(Uris.BuildStatuses, modelId, false));
            var response = _httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(String.Format("Error {0}: Failed to retrieve build for status for model {1} and build id {2}, \n reason {3}",
                    response.StatusCode, modelId, buildId, ExtractErrorInfo(response)));
            }
            string buildStatusStr = null;
            var node = XmlUtils.ExtractXmlElement(response.Content.ReadAsStreamAsync().Result, string.Format("//a:entry/a:content/m:properties[d:BuildId='{0}']/d:Status", buildId));
            if (node != null)
                buildStatusStr = node.InnerText;

            BuildStatus buildStatus;
            if (!Enum.TryParse(buildStatusStr, true, out buildStatus))
            {
                throw new Exception(string.Format("Failed to parse build status for value {0} of build {1} for model {2}", buildStatusStr, buildId, modelId));
            }

            return buildStatus;
        }

        /// <summary>
        /// Update model information
        /// </summary>
        /// <param name="modelId">the id of the model</param>
        /// <param name="description">the model description (optional)</param>
        /// <param name="activeBuildId">the id of the build to be active (optional)</param>
        public void UpdateModel(string modelId, string description, string activeBuildId)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, String.Format(Uris.UpdateModel, modelId));

            var sb = new StringBuilder("<ModelUpdateParams xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");

            if (!string.IsNullOrEmpty(description))
            {
                sb.AppendFormat("<Description>{0}</Description>", description);
            }

            if (!string.IsNullOrEmpty(activeBuildId))
            {
                sb.AppendFormat("<ActiveBuildId>{0}</ActiveBuildId>", activeBuildId);
            }

            sb.Append("</ModelUpdateParams>");

            request.Content = new StreamContent(new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString())));
            request.Content.Headers.Add("Content-Type", "Application/xml");

            var response = _httpClient.SendAsync(request).Result;

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(String.Format("Error {0}: Failed to update model for model {1}, \n reason {2}",
                    response.StatusCode, modelId, ExtractErrorInfo(response)));
            }
        }

        /// <summary>
        /// Import the given file (catalog/usage) to the given model. 
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="filePath"></param>
        /// <param name="importUri"></param>
        /// <returns></returns>
        private ImportReport ImportFile(string modelId, string filePath, string importUri)
        {
            var filestream = new FileStream(filePath, FileMode.Open);
            var fileName = Path.GetFileName(filePath);
            var request = new HttpRequestMessage(HttpMethod.Post, String.Format(importUri, modelId, fileName));

            request.Content = new StreamContent(filestream);
            var response = _httpClient.SendAsync(request).Result;


            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    String.Format("Error {0}: Failed to import file {1}, for model {2} \n reason {3}",
                        response.StatusCode, filePath, modelId, ExtractErrorInfo(response)));
            }

            //process response if success
            var nodeList = XmlUtils.ExtractXmlElementList(response.Content.ReadAsStreamAsync().Result,
                "//a:entry/a:content/m:properties/*");

            var report = new ImportReport { Info = fileName };
            foreach (XmlNode node in nodeList)
            {
                if ("LineCount".Equals(node.LocalName))
                {
                    report.LineCount = int.Parse(node.InnerText);
                }
                if ("ErrorCount".Equals(node.LocalName))
                {
                    report.ErrorCount = int.Parse(node.InnerText);
                }
            }
            return report;
        }

        /// <summary>
        /// Generate the key to allow accessing DM API
        /// </summary>
        /// <param name="email">the user email</param>
        /// <param name="accountKey">the user account key</param>
        /// <returns></returns>
        private string GeneratePass(string email, string accountKey)
        {
            var byteArray = Encoding.ASCII.GetBytes(string.Format("{0}:{1}", email, accountKey));
            return Convert.ToBase64String(byteArray);
        }

        /// <summary>
        /// Extract error message from the httpResponse, (reason phrase + body)
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string ExtractErrorInfo(HttpResponseMessage response)
        {
            //DM send the error message in body so need to extract the info from there
            string detailedReason = null;
            if (response.Content != null)
            {
                detailedReason = response.Content.ReadAsStringAsync().Result;
            }
            var errorMsg = detailedReason == null ? response.ReasonPhrase : response.ReasonPhrase + "->" + detailedReason;
            return errorMsg;

        }
    }

    /// <summary>
    /// represent the build status
    /// </summary>
    public enum BuildStatus
    {
        Create,
        Queued,
        Building,
        Success,
        Error,
        Cancelling,
        Cancelled
    }

    /// <summary>
    /// Utility class holding the result of import operation
    /// </summary>
    internal class ImportReport
    {
        public string Info { get; set; }
        public int ErrorCount { get; set; }
        public int LineCount { get; set; }

        public override string ToString()
        {
            return string.Format("successfully imported {0}/{1} lines for {2}", LineCount - ErrorCount, LineCount,
                Info);
        }
    }

    internal class XmlUtils
    {
        /// <summary>
        /// extract a single xml node from the given stream, given by the xPath
        /// </summary>
        /// <param name="xmlStream"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        internal static XmlNode ExtractXmlElement(Stream xmlStream, string xPath)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlStream);
            //Create namespace manager
            var nsmgr = CreateNamespaceManager(xmlDoc);

            var node = xmlDoc.SelectSingleNode(xPath, nsmgr);
            return node;
        }

        private static XmlNamespaceManager CreateNamespaceManager(XmlDocument xmlDoc)
        {
            var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
            nsmgr.AddNamespace("a", "http://www.w3.org/2005/Atom");
            nsmgr.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            nsmgr.AddNamespace("d", "http://schemas.microsoft.com/ado/2007/08/dataservices");
            return nsmgr;
        }

        /// <summary>
        /// extract the xml nodes from the given stream, given by the xPath
        /// </summary>
        /// <param name="xmlStream"></param>
        /// <param name="xPath"></param>
        /// <returns></returns>
        internal static XmlNodeList ExtractXmlElementList(Stream xmlStream, string xPath)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(xmlStream);
            var nsmgr = CreateNamespaceManager(xmlDoc);
            var nodeList = xmlDoc.SelectNodes(xPath, nsmgr);
            return nodeList;
        }       
    }

    /// <summary>
    /// holds the API uri.
    /// </summary>
    internal static class Uris
    {

        public const string CreateModelUrl = "CreateModel?modelName=%27{0}%27&apiVersion=%271.0%27";

        public const string ImportCatalog =
            "ImportCatalogFile?modelId=%27{0}%27&filename=%27{1}%27&apiVersion=%271.0%27";


        public const string ImportUsage =
            "ImportUsageFile?modelId=%27{0}%27&filename=%27{1}%27&apiVersion=%271.0%27";

        public const string BuildModel =
            "BuildModel?modelId=%27{0}%27&userDescription=%27{1}%27&apiVersion=%271.0%27";

        public const string BuildStatuses = "GetModelBuildsStatus?modelId=%27{0}%27&onlyLastBuild={1}&apiVersion=%271.0%27";

        public const string GetRecommendation =
            "ItemRecommend?modelId=%27{0}%27&itemIds=%27{1}%27&numberOfResults={2}&includeMetadata={3}&apiVersion=%271.0%27";

        public const string GetUserRecommendation =
            "UserRecommend?modelId=%27{0}%27&userId=%27{1}%27&numberOfResults={2}&includeMetadata={3}&apiVersion=%271.0%27";

        public const string GetFbtRecommendation =
            "ItemFbtRecommend?modelId=%27{0}%27&itemId=%27{1}%27&numberOfResults={2}&minimalScore={3}&includeMetadata={4}&apiVersion=%271.0%27&buildId={5}";

        public const string UpdateModel = "UpdateModel?id=%27{0}%27&apiVersion=%271.0%27";

    }
}
