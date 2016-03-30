using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Common;
using MSCorp.AdventureWorks.Core.Repository;
using Microsoft.Azure.Documents;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents the review of a product.
    /// </summary>
    public class ProductReview : Document
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductReview"/> class.
        /// </summary>
        public ProductReview()
        {
            IsForIndex = false;
            Date = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductReview"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ProductReview(Identifier reviewIdentifier, string productCode, string reviewText, int rating, DateTime date,
            CustomerIdentifier reviewingCustomer, IEnumerable<ProductReviewResponse> responses) : this()
        {
            Argument.CheckIfNull(reviewIdentifier, "reviewIdentifier");
            Argument.CheckIfNull(productCode, "productCode");
            Argument.CheckIfNullOrEmpty(reviewText, "reviewText");
            Argument.CheckIfNull(reviewingCustomer, "reviewingCustomer");
            Argument.CheckIfNullOrEmpty(responses, "responses");

            ReviewIdentifier = reviewIdentifier;
            ProductCode = productCode;
            ReviewText = reviewText;
            Rating = rating;
            ReviewingCustomer = reviewingCustomer;
            Responses = responses;
            Date = date;
        }

        private void SetCustomValue(object valueToSet, [CallerMemberName] string propertyName = null)
        {
            SetPropertyValue(propertyName, valueToSet);
        }


        /// <summary>
        /// Gets the product identifier.
        /// </summary>
        public string ProductCode
        {
            get { return this.GetCustomValue<string>(); }
            set { SetCustomValue(value); }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        [JsonIgnore]
        public Identifier ReviewIdentifier
        {
            get
            {
                if (Id == null && ETag == null)
                    return null;
                return new Identifier(Id, new VersionNumber(ETag));
            }
            private set
            {
                Id = value.Value;
                SetPropertyValue(DocumentIdentity.Etag, value.VersionNumber.Value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        public bool IsNew { get { return string.IsNullOrWhiteSpace(Id); } }

        /// <summary>
        /// Gets the review text.
        /// </summary>
        public string ReviewText
        {
            get { return this.GetCustomValue<string>(); }
            set { SetCustomValue(value); }
        }

        /// <summary>
        /// Gets the review date.
        /// </summary>
        public DateTime Date
        {
            get { return this.GetCustomValue<DateTime>(); }
            set { SetCustomValue(value); }
        }

        /// <summary>
        /// Gets the rating.
        /// </summary>
        public int Rating
        {
            get { return this.GetCustomValue<int>(); }
            set { SetCustomValue(value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [is for index].
        /// </summary>
        public bool IsForIndex
        {
            get { return this.GetCustomValue<bool>(); }
            set { SetCustomValue(value); }
        }

        /// <summary>
        /// Gets the reviewing customer.
        /// </summary>
        public CustomerIdentifier ReviewingCustomer
        {
            get { return this.GetCustomValue<CustomerIdentifier>(); }
            set { SetCustomValue(value); }
        }

        /// <summary>
        /// Gets the responses.
        /// </summary>
        public IEnumerable<ProductReviewResponse> Responses
        {
            get { return this.GetCustomValue<IEnumerable<ProductReviewResponse>>(); }
            set { SetCustomValue(value); }
        }

        /// <summary>
        /// Saves the <see cref="ProductReview"/> into the repository.
        /// </summary>
        public async Task Save(IProductReviewRepository repository)
        {
            Argument.CheckIfNull(repository, "repository");
            SaveResponse response = await repository.Save(this);
            ReviewIdentifier = response.Identifier;
        }
    }
}