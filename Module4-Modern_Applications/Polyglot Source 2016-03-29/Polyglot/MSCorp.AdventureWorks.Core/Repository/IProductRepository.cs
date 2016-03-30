using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Import;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Represents a Product repository
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Saves the specified product.
        /// </summary>
        Task<SaveResponse> SaveAsync(Product product);

        /// <summary>
        /// Loads the product with the given product code.
        /// </summary>
        Task<Product> LoadProduct(string productCode);

        /// <summary>
        /// Loads all the available Product Category Sets.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IEnumerable<ProductCategorySet>> LoadAllCategorySets();

        /// <summary>
        /// Adds the attachments.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IEnumerable<Attachment>> AddAttachmentsAsync(Product product, params ProductImage[] images);

        /// <summary>
        /// Loads the product in raw JSON with the given product code.
        /// </summary>
        Task<string> LoadProductJson(string productCode);

        /// <summary>
        /// Loads the attachments.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IEnumerable<Attachment>> LoadAttachments(string documentLink);

        /// <summary>
        /// Deletes the attachment.
        /// </summary>
        Task DeleteAttachment(Attachment attachment);

        /// <summary>
        /// Loads all the products.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IEnumerable<Product>> LoadProducts();

        /// <summary>
        /// Saves the specified product.
        /// </summary>
        SaveResponse Save(Product product);

        /// <summary>
        /// Adds the attachment.
        /// </summary>
        IEnumerable<Attachment> AddAttachments(Product product, params ProductImage[] images);

        /// <summary>
        /// Loads all the products for a product category.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        Task<IEnumerable<Product>> LoadProductsByCategory(string category);

        /// <summary>
        /// Loads the product with the given Identifier.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Proc")]
        Task<string> LoadProductByCodeViaStoredProcedure(string productCode);

        /// <summary>
        /// Loads the related products by attribute, returning the raw JSON string result. Asynchronous.
        /// </summary>
        Task<string> LoadRelatedProductsByAttributeJsonAsync(string searchAttribute, string searchText);

        /// <summary>
        /// Loads a list of all product attributes as a single formatted JSON string.
        /// </summary>
        Task<string> LoadAllProductAttributesJsonAsync();

        /// <summary>
        /// Applies the discount to products.
        /// </summary>
        /// <param name="requestData">The request data as a JSON formatted string.</param>
        void ApplyDiscountToProducts(string requestData);
    }
}