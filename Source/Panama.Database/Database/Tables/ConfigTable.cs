using System;
using System.Data;
using Restless.App.Panama.Database;
using Restless.Tools.Database.Generic;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains the application configuration.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class ConfigTable : TableBase
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
            public const string TableName = "config";

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
                /// The name of the description column.
                /// </summary>
                public const string Description = "description";
                
                /// <summary>
                /// The name of the type column.
                /// </summary>
                public const string Type = "type";

                /// <summary>
                /// The name of the value column.
                /// </summary>
                public const string Value = "value";

                /// <summary>
                /// The name of the edit column.
                /// </summary>
                public const string Edit = "edit";
            }

            /// <summary>
            /// Provides static names of values contained in the <see cref="Columns.Type"/> column.
            /// </summary>
            public static class Types
            {
                /// <summary>
                /// The name that identifies the type as a boolean value.
                /// </summary>
                public const string Boolean = "bool";

                /// <summary>
                /// The name that identifies the type as a string value.
                /// </summary>
                public const string String = "string";

                /// <summary>
                /// The name that identifies the type as a string value that can span multiple lines.
                /// </summary>
                public const string MultiString = "mstring";

                /// <summary>
                /// The name that identifies the type as a path value.
                /// </summary>
                public const string Path = "path";

                /// <summary>
                /// The name that identifies the type as a mapi path value.
                /// </summary>
                public const string Mapi = "mapi";

                /// <summary>
                /// The name that identifies the type as an integer value.
                /// </summary>
                public const string Integer = "int";

                /// <summary>
                /// The name that identifies the type as a color value.
                /// </summary>
                public const string Color = "color";

                /// <summary>
                /// The name that identifies the type as an object value. These values are XML serialized.
                /// </summary>
                public const string Object = "object";
            }

            /// <summary>
            /// Provides static names of primary key id values.
            /// </summary>
            public static class FieldIds
            {
                /// <summary>
                /// The id value that identifies the title root folder.
                /// </summary>
                public const string FolderTitleRoot = "FolderTitleRoot";

                /// <summary>
                /// The id value that identifies the submission document folder.
                /// </summary>
                public const string FolderSubmissionDocument = "FolderSubmissionDocument";

                /// <summary>
                /// The id value that identifies the submission message attachment folder.
                /// </summary>
                public const string FolderSubmissionMessageAttachment = "FolderSubmissionMessageAttachment";

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
        public ConfigTable() : base(DatabaseController.Instance, Defs.TableName)
        {
            IsDeleteRestricted = true;
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
            Load(null, Defs.Columns.Id);
        }

        /// <summary>
        /// Adds a configuration value to the table if the id doesn't already exist.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="description">The description </param>
        /// <param name="type">The type. Use ConfigTable.Defs.Types</param>
        /// <param name="value">The initial value</param>
        /// <param name="edit">true if this can be edited by the user; false to be a behind-the-scenes value</param>
        /// <remarks>
        /// If the configuration value specified by <paramref name="id"/> already exists, this method does nothing.
        /// </remarks>
        public void AddConfigValueIf(string id, string description, string type, string value, bool edit)
        {
            Validations.ValidateNullEmpty(id, "id");
            Validations.ValidateNullEmpty(description, "description");
            Validations.ValidateNullEmpty(type, "type");

            DataRow[] rows = Select(String.Format("{0}='{1}'", Defs.Columns.Id, id));
            if (rows.Length == 0)
            {
                DataRow row = NewRow();
                row[Defs.Columns.Id] = id;
                row[Defs.Columns.Description] = description;
                row[Defs.Columns.Type] = type;
                if (String.IsNullOrEmpty(value))
                    row[Defs.Columns.Value] = DBNull.Value;
                else
                    row[Defs.Columns.Value] = value;
                row[Defs.Columns.Edit] = edit;
                Rows.Add(row);
                Save();
            }
        }

        /// <summary>
        /// Removes the specified configuration item if it exists
        /// </summary>
        /// <param name="id">The id of the configuration item to remove.</param>
        public void RemoveConfigValueIf(string id)
        {
            IsDeleteRestricted = false;
            DataRow[] rows = Select(String.Format("{0}='{1}'", Defs.Columns.Id, id));
            if (rows.Length == 1)
            {
                rows[0].Delete();
                Save();
            }
            IsDeleteRestricted = true;
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
            return Resources.Create.Config;
        }

        /// <summary>
        /// Gets the SQL needed to populate this table with its default values.
        /// </summary>
        /// <returns>A SQL string to insert the default data.</returns>
        protected override string GetPopulateSql()
        {
            return Resources.Data.Config;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            //Columns[Defs.Columns.Id].ReadOnly = true;
            //Columns[Defs.Columns.Type].ReadOnly = true;
            //Columns[Defs.Columns.Edit].ReadOnly = true;
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromUpdate);
            SetColumnProperty(Columns[Defs.Columns.Type], DataColumnPropertyKey.ExcludeFromUpdate);
            //SetColumnProperty(Columns[Defs.Columns.Edit], DataColumnPropertyKey.ExcludeFromUpdate);

        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// From within this assembly, gets a configuration value specified by id.
        /// </summary>
        /// <param name="id">The id</param>
        /// <returns>The value</returns>
        internal string GetRowValue(string id)
        {
            DataRow[] rows = Select(String.Format("{0}='{1}'", Defs.Columns.Id, id));
            if (rows.Length == 1)
            {
                return rows[0][Defs.Columns.Value].ToString();
            }

            throw new IndexOutOfRangeException();
        }
        #endregion

        /************************************************************************/

        #region ITableImport and IColumnRowImporter implementation (commented out)
        //public bool PerformImport()
        //{
        //    return DatabaseImporter.Instance.ImportTable(this, this, null);
        //}

        //public string GetColumnName(string origColName)
        //{
        //    switch (origColName)
        //    {
        //        case "config_id": return Defs.Columns.Id;
        //        case "config_desc": return Defs.Columns.Description;
        //        case "config_type": return Defs.Columns.Type;
        //        case "config_value": return Defs.Columns.Value;
        //        default: return origColName;
        //    }            
        //}
        
        //public bool IncludeColumn(string origColName)
        //{
        //    return true;
        //}

        //public bool GetRowConfirmation(System.Data.DataRow row)
        //{
        //    string id = row[0].ToString();
        //    if (id.StartsWith("outlook.") || id.Contains(".datagrid.") || id.Contains(".flush."))
        //    {
        //        return false;
        //    }
        //    if (id.StartsWith("color."))
        //    {
        //        row[2] = "color";
        //    }
        //    return true;
        //}
        #endregion 



    }
}
