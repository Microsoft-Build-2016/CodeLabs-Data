using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using MSCorp.AdventureWorks.Core.Repository;

namespace MSCorp.AdventureWorks.Core.Initialiser
{
    /// <summary>
    /// Initialises a Blob storage.
    /// </summary>
    public class BlobStorageInitialiser
    {
        private readonly CloudStorageAccount _cloudStorageAccount;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageInitialiser"/> class.
        /// </summary>
        public BlobStorageInitialiser(CloudCredentials credentials)
        {
            Argument.CheckIfNull(credentials, "credentials");
            _cloudStorageAccount = CloudStorageAccount.Parse(credentials.ConnectionString);
        }
        
        /// <summary>
        /// Creates the blob container with the given name
        /// </summary>
        public async Task CreateContainer(CloudContainerName containerName)
        {
            Argument.CheckIfNull(containerName, "containerName");

            CloudBlobClient blobClient = _cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName.Text);

            if (!await ContainerExists(containerName))
            {
                bool wasCreated = false;
                do
                {
                    try
                    {
                        container.Create();
                        wasCreated = true;
                    }
                    catch (StorageException e)
                    {
                        if ((e.RequestInformation.HttpStatusCode == 409) && (e.RequestInformation.ExtendedErrorInformation.ErrorCode.Equals(BlobErrorCodeStrings.ContainerBeingDeleted)))
                            Thread.Sleep(1000);// The blob is currently being deleted. Try again until it works.
                        else
                            throw;
                    }
                } while (!wasCreated);

                //Set public access to the blobs in the container so we can use the picture URLs in the HTML client.
                container.SetPermissions(new BlobContainerPermissions
                {
                    PublicAccess = BlobContainerPublicAccessType.Blob
                });
            }
        }

        /// <summary>
        /// Checks if the tables exists.
        /// </summary>
        public async Task<bool> ContainerExists(CloudContainerName containerName)
        {
            CloudBlobClient blobClient = _cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName.Text);

            return await container.ExistsAsync();
        }

        /// <summary>
        /// Removes the table and contents
        /// </summary>
        public async Task RemoveContainer(CloudContainerName containerName)
        {
            Argument.CheckIfNull(containerName, "containerName");

            CloudBlobClient blobClient = _cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(containerName.Text);
            if (await ContainerExists(containerName))
            {
                container.Delete();
            }
        }
    }
}