﻿using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.SubmissionPeriodTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="SubmissionPeriodTable"/>
    /// </summary>
    public class SubmissionPeriodRow : RowObjectBase<SubmissionPeriodTable>
    {
        #region Properties
        /// <summary>
        /// Gets the record id.
        /// </summary>
        public long Id => GetInt64(Columns.Id);

        /// <summary>
        /// Gets the publisher id.
        /// </summary>
        public long PublisherId => GetInt64(Columns.PublisherId);

        /// <summary>
        /// Gets or sets the submission period start
        /// </summary>
        public DateTime Start
        {
            get => GetDateTime(Columns.Start);
            set => SetPeriodStartDate(value);
        }

        /// <summary>
        /// Gets or sets the submission period end
        /// </summary>
        public DateTime End
        {
            get => GetDateTime(Columns.End);
            set => SetPeriodEndDate(value);
        }

        /// <summary>
        /// Gets or sets the notes associated with this entry
        /// </summary>
        public string Notes
        {
            get => GetString(Columns.Notes);
            set => SetValue(Columns.Notes, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionPeriodRow"/> class
        /// </summary>
        /// <param name="row">The data row</param>
        public SubmissionPeriodRow(DataRow row) : base(row)
        {
        }

        /// <summary>
        /// Creates a new <see cref="SubmissionPeriodRow"/> object if <paramref name="row"/> is not null
        /// </summary>
        /// <param name="row">The row</param>
        /// <returns>A new row, or null.</returns>
        public static SubmissionPeriodRow Create(DataRow row)
        {
            return row != null ? new SubmissionPeriodRow(row) : null;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Sets <see cref="Start"/> to January 1 and <see cref="End"/> to December 31
        /// </summary>
        public void MakeAllYear()
        {
            SetValue(Columns.Start, new DateTime(DateTime.Now.Year, 1, 1));
            SetValue(Columns.End, new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59));
            Table.UpdateInPeriod(this);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void SetPeriodStartDate(DateTime value)
        {
            SetValue(Columns.Start, new DateTime(value.Year, value.Month, value.Day));
            Table.UpdateInPeriod(this);
        }

        private void SetPeriodEndDate(DateTime value)
        {
            SetValue(Columns.End, new DateTime(value.Year, value.Month, value.Day, 23, 59, 59));
            Table.UpdateInPeriod(this);
        }
        #endregion
    }
}