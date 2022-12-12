using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.QueueTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="QueueTable"/>
    /// </summary>
    public class QueueRow : RowObjectBase<QueueTable>
    {
        #region Properties
        /// <summary>
        /// Gets the default name value.
        /// </summary>
        public const string DefaultValue = "(none)";

        /// <summary>
        /// Gets the record id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets or sets the author name.
        /// </summary>
        public string Name
        {
            get => GetString(Columns.Name);
            set => SetValue(Columns.Name, value.ToDefaultValue(DefaultValue));
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueRow"/> class
        /// </summary>
        /// <param name="row">The data row</param>
        public QueueRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates a new <see cref="QueueRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static QueueRow Create(DataRow row)
        {
            return row != null ? new QueueRow(row) : null;
        }

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