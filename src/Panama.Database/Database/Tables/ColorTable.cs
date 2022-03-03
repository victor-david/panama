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
    /// Represents the table that contains the application color configuration.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class ColorTable : TableBase
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
            public const string TableName = "color";

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
                /// The name of the foreground column. Holds the foreground color.
                /// </summary>
                public const string Foreground = "foreground";

                /// <summary>
                /// The name of the background column. Holds the background color.
                /// </summary>
                public const string Background = "background";
            }
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get { return Defs.Columns.Id; }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorTable"/> class.
        /// </summary>
        public ColorTable() : base(DatabaseController.Instance, Defs.TableName)
        {
            IsDeleteRestricted = true;
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
        /// Gets the configuration row with the specified id. Adds a row first, if it doesn't already exist.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="defaultForeColor">The initial value for the foreground color.</param>
        /// <param name="defaultBackColor">The initial value for the background color.</param>
        /// <returns>The data row</returns>
        /// <remarks>
        /// If the color configuration value specified by <paramref name="id"/> does not already exist, this method first creates it.
        /// </remarks>
        public DataRow GetConfigurationRow(string id, object defaultForeColor, object defaultBackColor)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            DataRow[] rows = Select(string.Format("{0}='{1}'", Defs.Columns.Id, id));
            if (rows.Length == 1) return rows[0];

            DataRow row = NewRow();
            row[Defs.Columns.Id] = id;
            row[Defs.Columns.Foreground] = defaultForeColor ?? throw new ArgumentNullException(nameof(defaultForeColor));
            row[Defs.Columns.Background] = defaultBackColor ?? throw new ArgumentNullException(nameof(defaultBackColor));
            Rows.Add(row);
            Save();
            return row;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets the DDL needed to create this table.
        /// </summary>
        /// <returns>A SQL string that describes how to create this table.</returns>
        protected override string GetDdl()
        {
            return Resources.Create.Color;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromUpdate);
        }
        #endregion
    }
}