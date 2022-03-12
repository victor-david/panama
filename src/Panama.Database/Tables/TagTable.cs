/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System.Collections.Generic;
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
                public const string Id = DefaultPrimaryKeyName;

                /// <summary>
                /// The name of the tag column.
                /// </summary>
                public const string Tag = "tag";

                /// <summary>
                /// The name of the tag description column.
                /// </summary>
                public const string Description = "description";

                /// <summary>
                /// Provides static names for calculated columns
                /// </summary>
                public static class Calculated
                {
                    public const string UsageCount = "CalcUsageCount";
                }
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
        /// Provides an enumerable that enumerates all tags in order of tag name
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TagRow> EnumerateAll()
        {
            foreach (DataRow row in EnumerateRows(null, Defs.Columns.Tag))
            {
                yield return new TagRow(row);
            }
        }

        /// <summary>
        /// Checks child rows via the <see cref="Defs.Relations.ToTitleTag"/> relation and updates the tag usage count.
        /// </summary>
        //public void RefreshTagUsage()
        //{
        //    foreach (TagRow tagRow in EnumerateAll())
        //    {
        //        tagRow.UsageCount = tagRow.Row.GetChildRows(Defs.Relations.ToTitleTag).LongLength;
        //    }
        //}
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
            };
        }

        /// <summary>
        /// Gets a list of column names to use in subsequent initial insert operations.
        /// These are used only when the table is empty, i.e. upon first creation.
        /// </summary>
        /// <returns>A list of column names</returns>
        protected override List<string> GetPopulateColumnList()
        {
            return new List<string>() { Defs.Columns.Id, Defs.Columns.Tag, Defs.Columns.Description };
        }

        /// <summary>
        /// Provides an enumerable that returns values for each row to be populated.
        /// </summary>
        /// <returns>An IEnumerable</returns>
        protected override IEnumerable<object[]> EnumeratePopulateValues()
        {
            yield return new object[] { 1, "Poetry", "Poetry is what poetry does" };
            yield return new object[] { 2, "Fiction", "A standard story, generally at least 2000 words" };
            yield return new object[] { 3, "Flash", "Flash fiction, closer to a story, and not that long, 1 page or so" };
            yield return new object[] { 4, "Essay", "An essay or other article" };
            yield return new object[] { 5, "Manuscript", "Part of a larger manuscript" };
            yield return new object[] { 6, "Rant", "You know what a rant is, right?" };
            yield return new object[] { 7, "Raw", "Work in progress" };
        }

        /// <summary>
        /// Establishes parent / child relationships with other tables.
        /// </summary>
        protected override void SetDataRelations()
        {
            CreateParentChildRelation<TitleTagTable>(Defs.Relations.ToTitleTag, Defs.Columns.Id, TitleTagTable.Defs.Columns.TagId);
        }

        /// <inheritdoc/>
        protected override void UseDataRelations()
        {
            string expr = string.Format("Count(Child({0}).{1})", Defs.Relations.ToTitleTag, TitleTagTable.Defs.Columns.TagId);
            CreateExpressionColumn<long>(Defs.Columns.Calculated.UsageCount, expr);
        }

        /// <summary>
        /// Populates a new row with default (starter) values
        /// </summary>
        /// <param name="row">The freshly created DataRow to poulate</param>
        protected override void PopulateDefaultRow(System.Data.DataRow row)
        {
            row[Defs.Columns.Tag] = "(new tag)";
            row[Defs.Columns.Description] = "(new description)";
        }
        #endregion
    }
}