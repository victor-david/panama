namespace Restless.Panama.Database.Core
{
    /// <summary>
    /// Provides a sort direction enumeration
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Direction is ascending
        /// </summary>
        Ascending,
        /// <summary>
        /// Direction is descending
        /// </summary>
        Descending
    }

    internal static class SortDirectionExtensions
    {
        internal static string ToSql(this SortDirection sort)
        {
            return sort == SortDirection.Ascending ? "ASC" : "DESC";
        }
    }
}