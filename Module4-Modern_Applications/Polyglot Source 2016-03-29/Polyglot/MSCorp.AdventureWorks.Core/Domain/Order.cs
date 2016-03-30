using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Common;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents an Order
    /// </summary>
    public class Order
    {
        private Order()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Order"/> class.
        /// </summary>
        public Order(DateTime orderPlaced, string customerCode, string currencyCode, ICollection<OrderLine> lines)
        {
            Id = -1;
            OrderPlaced = orderPlaced;
            CustomerCode = customerCode;
            CurrencyCode = currencyCode;
            Lines = lines;
        }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }

        [Required]
        public DateTime OrderPlaced { get; private set; }
        
        [Required]
        public string CustomerCode { get; private set; }
        
        [Required]
        public string CurrencyCode { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly"), Required]
        public ICollection<OrderLine> Lines { get; set; }

        [Required]
        public double Shipping { get; set; }

        [NotMapped]
        public Currency Currency
        {
            get { return new Currency(CurrencyCode); }
        }

        [NotMapped]
        public Money SubTotal
        {
            get
            {
                double sum = Lines.Select(l => l.Subtotal).Sum();
                return new Money((decimal)sum, Currency);
            }
        }

        [NotMapped]
        public Money GstTotal
        {
            get
            {
                double sum = Lines.Select(l => l.Gst).Sum();
                return new Money((decimal)sum, Currency);
            }
        }

        [NotMapped]
        public Money Total
        {
            get
            {
                double sum = Lines.Select(l => l.Total).Sum() + Shipping;
                return new Money((decimal)sum, Currency);
            }
        }

    }
}