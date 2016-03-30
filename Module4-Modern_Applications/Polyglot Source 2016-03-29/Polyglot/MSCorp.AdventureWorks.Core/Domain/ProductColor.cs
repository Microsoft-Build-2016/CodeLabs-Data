using System.Drawing;
using Common;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents the color of the product
    /// </summary>
    public class ProductColor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductColor"/> class.
        /// </summary>
        public ProductColor(Color color)
        {
            Argument.CheckIfNull(color, "color");
            Color = color;
        }

        /// <summary>
        /// Gets the color of the product
        /// </summary>
        public Color Color { get; private set; }
    }
}