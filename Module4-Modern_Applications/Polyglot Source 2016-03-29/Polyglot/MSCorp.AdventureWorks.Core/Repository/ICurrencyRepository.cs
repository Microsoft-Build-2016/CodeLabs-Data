using System.Collections.Generic;
using System.Threading.Tasks;
using MSCorp.AdventureWorks.Core.Domain;

namespace MSCorp.AdventureWorks.Core.Repository
{
    /// <summary>
    /// A repository for currency.
    /// </summary>
    public interface ICurrencyRepository
    {
        /// <summary>
        /// Saves the specified exchangeRate.
        /// </summary>
        Task<SaveResponse> Save(ExchangeRate exchangeRate);

        /// <summary>
        /// Loads the exchange rate.
        /// </summary>
        Task<ExchangeRate> LoadExchangeRate(string currencyCode);

        /// <summary>
        /// Load all exchange rates.
        /// </summary>
        Task<string> LoadAllExchangeRatesJsonAsync();
    }
}