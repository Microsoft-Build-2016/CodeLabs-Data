using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;

namespace Common
{
    /// <summary>
    /// Represents the system of money in use for a particular country
    /// </summary>
    public class Currency
    {
        private readonly string _code;

        private static readonly ConcurrentDictionary<string, RegionInfo> IsoCodeToRegion = new ConcurrentDictionary<string, RegionInfo>();

        /// <summary>
        /// Represents an empty currency
        /// </summary>
        private static readonly Currency Empty = new Currency("Empty");

        private static Currency _defaultCurrency = Empty;
        private readonly RegionInfo _regionInfo;
        private const int DecimalPlaces = 2;

        /// <summary>
        /// Configures the specified default currency.
        /// </summary>
        /// <param name="defaultCurrency">The default currency.</param>
        public static void Configure(Currency defaultCurrency)
        {
            _defaultCurrency = defaultCurrency;
        }

        /// <summary>
        /// Gets the default currency for the system.
        /// </summary>
        /// <value>The default.</value>
        public static Currency Default
        {
            get { return _defaultCurrency; }
        }

        /// <summary>
        /// The constructor of a <see cref="Currency"/>
        /// </summary>
        /// <param name="code">The code of the currency</param>
        public Currency(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentNullException(code);
            }
            _code = code;
            _regionInfo = RegionInfoFromCurrencyIso(code);
        }

        private static RegionInfo RegionInfoFromCurrencyIso(string isoCode)
        {
            if (!IsoCodeToRegion.ContainsKey(isoCode))
            {
                lock (IsoCodeToRegion)
                {
                    CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

                    foreach (CultureInfo ci in cultures)
                    {
                        RegionInfo ri = new RegionInfo(ci.LCID);
                        if (ri.ISOCurrencySymbol == isoCode)
                        {
                            IsoCodeToRegion.TryAdd(isoCode, ri);
                            return ri;
                        }
                    }
                    IsoCodeToRegion.TryAdd(isoCode, null);
                }
            }

            return IsoCodeToRegion[isoCode];
        }

        /// <summary>
        /// Gets the currency symbol.
        /// </summary>
        /// <value>The currency symbol.</value>
        public string CurrencySymbol
        {
            get
            {
                if (_regionInfo != null)
                {
                    return _regionInfo.CurrencySymbol;
                }
                return "?";
            }
        }

        private string CreateFormatString(int decimalPlaces)
        {
            return "{0}{{0:N{1}}}".FormatWith(CurrencySymbol, decimalPlaces);
        }

        /// <summary>
        /// Formats the specified amount using this currency
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public string Format(decimal amount)
        {
            decimal roundedAmount = Math.Round(amount, DecimalPlaces, MidpointRounding.AwayFromZero);

            NumberFormatInfo localNumberFormatter = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            localNumberFormatter.CurrencySymbol = CurrencySymbol;

            return string.Format(localNumberFormatter, "{0:C}", roundedAmount);
        }

        /// <summary>
        /// Formats the specified amount using this currency
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns></returns>
        public string ShortFormat(decimal amount)
        {
            decimal roundedAmount = Math.Round(amount, 0, MidpointRounding.AwayFromZero);
            return CreateFormatString(0).FormatWith(roundedAmount);
        }

        /// <summary>
        /// The currency code.
        /// </summary>
        public string Code
        {
            get { return _code; }
        }

        /// <summary>
        /// Returns true if the currency values are equal
        /// </summary>
        public static bool operator ==(Currency currency1, Currency currency2)
        {
            return Equals(currency1, currency2);
        }

        /// <summary>
        /// Returns true if the currency values are not equal
        /// </summary>
        public static bool operator !=(Currency currency1, Currency currency2)
        {
            return !(currency1 == currency2);
        }

        /// <summary>
        /// Returns true if the object provided is a <see cref="Currency"/> and the id and code are the same
        /// </summary>
        public override bool Equals(object obj)
        {
            Currency other = obj as Currency;
            if (other == null)
            {
                return false;
            }
            return Code == other.Code;
        }

        ///<summary>
        /// Serves as a hash function for a particular type. 
        ///</summary>
        ///<returns>
        /// A hash code for the current <see cref="Currency" />.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
    }
}