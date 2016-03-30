using Common;
using MSCorp.AdventureWorks.Core.Search;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents a product size
    /// </summary>
    public class ProductSize : IRefiner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSize" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="sortValue">The relative weighting of the size.  This is used to sort (e.g. S=1, M=2, L=3).</param>
        [JsonConstructor]
        public ProductSize(string name, int sortValue)
        {
            Argument.CheckIfNullOrEmpty(name, "name");
            Name = name;
            SortValue = sortValue;
        }

        /// <summary>
        /// Gets the size of the product.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public int SortValue { get; private set; }
    }
}