using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Search;

namespace MSCorp.AdventureWorks.Core.Mappers
{
    public interface IReviewIndexMapper
    {
        ReviewIndexEntry MapToIndex(ProductReview entity);
    }
}
