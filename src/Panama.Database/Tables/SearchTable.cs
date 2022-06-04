using Restless.Panama.Database.Core;
using Restless.Toolkit.Core.Database.SQLite;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that hold temporary search results
    /// </summary>
    public class SearchTable : Toolkit.Core.Database.SQLite.ApplicationTableBase
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
            public const string TableName = "search";

            /// <summary>
            /// Provides static column names for this table.
            /// </summary>
            public static class Columns
            {
                /// <summary>
                /// Primary key.
                /// </summary>
                public const string Id = "id";

                /// <summary>
                /// Search result file type
                /// </summary>
                public const string Type = "type";

                /// <summary>
                /// Search result file name
                /// </summary>
                public const string File = "file";

                /// <summary>
                /// Seahc result title
                /// </summary>
                public const string Title = "title";

                /// <summary>
                /// Searcg result author
                /// </summary>
                public const string Author = "author";

                /// <summary>
                /// Search result company
                /// </summary>
                public const string Company = "company";

                /// <summary>
                /// Search result size
                /// </summary>
                public const string Size = "size";

                /// <summary>
                /// Whether search result is a version
                /// </summary>
                public const string IsVersion = "isversion";

                /// <summary>
                /// Search result created date
                /// </summary>
                public const string Created = "created";

                /// <summary>
                /// Search result modified date
                /// </summary>
                public const string Modified = "modified";
            }
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName => Defs.Columns.Id;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchTable"/> class.
        /// </summary>
        public SearchTable() : base(DatabaseController.Instance, DatabaseController.MemorySchemaName, Defs.TableName)
        {
            IsReadOnly = true;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        public override void Load()
        {
            Load(null, null);
        }

        /// <summary>
        /// Adds an item
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Add(SearchRowItem item)
        {
            DataRow row = NewRow();
            row[Defs.Columns.Type] = item.Type;
            row[Defs.Columns.File] = item.File;
            row[Defs.Columns.Title] = item.Title;
            row[Defs.Columns.Author] = item.Author;
            row[Defs.Columns.Company] = item.Company;
            row[Defs.Columns.Size] = item.Size;
            row[Defs.Columns.IsVersion] = item.IsVersion;
            row[Defs.Columns.Created] = item.Created;
            row[Defs.Columns.Modified] = item.Modified;
            Rows.Add(row);
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
                { Defs.Columns.Type, ColumnType.Text },
                { Defs.Columns.File, ColumnType.Text },
                { Defs.Columns.Title, ColumnType.Text, false, true },
                { Defs.Columns.Author, ColumnType.Text, false, true },
                { Defs.Columns.Company, ColumnType.Text, false, true },
                { Defs.Columns.Size, ColumnType.Integer },
                { Defs.Columns.IsVersion, ColumnType.Boolean },
                { Defs.Columns.Created, ColumnType.Timestamp },
                { Defs.Columns.Modified, ColumnType.Timestamp },
            };
        }
        #endregion
    }
}