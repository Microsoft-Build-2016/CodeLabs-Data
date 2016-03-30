using System;
using System.Collections.Generic;
using System.Linq;
using MSCorp.AdventureWorks.Core.Domain;

namespace MSCorp.AdventureWorks.Core.Import
{
    /// <summary>
    /// An import data scenario.
    /// </summary>
    public class ImportScenario
    {
        /// <summary>
        /// Gets or sets the products.
        /// </summary>
        public IEnumerable<Product> Products { get; set; }
        
        /// <summary>
        /// Gets or sets the customers.
        /// </summary>
        public IEnumerable<Customer> Customers { get; set; }
        
        /// <summary>
        /// Gets or sets the exchange rates.
        /// </summary>
        public IEnumerable<ExchangeRate> ExchangeRates { get; set; }
        
        /// <summary>
        /// Gets or sets the product images.
        /// </summary>
        public IEnumerable<ProductImage> ProductImages { get; set; }

        /// <summary>
        /// Products the images for the specified product.
        /// </summary>
        public IEnumerable<ProductImage> ProductImagesFor(string productCode)
        {
            return ProductImages.Where(image => image.ProductCode.Equals(productCode, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets or sets the product reviews.
        /// </summary>
        public IEnumerable<ProductReview> ProductReviews { get; set; }
    }
}