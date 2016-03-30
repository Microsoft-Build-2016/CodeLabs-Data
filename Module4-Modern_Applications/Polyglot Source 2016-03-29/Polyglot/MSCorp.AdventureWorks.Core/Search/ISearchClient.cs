using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// A client that connects to a search service.
    /// </summary>
    public interface ISearchClient
    {
        /// <summary>
        /// Checks if index exists.
        /// </summary>
        Task<bool> CheckIfIndexExists();

        /// <summary>
        /// Creates the index.
        /// </summary>
        /// <param name="scoringProfiles">Provides scoring profile to assist schema.</param>
        /// <returns></returns>
        Task<bool> CreateIndex(params ScoringProfile[] scoringProfiles);

        Task<dynamic> ExecuteSuggest(string query, SuggestParameters suggestParameters);

        Task<dynamic> ExecuteSearch(string query, SearchParameters searchParameters);

        Task<dynamic> ExecuteSearch(string query);

        /// <summary>
        /// Deletes the index.
        /// </summary>
        Task<bool> DeleteIndex();

        /// <summary>
        /// Gets the schema.
        /// </summary>
        Task<string> LoadSchema();
    }
}