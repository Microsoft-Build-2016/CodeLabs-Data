using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Common;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using MSCorp.AdventureWorks.Core.Initialiser;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// The base repository access for the DocumentDb.
    /// </summary>
    public class DocumentDbRepository : IDisposable
    {
        private DocumentClient _client;
        private readonly DocumentDbCredentials _credentials;

        protected DocumentDbRepository(DocumentDbCredentials credentials, string database, string collection)
        {
            Argument.CheckIfNull(credentials, "credentials");
            Argument.CheckIfNull(collection, "collection");
            _credentials = credentials;
            Collection = collection;
            Database = database;
            CollectionUri = UriFactory.CreateDocumentCollectionUri(database, collection);
        }

        protected string Database
        {
            get; private set;
        }
        protected string Collection
        {
            get; private set;
        }
        protected Uri CollectionUri
        {
            get; private set;
        }

        public virtual async Task Initialise(DocumentDbInitialiser initialiser)
        {
            await InitialiseCollection(initialiser, Database, Collection);
        }

        protected static async Task InitialiseCollection(DocumentDbInitialiser initialiser, string database, string collection)
        {
            if (!await initialiser.CollectionExists(database, collection).ConfigureAwait(false))
            {
                await initialiser.CreateCollection(database, collection).ConfigureAwait(false);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uninitialise")]
        public virtual async Task Uninitialise(DocumentDbInitialiser initialiser)
        {
            if (string.IsNullOrWhiteSpace(Collection))
            {
                // Collection was not initialised or is already cleared.
                return;
            }

            if (await initialiser.CollectionExists(Database, Collection))
            {

                Uri collectonUri = UriFactory.CreateDocumentCollectionUri(Database, Collection);
                IOrderedQueryable<Trigger> triggers = Client.CreateTriggerQuery(collectonUri);
                foreach (Trigger trigger in triggers)
                {
                    try
                    {
                        await Client.DeleteTriggerAsync(trigger.SelfLink);
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

                IOrderedQueryable<StoredProcedure> storedProcs = Client.CreateStoredProcedureQuery(collectonUri);
                foreach (StoredProcedure storedProc in storedProcs)
                {
                    try
                    {
                        await Client.DeleteStoredProcedureAsync(storedProc.SelfLink);
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


                IOrderedQueryable<UserDefinedFunction> userFunctions =
                    Client.CreateUserDefinedFunctionQuery(collectonUri);
                foreach (UserDefinedFunction functions in userFunctions)
                {
                    try
                    {
                        await Client.DeleteUserDefinedFunctionAsync(functions.SelfLink);
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

            await UninitialiseCollection(initialiser, Database, Collection);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uninitialise")]
        protected static async Task UninitialiseCollection(DocumentDbInitialiser initialiser, string database, string collection)
        {
            if (await initialiser.CollectionExists(database, collection).ConfigureAwait(false))
            {
                await initialiser.RemoveCollection(database, collection).ConfigureAwait(false);
            }
        }

        //protected string Collection { get; private set; }

        protected DocumentClient Client
        {
            get
            {
                if (_client == null)
                {
                    Uri serviceEndpoint = new Uri(_credentials.EndpointUrl);
                    string authKey = _credentials.AuthorizationKey.ToInsecureString();
                    _client = new DocumentClient(serviceEndpoint, authKey, desiredConsistencyLevel: ConsistencyLevel.Session);
                }
                return _client;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_client != null)
                {
                    _client.Dispose();
                    _client = null;
                }
            }
        }
    }
}