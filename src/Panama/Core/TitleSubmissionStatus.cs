namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides an enumeration of value that can be applied to a <see cref="TitleSubmission"/>
    /// </summary>
    public enum TitleSubmissionStatus
    {
        /// <summary>
        /// Title submission has no conflicts
        /// </summary>
        Okay,
        /// <summary>
        /// Title is currently submitted to a publisher that does not accept simultaneous
        /// </summary>
        Exclusive,
        /// <summary>
        /// Title has been previously submitted to the same publisher
        /// </summary>
        SamePublisher
    }
}