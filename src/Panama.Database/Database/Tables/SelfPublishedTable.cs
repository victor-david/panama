/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tools.Database.SQLite;
using System;
using System.Data;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains information about titles that have been self published.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class SelfPublishedTable : TableBase
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
            public const string TableName = "selfpublished";

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
                /// The name of the title id column.
                /// </summary>
                public const string TitleId = "titleid";

                /// <summary>
                /// The name of the self publisher id column.
                /// </summary>
                public const string SelfPublisherId = "selfpublisherid";

                /// <summary>
                /// The name of the added column. This column holds the date that the published record was added.
                /// </summary>
                public const string Added = "added";

                /// <summary>
                /// The name of the published column. This column holds the date that the corresponding title was published.
                /// </summary>
                public const string Published = "published";

                /// <summary>
                /// The name of the url column. This column holds the url to the published title.
                /// </summary>
                public const string Url = "url";

                /// <summary>
                /// The name of the notes column. This column holds any notes associated with the published record.
                /// </summary>
                public const string Notes = "notes";

                /// <summary>
                /// Provides static column names for columns that get their value fron another table.
                /// </summary>
                public static class Joined
                {
                    /// <summary>
                    /// The name of the self publisher column. This column gets its value from the <see cref="SelfPublisherTable"/>.
                    /// </summary>
                    public const string SelfPublisher = "JoinSelfPubName";
                }
            }
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get => Defs.Columns.Id;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfPublishedTable"/> class.
        /// </summary>
        public SelfPublishedTable() : base(DatabaseController.Instance, Defs.TableName)
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
            Load(null, Defs.Columns.TitleId);
        }

        /// <summary>
        /// Adds a published record
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="selfPublisherId">The self publisher id</param>
        public void Add(long titleId, long selfPublisherId)
        {
            DataRow row = NewRow();
            row[Defs.Columns.TitleId] = titleId;
            row[Defs.Columns.SelfPublisherId] = selfPublisherId;
            row[Defs.Columns.Added] = DateTime.UtcNow;
            Rows.Add(row);
            Save();
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
            return Resources.Create.SelfPublished;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate, DataColumnPropertyKey.ReceiveInsertedId);
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Joined"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            CreateChildToParentColumn(Defs.Columns.Joined.SelfPublisher, SelfPublisherTable.Defs.Relations.ToPublished, SelfPublisherTable.Defs.Columns.Name);
        }
        #endregion
    }
}