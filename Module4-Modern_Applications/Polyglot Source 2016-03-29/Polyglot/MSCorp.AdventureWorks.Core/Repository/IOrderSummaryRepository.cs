using System.Collections.Generic;
using System.Threading.Tasks;
using MSCorp.AdventureWorks.Core.Domain;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Represents a order summary repository
    /// </summary>
    public interface IOrderSummaryRepository
    {
        /// <summary>
        /// Saves the specified summary.
        /// </summary>
        Task<SaveResponse> Save(OrderSummary summary);

        /// <summary>
        /// Loads the summaries by customer.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<string> LoadSummariesByCustomerJson(string customerCode);
    }
}