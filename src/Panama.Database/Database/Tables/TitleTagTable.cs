/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using Restless.Toolkit.Core.Utility;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that maps tag values to their use with title records.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class TitleTagTable : TableBase
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
                }
            }
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get { return null; }
        }
        #endregion

        /************************************************************************/
        
        #region Constructor
        public TitleTagTable() : base(DatabaseController.Instance, Defs.TableName)
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
        /// Adds the specified tag to the specified title if it doesn't already exist
        /// </summary>
        /// <param name="tagId">The tag id to add</param>
        /// <param name="titleId">The title id</param>
        public void Add(long tagId, long titleId)
        {
            if (!TagExists(tagId, titleId))
            {
                DataRow row = NewRow();
                row[Defs.Columns.TagId] = tagId;
                row[Defs.Columns.TitleId] = titleId;
                Rows.Add(row);
            }
        }

        /// <summary>
        /// Removes the specified tag from the specified title if it exists
        /// </summary>
        /// <param name="tagId">The tag id to add</param>
        /// <param name="titleId">The title id</param>
        public void Remove(long tagId, long titleId)
        {
            DataRow row = GetTitleTagRow(tagId, titleId);
            if (row != null)
            {
                row.Delete();
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates if the specified tag exists for the specified title
        /// </summary>
        /// <param name="tagId">The tag id</param>
        /// <param name="titleId">The title id</param>
        /// <returns>true if the tag exists for the title; otherwise, false.</returns>
        public bool TagExists(long tagId, long titleId)
        {
            DataRow row = GetTitleTagRow(tagId, titleId);
            return row != null;
        }

        /// <summary>
        /// Gets a string list of title id values associated with the specified tag id. Adds -1 even if empty.
        /// </summary>
        /// <param name="tagId">The tag id</param>
        /// <returns>A string ready for filter use.</returns>
        public string GetTitleIdsForTag(long tagId)
        {
            Integer64List ids = new Integer64List();
            DataRow[] rows = Select(string.Format("{0}={1}", Defs.Columns.TagId, tagId));
            foreach (DataRow row in rows)
            {
                ids.Add((long)row[Defs.Columns.TitleId]);
            }
            return ids.ToString(Integer64List.DefaultDelimter, Integer64List.ToStringDisposition.AddNegativeOneEvenIfEmpty);
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
            return Resources.Create.TitleTag;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Joined"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            CreateChildToParentColumn(Defs.Columns.Joined.TagName, TagTable.Defs.Relations.ToTitleTag, TagTable.Defs.Columns.Tag);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Gets the specified row
        /// </summary>
        /// <param name="tagId">The tag id</param>
        /// <param name="titleId">The title id</param>
        /// <returns>The row, or null if it doesn't exist</returns>
        private DataRow GetTitleTagRow(long tagId, long titleId)
        {
            DataRow[] rows = Select(string.Format("{0}={1} AND {2}={3}", Defs.Columns.TagId, tagId, Defs.Columns.TitleId, titleId));
            if (rows.Length == 1)
            {
                return rows[0];
            }
            return null;
        }
        #endregion
        
        /************************************************************************/

        #region ITableImport and IColumnRowImporter implementation (commented out)
        //public bool PerformImport()
        //{
        //    return DatabaseImporter.Instance.ImportTable(this, this, "title_tag");
        //}

        //public string GetColumnName(string origColName)
        //{
        //    return origColName;
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