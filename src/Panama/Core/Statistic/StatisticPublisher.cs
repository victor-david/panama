using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides statistics for the <see cref="PublisherTable"/>.
    /// </summary>
    public class StatisticPublisher : StatisticBase
    {
        #region Private
        private int followup, goner, exclusive, paying, inPeriod;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticPublisher"/> class.
        /// </summary>
        /// <param name="table">The publisher table.</param>
        public StatisticPublisher(PublisherTable table) : base(table)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        public override IEnumerator<Statistic> GetEnumerator()
        {
            yield return Statistic.Create(Text.Total , RowCount);
            yield return Statistic.Create(Text.InPeriod, inPeriod);
            yield return Statistic.Create(Text.FollowUp, followup);
            yield return Statistic.Create(Text.Paying, paying);
            yield return Statistic.Create(Text.Exclusive, exclusive);
            yield return Statistic.Create(Text.Goner, goner);
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
            followup = 0;
            goner = 0;
            paying = 0;
            exclusive = 0;
            inPeriod = 0;
            foreach (DataRow row in Table.Rows)
            {
                if ((bool)row[PublisherTable.Defs.Columns.Followup]) followup++;
                if ((bool)row[PublisherTable.Defs.Columns.Goner]) goner++;
                if ((bool)row[PublisherTable.Defs.Columns.Paying]) paying++;
                if ((bool)row[PublisherTable.Defs.Columns.Exclusive]) exclusive++;
                if ((bool)row[PublisherTable.Defs.Columns.Calculated.InSubmissionPeriod]) inPeriod++;
            }
        }
        #endregion
    }
}