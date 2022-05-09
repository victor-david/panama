using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    public class OrphanExclusionTable : Core.ApplicationTableBase
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
            public const string TableName = "orphanex";

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
                /// Holds the exclusion type
                /// </summary>
                public const string Type = "type";

                /// <summary>
                /// Holds the value
                /// </summary>
                public const string Value = "value";

                /// <summary>
                /// Holds whether a system generated exclusion
                /// </summary>
                public const string IsSystem = "sys";

                /// <summary>
                /// Holds the created date / time
                /// </summary>
                public const string Created = "created";
            }

            /// <summary>
            /// Provides static values for the <see cref="Columns.Type"/>  column.
            /// </summary>
            public static class Values
            {
                public const long FileType = 1;
                public const long FileExtensionType = 2;
                public const long DirectoryType = 3;
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OrphanExclusionTable"/> class.
        /// </summary>
        public OrphanExclusionTable() : base(Defs.TableName)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <inheritdoc/>
        public override void Load()
        {
            Load(null, Defs.Columns.Id);
        }

        public void AddFileExclusion(string value)
        {
            Add(Defs.Values.FileType, value);
        }

        public void AddFileExtensionExclusion(string value)
        {
            Add(Defs.Values.FileExtensionType, value);
        }

        public void AddDirectoryExclusion(string value)
        {
            Add(Defs.Values.DirectoryType, value);
        }

        /// <summary>
        /// Provides an enumerable that enumerates all entries of the specified type
        /// </summary>
        /// <param name="exclusionType">The type to enumerate</param>
        /// <returns>An enumerable</returns>
        public IEnumerable<OrphanExclusionRow> EnumerateExclusion(long exclusionType)
        {
            foreach (DataRow row in EnumerateRows($"{Defs.Columns.Type}={exclusionType}"))
            {
                yield return new OrphanExclusionRow(row);
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override ColumnDefinitionCollection GetColumnDefinitions()
        {
            return new ColumnDefinitionCollection()
            {
                { Defs.Columns.Id, ColumnType.Integer, true },
                { Defs.Columns.Type, ColumnType.Integer },
                { Defs.Columns.Value, ColumnType.Text },
                { Defs.Columns.IsSystem, ColumnType.Boolean },
                { Defs.Columns.Created, ColumnType.Timestamp },
            };
        }

        /// <inheritdoc/>
        protected override List<string> GetPopulateColumnList()
        {
            return new List<string>() { Defs.Columns.Id, Defs.Columns.Type, Defs.Columns.Value, Defs.Columns.IsSystem, Defs.Columns.Created };
        }

        /// <inheritdoc/>
        protected override IEnumerable<object[]> EnumeratePopulateValues()
        {
            yield return new object[] { 1, Defs.Values.FileExtensionType, ".cmd", true, DateTime.UtcNow };
            yield return new object[] { 2, Defs.Values.FileExtensionType, ".bat", true, DateTime.UtcNow };
            yield return new object[] { 3, Defs.Values.FileExtensionType, ".lnk", true, DateTime.UtcNow };

        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void Add(long exclusionType, string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && !HaveExclusion(exclusionType, value))
            {
                DataRow row = NewRow();
                row[Defs.Columns.Type] = exclusionType;
                row[Defs.Columns.Value] = value;
                row[Defs.Columns.IsSystem] = false;
                row[Defs.Columns.Created] = DateTime.UtcNow;
                Rows.Add(row);
                Save();
            }
        }

        private bool HaveExclusion(long exclusionType, string value)
        {
            return Select($"{Defs.Columns.Type}={exclusionType} AND {Defs.Columns.Value}='{value}'").Length > 0;
        }
        #endregion
    }
}