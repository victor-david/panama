using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Columns = Restless.Panama.Database.Tables.SubmissionTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="SubmissionTable"/>
    /// </summary>
    public class SubmissionRow : RowObjectBase<SubmissionTable>
    {
        #region Properties
        /// <summary>
        /// Gets the record id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets the title id.
        /// </summary>
        public long TitleId => GetInt64(Columns.TitleId);

        /// <summary>
        /// Gets the batch id.
        /// </summary>
        public long BatchId => GetInt64(Columns.BatchId);

        /// <summary>
        /// Gets or sets the ordering
        /// </summary>
        public long Ordering
        {
            get => GetInt64(Columns.Ordering);
            set => SetValue(Columns.Ordering, value);
        }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public long Status
        {
            get => GetInt64(Columns.Status);
            set => SetValue(Columns.Status, value);
        }

        /// <summary>
        /// Gets the date / time record added
        /// </summary>
        public DateTime Added => GetDateTime(Columns.Added);

        /// <summary>
        /// Gets the title
        /// </summary>
        public string Title => GetString(Columns.Joined.Title);
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionRow"/> class
        /// </summary>
        /// <param name="row">The data row</param>
        private SubmissionRow(DataRow row) : base(row)
        {
        }

        /// <summary>
        /// Creates a new <see cref="SubmissionRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new tag row, or null.</returns>
        public static SubmissionRow Create(DataRow row)
        {
            return row != null ? new SubmissionRow(row) : null;
        }
        #endregion
    }
}