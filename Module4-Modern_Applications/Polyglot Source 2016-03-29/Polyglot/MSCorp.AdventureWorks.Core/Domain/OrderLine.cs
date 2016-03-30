using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents a line on an order
    /// </summary>
    public class OrderLine
    {
        protected OrderLine()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderLine"/> class.
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public OrderLine(double total, double subtotal, double gst, double discount, double USDPrice, double price, 
            string productCode, string productDescription, string size, int quantity)
        {
            Id = -1;
            Total = total;
            Subtotal = subtotal;
            this.USDPrice = USDPrice;
            Price = price;
            Gst = gst;
            Discount = discount;
            ProductCode = productCode;
            ProductDescription = productDescription;
            Size = size;
            Quantity = quantity;
        }

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; private set; }
        
        [Required]
        public double Total { get; private set; }
        
        [Required]
        public double Subtotal { get; private set; }
        
        [Required]
        public double Gst { get; private set; }

        [Required]
        public double USDPrice { get; private set; }

        [Required]
        public double Price { get; private set; }
        
        [Required]
        public double Discount { get; private set; }

        [Required]
        public string ProductCode { get; private set; }

        [Required]
        public string ProductDescription { get; private set; }

        public string Size { get; private set; }

        [Required]
        public int Quantity { get; private set; }
    }
}