using System;
using System.Globalization;
using System.Threading.Tasks;
using Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using MSCorp.AdventureWorks.Core.Initialiser;
using Microsoft.WindowsAzure.Storage.RetryPolicies;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Loads and saves order's into a customer cart
    /// </summary>
    public class CustomerCartRepository : ICustomerCartRepository
    {
        private readonly TableStorage _customerCartTable;
        private readonly CloudStorageAccount _cloudStorageAccount;
        private readonly LinearRetry _retryPolicy;
        private const string PartitionKey = "customer";
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerCartRepository"/> class.
        /// </summary>
        public CustomerCartRepository(CloudCredentials cloudCredential, TableStorage customerCartTable)
        {
            Argument.CheckIfNull(customerCartTable, "customerCartTable");
            Argument.CheckIfNull(cloudCredential, "cloudCredential");
            _customerCartTable = customerCartTable;
            _cloudStorageAccount = CloudStorageAccount.Parse(cloudCredential.ConnectionString);
            _retryPolicy = new LinearRetry(TimeSpan.FromSeconds(2), 4);
        }

        /// <summary>
        /// Saves the specified order.
        /// </summary>
        public async Task<bool> Save(int customerKey, string order)
        {
            Argument.CheckIfNullOrEmpty(order, "order");

            try
            {
                CloudTableClient client = _cloudStorageAccount.CreateCloudTableClient();
                client.DefaultRequestOptions.RetryPolicy = _retryPolicy;
                CloudTable table = client.GetTableReference(_customerCartTable.Name);

                //TODO pull out a more meaningfull partition key if more that this many customer
                TableEntity entity = new SimpleEntity { PartitionKey = PartitionKey, RowKey = customerKey.ToString(CultureInfo.InvariantCulture), Text = order };
                TableOperation insertOperation = TableOperation.InsertOrReplace(entity);
                TableResult tableResult = await table.ExecuteAsync(insertOperation);
                return tableResult.HttpStatusCode == 204;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("There was a problem saving your order", ex);
            }
        }

        /// <summary>
        /// Loads the specified order for the given customer identifier.
        /// </summary>
        public async Task<string> Load(int customerKey)
        {
            try
            {
                CloudTableClient client = _cloudStorageAccount.CreateCloudTableClient();
                client.DefaultRequestOptions.RetryPolicy = _retryPolicy;
                CloudTable table = client.GetTableReference(_customerCartTable.Name);

                TableOperation operation = TableOperation.Retrieve<SimpleEntity>(PartitionKey, customerKey.ToString(CultureInfo.InvariantCulture));
                TableResult result = await table.ExecuteAsync(operation);
                SimpleEntity simpleEntityResult = (SimpleEntity)result.Result;
                if (simpleEntityResult == null)
                {
                    return string.Empty;
                }

                return simpleEntityResult.Text;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("There was a problem loading your order", ex);   
            }
        }

        public async Task<bool> Delete(int customerKey)
        {
            try
            {
                CloudTableClient client = _cloudStorageAccount.CreateCloudTableClient();
                client.DefaultRequestOptions.RetryPolicy = _retryPolicy;
                CloudTable table = client.GetTableReference(_customerCartTable.Name);

                TableEntity entity = new SimpleEntity { 
                    PartitionKey = PartitionKey, 
                    RowKey = customerKey.ToString(CultureInfo.InvariantCulture),
                    ETag = "*"
                };
                TableOperation operation = TableOperation.Delete(entity);
                TableResult tableResult = await table.ExecuteAsync(operation);
                return tableResult.HttpStatusCode == 204;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("There was a problem removing your order", ex);
            }
        }
    }
}