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

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// Repository to save and load order summaries from the Document Database.
    /// </summary>
    public class DocumentDbOrderSummaryRepository : DocumentDbRepository, IOrderSummaryRepository
    {
        private const string LoadAllSummaries = "loadAllSummaries";
        private readonly Dictionary<string, string> _storedProcLinks;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDbOrderSummaryRepository"/> class.
        /// </summary>
        public DocumentDbOrderSummaryRepository(DocumentDbCredentials credentials, string database, string collection) : base(credentials, database, collection)
        {
            _storedProcLinks = new Dictionary<string, string>();
        }

        public override async Task Initialise(DocumentDbInitialiser initialiser)
        {
            await base.Initialise(initialiser);

            List<StoredProcedure> storedProcs = Client.CreateStoredProcedureQuery(CollectionUri).ToList();
            const string storedProc = @"
function loadAllSummaries(customerCode) {

    var context = getContext();
    var collection = context.getCollection();
    function callback(err, doc, options) {
        if(err) throw 'Error while querying documents for order summaries: ' + err;
        context.getResponse().setBody(doc);
    }

    // Begin query
    collection.queryDocuments(collection.getSelfLink(), 'SELECT * FROM OrderSummary s WHERE s.CustomerCode = ""' + customerCode + '""', {}, callback);
}";
            await CreatStoredProc(storedProcs, LoadAllSummaries, storedProc);
          
            
        }

        private async Task CreatStoredProc(IEnumerable<StoredProcedure> storedProcedures, string procName, string storedProcBody)
        {
            StoredProcedure existingProc = storedProcedures.SingleOrDefault(proc => proc.Id == procName);
            if (existingProc != null)
            {
                _storedProcLinks.Add(procName, existingProc.SelfLink);
            }
            else
            {
                StoredProcedure newProcedure = new StoredProcedure
                {
                    Id = procName,
                    Body = storedProcBody
                };

                ResourceResponse<StoredProcedure> createResponse = await Client.CreateStoredProcedureAsync(CollectionUri, newProcedure);
                _storedProcLinks.Add(procName, createResponse.Resource.SelfLink);
            }
        }

        /// <summary>
        /// Saves the specified summary.
        /// </summary>
        public async Task<SaveResponse> Save(OrderSummary summary)
        {
            Argument.CheckIfNull(summary, "summary");

            Document persistedDocument;
            if (summary.IsNew)
            {
                ResourceResponse<Document> newCreation = await Client.CreateDocumentAsync(CollectionUri, summary);
                persistedDocument = newCreation.Resource;
            }
            else
            {
                ResourceResponse<Document> updateResponse = await Client.ReplaceDocumentAsync(summary);
                persistedDocument = updateResponse.Resource;
            }
            Identifier identifier = new Identifier(persistedDocument.Id, new VersionNumber(persistedDocument.ETag));
            return new SaveResponse(identifier, persistedDocument);
        }

        /// <summary>
        /// Loads the reviews.
        /// </summary>
        public async Task<string> LoadSummariesByCustomerJson(string customerCode)
        {
            Argument.CheckIfNull(customerCode, "customerCode");

            try
            {
                string summariesProc = _storedProcLinks[LoadAllSummaries];
                StoredProcedureResponse<dynamic> response = await Client.ExecuteStoredProcedureAsync<dynamic>(summariesProc, customerCode);
                return response.Response.ToString();
            }
            catch (AggregateException aggregateException)
            {
                const string message = "Unable to find order summaries for customer {0}";
                throw new EntityNotFoundException(message.FormatWith(customerCode), aggregateException);
            }
        }

        public override async Task Uninitialise(DocumentDbInitialiser initialiser)
        {
            await base.Uninitialise(initialiser);
            _storedProcLinks.Clear();
        }
    }
}