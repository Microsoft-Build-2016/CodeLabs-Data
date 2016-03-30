using MSCorp.AdventureWorks.Core.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSCorp.AdventureWorks.Web.Utilities
{
    public class SqlUnitOfWorkFactory : IUnitOfWorkFactory
    {
        private readonly string _connectionString;

        public SqlUnitOfWorkFactory(string connectionString)
        {
            _connectionString = connectionString;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public IUnitOfWork GenerateUnitOfWork()
        {
            var context = new OrderDataContext(_connectionString);
            return new SqlUnitOfWork(context);
        }
    }
}
