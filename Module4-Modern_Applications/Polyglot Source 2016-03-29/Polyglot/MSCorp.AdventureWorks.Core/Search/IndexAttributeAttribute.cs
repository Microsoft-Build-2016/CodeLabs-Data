using System;

namespace MSCorp.AdventureWorks.Core.Search
{
    /// <summary>
    /// Specifies custom attributes for an index field.
    /// </summary>
    /// <example>
    /// "type": "Edm.String" | "Edm.Int32" | "Edm.Double" | "Edm.DateTimeOffset" | "Collection(Edm.String)"
    /// "suggestions": true
    /// "filterable": false
    /// "retrievable": true
    /// "searchable": true 
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class IndexAttributeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexAttributeAttribute"/> class.
        /// </summary>
        public IndexAttributeAttribute(string key, object value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public object Value { get; private set; }
    }
}