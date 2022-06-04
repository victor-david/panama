using Restless.Toolkit.Core.Database.SQLite;
using System.Data;
using Columns = Restless.Panama.Database.Tables.TitleTagTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="TitleTagTable"/>.
    /// </summary>
    public class TitleTagRow : RowObjectBase<TitleTagTable>
    {
        #region Properties
        /// <summary>
        /// Gets the title id
        /// </summary>
        public long TitleId => GetInt64(Columns.TitleId);

        /// <summary>
        /// Gets the tag id
        /// </summary>
        public long TagId => GetInt64(Columns.TagId);

        /// <summary>
        /// Gets the tag name
        /// </summary>
        public string TagName => GetString(Columns.Joined.TagName);

        /// <summary>
        /// Gets the tag description
        /// </summary>
        public string TagDescription => GetString(Columns.Joined.TagDescription);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleTagRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public TitleTagRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return TagName;
        }
        #endregion
    }
}