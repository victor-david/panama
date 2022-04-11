using Restless.Toolkit.Core.Database.SQLite;
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
        /// Gets or sets the month that the submission period starts
        /// </summary>
        public long MonthStart
        {
            get => GetInt64(Columns.MonthStart);
            set => SetMonthValue(Columns.MonthStart, value);
        }

        /// <summary>
        /// Gets or sets the day that the submission period starts
        /// </summary>
        public long DayStart
        {
            get => GetInt64(Columns.DayStart);
            set => SetDayValue(Columns.DayStart, value);
        }

        /// <summary>
        /// Gets or sets the month that the submission period ends
        /// </summary>
        public long MonthEnd
        {
            get => GetInt64(Columns.MonthEnd);
            set => SetMonthValue(Columns.MonthEnd, value);
        }

        /// <summary>
        /// Gets or sets the day that the submission period ends
        /// </summary>
        public long DayEnd
        {
            get => GetInt64(Columns.DayEnd);
            set => SetDayValue(Columns.DayEnd, value);
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
            SetValue(Columns.MonthStart, 1L);
            SetValue(Columns.DayStart, 1L);

            SetValue(Columns.MonthEnd, 12L);
            SetValue(Columns.DayEnd, 31L);

            Table.UpdateInPeriod(this);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void SetMonthValue(string columnName, long value)
        {
            SetValue(columnName, Math.Clamp(value, 1, 12));
            Table.UpdateInPeriod(this);
        }

        private void SetDayValue(string columnName, long value)
        {
            SetValue(columnName, Math.Clamp(value, 1, 31));
            Table.UpdateInPeriod(this);
        }
        #endregion
    }
}