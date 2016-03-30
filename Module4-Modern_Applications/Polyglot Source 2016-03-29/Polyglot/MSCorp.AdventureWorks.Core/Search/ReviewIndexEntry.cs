using System;
using System.Runtime.Serialization;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// The definition for the product review index.
    /// </summary>
    [DataContract]
    public class ReviewIndexEntry : ISearchIndexEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewIndexEntry"/> class.
        /// </summary>
        public ReviewIndexEntry(string key)
        {
            Key = key;
            Text = string.Empty;
            CustomerName = string.Empty;
            CustomerKey = string.Empty;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        [DataMember(Name = "Key")]
        [IndexField(FieldType = SearchDataType.String, IsKey = true)]
        public string Key { get; private set; }

        [DataMember(Name = "ProductCode")]
        [IndexField(FieldType = SearchDataType.String)]
        public string ProductCode { get; set; }

        [DataMember(Name = "ProductName")]
        [IndexField(FieldType = SearchDataType.String)]
        public string ProductName { get; set; }

        [DataMember(Name = "Text")]
        [IndexField(FieldType = SearchDataType.String)]
        public string Text { get; set; }

        [DataMember(Name = "CustomerName")]
        [IndexField(FieldType = SearchDataType.String)]
        public string CustomerName { get; set; }

        [DataMember(Name = "CustomerKey")]
        [IndexField(FieldType = SearchDataType.String, IsRetrievable = true)]
        public string CustomerKey { get; set; }

        [DataMember(Name = "Date")]
        [IndexField(FieldType = SearchDataType.DateTimeOffset, IsSearchable = false)]
        public DateTime Date { get; set; }

        [DataMember(Name = "Rating")]
        [IndexField(FieldType = SearchDataType.Int32, IsSearchable = false)]
        public int Rating { get; set; }
    }
}