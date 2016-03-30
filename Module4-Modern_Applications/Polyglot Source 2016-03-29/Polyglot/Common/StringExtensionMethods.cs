using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Common
{
    /// <summary>
    /// Extension methods for strings.
    /// </summary>
    public static class StringExtensionMethods
    {
        private const int MaxParseLengthOfDecimal = 28; 

        /// <summary>
        /// Returns true if the given text can be converted to a decimal. False otherwise. If the given
        /// text is longer than 29 characters which is close to the maximum size of a decimal it will be truncated.
        /// to 29 characters. This is a conversion that is inteneded for hanlding user input.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryParseDecimal(this string text, out decimal value)
        {
            if (string.IsNullOrEmpty(text))
            {
                value = 0;
                return true;
            }

            text = text.Substring(0, Math.Min(text.Length, MaxParseLengthOfDecimal));
            return Decimal.TryParse(text, out value);
        }

        /// <summary>
        /// Determines whether [is all lower case] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool IsAllLowercase(this String value)
        {
            Argument.CheckIfNull(value, "value");
            return string.Equals(value.ToLower(CultureInfo.CurrentCulture), value, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// Formats the specified value.
        /// </summary>
        public static string FormatWith(this String value, params object[] args)
        {
            Argument.CheckIfNull(value, "value");
            return string.Format(CultureInfo.CurrentCulture, value, args);
        }


        /// <summary>
        /// Converts string to lowercase.
        /// </summary>
        public static string ToLowercase(this String value)
        {
            Argument.CheckIfNull(value, "value");
            return value.ToLower(CultureInfo.CurrentCulture);
        }
        
        /// <summary>
        /// Determines whether [is not null or white space] [the specified instance].
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///   <c>true</c> if [is not null or white space] [the specified instance]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNullOrWhiteSpace(this string instance)
        {
            return !instance.IsNullOrWhiteSpace();
        }

        /// <summary>
        /// Determines whether [is null or white space] [the specified instance].
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///   <c>true</c> if [is null or white space] [the specified instance]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrWhiteSpace(this string instance)
        {
            return string.IsNullOrWhiteSpace(instance);
        }

        /// <summary>
        /// Determines whether a string contans another string.
        /// </summary>
        public static bool Contains(this string source, string value, StringComparison compareMode)
        {
            if (string.IsNullOrEmpty(source))
                return false;

            return source.IndexOf(value, compareMode) >= 0;
        }

        /// <summary>
        /// Trims the end off a string
        /// </summary>
        public static string TrimEnd(this string input, string suffixToRemove)
        {
            Argument.CheckIfNull(input, "input");
            Argument.CheckIfNull(suffixToRemove, "suffixToRemove");

            if (input != null && suffixToRemove != null && input.EndsWith(suffixToRemove, StringComparison.CurrentCulture))
            {
                return input.Substring(0, input.Length - suffixToRemove.Length);
            }

            return input;
        }

        /// <summary>
        /// Creates a single separated string out of a list of strings.
        /// </summary>
        public static string ToSeparated(this IEnumerable<string> source, string separator)
        {
            Argument.CheckIfNull(source, "source");
            Argument.CheckIfNull(separator, "separator");

            StringBuilder codes = new StringBuilder();
            foreach (string code in source)
            {
                codes.Append("{0}{1}".FormatWith(code, separator));
            }

            return codes.ToString().TrimEnd(separator);
        }

        /// <summary>
        /// Removes special characters from the string
        /// </summary>
        public static string RemoveLineBreaksAndCarriageReturnCharacters(this string input)
        {
            Argument.CheckIfNull(input, "input");
            return input.Replace("\n", string.Empty).Replace("\r", string.Empty);
        }
    }
}
