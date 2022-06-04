/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains publisher information.
    /// </summary>
    public class PublisherTable : Core.ApplicationTableBase
    {
        #region Private
        private bool isInitializing;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Provides static definitions for table properties such as column names and relation names.
        /// </summary>
        public static class Defs
        {
            /// <summary>
            /// Specifies the name of this table.
            /// </summary>
            public const string TableName = "publisher";

            /// <summary>
            /// Provides static column names for this table.
            /// </summary>
            public static class Columns
            {
                /// <summary>
                /// The name of the id column. This is the table's primary key.
                /// </summary>
                public const string Id = DefaultPrimaryKeyName;

                /// <summary>
                /// The name of the name column. This column holds the name of the publisher.
                /// </summary>
                public const string Name = "name";

                /// <summary>
                /// Holds address 1
                /// </summary>
                public const string Address1 = "address1";

                /// <summary>
                /// Holds address 2
                /// </summary>
                public const string Address2 = "address2";

                /// <summary>
                /// Holds address 3
                /// </summary>
                public const string Address3 = "address3";

                /// <summary>
                /// The name of the city column.
                /// </summary>
                public const string City = "city";

                /// <summary>
                /// The name of the state column.
                /// </summary>
                public const string State = "state";

                /// <summary>
                /// The name of the zip column.
                /// </summary>
                public const string Zip = "zip";

                /// <summary>
                /// The name of the url column. This column holds the url to the publisher's web site.
                /// </summary>
                public const string Url = "url";

                /// <summary>
                /// The name of the email column.
                /// </summary>
                public const string Email = "email";

                /// <summary>
                /// The name of the options column. This column is currently not used.
                /// </summary>
                public const string Options = "options";

                /// <summary>
                /// The name of the exclusive, whether non-zero, the publisher does not accept simultaneous submissions.
                /// </summary>
                public const string Exclusive = "exclusive";

                /// <summary>
                /// The name of the paying column, whether the publisher pays its contributors.
                /// </summary>
                public const string Paying = "paying";

                /// <summary>
                /// The name of the followup column. This column is used to flag a publisher for later followup.
                /// </summary>
                public const string Followup = "followup";

                /// <summary>
                /// The name of the goner column. This column is used to flag a publisher that has gone out of business, or is otherwise not of interest to the user.
                /// </summary>
                public const string Goner = "goner";

                /// <summary>
                /// The name of the credential id column.
                /// </summary>
                public const string CredentialId = "credentialid";

                /// <summary>
                /// The name of the notes column. This column holds any notes about the publisher.
                /// </summary>
                public const string Notes = "notes";

                /// <summary>
                /// The name of the added column. This column holds the date that the publisher was added.
                /// </summary>
                public const string Added = "added";

                /// <summary>
                /// Provides static column names for columns that are calculated from other values.
                /// </summary>
                public static class Calculated
                {
                    /// <summary>
                    /// The name of the submission count column. This calculated column
                    /// holds the total number of submissions made to the publisher.
                    /// </summary>
                    public const string SubCount = "CalcSubCount";

                    /// <summary>
                    /// The name of the last submission column. This calculated column
                    /// holds the date of the last submission to the publisher, or null
                    /// if there's never been a submission.
                    /// </summary>
                    public const string LastSub = "CalcLastSub";

                    /// <summary>
                    /// The name of the submission period count column. This calculated column
                    /// holds the number of records from the <see cref="SubmissionPeriodTable"/>
                    /// that belong to the publisher.
                    /// </summary>
                    public const string SubPeriodCount = "CalcSupPerCount";

                    /// <summary>
                    /// The name of the in-submission-period column. This calculated column
                    /// holds a boolean value that indicates if the current date falls within
                    /// any one of the defined submssion periods for the publisher.
                    /// </summary>
                    public const string InSubmissionPeriod = "CalcInPeriod";

                    /// <summary>
                    /// The name of the active submission column. This calculated column
                    /// holds a boolean value that indicates if the publisher has at least one
                    /// submission that is currently active, i.e. has not yet received a response.
                    /// </summary>
                    public const string HaveActiveSubmission = "CalcActiveSub";

                    /// <summary>
                    /// Alias for <see cref="Columns.Goner"/>, needed in style binding
                    /// </summary>
                    public const string Goner = "CalcGoner";
                }
            }

            /// <summary>
            /// Provides static relation names.
            /// </summary>
            public static class Relations
            {
                /// <summary>
                /// The name of the relation that relates the <see cref="PublisherTable"/> to the <see cref="SubmissionBatchTable"/>.
                /// </summary>
                public const string ToSubmissionBatch = "PubToSubmissionBatch";

                /// <summary>
                /// The name of the relation that relates the <see cref="PublisherTable"/> to the <see cref="SubmissionPeriodTable"/>.
                /// </summary>
                public const string ToSubmissionPeriod = "PubToSubmissionPeriod";

                /// <summary>
                /// The name of the relation that relates the <see cref="PublisherTable"/> to the <see cref="PublishedTable"/>.
                /// </summary>
                public const string ToPublished = "PubToPublished";
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherTable"/> class.
        /// </summary>
        public PublisherTable() : base(Defs.TableName)
        {
            isInitializing = true;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Rows collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, $"{Defs.Columns.Added} DESC");
        }

        /// <summary>
        /// Provides an enumerable that gets all entries in order of id ASC.
        /// </summary>
        /// <returns>An enumerable that gets all entries</returns>
        public IEnumerable<PublisherRow> EnumerateAll()
        {
            foreach (DataRow row in EnumerateRows(null, Defs.Columns.Id))
            {
                yield return new PublisherRow(row);
            }
        }

        /// <summary>
        /// Clears the credential id of all publishers with the specified credential id.
        /// </summary>
        /// <param name="id">The credential id.</param>
        public void ClearCredential(long id)
        {
            foreach (DataRow row in EnumerateRows($"{Defs.Columns.CredentialId}={id}"))
            {
                row[Defs.Columns.CredentialId] = 0;
            }
            Save();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets the column definitions for this table.
        /// </summary>
        /// <returns>A <see cref="ColumnDefinitionCollection"/>.</returns>
        protected override ColumnDefinitionCollection GetColumnDefinitions()
        {
            return new ColumnDefinitionCollection()
            {
                { Defs.Columns.Id, ColumnType.Integer, true },
                { Defs.Columns.Name, ColumnType.Text },
                { Defs.Columns.Address1, ColumnType.Text, false, true },
                { Defs.Columns.Address2, ColumnType.Text, false, true },
                { Defs.Columns.Address3, ColumnType.Text, false, true },
                { Defs.Columns.City, ColumnType.Text, false, true },
                { Defs.Columns.State, ColumnType.Text, false, true },
                { Defs.Columns.Zip, ColumnType.Text, false, true },
                { Defs.Columns.Url, ColumnType.Text, false, true },
                { Defs.Columns.Email, ColumnType.Text, false, true },
                { Defs.Columns.Options, ColumnType.Integer, false, false, 0 },
                { Defs.Columns.CredentialId, ColumnType.Integer, false, false, 0, IndexType.Index },
                { Defs.Columns.Notes, ColumnType.Text, false, true },
                { Defs.Columns.Added, ColumnType.Timestamp },
                { Defs.Columns.Exclusive, ColumnType.Boolean, false, false, 0 },
                { Defs.Columns.Paying, ColumnType.Boolean, false, false, 0 },
                { Defs.Columns.Followup, ColumnType.Boolean, false, false, 0 },
                { Defs.Columns.Goner, ColumnType.Boolean, false, false, 0 },
            };
        }

        /// <summary>
        /// Establishes parent / child relationships with other tables.
        /// </summary>
        protected override void SetDataRelations()
        {
            CreateParentChildRelation<SubmissionBatchTable>(Defs.Relations.ToSubmissionBatch, Defs.Columns.Id, SubmissionBatchTable.Defs.Columns.PublisherId);
            CreateParentChildRelation<SubmissionPeriodTable>(Defs.Relations.ToSubmissionPeriod, Defs.Columns.Id, SubmissionPeriodTable.Defs.Columns.PublisherId);
            CreateParentChildRelation<PublishedTable>(Defs.Relations.ToPublished, Defs.Columns.Id, PublishedTable.Defs.Columns.PublisherId);
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Calculated"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            string expr = $"Count(Child({Defs.Relations.ToSubmissionBatch}).{SubmissionBatchTable.Defs.Columns.Id})";
            CreateExpressionColumn<long>(Defs.Columns.Calculated.SubCount, expr);

            expr = $"Max(Child({Defs.Relations.ToSubmissionBatch}).{SubmissionBatchTable.Defs.Columns.Submitted})";
            CreateExpressionColumn<DateTime>(Defs.Columns.Calculated.LastSub, expr);

            expr = $"Count(Child({Defs.Relations.ToSubmissionPeriod}).{SubmissionPeriodTable.Defs.Columns.Id})";
            CreateExpressionColumn<long>(Defs.Columns.Calculated.SubPeriodCount, expr);

            CreateExpressionColumn<string>(Defs.Columns.Calculated.Goner, Defs.Columns.Goner);
        }

        /// <summary>
        /// Called when database initialization is complete. Calculates the publisher in-period values.
        /// </summary>
        protected override void OnInitializationComplete()
        {
            DataColumn col1 = new DataColumn(Defs.Columns.Calculated.InSubmissionPeriod, typeof(bool));
            Columns.Add(col1);
            SetColumnProperty(col1, DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate);

            DataColumn col2 = new DataColumn(Defs.Columns.Calculated.HaveActiveSubmission, typeof(bool));
            Columns.Add(col2);
            SetColumnProperty(col2, DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate);

            foreach (DataRow row in Rows)
            {
                UpdateInPeriod(row);
                UpdateHaveActive(row);
            }

            isInitializing = false;
        }

        /// <summary>
        /// Populates a new row with default (starter) values
        /// </summary>
        /// <param name="row">The freshly created DataRow to poulate</param>
        protected override void PopulateDefaultRow(System.Data.DataRow row)
        {
            row[Defs.Columns.Added] = DateTime.UtcNow;
            row[Defs.Columns.CredentialId] = 0;
            row[Defs.Columns.Followup] = false;
            row[Defs.Columns.Goner] = false;
            row[Defs.Columns.Name] = "(new publisher)";
            row[Defs.Columns.Options] = 0;
            row[Defs.Columns.Paying] = false;
            row[Defs.Columns.Exclusive] = false;
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Updates the specified publisher data row, setting its calculated <see cref="Defs.Columns.Calculated.InSubmissionPeriod"/> column.
        /// </summary>
        /// <param name="row">The publisher data row</param>
        /// <remarks>
        /// This method is used by the SubmissionPeriodTable to sync the publisher table
        /// when a submission period record is added, deleted, or modified. It's also used by this
        /// class when performing the startup initialization.
        /// </remarks>
        internal void UpdateInPeriod(DataRow row)
        {
            if (row != null && row.Table.TableName == TableName)
            {
                // When initializing, this method is called from OnInitializationComplete()
                // for every row in the table. At this time, isInitializing is true.
                // Later, this method is called from SubmissionPeriodTable to update when
                // a period is added or deleted. At that point, isInitializing is false
                // and therefore we save the table first because we don't want any other pending 
                // changes on the row to be lost with row.AcceptChanges()
                if (!isInitializing)
                {
                    Save();
                }

                bool inPeriod = false;
                foreach (DataRow childRow in row.GetChildRows(Defs.Relations.ToSubmissionPeriod))
                {
                    long monthStart = (long)childRow[SubmissionPeriodTable.Defs.Columns.MonthStart];
                    long dayStart = (long)childRow[SubmissionPeriodTable.Defs.Columns.DayStart];

                    long monthEnd = (long)childRow[SubmissionPeriodTable.Defs.Columns.MonthEnd];
                    long dayEnd = (long)childRow[SubmissionPeriodTable.Defs.Columns.DayEnd];

                    inPeriod = inPeriod || IsInPeriod(monthStart, dayStart, monthEnd, dayEnd);
                }

                row[Defs.Columns.Calculated.InSubmissionPeriod] = inPeriod;
                row.AcceptChanges();
            }
        }

        /// <summary>
        /// Updates the specified publisher data row, setting its calculated <see cref="Defs.Columns.Calculated.HaveActiveSubmission"/> column.
        /// </summary>
        /// <param name="row">The publisher data row</param>
        internal void UpdateHaveActive(DataRow row)
        {
            if (row != null && row.Table.TableName == TableName)
            {
                // See comment on UpdateInPeriod(). Same idea.
                if (!isInitializing)
                {
                    Save();
                }

                bool haveActive = false;

                foreach (DataRow child in row.GetChildRows(Defs.Relations.ToSubmissionBatch))
                {
                    if (child[SubmissionBatchTable.Defs.Columns.Response] == DBNull.Value)
                    {
                        haveActive = true;
                    }
                }

                row[Defs.Columns.Calculated.HaveActiveSubmission] = haveActive;
                row.AcceptChanges();
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private bool IsInPeriod(long monthStart, long dayStart, long monthEnd, long dayEnd)
        {
            DateTime now = DateTime.Now;
            DateTime start = new DateTime(now.Year - 1, (int)monthStart, (int)dayStart);
            DateTime end = new DateTime(now.Year - 1, (int)monthEnd, (int)dayEnd, 23, 59, 59);

            /* If start date (day of year) comes later than end date (day of year), bump end date up one year */
            if (start.DayOfYear > end.DayOfYear)
            {
                end = end.AddYears(1);
            }

            /* If both are behind today, bump both up one year */
            if (DateTime.Compare(start, now) < 0 && DateTime.Compare(end, now) < 0)
            {
                start = start.AddYears(1);
                end = end.AddYears(1);
            }

            return DateTime.Compare(now, start) >= 0 && DateTime.Compare(now, end) <= 0;
        }
        #endregion
    }
}