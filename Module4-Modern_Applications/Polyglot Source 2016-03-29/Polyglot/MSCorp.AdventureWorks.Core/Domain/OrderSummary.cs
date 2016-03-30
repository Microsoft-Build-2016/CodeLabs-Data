using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Common;
using Microsoft.Azure.Documents;
using MSCorp.AdventureWorks.Core.Repository;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents the review of a product.
    /// </summary>
    public class OrderSummary : Document
    {
        public OrderSummary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderSummary"/> class.
        /// </summary>
        public OrderSummary(Order order)
        {
            Argument.CheckIfNull(order, "order");
            OrderId = order.Id;
            OrderDate = order.OrderPlaced;
            CustomerCode = order.CustomerCode;
            ItemQuantity = order.Lines.Select(line => line.Quantity).Sum();
            double orderTotal = order.Lines.Select(line => line.Total).Sum() + order.Shipping;
            OrderTotal = new Money((decimal)orderTotal, new Currency(order.CurrencyCode));
        }

        private void SetCustomValue(object valueToSet, [CallerMemberName] string propertyName = null)
        {
            SetPropertyValue(propertyName, valueToSet);
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        [JsonIgnore]
        public Identifier SummaryIdentifier
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
        /// Gets the order Id.
        /// </summary>
        public int OrderId
        {
            get { return this.GetCustomValue<int>(); }
            set { SetCustomValue(value); }
        }

        /// <summary>
        /// Gets the order date.
        /// </summary>
        public DateTime OrderDate
        {
            get { return this.GetCustomValue<DateTime>(); }
            set { SetCustomValue(value); }
        }

        /// <summary>
        /// Gets the item quantity.
        /// </summary>
        public int ItemQuantity
        {
            get { return this.GetCustomValue<int>(); }
            set { SetCustomValue(value); }
        }

        /// <summary>
        /// Gets the customer identifier.
        /// </summary>
        public string CustomerCode
        {
            get { return this.GetCustomValue<string>(); }
            set { SetCustomValue(value); }
        }

        /// <summary>
        /// Gets the order total.
        /// </summary>
        public Money OrderTotal
        {
            get { return this.GetCustomValue<Money>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Saves the <see cref="OrderSummary"/> into the repository.
        /// </summary>
        public async Task Save(IOrderSummaryRepository repository)
        {
            Argument.CheckIfNull(repository, "repository");
            SaveResponse response = await repository.Save(this);
            SummaryIdentifier = response.Identifier;
        }
    }
}