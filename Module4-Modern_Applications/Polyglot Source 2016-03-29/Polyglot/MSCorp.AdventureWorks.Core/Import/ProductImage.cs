using System.Drawing;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Import
{
    /// <summary>
    /// A product to image map.
    /// </summary>
    public class ProductImage
    {
        /// <summary>
        /// Gets or sets the product code.
        /// </summary>
        public string ProductCode { get; set; }
        /// <summary>
        /// Gets or sets the image path.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        [JsonIgnore]
        public Image Image { get; set; }
    }
}