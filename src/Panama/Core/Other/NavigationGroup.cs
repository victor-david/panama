namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides static values for navigation groups.
    /// </summary>
    /// <remarks>
    /// Values must be sequential; they represent indices into collections.
    /// </remarks>
    public static class NavigationGroup
    {
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

        /// <summary>
        /// Only in a menu (not in a navigation control)
        /// </summary>
        public const int OnlyMenu = 3;

        /// <summary>
        /// Not a navigation group; used to intitialize the navigator collection.
        /// If adding another group, increase this value
        /// </summary>
        public const int TotalNumberOfGroups = 4;
    }
}