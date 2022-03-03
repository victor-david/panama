/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Provides statistics for the <see cref="SubmissionBatchTable"/>.
    /// </summary>
    public class SubmissionBatchTableStats : TableStatisticBase
    {
        #region Public properties
        /// <summary>
        /// Gets the count of active submissions.
        /// </summary>
        public int ActiveCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of accepted submissions.
        /// </summary>
        public int AcceptedCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of rejected submissions.
        /// </summary>
        public int RejectedCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the maxium number of days for a reply to a submission.
        /// </summary>
        public int MaximumDays
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the minimum number of days for a reply to a submission.
        /// </summary>
        public int MinimumDays
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the average number of days for a reply to a submission.
        /// </summary>
        public int AverageDays
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the total fees.
        /// </summary>
        public decimal TotalFees
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionBatchTableStats"/> class.
        /// </summary>
        /// <param name="table">The submission batch table.</param>
        public SubmissionBatchTableStats(SubmissionBatchTable table)
            : base(table)
        {
            // the base constructor calls the Refresh method.
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Populates the statistics
        /// </summary>
        protected override void Refresh()
        {
            base.Refresh();
            AcceptedCount = 0;
            ActiveCount = 0;
            AverageDays = 0;
            MaximumDays = 0;
            MinimumDays = int.MaxValue;
            RejectedCount = 0;
            TotalFees = 0;

            double totalDays = 0;
            int respondedSubs = 0;

            foreach (DataRow row in Table.Rows)
            {
                long respType = (long)row[SubmissionBatchTable.Defs.Columns.ResponseType];
                if (respType == ResponseTable.Defs.Values.NoResponse) ActiveCount++;
                    else if   (respType == ResponseTable.Defs.Values.ResponseAccepted) AcceptedCount++;
                        else RejectedCount++ ;
                if (row[SubmissionBatchTable.Defs.Columns.Response] is DateTime response)
                {
                    DateTime submitted = (DateTime)row[SubmissionBatchTable.Defs.Columns.Submitted];
                    TimeSpan span = response - submitted;
                    if (span.TotalDays > MaximumDays) MaximumDays = (int)span.TotalDays;
                    if (span.TotalDays < MinimumDays) MinimumDays = (int)span.TotalDays;
                    totalDays += span.TotalDays;
                    respondedSubs++;
                }

                if (row[SubmissionBatchTable.Defs.Columns.Fee] is decimal fees)
                {
                    TotalFees += fees;
                }
            }
            // this would only happen if there were no submissions with a response.
            if (MinimumDays == int.MaxValue) MinimumDays = 0;
            // just in case there are zero submissions with a response, don't want to divide by zero.
            if (respondedSubs > 0) AverageDays = (int)totalDays / respondedSubs;
        }
        #endregion
    }
}