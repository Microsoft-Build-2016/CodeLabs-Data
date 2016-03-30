using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Search;

namespace MSCorp.AdventureWorks.Core.Mappers
{
    public class ReviewIndexMapper : IReviewIndexMapper
    {
        public ReviewIndexEntry MapToIndex(ProductReview entity)
        {
            var index = new ReviewIndexEntry(entity.ReviewIdentifier.Value)
            {
                CustomerKey = entity.ReviewingCustomer.CustomerKey.ToString(),
                CustomerName = entity.ReviewingCustomer.Name,
                Date = entity.Date,
                ProductCode = entity.ProductCode,
                Text = entity.ReviewText,
                Rating = entity.Rating
            };
            return index;
        }
    }
}
