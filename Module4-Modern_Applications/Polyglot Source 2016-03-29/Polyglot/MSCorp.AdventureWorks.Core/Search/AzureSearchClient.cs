using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Common;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using MSCorp.AdventureWorks.Core.Configuration;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// A search client which communicates to the azure search service.
    /// </summary>
    public class AzureSearchClient<T> : ISearchClient, IDisposable, ISearchRepository<T> where T : class, ISearchIndexEntry
    {
        private readonly Uri _serviceUri;
        private readonly string _indexName;
        private readonly IAppSettings _appSettings;
        private readonly SearchServiceClient _searchClient;
        private readonly string _indexerName;
        private readonly string _dataSourceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSearchClient{T}"/> class.
        /// </summary>
        /// <param name="serviceUri">The service URI.</param>
        /// <param name="indexName">Name of the index.</param>
        /// <param name="apiKey">The API key.</param>
        /// <param name="appSettings">The App Settings Object</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        public AzureSearchClient(Uri serviceUri, string indexName, string apiKey, IAppSettings appSettings)
        {
            Argument.CheckIfNull(serviceUri, "serviceUri");
            Argument.CheckIfNullOrEmpty(indexName, "indexName");
            Argument.CheckIfNullOrEmpty(apiKey, "apiKey");

            indexName = indexName.ToLower(CultureInfo.CurrentCulture);

            _serviceUri = FormatUri(serviceUri);
            _indexName = indexName;
            _appSettings = appSettings;

            var searchCredentials = new SearchCredentials(apiKey);
            _searchClient = new SearchServiceClient(searchCredentials, serviceUri);

            _dataSourceName = $"{_indexName}DataSource".ToLower(CultureInfo.CurrentCulture);
            _indexerName = $"{_indexName}Indexer".ToLower(CultureInfo.CurrentCulture);
        }

        private static Uri FormatUri(Uri serviceUri)
        {
            string absoluteUri = serviceUri.AbsoluteUri;
            if (!absoluteUri.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                absoluteUri += "/";
            }

            return new Uri(absoluteUri);
        }

        /// <summary>
        /// Checks if index exists.
        /// </summary>
        public async Task<bool> CheckIfIndexExists()
        {
            return await _searchClient.Indexes.ExistsAsync(_indexName);
        }

        /// <summary>
        /// Creates the index.
        /// </summary>
        /// <param name="scoringProfiles">Provides the scoring profiles</param>
        /// <returns></returns>
        public async Task<bool> CreateIndex(params ScoringProfile[] scoringProfiles)
        {
            var index = BuildIndex(scoringProfiles);

            if (scoringProfiles?.Length > 0)
            {
                index.DefaultScoringProfile = scoringProfiles[0].Name;
            }

            if (await CheckIfIndexExists())
            {
                return true;
            }

            var response = await _searchClient.Indexes.CreateAsync(index);
            return response.StatusCode == HttpStatusCode.OK;
        }

        public async Task RunIndexer()
        {
            if (await _searchClient.Indexers.ExistsAsync(_indexerName))
            {
                await _searchClient.Indexers.RunAsync(_indexerName);
            }
        }


        public Index BuildIndex(params ScoringProfile[] scoringProfiles)
        {
            Index index = SearchSchemaGenerator.CreateSchemaWithScoring<T>(scoringProfiles);
            index.Name = _indexName;
            return index;
        }

        public async Task<bool> CreateIndexer()
        {
            if (!await _searchClient.DataSources.ExistsAsync(_dataSourceName))
            {
                var dataSource = BuildDataSource();
                if (dataSource != null)
                {
                    await _searchClient.DataSources.CreateAsync(dataSource);
                }
            }

            if (!await _searchClient.Indexers.ExistsAsync(_indexerName))
            {
                var indexer = BuildIndexer();
                await _searchClient.Indexers.CreateAsync(indexer);
            }
            return true;
        }

        public Indexer BuildIndexer()
        {
            var indexer = new Indexer()
            {
                Name = _indexerName,
                DataSourceName = _dataSourceName,
                TargetIndexName = _indexName,
                Schedule = new IndexingSchedule(TimeSpan.FromMinutes(5), DateTimeOffset.Now.AddYears(-1))
            };
            return indexer;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
        public DataSource BuildDataSource()
        {
            var file = Path.Combine(Path.Combine(_appSettings.JsonPath, "DataSources"), $"{_indexName}.json");
            if (File.Exists(file))
            {
                var dataSourceDefinition = JsonConvert.DeserializeObject<DataSourceDefinition>(File.ReadAllText(file));
                var dataSource = new DataSource
                {
                    Container = new DataContainer
                    {
                        Name = dataSourceDefinition.CollectionName,
                        Query = dataSourceDefinition.Query
                    },
                    Credentials = new DataSourceCredentials
                    {
                        ConnectionString =
                            $"{_appSettings.DocumentDbConnectionString}Database={dataSourceDefinition.DatabaseName};"
                    },
                    DataChangeDetectionPolicy = new HighWaterMarkChangeDetectionPolicy("_ts"),
                    Name = _dataSourceName,
                    Type = dataSourceDefinition.Type
                };
                return dataSource;
            }
            return null;
        }

        public string IndexName => _indexName;

        /// <summary>
        /// Posts the documents.
        /// </summary>
        public async Task<bool> PostDocumentsAsync(params T[] documents)
        {
            return await PostDocumentsAsync(new List<T>(documents));
        }

        /// <summary>
        /// Posts the documents.
        /// </summary>
        public bool PostDocuments(params T[] documents)
        {
            return PostDocumentsAsync(new List<T>(documents)).Result;
        }

        /// <summary>
        /// Posts the documents.
        /// </summary>
        /// <param name="documents">The documents to post.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// The document(s) failed to upload.
        /// </exception>
        public async Task<bool> PostDocumentsAsync(IEnumerable<T> documents)
        {
            using (var indexClient = _searchClient.Indexes.GetClient(_indexName))
            {
                var response = await indexClient.Documents.IndexAsync(IndexBatch.Create(documents.Select(doc => IndexAction.Create(doc))));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Results.Any(i => !i.Succeeded))
                    {
                        string parameters = string.Join(",", response.Results.Where(i => !i.Succeeded).Select(i => (string)i.Key));
                        throw new InvalidOperationException("The documents with the following keys failed to upload: {0}".FormatWith(parameters));
                    }
                    return true;
                }
                throw new InvalidOperationException("Documents failed to upload: {0} \r\n".FormatWith((int)response.StatusCode));
            }
        }

        /// <summary>
        /// Merges the specified document.
        /// </summary>
        public async Task<bool> MergeDocumentAsync(T document)
        {
            using (var indexClient = _searchClient.Indexes.GetClient(_indexName))
            {
                var response = await indexClient.Documents.IndexAsync(IndexBatch.Create(IndexAction.Create(IndexActionType.Merge, document)));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Results.Any(i => !i.Succeeded))
                    {
                        string parameters = string.Join(",", response.Results.Where(i => !i.Succeeded).Select(i => (string)i.Key));
                        throw new InvalidOperationException("The documents with the following keys failed to merge: {0}".FormatWith(parameters));
                    }
                    return true;
                }
                throw new InvalidOperationException("Documents failed to merge: {0} \r\n".FormatWith((int)response.StatusCode));
            }
        }

        /// <summary>
        /// Merges the specified document.
        /// </summary>
        public async Task<bool> MergeDocumentAsync(Document document)
        {
            using (var indexClient = _searchClient.Indexes.GetClient(_indexName))
            {
                var response = await indexClient.Documents.IndexAsync(IndexBatch.Create(IndexAction.Create(IndexActionType.Merge, document)));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Results.Any(i => !i.Succeeded))
                    {
                        string parameters = string.Join(",", response.Results.Where(i => !i.Succeeded).Select(i => (string)i.Key));
                        throw new InvalidOperationException("The documents with the following keys failed to merge: {0}".FormatWith(parameters));
                    }
                    return true;
                }
                throw new InvalidOperationException("Documents failed to merge: {0} \r\n".FormatWith((int)response.StatusCode));
            }
        }

        /// <summary>
        /// Deletes the documents.
        /// </summary>
        public async Task<bool> DeleteDocumentsAsync(params T[] documents)
        {
            return await DeleteDocuments(new List<T>(documents));
        }

        /// <summary>
        /// Deletes the documents.
        /// </summary>
        /// <param name="documents">The documents to delete.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// The document(s) failed to delete.
        /// </exception>
        public async Task<bool> DeleteDocuments(IEnumerable<T> documents)
        {
            using (var indexClient = _searchClient.Indexes.GetClient(_indexName))
            {
                var response = await indexClient.Documents.IndexAsync(IndexBatch.Create(documents.Select(doc => IndexAction.Create(IndexActionType.Delete, doc))));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Results.Any(i => !i.Succeeded))
                    {
                        string parameters = string.Join(",", response.Results.Where(i => !i.Succeeded).Select(i => (string)i.Key));
                        throw new InvalidOperationException("The documents with the following keys failed to delete: {0}".FormatWith(parameters));
                    }
                    return true;
                }
                throw new InvalidOperationException("Documents failed to delete: {0} \r\n".FormatWith((int)response.StatusCode));
            }
        }

        /// <summary>
        /// Executes a suggest search on the search api
        /// </summary>
        /// <param name="query">Query String</param>
        /// <param name="suggestParameters">Parameters for the suggest, can be null</param>
        /// <returns>Dynamic object with results</returns>
        public async Task<dynamic> ExecuteSuggest(string query, SuggestParameters suggestParameters)
        {
            using (var indexClient = _searchClient.Indexes.GetClient(_indexName))
            {
                var suggestResponse = await indexClient.Documents.SuggestAsync(query, SearchSchemaGenerator.SuggesterName, suggestParameters);
                return new { suggestResponse.Results };
            }
        }

        /// <summary>
        /// Executes a search on the search api
        /// </summary>
        /// <param name="query">Query String</param>
        /// <returns>Dynamic object with results and facets</returns>
        public async Task<dynamic> ExecuteSearch(string query)
        {
            return await ExecuteSearch(query, new SearchParameters());
        }

        /// <summary>
        /// Executes a search on the search api
        /// </summary>
        /// <param name="query">Query String</param>
        /// <param name="searchParameters">Parameters for the search, cannot be null</param>
        /// <returns>Dynamic object with results and facets</returns>
        public async Task<dynamic> ExecuteSearch(string query, SearchParameters searchParameters)
        {
            using (var indexClient = _searchClient.Indexes.GetClient(_indexName))
            {
                var searchResponse = await indexClient.Documents.SearchAsync(query, searchParameters);
                return new { searchResponse.Results, searchResponse.Facets };
            }
        }

        /// <summary>
        /// Deletes the index.
        /// </summary>
        public async Task<bool> DeleteIndex()
        {
            if (await _searchClient.Indexes.ExistsAsync(_indexName))
            {
                var response = await _searchClient.Indexes.DeleteAsync(_indexName);
                return response.StatusCode == HttpStatusCode.OK;
            }
            return true;
        }

        /// <summary>
        /// Gets the schema.
        /// </summary>
        public async Task<string> LoadSchema()
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", _searchClient.Credentials.ApiKey);
                Uri requestUri = new Uri(_serviceUri + "indexes/" + IndexName + "/?api-version=" + _searchClient.ApiVersion);
                HttpResponseMessage response = await client.GetAsync(requestUri).ConfigureAwait(false);
                return await response.Content.ReadAsStringAsync();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _searchClient?.Dispose();
            }
        }
    }
}