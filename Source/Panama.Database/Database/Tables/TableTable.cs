using System;
using System.Data;
using Restless.App.Panama.Database;
using Restless.Tools.Database.Generic;
using Restless.Tools.Database.SQLite;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that displays a list of tables in the data set. This table
    /// is memory only; it is not part of the database on disk.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class TableTable : MemoryTable
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
            public const string TableName = "tablemeta";

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
                /// The name of the name column.
                /// </summary>
                public const string Name = "name";

                /// <summary>
                /// The name of the column count column.
                /// </summary>
                public const string ColumnCount = "colcount";

                /// <summary>
                /// The name of the row count column.
                /// </summary>
                public const string RowCount = "rowcount";

                /// <summary>
                /// The name of the parent relation count column.
                /// </summary>
                public const string ParentRelationCount = "parentrelcount";

                /// <summary>
                /// The name of the child relation count column.
                /// </summary>
                public const string ChildRelationCount = "childrelcount";

                /// <summary>
                /// The name of the contraint count column.
                /// </summary>
                public const string ConstraintCount = "constraintcount";
            }
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get { return Defs.Columns.Name; }
        }
        #endregion

        /************************************************************************/
        
        #region Constructor
        #pragma warning disable 1591
        public TableTable() : base(DatabaseController.Instance, Defs.TableName)
        {
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods

        /// <summary>
        /// Fills the table with data about the application's tables.
        /// </summary>
        public void LoadTableData()
        {
            if (Columns.Count == 0)
            {
                CreateColumns();
            }
            Rows.Clear();
            Int64 id = 100;
            foreach (DataTable table in Controller.DataSet.Tables)
            {
                if (table.TableName != Defs.TableName)
                {
                    DataRow row = NewRow();
                    row[Defs.Columns.Id] = id++;
                    row[Defs.Columns.Name] = table.TableName.ToUpper();
                    row[Defs.Columns.ColumnCount] = table.Columns.Count;
                    row[Defs.Columns.RowCount] = table.Rows.Count;
                    row[Defs.Columns.ParentRelationCount] = table.ParentRelations.Count;
                    row[Defs.Columns.ChildRelationCount] = table.ChildRelations.Count;
                    row[Defs.Columns.ConstraintCount] = table.Constraints.Count;
                    Rows.Add(row);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion

        /************************************************************************/

        /// <summary>
        /// Creates the columns
        /// </summary>
        private void CreateColumns()
        {
            Columns.Add(new DataColumn(Defs.Columns.Id, typeof(Int64)));
            Columns.Add(new DataColumn(Defs.Columns.Name, typeof(string)));
            Columns.Add(new DataColumn(Defs.Columns.ColumnCount, typeof(Int64)));
            Columns.Add(new DataColumn(Defs.Columns.RowCount, typeof(Int64)));
            Columns.Add(new DataColumn(Defs.Columns.ParentRelationCount, typeof(Int64)));
            Columns.Add(new DataColumn(Defs.Columns.ChildRelationCount, typeof(Int64)));
            Columns.Add(new DataColumn(Defs.Columns.ConstraintCount, typeof(Int64)));
        }
    }
}
