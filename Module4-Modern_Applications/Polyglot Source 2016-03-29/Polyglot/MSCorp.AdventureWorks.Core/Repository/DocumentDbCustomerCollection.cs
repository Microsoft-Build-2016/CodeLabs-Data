using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSCorp.AdventureWorks.Core.Repository
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class DocumentDbCustomerCollection
    {
        private readonly string _name;

        public DocumentDbCustomerCollection(string name)
        {
            _name = name;
            Range = new List<string>();
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        public List<string> Range
        {
            get;
            private set;
        }

        public bool InRange(string rangeName)
        {
            return
                Range.Any(
                    e =>
                        e.ToLower(CultureInfo.CurrentCulture) ==
                        rangeName.Substring(0, 1).ToLower(CultureInfo.CurrentCulture));
        }


    }
}
