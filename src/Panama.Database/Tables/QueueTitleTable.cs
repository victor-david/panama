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
    /// Provides title to title queue mapping.
    /// </summary>
    public class QueueTitleTable : Core.ApplicationTableBase
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
            public const string TableName = "queuetitle";

            /// <summary>
            /// Provides static column names for this table.
            /// </summary>
            public static class Columns
            {
                /// <summary>
                /// The title queue id. This and <see cref=" TitleId"/> are the table's primary key.
                /// </summary>
                public const string QueueId = "queueid";

                /// <summary>
                /// The title id. This and <see cref=" QueueId"/> are the table's primary key.
                /// </summary>
                public const string TitleId = "titleid";

                /// <summary>
                /// Status of this title within the queue
                /// </summary>
                public const string Status = "status";

                /// <summary>
                /// Date associated with the title
                /// </summary>
                public const string Date = "date";

                /// <summary>
                /// Provides static column names for columns that get their value fron another table.
                /// </summary>
                public static class Joined
                {
                    /// <summary>
                    /// Queue name. This column gets its value from the <see cref="QueueTable"/>.
                    /// </summary>
                    public const string QueueName = "JoinQueueName";

                    /// <summary>
                    /// Title name. This column gets its value from the <see cref="TitleTable"/>.
                    /// </summary>
                    public const string Title = "JoinTitle";

                    /// <summary>
                    /// Title written data. This column gets its value from the <see cref="TitleTable"/>.
                    /// </summary>
                    public const string Written = "JoinTitleWritten";
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
        /// Initializes a new instance of the <see cref="QueueTitleTable"/> class.
        /// </summary>
        public QueueTitleTable() : base(Defs.TableName)
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
            Load(null, Defs.Columns.Date);
        }

        /// <summary>
        /// Provides an enumerable that gets all records in order of id.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<QueueTitleRow> EnumerateAll()
        {
            foreach (DataRow row in EnumerateRows(null, Defs.Columns.TitleId))
            {
                yield return QueueTitleRow.Create(row);
            }
        }

        /// <summary>
        /// Provides an enumerable that enumerates all records for the specified queue.
        /// </summary>
        /// <param name="queueId">The queue id</param>
        /// <returns>An enumerable</returns>
        public IEnumerable<QueueTitleRow> EnumerateAll(long queueId)
        {
            foreach (DataRow row in EnumerateRows($"{Defs.Columns.TitleId}={queueId}", Defs.Columns.Joined.Title))
            {
                yield return new QueueTitleRow(row);
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
                { Defs.Columns.QueueId, ColumnType.Integer },
                { Defs.Columns.TitleId, ColumnType.Integer },
                { Defs.Columns.Status, ColumnType.Integer },
                { Defs.Columns.Date, ColumnType.Timestamp, false, true }
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
                Defs.Columns.QueueId,
                Defs.Columns.TitleId
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
            CreateChildToParentColumn(Defs.Columns.Joined.QueueName, QueueTable.Defs.Relations.ToQueueTitle, QueueTable.Defs.Columns.Name);
            CreateChildToParentColumn(Defs.Columns.Joined.Title, TitleTable.Defs.Relations.ToQueueTitle, TitleTable.Defs.Columns.Title);
            CreateChildToParentColumn<DateTime>(Defs.Columns.Joined.Written, TitleTable.Defs.Relations.ToQueueTitle, TitleTable.Defs.Columns.Written);
        }
        #endregion
    }
}