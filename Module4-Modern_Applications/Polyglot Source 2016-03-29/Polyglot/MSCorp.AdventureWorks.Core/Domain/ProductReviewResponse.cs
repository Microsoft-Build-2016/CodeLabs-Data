using System;
using Common;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Product Review response.
    /// </summary>
    public class ProductReviewResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductReviewResponse"/> class.
        /// </summary>
        public ProductReviewResponse()
        {
            Date = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductReviewResponse"/> class.
        /// </summary>
        public ProductReviewResponse(string responseText, DateTime date, CustomerIdentifier customerResponding) : this()
        {
            Argument.CheckIfNull(responseText, "responseText");
            Argument.CheckIfNull(customerResponding, "customerResponding");
            ResponseText = responseText;
            CustomerResponding = customerResponding;
            Date = date;
        }

        /// <summary>
        /// Gets or sets the response text.
        /// </summary>
        public string ResponseText { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// Gets or sets the customer responding.
        /// </summary>
        public CustomerIdentifier CustomerResponding { get; set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        [JsonIgnore]
        public int Key
        {
            get { return CustomerResponding.CustomerKey; }
        }
    }
}