using Common;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Container name
    /// </summary>
    public class CloudContainerName
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloudContainerName"/> class.
        /// </summary>
        public CloudContainerName(string containerName)
        {
            Argument.CheckIfNullOrEmpty(containerName, "Text");
            Text = containerName;
        }

        /// <summary>
        /// Gets or sets the name of the BLOB container.
        /// </summary>
        public string Text { get; set; }
    }
}