namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides an enumeration of publisher row filter types
    /// </summary>
    public enum PublisherRowFilterType
    {
        /// <summary>
        /// Publisher is active, not a goner
        /// </summary>
        Active,
        /// <summary>
        /// Publisher has at leat one open submission
        /// </summary>
        OpenSubmission,
        /// <summary>
        /// Publisher is currently in one of its submission periods
        /// </summary>
        InPeriod,
        /// <summary>
        /// Publisher is flagged as exclusive (no simultaneous submissions)
        /// </summary>
        Exclusive,
        /// <summary>
        /// Publisher is flagged for follow up
        /// </summary>
        FollowUp,
        /// <summary>
        /// Publisher is flagged as a paying market
        /// </summary>
        Paying,
        /// <summary>
        /// Publisher is flagged as a goner
        /// </summary>
        Goner
    }
}