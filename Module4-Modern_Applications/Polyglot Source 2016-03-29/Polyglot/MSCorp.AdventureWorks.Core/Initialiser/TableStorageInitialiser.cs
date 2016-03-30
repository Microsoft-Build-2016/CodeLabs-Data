using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Table.Protocol;
using MSCorp.AdventureWorks.Core.Repository;

namespace MSCorp.AdventureWorks.Core.Initialiser
{
    /// <summary>
    /// Initialises a Table storage
    /// </summary>
    public class TableStorageInitialiser
    {
        private readonly CloudStorageAccount _cloudStorageAccount;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStorageInitialiser"/> class.
        /// </summary>
        public TableStorageInitialiser(CloudCredentials credentials)
        {
            Argument.CheckIfNull(credentials, "credentials");
            _cloudStorageAccount = CloudStorageAccount.Parse(credentials.ConnectionString);
        }

        /// <summary>
        /// Removes the table and contents
        /// </summary>
        public async Task RemoveTable(TableStorage table)
        {
            Argument.CheckIfNull(table, "table");
            
            CloudTableClient tableClient = _cloudStorageAccount.CreateCloudTableClient();
            CloudTable loadedTable = tableClient.GetTableReference(table.Name);
            await loadedTable.DeleteAsync();
        }

        /// <summary>
        /// Checks if the tables exists.
        /// </summary>
        public Task<bool> TableExists(TableStorage table)
        {
            Argument.CheckIfNull(table, "table");
            
            CloudTableClient tableClient = _cloudStorageAccount.CreateCloudTableClient();
            CloudTable loadedTable = tableClient.GetTableReference(table.Name);
            return loadedTable.ExistsAsync();
        }

        /// <summary>
        /// Creates the table with the given name
        /// </summary>
        public async Task CreateTable(TableStorage table)
        {
            Argument.CheckIfNull(table, "table");
            
            if (!await TableExists(table))
            {
                bool wasCreated = false;
                do
                {
                    try
                    {
                        CloudTableClient tableClient = _cloudStorageAccount.CreateCloudTableClient();
                        CloudTable loadedTable = tableClient.GetTableReference(table.Name);
                        loadedTable.CreateIfNotExists();
                        wasCreated = true;
                    }
                    catch (StorageException e)
                    {
                        if ((e.RequestInformation.HttpStatusCode == 409) && (e.RequestInformation.ExtendedErrorInformation.ErrorCode.Equals(TableErrorCodeStrings.TableBeingDeleted)))
                            Thread.Sleep(1000);// The blob is currently being deleted. Try again until it works.
                        else
                            throw;
                    }
                } while (!wasCreated);

            }
        }
    }
}