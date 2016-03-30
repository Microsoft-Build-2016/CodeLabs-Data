namespace MSCorp.AdventureWorks.Web.Models.ShoppingCart
{
    public class PriceSet
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public Price[] Prices { get; set; }
        public DiscountedPrice[] DiscountedPrices { get; set; }
        public bool IsDiscounted { get; set; }
        public DisplayPrice DisplayPrice { get; set; }
        public DisplayDiscountedPrice DisplayDiscountedPrice { get; set; }
    }
}