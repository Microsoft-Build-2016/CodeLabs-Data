// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace PartsUnlimited.TextAnalytics
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class TextAnalyticsService : ITextAnalyticsService
    {
        private const string ServiceBaseUri = "https://api.datamarket.azure.com/";
        private readonly string AccountKey;

        public TextAnalyticsService(string accountKey)
        {
            if (string.IsNullOrWhiteSpace(accountKey))
            {
                throw new ArgumentNullException(nameof(accountKey));
            }
            this.AccountKey = accountKey;
        }

        public async Task<SentimentResult> GetSentiment(string inputTextEncoded)
        {
            var httpClient = ConstructTextAnalyticsHttpClient();

            var sentimentRequest = "data.ashx/amla/text-analytics/v1/GetSentiment?Text=" + inputTextEncoded;

            var response = await httpClient.GetAsync(sentimentRequest);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Did not receive a success status code");
            }
            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<SentimentResult>(content);
        }

        public async Task<KeyPhraseResult> GetKeyPhrases(string inputTextEncoded)
        {
            var httpClient = ConstructTextAnalyticsHttpClient();
            var keyPhrasesRequest = "data.ashx/amla/text-analytics/v1/GetKeyPhrases?Text=" + inputTextEncoded;
            
            var response = await httpClient.GetAsync(keyPhrasesRequest);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Did not receive a success status code");
            }

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<KeyPhraseResult>(content);
        }

        private HttpClient ConstructTextAnalyticsHttpClient()
        {
            var httpClient = new HttpClient { BaseAddress = new Uri(ServiceBaseUri) };
            var creds = "AccountKey:" + this.AccountKey;
            var authorizationHeader = "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(creds));

            httpClient.DefaultRequestHeaders.Add("Authorization", authorizationHeader);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
    }
}