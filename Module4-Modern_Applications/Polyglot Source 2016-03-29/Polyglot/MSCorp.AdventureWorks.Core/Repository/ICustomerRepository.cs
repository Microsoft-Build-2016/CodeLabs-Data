using System.Collections.Generic;
using System.Threading.Tasks;
using MSCorp.AdventureWorks.Core.Domain;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// A repository for currency.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Saves the specified customer.
        /// </summary>
        Task<SaveResponse> Save(Customer customer);

        /// <summary>
        /// Loads the customer.
        /// </summary>
        Task<Customer> LoadCustomer(Identifier identifier);

        /// <summary>
        /// Loads all customers.
        /// </summary>
        IEnumerable<Customer> LoadAllCustomers();
    }
}