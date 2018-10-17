using Restless.Tools.Database.SQLite;
using System;
using System.Data;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains tags that may be applied to a title record.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class TagTable : TableBase
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
            public const string TableName = "tag";

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
                /// The name of the tag column.
                /// </summary>
                public const string Tag = "tag";

                /// <summary>
                /// The name of the tag description column.
                /// </summary>
                public const string Description = "description";

                /// <summary>
                /// The name of the usage count column.
                /// </summary>
                public const string UsageCount = "usagecount";
            }

            /// <summary>
            /// Provides static relation names.
            /// </summary>
            public static class Relations
            {
                /// <summary>
                /// The name of the relation that relates the <see cref="TagTable"/> to the <see cref="TitleTagTable"/>.
                /// </summary>
                public const string ToTitleTag = "TagToTitleTag";
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
        public TagTable() : base(DatabaseController.Instance, Defs.TableName)
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
            Load(null, Defs.Columns.Tag);
        }

        /// <summary>
        /// Checks child rows via the <see cref="Defs.Relations.ToTitleTag"/> relation and updates the tag usage count.
        /// </summary>
        public void RefreshTagUsage()
        {
            foreach (DataRow row in Rows)
            {
                DataRow[] childRows = row.GetChildRows(Defs.Relations.ToTitleTag);

                if (childRows.LongLength != (long)row[Defs.Columns.UsageCount])
                {
                    row[Defs.Columns.UsageCount] = childRows.LongLength;
                }
            }
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
            return Resources.Create.Tag;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate, DataColumnPropertyKey.ReceiveInsertedId);
        }

        /// <summary>
        /// Establishes parent / child relationships with other tables.
        /// </summary>
        protected override void SetDataRelations()
        {
            CreateParentChildRelation<TitleTagTable>(Defs.Relations.ToTitleTag, Defs.Columns.Id, TitleTagTable.Defs.Columns.TagId);   
        }

        /// <summary>
        /// Populates a new row with default (starter) values
        /// </summary>
        /// <param name="row">The freshly created DataRow to poulate</param>
        protected override void PopulateDefaultRow(System.Data.DataRow row)
        {
            row[Defs.Columns.Tag] = "(new tag)";
            row[Defs.Columns.Description] = "(new description)";
            row[Defs.Columns.UsageCount] = 0;
        }

        /// <summary>
        /// The controller calls this method for each table from its Shutdown() method.
        /// </summary>
        /// <param name="saveOnShutdown">The value that was passed to the controller's Shutdown method. 
        /// If false, the table needs to call its Save() method if it makes changes to data.
        /// </param>
        protected override void OnShuttingDown(bool saveOnShutdown)
        {
            RefreshTagUsage();
            if (!saveOnShutdown)
            {
                Save();
            }
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
        //        case "tagid": return Defs.Columns.Id;
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
