using System;
using Restless.App.Panama.Database;
using Restless.Tools.Database.Generic;
using Restless.Tools.Database.SQLite;
using System.Data;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that hold links to web sites.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class LinkTable : TableBase
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
            public const string TableName = "link";

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
                /// The name of the name column. This column holds the descriptive name of the link.
                /// </summary>
                public const string Name = "name";

                /// <summary>
                /// The name of the url column.
                /// </summary>
                public const string Url = "url";

                /// <summary>
                /// The name of the notes column. This column holds any notes associated with the link.
                /// </summary>
                public const string Notes = "notes";

                /// <summary>
                /// The name of the credentail id column.
                /// </summary>
                public const string CredentialId = "credentialid";

                /// <summary>
                /// The name of the added column. This column holds the date that the link was added.
                /// </summary>
                public const string Added = "added";

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
        public LinkTable() : base(DatabaseController.Instance, Defs.TableName)
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


        /// <summary>
        /// Clears the credential id of all links with the specified credential id.
        /// </summary>
        /// <param name="id">The credential id.</param>
        public void ClearCredential(Int64 id)
        {
            DataRow[] rows = Select(String.Format("{0}={1}", Defs.Columns.CredentialId, id));
            foreach (DataRow row in rows)
            {
                row[Defs.Columns.CredentialId] = 0;
            }
            Save();
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
            return Resources.Create.Link;
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
            row[Defs.Columns.Name] = "(new link)";
            row[Defs.Columns.Url] = String.Empty;
            row[Defs.Columns.Notes] = DBNull.Value;
            row[Defs.Columns.CredentialId] = 0;
            row[Defs.Columns.Added] = DateTime.UtcNow;
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
        //        case "link_id": return Defs.Columns.Id;
        //        case "display_name": return Defs.Columns.Name;
        //        case "credential_id": return Defs.Columns.CredentialId;
        //        case "date_added": return Defs.Columns.Added;
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
