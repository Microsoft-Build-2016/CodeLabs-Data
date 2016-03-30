using Common;
using Newtonsoft.Json;

namespace MSCorp.AdventureWorks.Core.Domain
{
    /// <summary>
    /// The foreign currency rate for an exchange.
    /// </summary>
    public class ForeignRate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForeignRate"/> class.
        /// </summary>
        [JsonConstructor]
        public ForeignRate(Currency currency, double rate)
        {
            Currency = currency;
            Rate = rate;
        }
        /// <summary>
        /// Gets the currency.
        /// </summary>
        public Currency Currency { get; private set; }

        /// <summary>
        /// Gets the rate.
        /// </summary>
        public double Rate { get; set; }

        /// <summary>
        /// Gets the key.
        /// </summary>
        [JsonIgnore]
        public string Key { get { return Currency.Code; } }
    }
}