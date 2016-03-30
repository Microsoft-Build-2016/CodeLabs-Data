using System.Data.Entity;
using MSCorp.AdventureWorks.Core.Domain;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Order data context
    /// </summary>
    public class OrderDataContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderDataContext"/> class.
        /// </summary>
        public OrderDataContext(string connectionString) : base(connectionString)
        {
        }

        /// <summary>
        /// Gets or sets the orders.
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Gets or sets the order line.
        /// </summary>
        public DbSet<OrderLine> OrderLine { get; set; }
    }
}