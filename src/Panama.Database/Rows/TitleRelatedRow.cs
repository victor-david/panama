using Restless.Toolkit.Core.Database.SQLite;
using System.Data;
using Columns = Restless.Panama.Database.Tables.TitleRelatedTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="TitleRelatedTable"/>.
    /// </summary>
    public class TitleRelatedRow : RowObjectBase<TitleRelatedTable>
    {
        #region Properties
        /// <summary>
        /// Gets the title id
        /// </summary>
        public long TitleId => GetInt64(Columns.TitleId);

        /// <summary>
        /// Gets the related title id
        /// </summary>
        public long RelatedId => GetInt64(Columns.RelatedId);

        /// <summary>
        /// Gets the title
        /// </summary>
        public string Title => GetString(Columns.Joined.Title);

        /// <summary>
        /// Gets the related title
        /// </summary>
        public string RelatedTitle => GetString(Columns.Joined.RelatedTitle);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleRelatedRow"/> class.
        /// </summary>
        /// <param name="row">The data row</param>
        public TitleRelatedRow(DataRow row) : base(row)
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
            return $"{Title} => {RelatedTitle}";
        }
        #endregion
    }
}