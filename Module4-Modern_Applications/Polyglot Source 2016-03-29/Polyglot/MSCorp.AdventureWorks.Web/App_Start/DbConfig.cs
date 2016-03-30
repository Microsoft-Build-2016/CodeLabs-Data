using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using MSCorp.AdventureWorks.Core.Configuration;
using MSCorp.AdventureWorks.Core.Initialiser;
using MSCorp.AdventureWorks.Core.Repository;
using MSCorp.AdventureWorks.Core.Search;
using MSCorp.AdventureWorks.Web.Configuration;
using Newtonsoft.Json.Linq;
using MSCorp.AdventureWorks.Web.Utilities;

namespace MSCorp.AdventureWorks.Web
{
    public sealed class DbConfig : IDisposable
    {
        private DocumentDbCredentials _docDbCredentials;
        private CloudCredentials _tableCredentials;
        private CloudCredentials _cloudBlobCredentials;
        private CloudContainerName _imagesContainerName;

        private string _documentDatabase;
        private TableStorage _customerCartTable;
        private DocumentDbInitialiser _docDbInitialiser;
        private TableStorageInitialiser _tableStorageInitialiser;
        private BlobStorageInitialiser _blobStorageInitialiser;
        private SqlDatabaseInitialiser _sqlInitialiser;
        public DocumentDbProductRepository ProductRepository { get; private set; }
        public DocumentDbProductReviewRepository ProductReviewRepository { get; private set; }
        public DocumentDbCustomerRepository CustomerRepository { get; private set; }
        public DocumentDbCurrencyRepository CurrencyRepository { get; private set; }
        public DocumentDbOrderSummaryRepository OrderSummaryRepository { get; private set; }
        public DocumentDbDiscoveryRepository DiscoveryRepository { get; private set; }
        public CustomerCartRepository CustomerCartRepository { get; private set; }
        public IUnitOfWorkFactory UnitOfWorkFactory { get; private set; }
        public IUnitOfWork SqlUnitOfWork { get; private set; }
        public IAppSettings AppSettings { get; private set; }

