namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides enumeration values that describe the possible preview modes.
    /// </summary>
    public enum PreviewMode
    {
        /// <summary>
        /// Preview mode is not set
        /// </summary>
        None,
        /// <summary>
        /// Preview is text.
        /// </summary>
        Text,
        /// <summary>
        /// Preview is an image
        /// </summary>
        Image,
        /// <summary>
        /// The type of preview is unknown or unsupported.
        /// </summary>
        Unsupported
    }
}