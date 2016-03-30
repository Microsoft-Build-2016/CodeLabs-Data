using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Common;
using Microsoft.Azure.Documents;
using MSCorp.AdventureWorks.Core.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents a Product for sale through E-Commerce site
    /// </summary>
    [DataContract]
    public sealed class Product : Document
    {
        public Product()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class.
        /// </summary>
        public Product(string productCode, string productCategory, string productType, string productName, string productDescription, string manufacturer,
            Money price, Money discountedPrice, Percentage discount, Color color, IEnumerable<ProductSize> productSizes,
            JObject productAttributes, IEnumerable<Identifier> linkedProducts)
        {
            Argument.CheckIfNull(productType, "productType");
            Argument.CheckIfNull(productCategory, "productCategory");
            Argument.CheckIfNull(productName, "productName");
            Argument.CheckIfNull(productDescription, "productDescription");
            Argument.CheckIfNull(manufacturer, "manufacturer");
            Argument.CheckIfNull(price, "price");
            Argument.CheckIfNull(discountedPrice, "discountedPrice");
            Argument.CheckIfNull(discount, "discount");
            Argument.CheckIfNull(color, "color");
            Argument.CheckIfNullOrEmpty(productSizes, "productSizes");
            Argument.CheckIfNull(productAttributes, "productAttributes");
            Argument.CheckIfNull(linkedProducts, "linkedProducts");

            //todo put a brand here.

            //Aggregate products  - first aid kit.  
            ProductCode = productCode;
            ProductCategory = productCategory;
            ProductType = productType;
            ProductName = productName;
            ProductDescription = productDescription;
            Price = price;
            DiscountedPrice = discountedPrice;
            Discount = discount;
            Color = color;
            ProductSizes = productSizes;
            ProductAttributes = productAttributes;
            LinkedProducts = linkedProducts;
        }

        /// <summary>
        /// Gets the product code.
        /// </summary>
        public string ProductCode
        {
            get { return this.GetCustomValue<string>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the product category.
        /// </summary>
        public string ProductCategory
        {
            get { return this.GetCustomValue<string>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the productType of the product.
        /// </summary>
        public string ProductType
        {
            get { return this.GetCustomValue<string>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        public string ProductName
        {
            get { return this.GetCustomValue<string>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the product description.
        /// </summary>
        public string ProductDescription
        {
            get { return this.GetCustomValue<string>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the price.
        /// </summary>
        public Money Price
        {
            get { return this.GetCustomValue<Money>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the discounted price.
        /// </summary>
        public Money DiscountedPrice
        {
            get { return this.GetCustomValue<Money>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the discount.
        /// </summary>
        public Percentage Discount
        {
            get { return this.GetCustomValue<Percentage>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the color.
        /// </summary>
        public Color Color
        {
            get { return this.GetCustomValue<Color>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the product sizes.
        /// </summary>
        public IEnumerable<ProductSize> ProductSizes
        {
            get { return this.GetCustomValue<IEnumerable<ProductSize>>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the product attributes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public JObject ProductAttributes
        {
            get { return this.GetCustomValue<JObject>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the linked products.
        /// </summary>
        public IEnumerable<Identifier> LinkedProducts
        {
            get { return this.GetCustomValue<IEnumerable<Identifier>>(); }
            private set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets or sets the manufacturer.
        /// </summary>
        public string Manufacturer
        {
            get { return this.GetCustomValue<string>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        public bool IsNew { get { return String.IsNullOrEmpty(Id); } }

        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        public int Priority
        {
            get { return this.GetCustomValue<int>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets or sets the ThumbnailUrl.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        public string ThumbnailUrl
        {
            get { return this.GetCustomValue<string>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        [JsonIgnore]
        public Identifier Identifier
        {
            get
            {
                if (IsNew)
                {
                    return new Identifier();
                }
                return new Identifier(Id, new VersionNumber(ETag));
            }
            private set
            {
                Id = value.Value;
                SetPropertyValue(DocumentIdentity.Etag, value.VersionNumber.Value);
            }
        }

        /// <summary>
        /// Saves the <see cref="Product"/> in the given repository.
        /// </summary>
        public async Task SaveAsync(IProductRepository repository)
        {
            Argument.CheckIfNull(repository, "repository");
            SaveResponse response = await repository.SaveAsync(this);
            Identifier = response.Identifier;
            Document reposnseDocument = response.Document;
            SetPropertyValue(DocumentIdentity.Self, reposnseDocument.SelfLink);
            SetPropertyValue(DocumentIdentity.Attachments, reposnseDocument.AttachmentsLink.Replace(reposnseDocument.SelfLink, string.Empty));
            
            if (SelfLink != reposnseDocument.SelfLink)
            {
                throw new InvalidOperationException("Unable to save product async as the self links are not the same");
            }

            if (AttachmentsLink.CompareTo(reposnseDocument.AttachmentsLink) > 0)
            {
                throw new InvalidOperationException("Unable to save product async as the attachment links are not the same");
            }
        }
        
        /// <summary>
        /// Marks for deletion.
        /// </summary>
        public void MarkForDeletion()
        {
            IsDeleted = true;
        }

        /// <summary>
        /// Gets a value indicating whether [is deleted].
        /// </summary>
        [JsonIgnore]
        public bool IsDeleted { get; private set; }

    }
}
