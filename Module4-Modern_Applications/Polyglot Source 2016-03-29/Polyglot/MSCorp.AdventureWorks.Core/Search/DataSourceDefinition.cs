using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSCorp.AdventureWorks.Core.Search
{
    public class DataSourceDefinition
    {
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public string Query { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        public string Type { get; set; }
    }
}
