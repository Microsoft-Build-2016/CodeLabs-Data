using System;
using System.Threading;
using System.Threading.Tasks;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Unit of work interface
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the orders repository
        /// </summary>
        IOrderRepository Orders { get; }

        /// <summary>
        /// Commits this unit of work.
        /// </summary>
        Task Commit();

        Task ExecuteLoad(CancellationToken cancelToken);
    }
}