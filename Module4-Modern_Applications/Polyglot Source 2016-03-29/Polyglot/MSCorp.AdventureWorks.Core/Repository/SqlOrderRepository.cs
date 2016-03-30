using System.Data.Entity;
using System.Linq;
using Common;
using MSCorp.AdventureWorks.Core.Domain;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// The order repository to manage the percistance of orders.
    /// </summary>
    public class SqlOrderRepository : IOrderRepository
    {
        private readonly OrderDataContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlOrderRepository"/> class.
        /// </summary>
        public SqlOrderRepository(OrderDataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Loads the order with the specified order number.
        /// </summary>
        public Order Load(int orderId)
        {
            Order order = _context.Orders.Include(o => o.Lines).SingleOrDefault(o => o.Id == orderId);
            return order;
        }

        /// <summary>
        /// Saves the specified order into the database
        /// </summary>
        public void Save(Order order)
        {
            Argument.CheckIfNull(order, "order");
            _context.Orders.Add(order);
            foreach (OrderLine line in order.Lines)
            {
                _context.OrderLine.Add(line);
            }
        }


        public void Reset()
        {
            _context.OrderLine.Clear();
            _context.Orders.Clear();
            _context.SaveChanges();
        }
    }
}