/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains tags that may be applied to a title record.
    /// </summary>
    public class TagTable : Core.ApplicationTableBase
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
        /// <summary>
        /// Initializes a new instance of the <see cref="TagTable"/> class.
        /// </summary>
        public TagTable() : base(Defs.TableName)
        {
        }
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
        /// Gets the column definitions for this table.
        /// </summary>
        /// <returns>A <see cref="ColumnDefinitionCollection"/>.</returns>
        protected override ColumnDefinitionCollection GetColumnDefinitions()
        {
            return new ColumnDefinitionCollection()
            {
                { Defs.Columns.Id, ColumnType.Integer, true },
                { Defs.Columns.Tag, ColumnType.Text },
                { Defs.Columns.Description, ColumnType.Text },
                { Defs.Columns.UsageCount, ColumnType.Integer, false, false, 0 },
            };
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
    }
}