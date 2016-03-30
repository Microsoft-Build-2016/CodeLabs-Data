using MSCorp.AdventureWorks.Core.Domain;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Repository to save down <see cref="Order"/> and related <see cref="OrderLine"/>
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Loads the order with the specified order number.
        /// </summary>
        Order Load(int orderId);
        
        /// <summary>
        /// Removes all the lines from the database
        /// </summary>
        void Reset();
        
        /// <summary>
        /// Saves the specified order into the database
        /// </summary>
        void Save(Order order);
    }
}