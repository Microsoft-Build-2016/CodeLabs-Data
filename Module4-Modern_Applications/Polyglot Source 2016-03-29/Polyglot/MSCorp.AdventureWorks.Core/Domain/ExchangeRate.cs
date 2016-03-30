using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Common;
using Microsoft.Azure.Documents;
using MSCorp.AdventureWorks.Core.Repository;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// Represents a currency exchange rate.
    /// </summary>
    [DataContract]
    public class ExchangeRate : Document
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRate"/> class.
        /// </summary>
        public ExchangeRate()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExchangeRate"/> class.
        /// </summary>
        public ExchangeRate(Currency currency, IEnumerable<ForeignRate> rates)
        {
            Argument.CheckIfNull(currency, "Currency");

            Currency = currency;
            Rates = rates.ToList();
        }

        /// <summary>
        /// Gets the base currency.
        /// </summary>
        public Currency Currency
        {
            get { return this.GetCustomValue<Currency>(); }
            private set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets the value that one unit of <see cref="Currency"/> can purchase in a foreign currency.
        /// </summary>
        public IEnumerable<ForeignRate> Rates
        {
            get { return this.GetCustomValue<List<ForeignRate>>(); }
            set { this.SetCustomPropertyValue(value); }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        [JsonIgnore]
        public bool IsNew { get { return string.IsNullOrWhiteSpace(Id); } }

        /// <summary>
        /// Saves the <see cref="ExchangeRate"/> into the repository.
        /// </summary>
        public async Task Save(ICurrencyRepository repository)
        {
            Argument.CheckIfNull(repository, "repository");
            SaveResponse response = await repository.Save(this);
            Id = response.Identifier.Value;
            SetPropertyValue(DocumentIdentity.Etag, response.Identifier.VersionNumber.Value);
        }
    }
}