namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides static values for navigation groups.
    /// </summary>
    public static class NavigationGroup
    {
        /* Values must be sequential; they represent indices into collections.*/
        /// <summary>
        /// Titles, publishers, submissions
        /// </summary>
        public const int Title = 0;

        /// <summary>
        /// Tools, various other utilities.
        /// </summary>
        public const int Tools = 1;

        /// <summary>
        /// Other, alerts, links, etc.
        /// </summary>
        public const int Other = 2;
    }
}