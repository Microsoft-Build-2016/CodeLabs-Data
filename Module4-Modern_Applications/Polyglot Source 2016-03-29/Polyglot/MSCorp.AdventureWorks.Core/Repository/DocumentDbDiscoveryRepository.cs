using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using MSCorp.AdventureWorks.Core.Initialiser;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// A repository for exploring triggers and procs.
    /// </summary>
    public class DocumentDbDiscoveryRepository : IDisposable
    {
        private DocumentClient _client;
        private readonly DocumentDbCredentials _credentials;
        private readonly string _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDbDiscoveryRepository"/> class.
        /// </summary>
        public DocumentDbDiscoveryRepository(DocumentDbCredentials credentials, string database)
        {
            Argument.CheckIfNull(credentials, "credentials");
            _credentials = credentials;
            _database = database;
        }

        public DocumentClient Client
        {
            get
            {
                if (_client == null)
                {
                    Uri serviceEndpoint = new Uri(_credentials.EndpointUrl);
                    string authKey = _credentials.AuthorizationKey.ToInsecureString();
                    _client = new DocumentClient(serviceEndpoint, authKey, desiredConsistencyLevel:ConsistencyLevel.Session);
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

        public IEnumerable<Trigger> TriggersFor(string collectionName)
        {
            IOrderedQueryable<Trigger> triggers = Client.CreateTriggerQuery(UriFactory.CreateDocumentCollectionUri(_database, collectionName));
            return triggers.ToList();
        }

        public IEnumerable<StoredProcedure> StoredProceduresFor(string collectionName)
        {
            IOrderedQueryable<StoredProcedure> procs = Client.CreateStoredProcedureQuery(UriFactory.CreateDocumentCollectionUri(_database, collectionName));
            return procs.ToList();
        }

        public IEnumerable<UserDefinedFunction> UserDefinedFunctionsFor(string collectionName)
        {
            IOrderedQueryable<UserDefinedFunction> functions = Client.CreateUserDefinedFunctionQuery(UriFactory.CreateDocumentCollectionUri(_database, collectionName));
            return functions.ToList();
        }
    }
}