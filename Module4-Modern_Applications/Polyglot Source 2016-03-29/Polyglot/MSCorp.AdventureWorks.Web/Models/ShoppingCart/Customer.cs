namespace MSCorp.AdventureWorks.Web.Models.ShoppingCart
{
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CustomerKey { get; set; }
        public string EmailAddress { get; set; }
        public Currency PreferredCurrency { get; set; }
    }
}