using System;
using System.Collections.Generic;
using System.Text;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Static extension methods
    /// </summary>
    public static class RowUtility
    {
        /// <summary>
        /// Checks that the incoming string is not null, empty or all white space
        /// </summary>
        /// <param name="value">The string to check</param>
        /// <param name="defaultValue">A default value if incoming string is null, empty, or white space</param>
        /// <returns>
        /// The incoming string unaltered, or <paramref name="defaultValue"/>
        /// </returns>
        public static string ToDefaultValue(this string value, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(value) ? defaultValue : value;
        }
    }
}