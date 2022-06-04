using System;

namespace Restless.Panama.Utility
{
    /// <summary>
    /// Provides simple static methods to throw exceptions
    /// </summary>
    public static class Throw
    {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if <paramref name="obj"/> is null.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        public static void IfNull(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if <paramref name="str"/> is null, empty, or consists only of white space.
        /// </summary>
        /// <param name="str">The string to check</param>
        public static void IfEmpty(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException("Empty or null string");
            }
        }
    }
}