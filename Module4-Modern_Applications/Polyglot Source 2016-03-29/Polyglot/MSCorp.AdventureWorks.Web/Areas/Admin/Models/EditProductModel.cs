using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Mvc;
using MSCorp.AdventureWorks.Core.Domain;

namespace MSCorp.AdventureWorks.Web.Areas.Admin.Models
{
    public class EditProductModel
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategoriesList { get; set; }
        public IEnumerable<SelectListItem> ColorsList { get; set; }

        public decimal Price
        {
            get { return Product.Price.Amount; }
        }

        public decimal DiscountedPrice
        {
            get { return Product.DiscountedPrice.Amount; }
        }

        public string ColorName
        {
            get
            {
                if (Product.Color.IsNamedColor)
                {
                    return Product.Color.Name;
                }
                return "";
            }

        }
    }
}