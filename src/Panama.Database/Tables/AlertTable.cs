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
    /// Represents the table that holds alert information.
    /// </summary>
    public class AlertTable : Core.ApplicationTableBase
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
            public const string TableName = "alert";

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
                /// Holds the title of the alert, i.e. the alert text.
                /// </summary>
                public const string Title = "title";

                /// <summary>
                /// Holds the url associated with the url. May be null.
                /// </summary>
                public const string Url = "url";

                /// <summary>
                /// Holds the date for the alert.
                /// </summary>
                public const string Date = "date";

                /// <summary>
                /// Holds a boolean value that indicates if the alert is enabled
                /// </summary>
                public const string Enabled = "enabled";

            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AlertTable"/> class.
        /// </summary>
        public AlertTable() : base(Defs.TableName)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Rows collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, string.Format("{0} DESC",Defs. Columns.Date));
        }

        /// <summary>
        /// Provides an enumerable that enumerates all records in order of id
        /// </summary>
        /// <returns>An enumerable</returns>
        public IEnumerable<AlertRow> EnumerateAll()
        {
            foreach (DataRow row in EnumerateRows(null, Defs.Columns.Id))
            {
                yield return new AlertRow(row);
            }
        }

        /// <summary>
        /// Provides an enumerable alerts that are currently ready, i.e with a date that falls on or before the current date.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<AlertRow> EnumerateReady()
        {
            foreach (AlertRow alert in EnumerateAll())
            {
                if (alert.Enabled && DateTime.Compare(alert.Date, DateTime.Now) <= 0)
                {
                    yield return alert;
                }
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
                { Defs.Columns.Title, ColumnType.Text },
                { Defs.Columns.Url, ColumnType.Text, false, true },
                { Defs.Columns.Date, ColumnType.Timestamp },
                { Defs.Columns.Enabled, ColumnType.Boolean, false, false, 0 }
            };
        }

        /// <summary>
        /// Populates a new row with default (starter) values
        /// </summary>
        /// <param name="row">The freshly created DataRow to poulate</param>
        protected override void PopulateDefaultRow(DataRow row)
        {
            row[Defs.Columns.Title] = "(new alert)";
            row[Defs.Columns.Url] = DBNull.Value;
            row[Defs.Columns.Date] = Utility.GetUtcNowZero().AddDays(7);
            row[Defs.Columns.Enabled] = true;
        }
        #endregion
    }
}