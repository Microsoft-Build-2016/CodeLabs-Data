using System;
using Microsoft.Azure.Documents;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Product Review response.
    /// </summary>
    public class ProductReviewEntry : Document
    {
        /// <summary>
        /// Gets or sets the response text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets or sets the name of the customer.
        /// </summary>
        public string CustomerName { get; set; }
        
        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        public string CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the product code.
        /// </summary>
        public string ProductCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [is for index].
        /// </summary>
        public bool IsForIndex { get; set; }

        /// <summary>
        /// Gets or sets the rating.
        /// </summary>
        public int Rating { get; set; }
    }
}