using Microsoft.WindowsAzure.Storage.Table;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Simple Table Entity
    /// </summary>
    public class SimpleEntity : TableEntity
    {
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }
    }
}