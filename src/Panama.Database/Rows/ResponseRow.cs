using Restless.Toolkit.Core.Database.SQLite;
using System.Data;
using Columns = Restless.Panama.Database.Tables.ResponseTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="ResponseTable"/>
    /// </summary>
    public class ResponseRow : RowObjectBase<ResponseTable>
    {
        #region Properties
        /// <summary>
        /// Gets the record id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets the response name.
        /// </summary>
        public string Name => GetString(Columns.Name);

        /// <summary>
        /// Gets the response description.
        /// </summary>
        public string Description => GetString(Columns.Description);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseRow"/> class
        /// </summary>
        /// <param name="row">The data row</param>
        internal ResponseRow(DataRow row) : base(row)
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
            return Name;
        }
        #endregion
    }
}