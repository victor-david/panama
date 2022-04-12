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
    /// Represents the table that contains submission records. A submission record maps to a title and the submission batch.
    /// </summary>
    public class SubmissionTable : Core.ApplicationTableBase
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
            public const string TableName = "submission";

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
                /// The name of the title id column.
                /// </summary>
                public const string TitleId = "titleid";

                /// <summary>
                /// The name of the submission batch id column.
                /// </summary>
                public const string BatchId = "submissionbatchid";

                /// <summary>
                /// The name of the ordering column. Used to express the desired order of the titles within the submission document.
                /// </summary>
                public const string Ordering = "ordering";

                /// <summary>
                /// The name of the status column. See <see cref="Values"/>.
                /// </summary>
                public const string Status = "status";

                /// <summary>
                /// The name of the added column. Holds the date / time the
                /// title was added to the submission.
                /// </summary>
                public const string Added = "added";

                /// <summary>
                /// Provides static column names for columns that get their value fron another table.
                /// </summary>
                public static class Joined
                {
                    /// <summary>
                    /// The name of the column which holds the title.
                    /// </summary>
                    public const string Title = "JoinTitle";

                    /// <summary>
                    /// The name of the column which holds the date the title was written.
                    /// </summary>
                    public const string Written = "JoinWritten";

                    /// <summary>
                    /// The name of the column that holds the date that the submission batch was submitted.
                    /// </summary>
                    public const string Submitted = "JoinBatchSubmitted";

                    /// <summary>
                    /// The name of the column that holds the calculated submission date. 
                    /// This column is used for custom sorting.
                    /// </summary>
                    public const string SubmittedCalc = "JoinBatchSubmittedCalc";

                    /// <summary>
                    /// The name of the column that hold the date that the submission batch received a response.
                    /// </summary>
                    public const string Response = "JoinBatchResponse";

                    /// <summary>
                    /// The name of the column that holds the publisher id.
                    /// </summary>
                    public const string PublisherId = "JoinPublisherId";

                    /// <summary>
                    /// The name of the column that holds the publisher name.
                    /// </summary>
                    public const string Publisher = "JoinToJoinPublisher";

                    /// <summary>
                    /// The name of the publisher exclusive column.
                    /// </summary>
                    public const string PublisherExclusive = "JoinToJoinPubExclusive";

                    /// <summary>
                    /// The name of the column that holds the name of the response for the submission batch.
                    /// </summary>
                    public const string ResponseTypeName = "JoinToJoinReponseName";
                }

                /// <summary>
                /// Provides static column names for columns that are calculated from other values.
                /// </summary>
                public static class Calculated
                {
                    /// <summary>
                    /// The name of the column that holds the current submission count.
                    /// </summary>
                    public const string CurrentSubCount = "CalcCurrSubCount";
                }
            }

            /// <summary>
            /// Provide static integr values.
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// The value used when <see cref="Defs.Columns.Status"/> is unspecified.
                /// </summary>
                public const long StatusNotSpecified = 0;

                /// <summary>
                /// The value used when <see cref="Defs.Columns.Status"/> is withdrawn.
                /// </summary>
                public const long StatusWithdrawn = 1;

                /// <summary>
                /// The value used when <see cref="Defs.Columns.Status"/> is accepted.
                /// </summary>
                /// <remarks>
                /// This has the same value as ResponseTable's accepted value, which is 255.
                /// It doesn't have to, but for a bit more ease of clarity when viewing
                /// the table data directly during troubleshooting ,etc.
                /// </remarks>
                public const long StatusAccepted = ResponseTable.Defs.Values.ResponseAccepted;
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionTable"/> class.
        /// </summary>
        public SubmissionTable() : base(Defs.TableName)
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
            Load(null, Defs.Columns.Added);

            UniqueConstraint batchTitle = new UniqueConstraint(new DataColumn[]
            {
                Columns[Defs.Columns.BatchId],
                Columns[Defs.Columns.TitleId]
            });

            Constraints.Add(batchTitle);
        }

        /// <summary>
        /// Provides an enumerable that enumerates all record with the specified batch in order of ordering.
        /// </summary>
        /// <param name="batchId">The batch id</param>
        /// <returns>An enumerable</returns>
        public IEnumerable<SubmissionRow> EnumerateAll(long batchId)
        {
            foreach (DataRow row in EnumerateRows($"{Defs.Columns.BatchId}={batchId}", Defs.Columns.Ordering))
            {
                yield return SubmissionRow.Create(row);
            }
        }

        /// <summary>
        /// Adds a record to the submission table
        /// </summary>
        /// <param name="batchId">The id of the submission batch</param>
        /// <param name="titleId">The title id</param>
        public void AddSubmission(long batchId, long titleId)
        {
            if (!SubmissionExists(batchId, titleId))
            {
                DataRow row = NewRow();
                row[Defs.Columns.BatchId] = batchId;
                row[Defs.Columns.TitleId] = titleId;
                row[Defs.Columns.Ordering] = NextOrdering(batchId);
                row[Defs.Columns.Status] = Defs.Values.StatusNotSpecified;
                row[Defs.Columns.Added] = DateTime.UtcNow;
                Rows.Add(row);
                Save();
            }
        }

        /// <summary>
        /// Gets a value that indicates if the specified title exists for the specified submission batch.
        /// </summary>
        /// <param name="batchId">The submission batch id.</param>
        /// <param name="titleId">The title id.</param>
        /// <returns>true if exists; otherwise, false.</returns>
        public bool SubmissionExists(long batchId, long titleId)
        {
            return Select($"{Defs.Columns.BatchId}={batchId} AND {Defs.Columns.TitleId}={titleId}").Length > 0;
        }

        /// <summary>
        /// Gets the highest numbered ordering value for the specified submission batch.
        /// </summary>
        /// <param name="batchId">The batch to check.</param>
        /// <returns>The highest numbered ordering for <paramref name="batchId"/>.</returns>
        public long GetHighestOrdering(long batchId)
        {
            DataRow[] rows = Select($"{Defs.Columns.BatchId}={batchId}", $"{Defs.Columns.Ordering} DESC");
            return rows.Length > 0 ? (long)rows[0][Defs.Columns.Ordering] : 0;
        }

        /// <summary>
        /// Changes the ordering of the specified row by moving it up.
        /// </summary>
        /// <param name="rowToMove">The row to move up.</param>
        public void MoveSubmissionUp(SubmissionRow rowToMove)
        {
            if (rowToMove == null)
            {
                throw new ArgumentNullException(nameof(rowToMove));
            }

            EnsureCorrectOrdering(rowToMove.BatchId);
            ChangeSubmissionOrdering(rowToMove.BatchId, rowToMove.Ordering, rowToMove.Ordering - 1);
        }

        /// <summary>
        /// Changes the ordering of the specified row by moving it down.
        /// </summary>
        /// <param name="rowToMove">The row to move up.</param>
        public void MoveSubmissionDown(SubmissionRow rowToMove)
        {
            if (rowToMove == null)
            {
                throw new ArgumentNullException(nameof(rowToMove));
            }

            EnsureCorrectOrdering(rowToMove.BatchId);
            ChangeSubmissionOrdering(rowToMove.BatchId, rowToMove.Ordering, rowToMove.Ordering + 1);
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
            // TODO - Compound index
            // CREATE UNIQUE INDEX `submission_batchid_titleid` ON `submission` (`titleid` ,`submissionbatchid` )
            return new ColumnDefinitionCollection()
            {
                { Defs.Columns.Id, ColumnType.Integer, true },
                { Defs.Columns.TitleId, ColumnType.Integer },
                { Defs.Columns.BatchId, ColumnType.Integer },
                { Defs.Columns.Ordering, ColumnType.Integer, false, false, 0 },
                { Defs.Columns.Status, ColumnType.Integer, false, false, 0 },
                { Defs.Columns.Added, ColumnType.Timestamp },
            };
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Joined"/> and <see cref="Defs.Columns.Calculated"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            CreateChildToParentColumn(Defs.Columns.Joined.Title, TitleTable.Defs.Relations.ToSubmission, TitleTable.Defs.Columns.Title);
            CreateChildToParentColumn<DateTime>(Defs.Columns.Joined.Written, TitleTable.Defs.Relations.ToSubmission, TitleTable.Defs.Columns.Written);
            CreateChildToParentColumn<DateTime>(Defs.Columns.Joined.Submitted, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Submitted);
            // CreateChildToParentColumn<DateTime>(Defs.Columns.Joined.SubmittedCalc, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Calculated.Submitted);

            CreateExpressionColumn<long>(Defs.Columns.Calculated.CurrentSubCount, string.Format("IIF(Parent({0}).{1} IS NULL, 1, 0)", SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Response));
            CreateChildToParentColumn<DateTime>(Defs.Columns.Joined.Response, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Response);
            CreateChildToParentColumn<long>(Defs.Columns.Joined.PublisherId, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.PublisherId);
            CreateChildToParentColumn(Defs.Columns.Joined.Publisher, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Joined.Publisher);
            CreateChildToParentColumn<bool>(Defs.Columns.Joined.PublisherExclusive, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Joined.PublisherExclusive);
            CreateChildToParentColumn(Defs.Columns.Joined.ResponseTypeName, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Joined.ResponseTypeName);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Gets the next ordering value for the specified batch id.
        /// </summary>
        /// <param name="batchId">The batch id.</param>
        /// <returns>The next ordering value.</returns>
        private long NextOrdering(long batchId)
        {
            DataRow[] rows = Select(string.Format("{0}={1}", Defs.Columns.BatchId, batchId), string.Format("{0} DESC", Defs.Columns.Ordering));
            if (rows.Length > 0)
            {
                long lastOrdering = (long)rows[0][Defs.Columns.Ordering];
                return lastOrdering + 1;
            }
            return 1;
        }

        private void EnsureCorrectOrdering(long batchId)
        {
            long ordering = 1;
            foreach (SubmissionRow row in EnumerateAll(batchId))
            {
                row.Ordering = ordering++;
            }
        }

        private void ChangeSubmissionOrdering(long batchId, long ordering, long newOrdering)
        {
            SubmissionRow orderingRow = null;
            SubmissionRow newOrderingRow = null;

            foreach (SubmissionRow row in EnumerateAll(batchId))
            {
                // long rowOrdering = row.Ordering;
                if (row.Ordering == ordering)
                {
                    orderingRow = row;
                }

                if (row.Ordering == newOrdering)
                {
                    newOrderingRow = row;
                }
            }

            if (orderingRow != null && newOrderingRow != null)
            {
                orderingRow.Ordering = newOrdering;
                newOrderingRow.Ordering = ordering;
            }
        }
        #endregion
    }
}