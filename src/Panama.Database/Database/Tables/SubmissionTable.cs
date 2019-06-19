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
{    /// <summary>
     /// Represents the table that contains submission records. A submission record maps to a title and the submission batch.
     /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class SubmissionTable : TableBase
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
                public const string Id = "id";

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
        /// Initializes a new instance of the <see cref="SubmissionTable"/> class.
        /// </summary>
        public SubmissionTable() : base(DatabaseController.Instance, Defs.TableName)
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
            int test = Columns.Count;
            int some = Rows.Count;

            UniqueConstraint batchTitle =  new UniqueConstraint(new DataColumn[] 
            { 
                Columns[Defs.Columns.BatchId], 
                Columns[Defs.Columns.TitleId]
            });

            Constraints.Add(batchTitle);
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
            DataRow[] rows = Select(string.Format("{0}={1} AND {2}={3}", Defs.Columns.BatchId, batchId, Defs.Columns.TitleId, titleId));
            return (rows.Length > 0);
        }

        /// <summary>
        /// Gets the highest numbered ordering value for the specified submission batch.
        /// </summary>
        /// <param name="batchId">The batch to check.</param>
        /// <returns>The highest numbered ordering for <paramref name="batchId"/>.</returns>
        public long GetHighestOrdering(long batchId)
        {
            DataRow[] rows = Select($"{Defs.Columns.BatchId}={batchId}", $"{Defs.Columns.Ordering} DESC");
            if (rows.Length > 0)
            {
                return (long)rows[0][Defs.Columns.Ordering];
            }
            return 0;
        }

        /// <summary>
        /// Changes the ordering of the specified row by moving it up.
        /// </summary>
        /// <param name="rowToMove">The row to move up.</param>
        public void MoveSubmissionUp(DataRow rowToMove)
        {
            if (rowToMove == null) throw new ArgumentNullException(nameof(rowToMove));
            long batchId = (long)rowToMove[Defs.Columns.BatchId];
            EnsureCorrectOrdering(batchId);
            long ordering = (long)rowToMove[Defs.Columns.Ordering];
            ChangeSubmissionOrdering(batchId, ordering, ordering - 1);
        }

        /// <summary>
        /// Changes the ordering of the specified row by moving it down.
        /// </summary>
        /// <param name="rowToMove">The row to move up.</param>
        public void MoveSubmissionDown(DataRow rowToMove)
        {
            if (rowToMove == null) throw new ArgumentNullException(nameof(rowToMove));
            long batchId = (long)rowToMove[Defs.Columns.BatchId];
            EnsureCorrectOrdering(batchId);
            long ordering = (long)rowToMove[Defs.Columns.Ordering];
            ChangeSubmissionOrdering(batchId, ordering, ordering + 1);
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
            return Resources.Create.Submission;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate, DataColumnPropertyKey.ReceiveInsertedId);
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Joined"/> and <see cref="Defs.Columns.Calculated"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            CreateChildToParentColumn(Defs.Columns.Joined.Title, TitleTable.Defs.Relations.ToSubmission, TitleTable.Defs.Columns.Title);
            CreateChildToParentColumn<DateTime>(Defs.Columns.Joined.Written, TitleTable.Defs.Relations.ToSubmission, TitleTable.Defs.Columns.Written);
            CreateChildToParentColumn<DateTime>(Defs.Columns.Joined.Submitted, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Submitted);
            CreateChildToParentColumn<DateTime>(Defs.Columns.Joined.SubmittedCalc, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Calculated.Submitted);

            CreateExpressionColumn<long>(Defs.Columns.Calculated.CurrentSubCount, string.Format("IIF(Parent({0}).{1} IS NULL, 1, 0)", SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Response));
            CreateChildToParentColumn<DateTime>(Defs.Columns.Joined.Response, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Response);
            CreateChildToParentColumn(Defs.Columns.Joined.PublisherId, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.PublisherId);
            CreateChildToParentColumn(Defs.Columns.Joined.Publisher, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Joined.Publisher);
            CreateChildToParentColumn<bool>(Defs.Columns.Joined.PublisherExclusive, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Joined.PublisherExclusive);
            CreateChildToParentColumn(Defs.Columns.Joined.ResponseTypeName, SubmissionBatchTable.Defs.Relations.ToSubmission, SubmissionBatchTable.Defs.Columns.Joined.ResponseTypeName);
        }
        #endregion

        /************************************************************************/

        #region Internal methods
        /// <summary>
        /// Checks if the specified title exists within the specified batch.
        /// </summary>
        /// <param name="batchId">The submission batch id</param>
        /// <param name="titleId">The title id</param>
        /// <returns>true if the title is part of the batch;otherwise, false.</returns>
        internal bool TitleExistsInBatch(long batchId, long titleId)
        {
            DataRow[] rows = Select($"{Defs.Columns.BatchId}={batchId} AND {Defs.Columns.TitleId}={titleId}");
            return rows.Length > 0;
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

        private DataRow[] GetDataRowsByBatch(long batchId)
        {
            return Select($"{Defs.Columns.BatchId}={batchId}", Defs.Columns.Ordering);
        }

        private void EnsureCorrectOrdering(long batchId)
        {
            long ordering = 1;
            foreach (DataRow row in GetDataRowsByBatch(batchId))
            {
                row[Defs.Columns.Ordering] = ordering++;
            }
        }

        private void ChangeSubmissionOrdering(long batchId, long ordering, long newOrdering)
        {
            DataRow orderingRow = null;
            DataRow newOrderingRow = null;

            foreach (DataRow row in GetDataRowsByBatch(batchId))
            {
                long rowOrdering = (long)row[Defs.Columns.Ordering];
                if (rowOrdering == ordering) orderingRow = row;
                if (rowOrdering == newOrdering) newOrderingRow = row;
            }

            if (orderingRow != null && newOrderingRow != null)
            {
                orderingRow[Defs.Columns.Ordering] = newOrdering;
                newOrderingRow[Defs.Columns.Ordering] = ordering;
            }
        }
        #endregion
    }
}