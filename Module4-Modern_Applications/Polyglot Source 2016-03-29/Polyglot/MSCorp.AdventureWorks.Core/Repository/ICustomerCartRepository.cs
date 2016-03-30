using System.Threading.Tasks;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Responsible for loading and saving the customer cart.
    /// </summary>
    public interface ICustomerCartRepository
    {
        /// <summary>
        /// Saves the specified order as a shopping cart.
        /// </summary>
        Task<bool> Save(int customerKey, string order);

        /// <summary>
        /// Loads the specified order for the given customer identifier. 
        /// </summary>
        Task<string> Load(int customerKey);

        /// <summary>
        /// Deletes the cart belonging to the specified customer key.
        /// </summary>
        Task<bool> Delete(int customerKey);
    }
}