﻿using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.QueueTitleTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="QueueTitleTable"/>
    /// </summary>
    public class QueueTitleRow : RowObjectBase<QueueTitleTable>
    {
        #region Properties
        /// <summary>
        /// Gets the default name value.
        /// </summary>
        public const string DefaultValue = "(none)";

        /// <summary>
        /// Gets the queue id.
        /// </summary>
        public long QueueId => GetInt64(Columns.QueueId);

        /// <summary>
        /// Gets the title id.
        /// </summary>
        public long TitleId => GetInt64(Columns.TitleId);

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public long Status
        {
            get => GetInt64(Columns.Status);
            set => SetValue(Columns.Status, value);
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime ?Date
        {
            get => GetNullableDateTime(Columns.Date);
            set => SetValue(Columns.Date, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueTitleRow"/> class
        /// </summary>
        /// <param name="row">The data row</param>
        public QueueTitleRow(DataRow row) : base(row)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates a new <see cref="QueueTitleRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static QueueTitleRow Create(DataRow row)
        {
            return row != null ? new QueueTitleRow(row) : null;
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{QueueId} => {TitleId} {Status}";
        }
        #endregion
    }
}