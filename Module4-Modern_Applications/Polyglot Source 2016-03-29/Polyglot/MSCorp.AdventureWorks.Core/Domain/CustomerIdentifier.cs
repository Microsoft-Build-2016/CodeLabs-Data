using System.Runtime.CompilerServices;
using Common;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents a customer
    /// </summary>
    public class CustomerIdentifier 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerIdentifier"/> class.
        /// </summary>
        public CustomerIdentifier(string name, int customerKey)
        {
            Argument.CheckIfNull(name, "name");
            Name = name;
            CustomerKey = customerKey;
        }

        /// <summary>
        /// Gets the name of the customer.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The customer unique key
        /// </summary>
        public int CustomerKey { get; set; }
    }
}
