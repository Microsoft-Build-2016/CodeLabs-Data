using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSCorp.AdventureWorks.Core.Configuration
{
    public interface IAppSettings
    {
        string DocumentDbConnectionString { get; }
        string JsonPath { get; }
    }
}
