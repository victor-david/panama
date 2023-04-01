namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides an enumeration of title row filter types
    /// </summary>
    public enum TitleRowFilterType
    {
        /// <summary>
        /// Filter to a specified id
        /// </summary>
        Id,
        /// <summary>
        /// Filter to a collection of specified ids
        /// </summary>
        MultipleId,
        /// <summary>
        /// Filter according to the specified text
        /// </summary>
        Text,
        /// <summary>
        /// Filter according to the directory where a title is stored
        /// </summary>
        Directory,
        /// <summary>
        /// Title is flagged as ready
        /// </summary>
        Ready,
        /// <summary>
        /// Title is flagged with the quick flag
        /// </summary>
        Flagged,
        /// <summary>
        /// Title is currently submitted
        /// </summary>
        CurrentlySubmitted,
        /// <summary>
        /// Title has been submitted at least once, not necessarily currently submitted
        /// </summary>
        EverSubmitted,
        /// <summary>
        /// Title has been published
        /// </summary>
        Published,
        /// <summary>
        /// Title has been self published
        /// </summary>
        SelfPublished,
        /// <summary>
        /// Filter according to word count
        /// </summary>
        WordCount,
        /// <summary>
        /// Filter according to title tag or tags
        /// </summary>
        Tag
    }
}