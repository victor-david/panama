using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;
using System;
using System.Data;

namespace Restless.App.Panama.Database.Tables
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
                /// The name of the value column.
                /// </summary>
                public const string Value = "value";

                /// <summary>
                /// The name of the enabled column.
                /// </summary>
                public const string Enabled = "enabled";
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
        /// <param name="defaultColor">The initial value</param>
        /// <returns>The data row</returns>
        /// <remarks>
        /// If the configuration value specified by <paramref name="id"/> does not already exist, this method first creates it.
        /// </remarks>
        public DataRow GetConfigurationRow(string id, object defaultColor, bool defaultEnabled)
        {
            Validations.ValidateNullEmpty(id, "id");
            DataRow[] rows = Select(String.Format("{0}='{1}'", Defs.Columns.Id, id));
            if (rows.Length == 1) return rows[0];

            DataRow row = NewRow();
            row[Defs.Columns.Id] = id;
            if (defaultColor == null)
                row[Defs.Columns.Value] = DBNull.Value;
            else
                row[Defs.Columns.Value] = defaultColor;

            row[Defs.Columns.Enabled] = defaultEnabled;
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
