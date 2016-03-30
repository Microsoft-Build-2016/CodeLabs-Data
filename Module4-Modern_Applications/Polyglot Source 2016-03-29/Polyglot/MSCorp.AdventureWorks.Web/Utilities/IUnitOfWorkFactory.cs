using MSCorp.AdventureWorks.Core.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSCorp.AdventureWorks.Web.Utilities
{
    public interface IUnitOfWorkFactory
    {
        IUnitOfWork GenerateUnitOfWork();
    }
}
