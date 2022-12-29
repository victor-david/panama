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
    /// Represents the table that contains information on the status types a queue/title may have.
    /// This is a lookup table.
    /// </summary>
    public class QueueTitleStatusTable : Core.ApplicationTableBase
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
            public const string TableName = "queuetitlestatus";

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
                /// The name of the name column. This column holds the name of the status.
                /// </summary>
                public const string Name = "name";
            }

            /// <summary>
            /// Provides static relation names.
            /// </summary>
            public static class Relations
            {
                /// <summary>
                /// The name of the relation that relates the <see cref="QueueTitleStatusTable"/> to the <see cref="QueueTitleTable"/>.
                /// </summary>
                public const string ToQueueTitle = "StatusToQueueTitle";
            }

            /// <summary>
            /// Provides static reponse values.
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// The value used when status is idle / none
                /// </summary>
                public const long StatusIdle = 0;

                /// <summary>
                /// The value used when status is pending.
                /// </summary>
                public const long StatusPending = 1;

                /// <summary>
                /// The value used when status is published
                /// </summary>
                public const long StatusPublished = 2;
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="QueueTitleStatusTable"/> class.
        /// </summary>
        public QueueTitleStatusTable() : base(Defs.TableName)
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
            Load(null, Defs.Columns.Id);
        }

        /// <summary>
        /// Provides an enumerable that enumerates all status values
        /// </summary>
        /// <returns>An enumerable</returns>
        public IEnumerable<ResponseRow> EnumerateResponses()
        {
            foreach (DataRow row in EnumerateRows(null, Defs.Columns.Id))
            {
                yield return new ResponseRow(row);
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
                { Defs.Columns.Name, ColumnType.Text },
            };
        }

        /// <summary>
        /// Gets a list of column names to use in subsequent initial insert operations.
        /// These are used only when the table is empty, i.e. upon first creation.
        /// </summary>
        /// <returns>A list of column names</returns>
        protected override List<string> GetPopulateColumnList()
        {
            return new List<string>() { Defs.Columns.Id, Defs.Columns.Name };
        }

        /// <summary>
        /// Provides an enumerable that returns values for each row to be populated.
        /// </summary>
        /// <returns>An IEnumerable</returns>
        protected override IEnumerable<object[]> EnumeratePopulateValues()
        {
            yield return new object[] { Defs.Values.StatusIdle, "Idle" };
            yield return new object[] { Defs.Values.StatusPending, "Scheduled" };
            yield return new object[] { Defs.Values.StatusPublished, "Published" };
        }

        /// <summary>
        /// Establishes parent / child relationships with other tables.
        /// </summary>
        protected override void SetDataRelations()
        {
            CreateParentChildRelation<QueueTitleTable>(Defs.Relations.ToQueueTitle, Defs.Columns.Id, QueueTitleTable.Defs.Columns.Status);
        }
        #endregion
    }
}