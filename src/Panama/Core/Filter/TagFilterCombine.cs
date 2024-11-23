namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides enumeration values that describe how multiple tag selections in a title filter are combined.
    /// </summary>
    public enum TagFilterCombine
    {
        /// <summary>
        /// Any of the tags. Selections are combined with OR.
        /// </summary>
        Any,
        /// <summary>
        /// All of the tags. Selections are combined with AND.
        /// </summary>
        All,
    }
}