using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Core.Search;

namespace MSCorp.AdventureWorks.Web.Areas.Admin.Controllers
{
    [RouteArea("Admin")]
    public class SyncController : Controller
    {
        private readonly AzureSearchClient<ReviewIndexEntry> _searchClient;
        private readonly IProductRepository _productRepository;
        private readonly IProductReviewRepository _productReviewRepository;

        public SyncController(AzureSearchClient<ReviewIndexEntry> searchClient, IProductRepository productRepository, IProductReviewRepository productReviewRepository)
        {
            _searchClient = searchClient;
            _productRepository = productRepository;
            _productReviewRepository = productReviewRepository;
        }

        // /Admin/Sync
        public async Task<HttpResponseMessage> Index()
        {
            List<Product> products = (await _productRepository.LoadProducts()).ToList();

            await SyncComments(products);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private async Task SyncComments(List<Product> products)
        {
            // Perform push to Azure Search.
            List<ProductReviewEntry> reviewsForIndexing = (await _productReviewRepository.LoadReviewsForIndexing()).ToList();
            if (reviewsForIndexing.Any())
            {
                foreach (ProductReviewEntry productReviewEntry in reviewsForIndexing)
                {
                    ReviewIndexEntry reviewIndexEntry = MapToIndex(productReviewEntry, products);
                    await _searchClient.PostDocumentsAsync(reviewIndexEntry);
                    await _productReviewRepository.Delete(productReviewEntry);
                }
            }
        }

        private static ReviewIndexEntry MapToIndex(ProductReviewEntry item, IEnumerable<Product> products)
        {
            string productName = products.Single(p => p.ProductCode.Equals(item.ProductCode, StringComparison.OrdinalIgnoreCase)).ProductName;
            return new ReviewIndexEntry(item.Id)
            {
                CustomerKey = item.CustomerKey,
                CustomerName = item.CustomerName,
                Date = item.Date,
                Rating = item.Rating,
                Text = item.Text,
                ProductCode = item.ProductCode,
                ProductName = productName
            };
        }
    }
}
