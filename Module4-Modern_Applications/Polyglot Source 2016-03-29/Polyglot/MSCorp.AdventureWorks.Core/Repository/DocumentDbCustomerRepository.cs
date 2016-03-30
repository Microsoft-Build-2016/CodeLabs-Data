using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using MSCorp.AdventureWorks.Core.Domain;
using MSCorp.AdventureWorks.Core.Initialiser;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Repository to save and load customers from the Document Database
    /// </summary>
    public class DocumentDbCustomerRepository : DocumentDbRepository, ICustomerRepository
    {
        private readonly DocumentDbCustomerCollection[] _collections;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDbCustomerRepository"/> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public DocumentDbCustomerRepository(DocumentDbCredentials credentials, string database, params DocumentDbCustomerCollection[] collections) : base(credentials, database, collections?[0].Name)
        {
            Argument.CheckIfNullOrEmpty(collections, "collections");
            _collections = collections;
        }

        public async override Task Initialise(DocumentDbInitialiser initialiser)
        {
            foreach (var collection in _collections)
            {
                await InitialiseCollection(initialiser, Database, collection.Name);
            }
        }

        public async override Task Uninitialise(DocumentDbInitialiser initialiser)
        {
            foreach (var collection in _collections)
            {
                await UninitialiseCollection(initialiser, Database, collection.Name);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public string CollectionShardForCustomer(string lastName)
        {
            if (!string.IsNullOrWhiteSpace(lastName))
            {
                foreach (var collection in _collections)
                {
                    try
                    {
                        if (collection.InRange(lastName))
                        {
                            return collection.Name;
                        }
                    }
                    catch (Exception)
                    {
                        // If something dies here, fallback to the first collection.
                        return Collection;
                    }
                }
            }
            return Collection;
        }

        /// <summary>
        /// Saves the specified customer.
        /// </summary>
        public async Task<SaveResponse> Save(Customer customer)
        {
            Argument.CheckIfNull(customer, "customer");

            Document persistedDocument;
            if (customer.IsNew)
            {
                string collection = CollectionShardForCustomer(customer.LastName);
                ResourceResponse<Document> newCreation = await Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(Database, collection), customer).ConfigureAwait(false);
                persistedDocument = newCreation.Resource;
            }
            else
            {
                ResourceResponse<Document> updateResponse = await Client.ReplaceDocumentAsync(customer).ConfigureAwait(false);
                persistedDocument = updateResponse.Resource;
            }
            Identifier identifier = new Identifier(persistedDocument.Id, new VersionNumber(persistedDocument.ETag));
            return new SaveResponse(identifier, persistedDocument);
        }

        /// <summary>
        /// Loads the customer.
        /// </summary>
        public async Task<Customer> LoadCustomer(Identifier identifier)
        {
            Argument.CheckIfNull(identifier, "identifier");
            return await Task.Run(() => LoadCustomerById(identifier));
        }

        private Customer LoadCustomerById(Identifier identifier)
        {
            List<Customer> customers = new List<Customer>();

            foreach (var collection in _collections)
            {
                IQueryable<Customer> query =
                    from customer in Client.CreateDocumentQuery<Customer>(UriFactory.CreateDocumentCollectionUri(Database, collection.Name))
                    where customer.Id == identifier.Value
                    select customer;

                customers.AddRange(query.AsEnumerable());
            }

            try
            {
                return customers.FirstOrDefault();
            }
            catch (AggregateException aggregateException)
            {
                const string message = "Unable to find document with id {0}.  Document either does not exist or an unexpected error occurred on the server";
                throw new EntityNotFoundException(message.FormatWith(identifier.Value), aggregateException);
            }
        }

        /// <summary>
        /// Loads all customers.
        /// </summary>
        public IEnumerable<Customer> LoadAllCustomers()
        {
            List<Customer> customers = new List<Customer>();
            foreach (var collection in _collections)
            {
                IQueryable<Customer> query = Client.CreateDocumentQuery<Customer>(UriFactory.CreateDocumentCollectionUri(Database, collection.Name));
                customers.AddRange(query.AsEnumerable());
            }
            return customers;
        }
    }
}