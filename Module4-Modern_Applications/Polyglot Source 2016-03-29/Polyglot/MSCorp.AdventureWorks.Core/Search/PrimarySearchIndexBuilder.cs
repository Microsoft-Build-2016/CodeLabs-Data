using System;
using System.Dynamic;
using System.Linq;
using Common;
using Microsoft.Azure.Search.Models;
using MSCorp.AdventureWorks.Core.Domain;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// Maps domain objects into the primary search index.
    /// </summary>
    public static class PrimarySearchIndexBuilder
    {
        /// <summary>
        /// Builds the specified product index.
        /// </summary>
        public static PrimaryIndexEntry Build(Product product, params Microsoft.Azure.Documents.Attachment[] attachments)
        {
            Argument.CheckIfNull(product, "product");
            Argument.CheckIfNull(attachments, "attachments");

            PrimaryIndexEntry entry = new PrimaryIndexEntry(product.ProductCode);
            entry.Name = product.ProductName;
            entry.ProductCode = product.ProductCode;
            entry.Description = product.ProductDescription;
            entry.Color = product.Color.Name;
            entry.Price = product.Price.Amount;
            entry.CurrencyCode = product.Price.Currency.Code;
            entry.Discount = product.Discount.PercentageValue;
            entry.Manufacturer = product.Manufacturer;
            
            entry.SizeFacet = product.ProductSizes.Select(RefinerSerializer.Serialize).ToList();
            entry.Size = product.ProductSizes.Select(s => s.Name).ToList();

            entry.ProductCategory = product.ProductCategory;
            entry.ProductType = product.ProductType;
            entry.Priority = product.Priority;
            entry.LastPurchasedDate = DateTime.UtcNow;
            Microsoft.Azure.Documents.Attachment thumb = attachments.FirstOrDefault(att => att.GetPropertyValue<bool>("isThumbnail"));
            if (thumb != null)
            {
                entry.ThumbImageUrl = thumb.MediaLink;
            }

            return entry;
        }
        
        /// <summary>
        /// Builds the specified product index.
        /// </summary>
        public static PrimaryIndexEntry BuildForProductUpdate(Product product, params Microsoft.Azure.Documents.Attachment[] attachments)
        {
            Argument.CheckIfNull(product, "product");
            Argument.CheckIfNull(attachments, "attachments");

            PrimaryIndexEntry entry = new PrimaryIndexEntry(product.ProductCode)
            {
                Name = product.ProductName,
                Key = product.ProductCode,
                ProductCode = product.ProductCode,
                Description = product.ProductDescription,
                Color = product.Color.Name,
                Price = product.Price.Amount,
                CurrencyCode = product.Price.Currency.Code,
                Discount = product.Discount.PercentageValue,
                Manufacturer = product.Manufacturer,
                SizeFacet = product.ProductSizes.Select(RefinerSerializer.Serialize).ToList(),
                Size = product.ProductSizes.Select(s => s.Name).ToList(),
                ProductCategory = product.ProductCategory,
                ProductType = product.ProductType,
                Priority = product.Priority
            };


            Microsoft.Azure.Documents.Attachment thumb = attachments.FirstOrDefault(att => att.GetPropertyValue<bool>("isThumbnail"));
            if (thumb != null)
            {
                entry.ThumbImageUrl = thumb.MediaLink;
            }

            return entry;
        }

        /// <summary>
        /// Builds an index merge entry fot the last purchased date.
        /// </summary>
        public static Document BuildForLastPurchasedDate(string productCode, DateTime lastPurchasedDate)
        {
            var document = new Document();
            document.Add("Key", productCode);
            document.Add("LastPurchasedDate", lastPurchasedDate);
            return document;
        }
    }
}