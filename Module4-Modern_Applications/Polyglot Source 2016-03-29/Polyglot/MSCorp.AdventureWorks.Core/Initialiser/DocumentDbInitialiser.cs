using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using MSCorp.AdventureWorks.Core.Repository;

namespace MSCorp.AdventureWorks.Core.Initialiser
{
    /// <summary>
    /// Initialises a Document Db
    /// </summary>
    public class DocumentDbInitialiser
    {
        private readonly DocumentDbCredentials _credentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDbInitialiser"/> class.
        /// </summary>
        public DocumentDbInitialiser(DocumentDbCredentials credentials)
        {
            Argument.CheckIfNull(credentials, "credentials");
            _credentials = credentials;
        }

        /// <summary>
        /// Creates the document database.
        /// </summary>
        public async Task CreateDocumentDatabase(string documentDatabase)
        {
            Argument.CheckIfNull(documentDatabase, "collection");
            Console.WriteLine("Create Doc DB : '{0}'".FormatWith(documentDatabase));
            using (DocumentClient cli = CreateClient(ConsistencyLevel.Session))
            {
                ResourceResponse<Database> response =
                    await cli.CreateDatabaseAsync(new Database { Id = documentDatabase }, new RequestOptions { ConsistencyLevel = ConsistencyLevel.Session }).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Checks if a database exists. 
        /// </summary>
        public async Task<bool> DatabaseExists(string documentDatabase)
        {
            Argument.CheckIfNull(documentDatabase, "database");

            using (DocumentClient cli = CreateClient())
            {
                try
                {
                    FeedResponse<Database> databases = await cli.ReadDatabaseFeedAsync().ConfigureAwait(false);
                    UriFactory.CreateDatabaseUri(documentDatabase);
                    var database = await cli.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(documentDatabase));
                    if (database.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;
                    }
                }
                catch (DocumentClientException ex)
                {
                    //Simply return false if this is the error. Else throw it.
                    if (ex.StatusCode != HttpStatusCode.NotFound)
                    {
                        throw;
                    }
                }
                return false;
            }

        }
        /// <summary>
        /// Removes the database.
        /// </summary>
        public async Task RemoveDatabase(string documentDatabase)
        {
            Argument.CheckIfNull(documentDatabase, "documentDatabase");

            using (DocumentClient cli = CreateClient())
            {
                await cli.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(documentDatabase)).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Creates the document collection.
        /// </summary>
        public async Task CreateCollection(string database, string collection)
        {
            Argument.CheckIfNull(collection, "collection");
            Argument.CheckIfNull(database, "database");

            using (DocumentClient cli = CreateClient())
            {
                ResourceResponse<DocumentCollection> response = await cli.CreateDocumentCollectionAsync(UriFactory.CreateDatabaseUri(database), new DocumentCollection { Id = collection }).ConfigureAwait(false);
                Console.WriteLine("Create Collectoin in DB {0} with Collectoin Name : {1}".FormatWith(database, collection));
            }
        }


        private DocumentClient CreateClient(ConsistencyLevel? desiredConsistencyLevel = null)
        {
            return new DocumentClient(new Uri(_credentials.EndpointUrl), _credentials.AuthorizationKey.ToInsecureString(), desiredConsistencyLevel: desiredConsistencyLevel);
        }

        /// <summary>
        /// Checks if a collection exists. 
        /// </summary>
        public async Task<bool> CollectionExists(string database, string collection)
        {
            Argument.CheckIfNull(database, "database");
            Argument.CheckIfNull(collection, "collection");

            using (DocumentClient cli = CreateClient())
            {
                try
                {
                    var collectionResponse = await cli.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(database, collection)).ConfigureAwait(false);

                    if (collectionResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return true;
                    }
                }
                catch (DocumentClientException ex)
                {
                    //Simply return false if this is the error. Else throw it.
                    if (ex.StatusCode != HttpStatusCode.NotFound)
                    {
                        throw;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Removes the collection.
        /// </summary>
        public async Task RemoveCollection(string database, string collection)
        {
            Argument.CheckIfNull(database, "database");
            Argument.CheckIfNull(collection, "collection");
            Console.WriteLine("Remove Collection {0}".FormatWith(collection));
            using (DocumentClient cli = CreateClient())
            {
                try
                {
                    await cli.DeleteDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(database, collection)).ConfigureAwait(false);
                }
                catch (DocumentClientException ex)
                {
                    //Doesn't matter as we were deleting it anyway!
                    if (ex.StatusCode != HttpStatusCode.NotFound)
                    {
                        throw;
                    }
                }
            }
        }
    }
}