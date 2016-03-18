namespace PartsUnlimited.TextAnalytics
{    
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using Newtonsoft.Json;

    public class TextAnalyticsService : ITextAnalyticsService
    {
        private HttpClient httpClient = new HttpClient();
        private const string ServiceBaseUri = "https://api.datamarket.azure.com/";

        public TextAnalyticsService(string accountKey)
        {
            httpClient.BaseAddress = new Uri(ServiceBaseUri);
            var creds = "AccountKey:" + accountKey;
            var authorizationHeader = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(creds));

            httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public SentimentResult GetSentiment(string inputTextEncoded)
        {
            var sentimentRequest = "data.ashx/amla/text-analytics/v1/GetSentiment?Text=" + inputTextEncoded;
            var responseTask = httpClient.GetAsync(sentimentRequest);
            responseTask.Wait();

            var response = responseTask.Result;
            var contentTask = response.Content.ReadAsStringAsync();
            contentTask.Wait();

            var content = contentTask.Result;
            if (!response.IsSuccessStatusCode)
            {
                // TODO handle error response
                return new SentimentResult { Score = 1 };
            }

            return JsonConvert.DeserializeObject<SentimentResult>(content);
        }

        public SentimentResult GetKeyPhrases(string inputTextEncoded)
        {
            var sentimentRequest = "data.ashx/amla/text-analytics/v1/GetKeyPhrases?Text=" + inputTextEncoded;
            var responseTask = httpClient.GetAsync(sentimentRequest);
            responseTask.Wait();

            var response = responseTask.Result;
            var contentTask = response.Content.ReadAsStringAsync();
            contentTask.Wait();

            var content = contentTask.Result;
            if (!response.IsSuccessStatusCode)
            {
                // TODO handle error response
                return new SentimentResult { Score = 1 };
            }

            return JsonConvert.DeserializeObject<SentimentResult>(content);
        }
    }
}
