using Common;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents a customised attribute of a <see cref="Product"/>
    /// </summary>
    public class ProductProperty
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductProperty"/> class.
        /// </summary>
        [JsonConstructor]
        public ProductProperty(string key, string text)
        {
            Argument.CheckIfNullOrEmpty(text, "Text");
            Argument.CheckIfNullOrEmpty(key, "Key");

            Key = key;
            Text = text;
        }

        /// <summary>
        /// Gets the property key.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        public string Text { get; private set; }
    }
}