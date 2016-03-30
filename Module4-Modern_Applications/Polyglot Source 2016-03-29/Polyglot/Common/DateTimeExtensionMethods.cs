using System;
using System.Globalization;

namespace Common
{
    /// <summary>
    /// Extension methods for date times.
    /// </summary>
    public static class DateTimeExtensionMethods
    {
        /// <summary>
        /// Takes a date and converts it to UTC format. 
        /// If the date is already in UTC format, it is returned in the same state.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>The date / time in UTC format.</returns>
        public static DateTime AsUniversalTime(this DateTime date)
        {
            return date.Kind == DateTimeKind.Utc ? date : date.ToUniversalTime();
        }

        /// <summary>
        /// Takes a date and converts it to UTC format.
        /// If the date is already in UTC format, it is returned in the same state.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns>
        /// The date / time in UTC format.
        /// </returns>
        /// <remarks>
        /// If the date provided is null the constant value of DateTime.MinValue is returned.
        /// </remarks>
        public static DateTime AsUniversalTime(this DateTime? date)
        {
            return date.HasValue ? date.Value.AsUniversalTime() : DateTime.MinValue;
        }

        /// <summary>
        /// A formatted long date and time string.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns>A formatted date and time.</returns>
        public static string ToLongDateAndTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("f", CultureInfo.CurrentCulture);
        }
    }
}