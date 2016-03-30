using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Common;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// The definition for the main index.
    /// </summary>
    [DataContract]
    public class PrimaryIndexEntry : ISearchIndexEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrimaryIndexEntry"/> class.
        /// </summary>
        public PrimaryIndexEntry(string key)
        {
            Argument.CheckIfNullOrEmpty(key, "key");
            Key = key;
            Name = string.Empty;
            Description = string.Empty;
        }

        [DataMember(Name = "Key")]
        [IndexField(FieldType = SearchDataType.String, IsKey = true)]
        public string Key { get; set; }

        [DataMember(Name = "Name")]
        [IndexField(FieldType = SearchDataType.String, IsSuggestable = true)]
        public string Name { get; set; }

        [DataMember(Name = "ProductCode")]
        [IndexField(FieldType = SearchDataType.String, IsSuggestable = true)]
        public string ProductCode { get; set; }

        [DataMember(Name = "Description")]
        [IndexField(FieldType = SearchDataType.String, IsSuggestable = true)]
        public string Description { get; set; }

        [DataMember(Name = "Manufacturer")]
        [IndexField(FieldType = SearchDataType.String, IsFacetable = true)]
        public string Manufacturer { get; set; }

        [DataMember(Name = "Color")]
        [IndexField(FieldType = SearchDataType.String, IsFacetable = true)]
        public string Color { get; set; }

        [DataMember(Name = "Size")]
        [IndexField(FieldType = SearchDataType.StringCollection)]
        public IEnumerable<string> Size { get; set; }

        [DataMember(Name = "SizeFacet")]
        [IndexField(FieldType = SearchDataType.StringCollection, IsFacetable = true)]
        public IEnumerable<string> SizeFacet { get; set; }

        [DataMember(Name = "Price")]
        [IndexField(FieldType = SearchDataType.Double, IsFilterable = true, IsSearchable = false)]
        public decimal Price { get; set; }

        [DataMember(Name = "CurrencyCode")]
        [IndexField(FieldType = SearchDataType.String, IsRetrievable = true)]
        public string CurrencyCode { get; set; }

        [DataMember(Name = "Discount")]
        [IndexField(FieldType = SearchDataType.Double, IsSearchable = false)]
        public decimal Discount { get; set; }

        [DataMember(Name = "ProductType")]
        [IndexField(FieldType = SearchDataType.String)]
        public string ProductType { get; set; }

        [DataMember(Name = "ProductCategory")]
        [IndexField(FieldType = SearchDataType.String, IsFacetable = true)]
        public string ProductCategory { get; set; }

        [DataMember(Name = "Priority")]
        [IndexField(FieldType = SearchDataType.Int32, IsFilterable = true, IsSearchable = false)]
        public int Priority { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings")]
        [DataMember(Name = "ThumbImageUrl")]
        [IndexField(FieldType = SearchDataType.String, IsFilterable = true, IsRetrievable = true)]
        public string ThumbImageUrl { get; set; }

        [DataMember(Name = "LastPurchasedDate")]
        [IndexField(FieldType = SearchDataType.DateTimeOffset, IsFilterable = true, IsRetrievable = true, IsSearchable = false)]
        public DateTime LastPurchasedDate { get; set; }
    }
}