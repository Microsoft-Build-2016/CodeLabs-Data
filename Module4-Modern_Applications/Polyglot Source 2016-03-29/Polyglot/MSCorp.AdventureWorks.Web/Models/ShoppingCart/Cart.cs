namespace MSCorp.AdventureWorks.Web.Models.ShoppingCart
{
    public class Cart
    {
        public Order Order { get; set; }
        public Customer Customer { get; set; }
    }
}