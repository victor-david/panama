/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Collections.ObjectModel;
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
                public const string Id = "id";

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

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get => Defs.Columns.Id;
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
        /// Gets a list of alerts that are currently ready, i.e with a date that falls on or before the current date.
        /// </summary>
        /// <returns>An Observable collection of <see cref="RowObject"/> items that contains all active alerts.</returns>
        public ObservableCollection<RowObject> GetReadyAlerts()
        {
            ObservableCollection<RowObject> result = new ObservableCollection<RowObject>();
            DateTime utc = DateTime.UtcNow;

            foreach (DataRow row in Rows)
            {
                var obj = new RowObject(row);
                if (obj.Enabled && DateTime.Compare(obj.Date, utc) <= 0)
                {
                    result.Add(obj);
                }
            }
            return result;
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
                { Defs.Columns.Url, ColumnType.Text },
                { Defs.Columns.Date, ColumnType.Timestamp },
                { Defs.Columns.Enabled, ColumnType.Boolean, false, false, 0 }
            };
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate, DataColumnPropertyKey.ReceiveInsertedId);
        }

        /// <summary>
        /// Populates a new row with default (starter) values
        /// </summary>
        /// <param name="row">The freshly created DataRow to poulate</param>
        protected override void PopulateDefaultRow(System.Data.DataRow row)
        {
            DateTime utc = DateTime.UtcNow.AddDays(7);
            row[Defs.Columns.Title] = "(new alert)";
            row[Defs.Columns.Date] = new DateTime(utc.Year, utc.Month, utc.Day);
            row[Defs.Columns.Enabled] = true;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion


        /************************************************************************/

        #region Row Object
        /// <summary>
        /// Encapsulates a single row from the <see cref="AlertTable"/>.
        /// </summary>
        public class RowObject : RowObjectBase<AlertTable>
        {
            #region Public properties
            /// <summary>
            /// Gets the id for this row object.
            /// </summary>
            public long Id
            {
                get => GetInt64(Defs.Columns.Id);
            }

            /// <summary>
            /// Gets or sets the title for this row object.
            /// </summary>
            public string Title
            {
                get => GetString(Defs.Columns.Title);
                set => SetValue(Defs.Columns.Title, value);
            }

            /// <summary>
            /// Gets or sets the url for this row object.
            /// </summary>
            public string Url
            {
                get => GetString(Defs.Columns.Url);
                set => SetValue(Defs.Columns.Url, value);
            }

            /// <summary>
            /// Gets or sets the date for alert.
            /// When setting this property, the time portion of the value is zeroed out.
            /// </summary>
            public DateTime Date
            {
                get => GetDateTime(Defs.Columns.Date);
                set => SetValue(Defs.Columns.Date, new DateTime(value.Year, value.Month, value.Day));
            }

            /// <summary>
            /// Gets or sets the enabled status.
            /// </summary>
            public bool Enabled
            {
                get => GetBoolean(Defs.Columns.Enabled);
                set => SetValue(Defs.Columns.Enabled, value);
            }

            /// <summary>
            /// Gets a boolean value that indicates if this object contains a url.
            /// </summary>
            public bool HasUrl
            {
                get => !string.IsNullOrEmpty(Url);
            }
            #endregion

            /************************************************************************/

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="RowObject"/> class.
            /// </summary>
            /// <param name="row">The data row</param>
            public RowObject(DataRow row)
                : base(row)
            {
            }
            #endregion
        }
        #endregion

    }
}