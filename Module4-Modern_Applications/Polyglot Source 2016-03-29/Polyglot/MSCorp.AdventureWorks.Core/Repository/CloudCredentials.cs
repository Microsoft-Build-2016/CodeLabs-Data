using Common;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// A container for Azure cloud storage connection details.
    /// </summary>
    public class CloudCredentials
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CloudCredentials"/> class.
        /// </summary>
        public CloudCredentials(string connectionString)
        {
            Argument.CheckIfNullOrEmpty(connectionString, "connectionString");
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets or sets the BLOB connection string.
        /// </summary>
        public string ConnectionString { get; set; }


    }
}