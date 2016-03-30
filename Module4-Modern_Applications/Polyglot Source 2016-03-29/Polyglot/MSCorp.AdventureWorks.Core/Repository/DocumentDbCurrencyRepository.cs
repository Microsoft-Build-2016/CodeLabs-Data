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
    /// Repository to save and load currency data from the Document Database
    /// </summary>
    public class DocumentDbCurrencyRepository : DocumentDbRepository, ICurrencyRepository
    {
        private const string LoadAllCurrencies = "loadAllCurrencies";
        private readonly Dictionary<string, string> _storedProcLinks;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentDbCurrencyRepository"/> class.
        /// </summary>
        public DocumentDbCurrencyRepository(DocumentDbCredentials credentials, string database, string collection)
            : base(credentials, database, collection)
        {
            _storedProcLinks = new Dictionary<string, string>();
        }

        public override async Task Initialise(DocumentDbInitialiser initialiser)
        {
            await base.Initialise(initialiser);
            List<StoredProcedure> storedProcs = Client.CreateStoredProcedureQuery(CollectionUri).ToList();

            const string storedProc = @"
function loadAllCurrencies() {

    var context = getContext();
    var collection = context.getCollection();
    function callback(err, doc, options) {
        if(err) throw 'Error while querying documents for exchange rates: ' + err;
        context.getResponse().setBody(doc);
    }

    // Begin query
    collection.queryDocuments(collection.getSelfLink(), 'SELECT * FROM ExchangeRate e WHERE e.Rates != {}', {}, callback);
}";

            await CreatStoredProc(storedProcs, LoadAllCurrencies, storedProc);
        }

        public override async Task Uninitialise(DocumentDbInitialiser initialiser)
        {
            await base.Uninitialise(initialiser);
            _storedProcLinks.Clear();
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
        /// Saves the specified review.
        /// </summary>
        public async Task<SaveResponse> Save(ExchangeRate exchangeRate)
        {
            Argument.CheckIfNull(exchangeRate, "exchangeRate");

            Document persistedDocument;
            if (exchangeRate.IsNew)
            {
                ResourceResponse<Document> newCreation = await Client.CreateDocumentAsync(CollectionUri, exchangeRate).ConfigureAwait(false);
                persistedDocument = newCreation.Resource;
            }
            else
            {
                ResourceResponse<Document> updateResponse = await Client.ReplaceDocumentAsync(exchangeRate).ConfigureAwait(false);
                persistedDocument = updateResponse.Resource;
            }
            Identifier identifier = new Identifier(persistedDocument.Id, new VersionNumber(persistedDocument.ETag));
            return new SaveResponse(identifier, persistedDocument);
        }

        /// <summary>
        /// Loads the reviews.
        /// </summary>
        public Task<ExchangeRate> LoadExchangeRate(string currencyCode)
        {
            Argument.CheckIfNull(currencyCode, "currencyCode");
            return Task.Run(() => LoadRateByCode(currencyCode));
        }
        
        private ExchangeRate LoadRateByCode(string currencyCode)
        {
            IQueryable<ExchangeRate> query =
                from rate in Client.CreateDocumentQuery<ExchangeRate>(CollectionUri)
                where rate.Currency.Code == currencyCode
                select rate;

            IEnumerable<ExchangeRate> results = query.AsEnumerable();

            try
            {
                return results.SingleOrDefault();
            }
            catch (AggregateException aggregateException)
            {
                const string message = "Unable to find document with id {0}.  Document either does not exist or an unexpected error occurred on the server";
                throw new EntityNotFoundException(message.FormatWith(currencyCode), aggregateException);
            }
        }

        /// <summary>
        /// Load all exchange rates and returns a single JSON string.
        /// </summary>
        public async Task<string> LoadAllExchangeRatesJsonAsync()
        {
            try
            {
                string procId = _storedProcLinks[LoadAllCurrencies];
                StoredProcedureResponse<dynamic> response =
                    await Client.ExecuteStoredProcedureAsync<dynamic>(procId);
                return response.Response.ToString();
            }
            catch (AggregateException aggregateException)
            {
                const string message = "Unable to find exchange rates.";
                throw new EntityNotFoundException(message, aggregateException);
            }
        }
    }
}