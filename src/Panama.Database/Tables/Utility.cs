using System;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Static extension methods
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Checks that the incoming string is not null, empty or all white space
        /// </summary>
        /// <param name="value">The string to check</param>
        /// <param name="defaultValue">A default value if incoming string is null, empty, or white space</param>
        /// <returns>
        /// The incoming string unaltered, or <paramref name="defaultValue"/>
        /// </returns>
        public static string ToDefaultValue(this string value, string defaultValue) => string.IsNullOrWhiteSpace(value) ? defaultValue : value;

        /// <summary>
        /// Gets the current date as UTC with hours offset, but without any minutes / seconds.
        /// </summary>
        /// <returns>A date time</returns>
        public static DateTime GetUtcNowZero()
        {
            DateTime now = DateTime.Now;
            return new DateTime(now.Year, now.Month, now.Day).ToUniversalTime();
        }

        /// <summary>
        /// Gets a date / time based on the specified date / time without andy minutes / seconds, and in universal time.
        /// </summary>
        /// <param name="dateTime">The date time</param>
        /// <returns>A new date / time</returns>
        public static DateTime ToUtcZero(this DateTime dateTime) => new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).ToUniversalTime();
    }
}