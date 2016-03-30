namespace MSCorp.AdventureWorks.Web.Models.ShoppingCart
{
    public class Order
    {
        public double Shipping { get; set; }
        public double Gst { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
        public Item[] Items { get; set; }
    }
}