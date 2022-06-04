using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.OrphanExclusionTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    public class OrphanExclusionRow : RowObjectBase<OrphanExclusionTable>
    {
        #region Properties
        /// <summary>
        /// Gets the id
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets the type
        /// </summary>
        public long Type => GetInt64(Columns.Type);

        /// <summary>
        /// Gets a boolean that indicates whether this is a system generated exclusion
        /// </summary>
        public bool IsSystem => GetBoolean(Columns.IsSystem);

        /// <summary>
        /// Gets the value
        /// </summary>
        public string Value => GetString(Columns.Value);

        /// <summary>
        /// Gets the created date
        /// </summary>
        public DateTime Created => GetDateTime(Columns.Created);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OrphanExclusionRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public OrphanExclusionRow(DataRow row) : base(row)
        {
        }

        /// <summary>
        /// Creates a new <see cref="OrphanExclusionRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static OrphanExclusionRow Create(DataRow row)
        {
            return row != null ? new OrphanExclusionRow(row) : null;
        }
        #endregion
    }
}