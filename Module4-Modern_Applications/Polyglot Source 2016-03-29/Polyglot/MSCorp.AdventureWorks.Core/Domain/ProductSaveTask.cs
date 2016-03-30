using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Azure.Documents;
using MSCorp.AdventureWorks.Core.Import;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Core.Search;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Coordinates the actions required when saving a product.
    /// </summary>
    public class ProductSaveTask
    {
        private readonly IProductRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSaveTask"/> class.
        /// </summary>
        public ProductSaveTask(IProductRepository repository)
        {
            Argument.CheckIfNull(repository, "repository");
            _repository = repository;
        }

        /// <summary>
        /// Saves the specified product.
        /// </summary>
        public async Task SaveAsync(Product product, params ProductImage[] images)
        {
            Argument.CheckIfNull(product, "product");
            Argument.CheckIfNull(images, "images");

            bool isNew = product.IsNew;
            await product.SaveAsync(_repository);
            
            IEnumerable<Attachment> attachments = new List<Attachment>();

            if (isNew)
            {
                await _repository.AddAttachmentsAsync(product, images);
                //Thumbnail Url
                await product.SaveAsync(_repository);
            }
            else
            {
                JObject productJson = JObject.Parse(product.ToString());
                if (productJson["attachmentCollection"] != null)
                {
                    productJson["attachmentCollection"].Select(att => JsonConvert.DeserializeObject<Attachment>(att.ToString())).ToList();
                }
            }
            
        }

        /// <summary>
        /// Saves the specified product.
        /// </summary>
        public void Save(Product product, params ProductImage[] images)
        {
            Argument.CheckIfNull(product, "product");
            Argument.CheckIfNull(images, "images");

            SaveAsync(product, images).Wait();
        }
    }
}