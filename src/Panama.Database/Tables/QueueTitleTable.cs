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
                    /// Title written date. This column gets its value from the <see cref="TitleTable"/>.
                    /// </summary>
                    public const string Written = "JoinTitleWritten";
                    
                    /// <summary>
                    /// Title updated date. This column gets its value from the <see cref="TitleTable"/>.
                    /// </summary>
                    public const string Updated = "JoinTitleUpdated";

                    /// <summary>
                    /// Title word count (latest version). This column gets its value from the <see cref="TitleTable"/>.
                    /// </summary>
                    public const string WordCount = "JoinTitleWordCount";

                    /// <summary>
                    /// Status friendly string. This column gets it value from <see cref="QueueTitleStatusTable"/>.
                    /// </summary>
                    public const string Status = "JoinStatus";
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

        /// <summary>
        /// Adds the specified title to the specified queue
        /// </summary>
        /// <param name="queueId">The queue id</param>
        /// <param name="titleId">The title id</param>
        /// <remarks>
        /// A title cannot be added to the same queue more than once. 
        /// This method checks whether the title is already in the queue.
        /// If so, the title is ignored.
        /// </remarks>
        public void AddTitle(long queueId, long titleId)
        {
            if (!QueueTitleExists(queueId, titleId))
            {
                DataRow row = NewRow();
                row[Defs.Columns.QueueId] = queueId;
                row[Defs.Columns.TitleId] = titleId;
                row[Defs.Columns.Status] = 0;
                row[Defs.Columns.Date] = DBNull.Value;
                Rows.Add(row);
            }
        }

        /// <summary>
        /// Adds the specified titles to the specified queue
        /// </summary>
        /// <param name="queueId">The queue id</param>
        /// <param name="titles">The titles to add</param>
        /// <remarks>
        /// A title cannot be added to the same queue more than once. 
        /// This method checks each title to see if it is already in the queue.
        /// If so, the title is ignored.
        /// </remarks>/// 
        public void AddTitles(long queueId, List<TitleRow> titles)
        {
            _ = titles ?? throw new ArgumentNullException(nameof(titles));
            titles.ForEach(title =>
            {
                AddTitle(queueId, title.Id);
            });
            Save();
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
            CreateChildToParentColumn<DateTime>(Defs.Columns.Joined.Updated, TitleTable.Defs.Relations.ToQueueTitle, TitleTable.Defs.Columns.Calculated.LatestVersionDate);
            CreateChildToParentColumn(Defs.Columns.Joined.WordCount, TitleTable.Defs.Relations.ToQueueTitle, TitleTable.Defs.Columns.Calculated.LatestVersionWordCount);
            CreateChildToParentColumn(Defs.Columns.Joined.Status, QueueTitleStatusTable.Defs.Relations.ToQueueTitle, QueueTitleStatusTable.Defs.Columns.Name);
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Removes all queue/title records for the specified queue.
        /// </summary>
        /// <param name="queueId">The queue id</param>
        internal void RemoveQueue(long queueId)
        {
            DataRow[] rows = Select($"{Defs.Columns.QueueId}={queueId}");
            foreach(DataRow row in rows)
            {
                row.Delete();
            }
            Save();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private bool QueueTitleExists(long queueId, long titleId)
        {
            long len = Select($"{Defs.Columns.QueueId}={queueId} and {Defs.Columns.TitleId}={titleId}").Length;
            return len > 0;
        }
        #endregion
    }
}