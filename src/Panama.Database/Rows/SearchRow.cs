using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.SearchTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single read-only row from the <see cref="SearchTable"/>
    /// </summary>
    public class SearchRow : RowObjectBase<SearchTable>
    {
        public string Type => GetString(Columns.Type);
        public string File => GetString(Columns.File);
        public string Title => GetString(Columns.Title);
        public string Author => GetString(Columns.Author);
        public string Company => GetString(Columns.Company);
        public long Size => GetInt64(Columns.Size);
        public bool IsVersion => GetBoolean(Columns.IsVersion);
        public DateTime Created => GetDateTime(Columns.Created);
        public DateTime Modified => GetDateTime(Columns.Modified);

        private SearchRow(DataRow row) : base(row)
        {
        }

        /// <summary>
        /// Creates a new <see cref="SearchRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new search row, or null.</returns>
        public static SearchRow Create(DataRow row)
        {
            return row != null ? new SearchRow(row) : null;
        }
    }
}