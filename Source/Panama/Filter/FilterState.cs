namespace Restless.App.Panama.Filter
{
    /// <summary>
    /// Provides the enumeration values that describe the state of a filter.
    /// </summary>
    public enum FilterState
    {
        /// <summary>
        /// No. The value compared to the filter state must be false
        /// </summary>
        No = 0,
        /// <summary>
        /// Yes. The value compared to the filter state must be true.
        /// </summary>
        Yes = 1,
        /// <summary>
        /// Either. The filter is not used.
        /// </summary>
        Either = 2
    }
}
