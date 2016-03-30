using System;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    /// <summary>
    /// Extension methods for Linq queries for the <see cref="Money"/> class
    /// </summary>
    public static class MoneyExtensionMethods
    {
        /// <summary>
        /// Sums the items in the specified source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Money Sum(this IEnumerable<Money> source)
        {
            Money result = source.First();
            return source.Skip(1).Aggregate(result, (current, money) => current.Add(money));
        }

        /// <summary>
        /// Sums the items in the specified source.
        /// </summary>
        public static Money Sum(this IEnumerable<Money> source, Currency currency)
        {
            Money result = new Money(0, currency);
            return source.Aggregate(result, (current, money) => current.Add(money));
        }

        /// <summary>
        /// Sums the items in the specified source.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        public static Money Sum<T>(this IEnumerable<T> source, Func<T, Money> selector)
        {
            return source.Select(selector).Sum();
        }

        /// <summary>
        /// Sums the items in the specified source.
        /// </summary>
        public static Money Sum<T>(this IEnumerable<T> source, Func<T, Money> selector, Currency currency)
        {
            return source.Select(selector).Sum(currency);
        }

    }
}