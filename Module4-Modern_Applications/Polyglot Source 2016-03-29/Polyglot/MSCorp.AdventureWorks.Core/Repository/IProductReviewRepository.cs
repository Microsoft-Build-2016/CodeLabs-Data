using System.Collections.Generic;
using System.Threading.Tasks;
using MSCorp.AdventureWorks.Core.Domain;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Represents a Product repository
    /// </summary>
    public interface IProductReviewRepository
    {
        /// <summary>
        /// Saves the specified product review.
        /// </summary>
        Task<SaveResponse> Save(ProductReview review);

        /// <summary>
        /// Loads all reviews for the specified product.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IEnumerable<ProductReview>> LoadReviews(string productCode);

        /// <summary>
        /// Loads the reviews.
        /// </summary>
        Task<string> LoadReviewsJson(string productCode);

        /// <summary>
        /// Adds the review response.
        /// </summary>
        /// <param name="reviewId">The review identifier.</param>
        /// <param name="reviewResponse">The review response.</param>
        /// <returns></returns>
        Task<string> AddReviewResponse(string reviewId, ProductReviewResponse reviewResponse);

        /// <summary>
        /// Loads the reviews.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IEnumerable<ProductReviewEntry>> LoadReviewsForIndexing();

        /// <summary>
        /// Deletes the specified entry.
        /// </summary>
        Task Delete(ProductReviewEntry entry);
    }
}