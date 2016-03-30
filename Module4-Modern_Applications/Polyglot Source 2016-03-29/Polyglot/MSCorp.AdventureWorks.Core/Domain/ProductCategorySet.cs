using System.Collections.Generic;
using Common;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents a product category and all the related Product types underneath that category.
    /// E.g.  -Mens Clothing (Category)
    ///             -Jacket (Type)
    ///             -Pants  (Type)
    ///             -Socks  (Type)
    /// </summary>
    public class ProductCategorySet
    {
        private readonly string _category;
        private readonly IEnumerable<string> _nestedTypes;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductCategorySet"/> class.
        /// </summary>
        public ProductCategorySet(string category, IEnumerable<string> nestedTypes)
        {
            Argument.CheckIfNull(category, "category");
            Argument.CheckIfNull(nestedTypes, "nestedTypes");
            _category = category;
            _nestedTypes = nestedTypes;
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public string Category
        {
            get { return _category; }
        }

        /// <summary>
        /// Gets the nested types.
        /// </summary>
        public IEnumerable<string> NestedTypes
        {
            get { return _nestedTypes; }
        }
    }
}