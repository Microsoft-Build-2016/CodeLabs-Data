using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Core.Search;
using Microsoft.Azure.Documents;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Coordinates the actions required when deleting a product.
    /// </summary>
    public class ProductDeleteTask
    {
        private readonly IProductRepository _repository;
        private readonly ISearchRepository<PrimaryIndexEntry> _searchRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductDeleteTask"/> class.
        /// </summary>
        public ProductDeleteTask(IProductRepository repository, ISearchRepository<PrimaryIndexEntry> searchRepository)
        {
            Argument.CheckIfNull(repository, "repository");
            Argument.CheckIfNull(searchRepository, "searchRepository");
            _repository = repository;
            _searchRepository = searchRepository;
        }

        /// <summary>
        /// Deletes the specified product.
        /// </summary>
        public async Task Delete(Product product)
        {
            // Remove from search service.
            PrimaryIndexEntry document = PrimarySearchIndexBuilder.Build(product);
            await _searchRepository.DeleteDocumentsAsync(document);

            // Remove attachments.
            IEnumerable<Attachment> attachments = await _repository.LoadAttachments(product.SelfLink);
            foreach (Attachment attachment in attachments)
            {
                await _repository.DeleteAttachment(attachment);
            }

            // Delete product.
            product.MarkForDeletion();
            await product.SaveAsync(_repository);
        }
    }
}