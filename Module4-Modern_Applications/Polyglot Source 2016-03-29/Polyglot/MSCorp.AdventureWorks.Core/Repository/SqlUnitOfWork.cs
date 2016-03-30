using System.Threading;
using System.Threading.Tasks;
using Common;
using System;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Represents a unit of work across a SQL Database
    /// </summary>
    public class SqlUnitOfWork : IUnitOfWork
    {
        private readonly OrderDataContext _context;
        private IOrderRepository _orderRepository;
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlUnitOfWork"/> class.
        /// </summary>
        public SqlUnitOfWork(OrderDataContext context)
        {
            Argument.CheckIfNull(context, "context");
            _context = context;
        }

        /// <summary>
        /// Gets the orders.
        /// </summary>
        public IOrderRepository Orders
        {
            get { return _orderRepository ?? (_orderRepository = new SqlOrderRepository(_context)); }
        }

        /// <summary>
        /// Commits this instance.
        /// </summary>
        public Task Commit()
        {
            return _context.SaveChangesAsync();
        }

        /// <summary>
        /// Executes the load.
        /// </summary>
        /// <param name="cancelToken">The cancel token.</param>
        public Task ExecuteLoad(CancellationToken cancelToken)
        {
            string query = @"
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_NAME = 'loadTable')
BEGIN
    DROP TABLE loadTable
END

SET NOCOUNT ON
CREATE TABLE loadTable (c1 int PRIMARY KEY,c2 char(100),c3 char(100),c4 char(100),c5 char(100))
declare @i as bigint
set @i = 1
while @i < 50000
begin
insert into loadTable values(@i, CONVERT(varchar(max), NEWID()),'def','ghi','jkl')
set @i = @i + 1
print @i
end
SET NOCOUNT OFF

DROP TABLE loadTable
";
            Task<int> commandAsync = _context.Database.ExecuteSqlCommandAsync(query, cancelToken);
            return commandAsync;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}