using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using Columns = Restless.Panama.Database.Tables.SubmissionPeriodTable.Defs.Columns;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Encapsulates a single row from the <see cref="SubmissionPeriodTable"/>
    /// </summary>
    public class SubmissionPeriodRow : RowObjectBase<SubmissionPeriodTable>
    {
        #region Private
        private static readonly Dictionary<long, long> MonthDayMap = new Dictionary<long, long>()
        {
            { 1, 31 }, { 2, 28 }, { 3, 31 }, { 4, 30 },
            { 5, 31 }, { 6, 30 }, { 7, 31 }, { 8, 31 },
            { 9, 30 }, { 10, 31 }, { 11, 30 }, { 12, 31 },
        };
        #endregion

        /************************************************************************/

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
            set => SetMonthStart(value);
        }

        /// <summary>
        /// Gets or sets the day that the submission period starts
        /// </summary>
        public long DayStart
        {
            get => GetInt64(Columns.DayStart);
            set => SetDayStart(value);
        }

        /// <summary>
        /// Gets or sets the month that the submission period ends
        /// </summary>
        public long MonthEnd
        {
            get => GetInt64(Columns.MonthEnd);
            set => SetMonthEnd(value);
        }

        /// <summary>
        /// Gets or sets the day that the submission period ends
        /// </summary>
        public long DayEnd
        {
            get => GetInt64(Columns.DayEnd);
            set => SetDayEnd(value);
        }

        /// <summary>
        /// Gets or sets the notes associated with this entry
        /// </summary>
        public string Notes
        {
            get => GetString(Columns.Notes);
            set => SetValue(Columns.Notes, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates whether this period spans the entire year
        /// </summary>
        public bool IsAllYear => MonthStart == 1 && DayStart == 1 && MonthEnd == 12 && DayEnd == 31;
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
        private void SetMonthStart(long value)
        {
            SetValue(Columns.MonthStart, Math.Clamp(value, 1, 12));
            if (DayStart > MonthDayMap[MonthStart])
            {
                SetValue(Columns.DayStart, MonthDayMap[MonthStart]);
            }
            Table.UpdateInPeriod(this);
        }

        private void SetMonthEnd(long value)
        {
            SetValue(Columns.MonthEnd, Math.Clamp(value, 1, 12));
            if (DayEnd > MonthDayMap[MonthEnd])
            {
                SetValue(Columns.DayEnd, MonthDayMap[MonthEnd]);
            }
            Table.UpdateInPeriod(this);
        }

        private void SetDayStart(long value)
        {
            SetValue(Columns.DayStart, Math.Clamp(value, 1, MonthDayMap[MonthStart]));
            Table.UpdateInPeriod(this);
        }

        private void SetDayEnd(long value)
        {
            SetValue(Columns.DayEnd, Math.Clamp(value, 1, MonthDayMap[MonthEnd]));
            Table.UpdateInPeriod(this);
        }
        #endregion
    }
}