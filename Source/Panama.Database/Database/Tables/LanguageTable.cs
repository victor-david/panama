using System;
using Restless.App.Panama.Database;
using Restless.Tools.Database.Generic;
using Restless.Tools.Database.SQLite;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that holds the language identifiers that are available when assigning
    /// a language id to a title version. This is a lookup table.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class LanguageTable : TableBase
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
            public const string TableName = "language";

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
                /// The name of the name column. This column holds the descriptive name of the language.
                /// </summary>
                public const string Name = "name";
            }

            /// <summary>
            /// Provides static values for the <see cref="Columns.Id"/>  column.
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// The value of the <see cref="Columns.Id"/> column for the default language.
                /// </summary>
                public const string DefaultLanguageId = "en-us";
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
        #pragma warning disable 1591
        public LanguageTable() : base(DatabaseController.Instance, Defs.TableName)
        {
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Data collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, Defs.Columns.Name);
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
            return Resources.Create.Language;
        }

        /// <summary>
        /// Gets the SQL needed to populate this table with its default values.
        /// </summary>
        /// <returns>A SQL string to insert the default data.</returns>
        protected override string GetPopulateSql()
        {
            return Resources.Data.Language;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
        }
        #endregion

        /************************************************************************/

        #region ITableImport and IColumnRowImporter implementation (commented out)
        //public bool PerformImport()
        //{
        //    return DatabaseImporter.Instance.ImportTable(this, this);
        //}

        //public string GetColumnName(string origColName)
        //{
        //    switch (origColName)
        //    {
        //        case "lang_id": return Defs.Columns.Id;
        //        case "lang_name": return Defs.Columns.Name;
        //        default: return origColName;
        //    }
        //}

        //public bool IncludeColumn(string origColName)
        //{
        //    return true;
        //}

        //public bool GetRowConfirmation(System.Data.DataRow row)
        //{
        //    return true;
        //}
        #endregion



    }
}
