/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains submission period information for publishers.
    /// </summary>
    public class SubmissionPeriodTable : Core.ApplicationTableBase
    {
        #region Public properties
        /// <summary>
        /// Provides static definitions for table properties such as column names and relation names.
        /// </summary>
        public static class Defs
        {
            /// <summary>
            /// Specifies the name of this table.
            /// </summary>
            public const string TableName = "submissionperiod";

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
                /// The name of the publisher id column.
                /// </summary>
                public const string PublisherId = "publisherid";

                /// <summary>
                /// Month number when submission period starts
                /// </summary>
                public const string MonthStart = "monthstart";

                /// <summary>
                /// Day number when submission period starts
                /// </summary>
                public const string DayStart = "daystart";

                /// <summary>
                /// Month number when submission period ends
                /// </summary>
                public const string MonthEnd = "monthend";

                /// <summary>
                /// Day number when submission period ends
                /// </summary>
                public const string DayEnd = "dayend";

                /// <summary>
                /// The name of the notes column.
                /// </summary>
                public const string Notes = "notes";

                /// <summary>
                /// Provides static column names for columns that get their value fron another table.
                /// </summary>
                public static class Joined
                {
                    /// <summary>
                    /// Publisher name, gets its value from the <see cref="PublisherTable"/>.
                    /// </summary>
                    public const string Publisher = "JoinPubName";
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionPeriodTable"/> class.
        /// </summary>
        public SubmissionPeriodTable() : base(Defs.TableName)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Data collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, Defs.Columns.Id);
        }

        /// <summary>
        /// Provides an enumerable that enumerates all record, ordered by id
        /// </summary>
        /// <returns>An enumerable</returns>
        public IEnumerable<SubmissionPeriodRow> EnumerateAll()
        {
            foreach (DataRow row in EnumerateRows(null, Defs.Columns.Id))
            {
                yield return new SubmissionPeriodRow(row);
            }
        }

        /// <summary>
        /// Adds an all-year record to the submission period table
        /// </summary>
        /// <param name="publisherId">The publisher id</param>
        public void AddSubmissionPeriod(long publisherId)
        {
            DataRow row = NewRow();
            row[Defs.Columns.PublisherId] = publisherId;
            row[Defs.Columns.MonthStart] = 1;
            row[Defs.Columns.DayStart] = 1;
            row[Defs.Columns.MonthEnd] = 12;
            row[Defs.Columns.DayEnd] = 31;
            Rows.Add(row);
            Save();
            /* Get the publisher parent row and update it */
            DataRow parentRow = row.GetParentRow(PublisherTable.Defs.Relations.ToSubmissionPeriod);
            Controller.GetTable<PublisherTable>().UpdateInPeriod(parentRow);
        }

        /// <summary>
        /// Removes the submission period specified by its data row and updates the parent publisher record
        /// </summary>
        /// <param name="row">The data row</param>
        /// <remarks>
        /// This method deletes the specified row and updates the parent publisher table
        /// to reflect the changed set of submission periods. You should call this method
        /// rather than deleting the row directly in order to update the parent.
        /// </remarks>
        public void DeleteSubmissionPeriod(DataRow row)
        {
            if (row != null && row.Table.TableName == TableName)
            {
                DataRow parentRow = row.GetParentRow(PublisherTable.Defs.Relations.ToSubmissionPeriod);
                row.Delete();
                /* Update the parent publisher record */
                Controller.GetTable<PublisherTable>().UpdateInPeriod(parentRow);
                Save();
            }
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
                { Defs.Columns.PublisherId, ColumnType.Integer },
                { Defs.Columns.MonthStart, ColumnType.Integer },
                { Defs.Columns.DayStart, ColumnType.Integer },
                { Defs.Columns.MonthEnd, ColumnType.Integer },
                { Defs.Columns.DayEnd, ColumnType.Integer },
                { Defs.Columns.Notes, ColumnType.Text, false, true },
            };
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Joined"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            CreateChildToParentColumn(Defs.Columns.Joined.Publisher, PublisherTable.Defs.Relations.ToSubmissionPeriod, PublisherTable.Defs.Columns.Name);
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Updates the corresponding publisher's in-period status 
        /// </summary>
        /// <param name="period">The submission periodw</param>
        /// <remarks>
        /// This method is used by <see cref="SubmissionPeriodRow"/> to update
        /// a publisher's in period status when the user changes either the 
        /// period start or period end date.
        /// </remarks>
        internal void UpdateInPeriod(SubmissionPeriodRow period)
        {
            DataRow parentRow = period.Row.GetParentRow(PublisherTable.Defs.Relations.ToSubmissionPeriod);
            Controller.GetTable<PublisherTable>().UpdateInPeriod(parentRow);
        }
        #endregion
    }
}