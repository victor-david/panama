/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Provides statistics for the <see cref="PublisherTable"/>.
    /// </summary>
    public class PublisherTableStats : TableStatisticBase
    {
        #region Public properties
        /// <summary>
        /// Gets the count of publishers with a followup status.
        /// </summary>
        public int FollowupCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of publishers with a goner status.
        /// </summary>
        public int GonerCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of publishers that don't take simultaneous submissions.
        /// </summary>
        public int ExclusiveCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of publishers flagged as a paying market.
        /// </summary>
        public int PayingCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of publishers who are within a submission period
        /// </summary>
        public int InSubmissionPeriodCount
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherTableStats"/> class.
        /// </summary>
        /// <param name="table">The publisher table.</param>
        public PublisherTableStats(PublisherTable table) : base(table)
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
            FollowupCount = 0;
            GonerCount = 0;
            PayingCount = 0;
            ExclusiveCount = 0;
            InSubmissionPeriodCount = 0;
            foreach (DataRow row in Table.Rows)
            {
                if ((bool)row[PublisherTable.Defs.Columns.Followup]) FollowupCount++;
                if ((bool)row[PublisherTable.Defs.Columns.Goner]) GonerCount++;
                if ((bool)row[PublisherTable.Defs.Columns.Paying]) PayingCount++;
                if ((bool)row[PublisherTable.Defs.Columns.Exclusive]) ExclusiveCount++;
                if ((bool)row[PublisherTable.Defs.Columns.Calculated.InSubmissionPeriod]) InSubmissionPeriodCount++;
            }
        }
        #endregion
    }
}