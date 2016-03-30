using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// A search service that can have its indexes updated.
    /// </summary>
    public interface ISearchRepository<T> where T : ISearchIndexEntry
    {
        /// <summary>
        /// Posts the documents.
        /// </summary>
        Task<bool> PostDocumentsAsync(params T[] documents);

        /// <summary>
        /// Posts the documents.
        /// </summary>
        /// <param name="documents">The documents to post.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">
        /// The document(s) failed to upload.
        /// </exception>
        Task<bool> PostDocumentsAsync(IEnumerable<T> documents);

        /// <summary>
        /// Deletes the documents.
        /// </summary>
        Task<bool> DeleteDocumentsAsync(params T[] documents);

        /// <summary>
        /// Posts the documents.
        /// </summary>
        bool PostDocuments(params T[] documents);

        /// <summary>
        /// Merges the specified document.
        /// </summary>
        Task<bool> MergeDocumentAsync(T document);

        /// <summary>
        /// Merges the specified document.
        /// </summary>
        Task<bool> MergeDocumentAsync(Document document);
    }
}