using System;

namespace Common
{
    /// <summary>
    /// Extension methods for guids.
    /// </summary>
    public static class GuidExtensionMethods
    {
        /// <summary>
        /// Determines whether [is all lower case] [the specified value].
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string ToDashlessString(this Guid value)
        {
            Argument.CheckIfNull(value, "value");
            return value.ToString("N");
        }
    }
}