        public async Task Initialise()
        {
            _docDbCredentials = new DocumentDbCredentials(
                 SettingLoader.Load("DocumentDbUri").Value,
                 SettingLoader.Load("DocumentDbAuthKey").Value.ToSecureString());

            _cloudBlobCredentials = new CloudCredentials(SettingLoader.Load("AzureImageBlobConnectionString").Value);
            _imagesContainerName = new CloudContainerName(SettingLoader.Load("AzureImageBlobContainerName").Value);
            _tableCredentials = new CloudCredentials(SettingLoader.Load("AzureTableBlobConnectionString").Value);
            _documentDatabase = SettingLoader.Load("DocumentDbDatabase").Value;

            DiscoveryRepository = new DocumentDbDiscoveryRepository(_docDbCredentials, _documentDatabase);

            AppSettings = new AppSettings()
            {
                DocumentDbConnectionString = $"AccountEndpoint={_docDbCredentials.EndpointUrl}/;AccountKey={SettingLoader.Load("DocumentDbAuthKey").Value};"
            };

            _customerCartTable = new TableStorage("CustomerCart");

            CreateRepositories(_documentDatabase);

            _docDbInitialiser = new DocumentDbInitialiser(_docDbCredentials);
            _tableStorageInitialiser = new TableStorageInitialiser(_tableCredentials);
            _blobStorageInitialiser = new BlobStorageInitialiser(_tableCredentials);
            _sqlInitialiser = new SqlDatabaseInitialiser(SettingLoader.Load("OrderDatabaseConnectionString").Value);

            if (!await _docDbInitialiser.DatabaseExists(_documentDatabase))
            {
                await _docDbInitialiser.CreateDocumentDatabase(_documentDatabase);
            }

            await _tableStorageInitialiser.CreateTable(_customerCartTable);
            await _blobStorageInitialiser.CreateContainer(_imagesContainerName);
            _sqlInitialiser.CreateDatabase();
            await ProductRepository.Initialise(_docDbInitialiser);
            await ProductReviewRepository.Initialise(_docDbInitialiser);
            await CustomerRepository.Initialise(_docDbInitialiser);
            await CurrencyRepository.Initialise(_docDbInitialiser);
            await OrderSummaryRepository.Initialise(_docDbInitialiser);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private void CreateRepositories(string database)
        {
            // Maintain single instance of the repositories, only construct them once.  Otherwise the controllers may loose reference to the doc db
            // locations when resetting demo data.
            if (ProductRepository == null)
            {
                var dbSuffix = SettingLoader.Load("DocumentDbCollectionSuffix").Value;

                var productCollection = new DocumentDbCustomerCollection("Product" + dbSuffix);
                var productReviewCollection = new DocumentDbCustomerCollection("ProductReview" + dbSuffix);
                var customerCollectionAG = new DocumentDbCustomerCollection("CustomerAG" + dbSuffix);
                var customerCollectionHP = new DocumentDbCustomerCollection("CustomerHP" + dbSuffix);
                var customerCollectionQZ = new DocumentDbCustomerCollection("CustomerQZ" + dbSuffix);
                var orderSummaryCollection = new DocumentDbCustomerCollection("OrderSummary" + dbSuffix);
                string orderConnectionString = SettingLoader.Load("OrderDatabaseConnectionString").Value;
                customerCollectionAG.Range.AddRange(new string[] { "a", "b", "c", "d", "e", "f", "g" });
                customerCollectionHP.Range.AddRange(new string[] { "h", "i", "j", "k", "l", "m", "n", "o", "p" });
                customerCollectionQZ.Range.AddRange(new string[] { "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" });

                ProductRepository = new DocumentDbProductRepository(_docDbCredentials, database, productCollection.Name, _cloudBlobCredentials, _imagesContainerName);
                ProductReviewRepository = new DocumentDbProductReviewRepository(_docDbCredentials, database, productReviewCollection.Name);
                CustomerRepository = new DocumentDbCustomerRepository(_docDbCredentials, database, customerCollectionAG, customerCollectionHP, customerCollectionQZ);
                OrderSummaryRepository = new DocumentDbOrderSummaryRepository(_docDbCredentials, database, orderSummaryCollection.Name);
                CurrencyRepository = new DocumentDbCurrencyRepository(_docDbCredentials, database, productCollection.Name); // Store currency in same collection as products.

                //DiscoveryRepository = new DocumentDbDiscoveryRepository(_docDbCredentials, database, productCollection.Name,
                //    productReviewCollection.Name, customerCollectionAG.Name, customerCollectionHP.Name, customerCollectionQZ.Name, orderSummaryCollection.Name);

                CustomerCartRepository = new CustomerCartRepository(_cloudBlobCredentials, _customerCartTable);

                UnitOfWorkFactory = new SqlUnitOfWorkFactory(orderConnectionString);

                SqlUnitOfWork = UnitOfWorkFactory.GenerateUnitOfWork();
            }
        }

        public async Task Delete()
        {
            await ProductRepository.Uninitialise(_docDbInitialiser);
            await ProductReviewRepository.Uninitialise(_docDbInitialiser);
            await CustomerRepository.Uninitialise(_docDbInitialiser);
            await CurrencyRepository.Uninitialise(_docDbInitialiser);
            await OrderSummaryRepository.Uninitialise(_docDbInitialiser);
            SqlUnitOfWork.Orders.Reset();
            await _docDbInitialiser.RemoveDatabase(_documentDatabase);
            await _tableStorageInitialiser.RemoveTable(_customerCartTable);
            await _blobStorageInitialiser.RemoveContainer(_imagesContainerName);
            ClearBlobStorage();
        }

        private void ClearBlobStorage()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_cloudBlobCredentials.ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(_imagesContainerName.Text);
            if (container.Exists())
            {
                container.Delete();
            }
        }
        public void Dispose()
        {
            if (_docDbCredentials != null)
            {
                _docDbCredentials.Dispose();
            }

            DisposeRepository(ProductRepository);
            DisposeRepository(ProductReviewRepository);
            DisposeRepository(CustomerRepository);
            DisposeRepository(CurrencyRepository);
            SqlUnitOfWork?.Dispose();
            GC.SuppressFinalize(this);
        }

        private static void DisposeRepository(DocumentDbRepository repository)
        {
            if (repository != null)
            {
                repository.Dispose();
            }
        }
    }
}