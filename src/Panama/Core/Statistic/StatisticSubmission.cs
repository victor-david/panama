using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using System;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides statistics for the <see cref="SubmissionBatchTable"/>.
    /// </summary>
    public class StatisticSubmission : StatisticBase
    {
        #region Private
        private int active, accepted, rejected, minDays, maxDays, averageDays;
        private decimal fees;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticSubmission"/> class.
        /// </summary>
        /// <param name="table">The submission batch table.</param>
        public StatisticSubmission(SubmissionBatchTable table) : base(table)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        public override IEnumerator<Statistic> GetEnumerator()
        {
            yield return Statistic.Create(Text.Total, RowCount);
            yield return Statistic.Create(Text.Active, active);
            yield return Statistic.Create(Text.Accepted, accepted);
            yield return Statistic.Create(Text.Rejected, rejected);
            yield return Statistic.Create(Text.MinDays, minDays);
            yield return Statistic.Create(Text.MaxDays, maxDays);
            yield return Statistic.Create(Text.AverageDays, averageDays);
            yield return Statistic.Create(Text.Fees, fees);
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
            accepted = 0;
            active = 0;
            averageDays = 0;
            maxDays = 0;
            minDays = int.MaxValue;
            rejected = 0;
            fees = 0;

            double totalDays = 0;
            int respondedSubs = 0;

            foreach (DataRow row in Table.Rows)
            {
                long respType = (long)row[SubmissionBatchTable.Defs.Columns.ResponseType];
                if (respType == ResponseTable.Defs.Values.NoResponse) active++;
                    else if   (respType == ResponseTable.Defs.Values.ResponseAccepted) accepted++;
                        else rejected++ ;
                if (row[SubmissionBatchTable.Defs.Columns.Response] is DateTime response)
                {
                    DateTime submitted = (DateTime)row[SubmissionBatchTable.Defs.Columns.Submitted];
                    TimeSpan span = response - submitted;
                    if (span.TotalDays > maxDays) maxDays = (int)span.TotalDays;
                    if (span.TotalDays < minDays) minDays = (int)span.TotalDays;
                    totalDays += span.TotalDays;
                    respondedSubs++;
                }

                if (row[SubmissionBatchTable.Defs.Columns.Fee] is decimal fees)
                {
                    this.fees += fees;
                }
            }
            // this would only happen if there were no submissions with a response.
            if (minDays == int.MaxValue) minDays = 0;
            // just in case there are zero submissions with a response, don't want to divide by zero.
            if (respondedSubs > 0) averageDays = (int)totalDays / respondedSubs;
        }
        #endregion
    }
}