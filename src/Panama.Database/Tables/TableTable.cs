/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Toolkit.Core.Database.SQLite;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that displays a list of tables in the data set. This table
    /// is memory only; it is not part of the database on disk.
    /// </summary>
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
            get => Defs.Columns.Name;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableTable"/> class.
        /// </summary>
        public TableTable() : base(DatabaseController.Instance, Defs.TableName)
        {
        }
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
            long id = 100;
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
            Columns.Add(new DataColumn(Defs.Columns.Id, typeof(long)));
            Columns.Add(new DataColumn(Defs.Columns.Name, typeof(string)));
            Columns.Add(new DataColumn(Defs.Columns.ColumnCount, typeof(long)));
            Columns.Add(new DataColumn(Defs.Columns.RowCount, typeof(long)));
            Columns.Add(new DataColumn(Defs.Columns.ParentRelationCount, typeof(long)));
            Columns.Add(new DataColumn(Defs.Columns.ChildRelationCount, typeof(long)));
            Columns.Add(new DataColumn(Defs.Columns.ConstraintCount, typeof(long)));
        }
    }
}