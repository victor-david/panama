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
    /// Represents the table that maps tag values to their use with title records.
    /// </summary>
    public class TitleTagTable : Core.ApplicationTableBase
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
            public const string TableName = "titletag";

            /// <summary>
            /// Provides static column names for this table.
            /// </summary>
            public static class Columns
            {
                /// <summary>
                /// The name of the title id column. This and <see cref=" TagId"/> are the table's primary key.
                /// </summary>
                public const string TitleId = "titleid";
                /// <summary>
                /// The name of the tag id column. This and <see cref="TitleId"/> are the table's primary key.
                /// </summary>
                public const string TagId = "tagid";

                /// <summary>
                /// Provides static column names for columns that get their value fron another table.
                /// </summary>
                public static class Joined
                {
                    /// <summary>
                    /// The name of the tag name column. This column gets its value from the <see cref="TagTable"/>.
                    /// </summary>
                    public const string TagName = "JoinTagName";

                    /// <summary>
                    /// The name of the tag name column. This column gets its value from the <see cref="TagTable"/>.
                    /// </summary>
                    public const string TagDescription = "JoinTagDescription";
                }
            }
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName => null;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleTagTable"/> class.
        /// </summary>
        public TitleTagTable() : base(Defs.TableName)
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
            Load(null, null);
        }

        /// <summary>
        /// Provides an enumerable that enumerates all records.
        /// </summary>
        /// <returns>An enumerable</returns>
        public IEnumerable<TitleTagRow> EnumerateAll()
        {
            foreach (DataRow row in EnumerateRows())
            {
                yield return new TitleTagRow(row);
            }
        }

        /// <summary>
        /// Provides an enumerable that enumerates all records for the specified title.
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <returns>An enumerable</returns>
        public IEnumerable<TitleTagRow> EnumerateAll(long titleId)
        {
            foreach (DataRow row in EnumerateRows($"{Defs.Columns.TitleId}={titleId}", Defs.Columns.Joined.TagName))
            {
                yield return new TitleTagRow(row);
            }
        }

        /// <summary>
        /// Provides an enumerable that enumerates all title ids associated with the sepcified tag id
        /// </summary>
        /// <param name="tagId">The tag id</param>
        /// <returns>An enumerable</returns>
        public IEnumerable<long> EnumerateTitleIdsForTag(long tagId)
        {
            DataRow[] rows = Select($"{Defs.Columns.TagId}={tagId}");
            foreach (DataRow row in rows)
            {
                yield return (long)row[Defs.Columns.TitleId];
            }
        }

        /// <summary>
        /// Adds the specified tag to the specified title if it doesn't already exist
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="tagId">The tag id to add</param>
        /// <returns>true if added; false if tag already exists</returns>
        public bool AddIfNotExist(long titleId, long tagId)
        {
            return !TagExists(titleId, tagId) && AddRow(titleId, tagId);
        }

        /// <summary>
        /// Removes the specified tag from the specified title if it exists
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="tagId">The tag id to add</param>
        /// <returns>true if removed; false if tag does not exist</returns>
        public bool RemoveIfExist(long titleId, long tagId)
        {
            DataRow row = GetRow(titleId, tagId);
            row?.Delete();
            Save();
            return row != null;
        }

        /// <summary>
        /// Gets a boolean value that indicates if the specified tag exists for the specified title
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="tagId">The tag id</param>
        /// <returns>true if the tag exists for the title; otherwise, false.</returns>
        public bool TagExists(long titleId, long tagId)
        {
            return GetRow(titleId, tagId) != null;
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
                { Defs.Columns.TitleId, ColumnType.Integer },
                { Defs.Columns.TagId, ColumnType.Integer },
            };
        }

        /// <summary>
        /// Gets the primary key collection
        /// </summary>
        /// <returns>A primary key collection for a compound primary key</returns>
        protected override PrimaryKeyCollection GetPrimaryKeyDefinition()
        {
            return new PrimaryKeyCollection()
            {
                Defs.Columns.TitleId,
                Defs.Columns.TagId
            };
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            // override the base method to do nothing
        }
    
        /// <summary>
        /// Creates the <see cref="Defs.Columns.Joined"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            CreateChildToParentColumn(Defs.Columns.Joined.TagName, TagTable.Defs.Relations.ToTitleTag, TagTable.Defs.Columns.Tag);
            CreateChildToParentColumn(Defs.Columns.Joined.TagDescription, TagTable.Defs.Relations.ToTitleTag, TagTable.Defs.Columns.Description);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private DataRow GetRow(long titleId, long tagId)
        {
            return GetSingleRow($"{Defs.Columns.TitleId}={titleId} AND {Defs.Columns.TagId}={tagId}");
        }

        private bool AddRow(long titleId, long tagId)
        {
            DataRow row = NewRow();
            row[Defs.Columns.TagId] = tagId;
            row[Defs.Columns.TitleId] = titleId;
            Rows.Add(row);
            Save();
            return true;
        }
        #endregion
    }
}