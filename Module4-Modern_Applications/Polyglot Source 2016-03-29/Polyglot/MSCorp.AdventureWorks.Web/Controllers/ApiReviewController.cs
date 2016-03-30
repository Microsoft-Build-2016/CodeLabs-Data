using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Common;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Web.Utilities;

namespace MSCorp.AdventureWorks.Web.Controllers
{
    public class ApiReviewController : ApiController
    {
        private readonly IProductReviewRepository _productReviewRepository;

        public ApiReviewController(IProductReviewRepository productReviewRepository)
        {
            Argument.CheckIfNull(productReviewRepository, "_productReviewRepository");
            _productReviewRepository = productReviewRepository;
        }


        [Route("Api/ProductReview/{productCode}")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetProductReviews(string productCode)
        {
            string reviewJson = await _productReviewRepository.LoadReviewsJson(productCode);
            return WebApiHelper.ReturnRawJson(reviewJson);
        }


        [Route("Api/ProductReview/Add")]
        [HttpPost]
        public async Task<HttpResponseMessage> AddProductReview([FromBody] ProductReview review)
        {
            try
            {
                string productCode = review.ProductCode;
                await review.Save(_productReviewRepository);
                if (!review.ReviewIdentifier.VersionNumber.IsEmpty)
                {
                    // return the full updated list of reviews for this product
                    return await GetProductReviews(productCode);
                }
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotModified,
                    Content = new StringContent(ex.ToString()),
                };
            }
            return new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotModified,
            };
        }

        [Route("Api/ProductReview/Respond/{reviewId}")]
        [HttpPost]
        public async Task<HttpResponseMessage> AddProductReviewResponse(string reviewId, [FromBody] ProductReviewResponse response)
        {
            try
            {
                string result = await _productReviewRepository.AddReviewResponse(reviewId, response);
                return WebApiHelper.ReturnRawJson(result);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage
                {

                    StatusCode = HttpStatusCode.NotModified,
                    Content = new StringContent(ex.ToString()),
                };
            }
        }

    }
}