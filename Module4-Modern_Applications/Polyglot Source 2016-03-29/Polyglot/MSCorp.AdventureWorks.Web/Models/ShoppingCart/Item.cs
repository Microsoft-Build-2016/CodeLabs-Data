namespace MSCorp.AdventureWorks.Web.Models.ShoppingCart
{
    public class Item
    {
        public string ProductCode { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public PriceSet PriceSet { get; set; }
    }
}