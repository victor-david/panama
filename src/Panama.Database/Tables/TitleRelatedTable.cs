/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that maps tag values to their use with title records.
    /// </summary>
    public class TitleRelatedTable : Core.ApplicationTableBase
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
            public const string TableName = "titlerelated";

            /// <summary>
            /// Provides static column names for this table.
            /// </summary>
            public static class Columns
            {
                /// <summary>
                /// The name of the title id column. This and <see cref=" RelatedId"/> are the table's primary key.
                /// </summary>
                public const string TitleId = "titleid";
                /// <summary>
                /// The name of the related id column. This and <see cref="TitleId"/> are the table's primary key.
                /// </summary>
                public const string RelatedId = "relatedid";

                /// <summary>
                /// Provides static column names for columns that get their value fron another table.
                /// </summary>
                public static class Joined
                {
                    /// <summary>
                    /// The name of the related title column. This column gets its value from the <see cref="TitleTable"/>.
                    /// </summary>
                    public const string Title = "JoinTitle";

                    /// <summary>
                    /// The name of the related date written column. This column gets its value from the <see cref="TitleTable"/>.
                    /// </summary>
                    public const string Written = "JoinDateWritten";

                    /// <summary>
                    /// The name of the related date updated column. This column gets its value from the <see cref="TitleTable"/>.
                    /// </summary>
                    public const string Updated = "JoinDateUpdated";

                    /// <summary>
                    /// The name of the latest version path. This column gets its value from the <see cref="TitleTable"/>.
                    /// </summary>
                    public const string LatestVersionPath = "JoinLatestVerPath";

                    /// <summary>
                    /// The name of the latest version word count. This column gets its value from the <see cref="TitleTable"/>.
                    /// </summary>
                    public const string LatestVersionWordCount = "JoinLatestVerWordCount";
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
        /// Initializes a new instance of the <see cref="TitleRelatedable"/> class.
        /// </summary>
        public TitleRelatedTable() : base(Defs.TableName)
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
        public IEnumerable<TitleRelatedRow> EnumerateAll()
        {
            foreach (DataRow row in EnumerateRows())
            {
                yield return new TitleRelatedRow(row);
            }
        }

        /// <summary>
        /// Provides an enumerable that enumerates all records for the specified title.
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <returns>An enumerable</returns>
        public IEnumerable<TitleRelatedRow> EnumerateAll(long titleId)
        {
            foreach (DataRow row in EnumerateRows($"{Defs.Columns.TitleId}={titleId}", Defs.Columns.Joined.Title))
            {
                yield return new TitleRelatedRow(row);
            }
        }

        /// <summary>
        /// Adds the specifed related titles to the specified title
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="titles">The related titles to add</param>
        public void AddIfNotExist(long titleId, List<TitleRow> titles)
        {
            _ = titles ?? throw new ArgumentNullException(nameof(titles));
            titles.ForEach(title => AddIfNotExist(titleId, title.Id));
        }

        /// <summary>
        /// Adds the specified related title to the specified title if it doesn't already exist
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="relatedId">The related id to add</param>
        public void AddIfNotExist(long titleId, long relatedId)
        {
            if (titleId != relatedId && !RelatedExists(titleId, relatedId))
            {
                AddRelatedRowPair(titleId, relatedId);
            }
        }

        /// <summary>
        /// Removes the specified related title from the specified title if it exists
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="relatedId">The related id to remove</param>
        public void RemoveIfExist(long titleId, long relatedId)
        {
            RelatedRowPair pair = GetRelatedRowPair(titleId, relatedId);
            pair.DeleteIfExists();
            Save();
        }

        /// <summary>
        /// Gets a boolean value that indicates if the specified related title exists for the specified title
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="relatedId">The related id</param>
        /// <returns>true if the related title exists for the title; otherwise, false.</returns>
        public bool RelatedExists(long titleId, long relatedId)
        {
            return GetRelatedRowPair(titleId, relatedId).Exists();
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
                { Defs.Columns.RelatedId, ColumnType.Integer },
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
                Defs.Columns.RelatedId
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
            CreateChildToParentColumn(Defs.Columns.Joined.Title, TitleTable.Defs.Relations.ToTitleRelated, TitleTable.Defs.Columns.Title);
            CreateChildToParentColumn<DateTime>(Defs.Columns.Joined.Written, TitleTable.Defs.Relations.ToTitleRelated, TitleTable.Defs.Columns.Written);
            CreateChildToParentColumn<DateTime>(Defs.Columns.Joined.Updated, TitleTable.Defs.Relations.ToTitleRelated, TitleTable.Defs.Columns.Calculated.LastestVersionDate);
            CreateChildToParentColumn(Defs.Columns.Joined.LatestVersionPath, TitleTable.Defs.Relations.ToTitleRelated, TitleTable.Defs.Columns.Calculated.LastestVersionPath);
            CreateChildToParentColumn(Defs.Columns.Joined.LatestVersionWordCount, TitleTable.Defs.Relations.ToTitleRelated, TitleTable.Defs.Columns.Calculated.LastestVersionWordCount);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private RelatedRowPair GetRelatedRowPair(long titleId, long relatedId)
        {
            return new RelatedRowPair
                (
                    GetSingleRow($"{Defs.Columns.TitleId}={titleId} AND {Defs.Columns.RelatedId}={relatedId}"),
                    GetSingleRow($"{Defs.Columns.TitleId}={relatedId} AND {Defs.Columns.RelatedId}={titleId}")
                );
        }

        private void AddRelatedRowPair(long titleId, long relatedId)
        {
            RelatedRowPair pair = new RelatedRowPair(NewRow(), NewRow());

            pair.TitleRow[Defs.Columns.TitleId] = titleId;
            pair.TitleRow[Defs.Columns.RelatedId] = relatedId;
            pair.RelatedRow[Defs.Columns.TitleId] = relatedId;
            pair.RelatedRow[Defs.Columns.RelatedId] = titleId;

            Rows.Add(pair.TitleRow);
            Rows.Add(pair.RelatedRow);
            Save();
        }
        #endregion

        /************************************************************************/

        #region Private helper class
        private class RelatedRowPair
        {
            public DataRow TitleRow { get; }
            public DataRow RelatedRow { get; }
            public RelatedRowPair(DataRow titleRow, DataRow relatedRow)
            {
                TitleRow = titleRow;
                RelatedRow = relatedRow;
            }

            public bool Exists()
            {
                Validate();
                return TitleRow != null && RelatedRow != null;
            }

            public void DeleteIfExists()
            {
                if (Exists())
                {
                    TitleRow.Delete();
                    RelatedRow.Delete();
                }
            }

            private void Validate()
            {
                if ((TitleRow == null && RelatedRow != null) || (RelatedRow == null && TitleRow != null))
                {
                    throw new InvalidOperationException("Title related mismatch");
                }
            }
        }
        #endregion
    }
}