namespace Restless.Panama.Core.Filter
{
    /// <summary>
    /// Provides an enumeration of submission row filter types
    /// </summary>
    public enum SubmissionRowFilterType
    {
        /// <summary>
        /// Filter to a specified id
        /// </summary>
        Id,
        /// <summary>
        /// Filter according to the specified text
        /// </summary>
        Text,
        /// <summary>
        /// Submission is active
        /// </summary>
        Active,
        /// <summary>
        /// Submssion has a try again response
        /// </summary>
        TryAgain,
        /// <summary>
        /// Submission has a personal response
        /// </summary>
        Personal,
        /// <summary>
        /// Submission has an accepted response
        /// </summary>
        Accepted,
        /// <summary>
        /// Submission has a withdrawn state
        /// </summary>
        Withdrawn,
    }
}