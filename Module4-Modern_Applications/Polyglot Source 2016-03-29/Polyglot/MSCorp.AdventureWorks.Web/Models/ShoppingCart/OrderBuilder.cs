using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Common;
using MSCorp.AdventureWorks.Core.Domain;

namespace MSCorp.AdventureWorks.Web.Models.ShoppingCart
{
    public static class OrderBuilder
    {
        public static Core.Domain.Order Build(Cart cart)
        {
            Argument.CheckIfNull(cart, "cart");
            
            ICollection<OrderLine> lines = new Collection<OrderLine>();
            Order order = cart.Order;
            
            foreach (Item item in order.Items)
            {
                double discount = item.Quantity * item.PriceSet.DisplayDiscountedPrice.Amount;
                double total = item.Quantity * item.PriceSet.DisplayDiscountedPrice.Amount;
                double gst = order.Gst * item.PriceSet.DisplayDiscountedPrice.Amount;
                double subTotal = total - gst;
                double price = item.PriceSet.DisplayDiscountedPrice.Amount;
                double usdPrice = item.PriceSet.DiscountedPrices.First(p => p.Currency.Code == "USD").Amount;

                string productDesc = item.Description;
                string productCode = item.ProductCode;
                string size = item.Size;
                int quantity =item.Quantity;
                lines.Add(new OrderLine(total, subTotal, gst, discount, usdPrice, price, productCode, productDesc, size, quantity));    
            }

            Core.Domain.Order domainOrder = 
                new Core.Domain.Order(Clock.Now, cart.Customer.CustomerKey.ToString(CultureInfo.InvariantCulture), cart.Customer.PreferredCurrency.Code, lines);
            domainOrder.Shipping = cart.Order.Shipping;

            return domainOrder;
        }
    }
}