using Restless.Panama.Database.Core;
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains information about a submission batch. A batch is associated
    /// with a single publisher and may reference various titles, documents, and messages.
    /// </summary>
    public class SubmissionBatchTable : Core.ApplicationTableBase
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
            public const string TableName = "submissionbatch";

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
                /// The name of the publisher id column.
                /// </summary>
                public const string PublisherId = "publisherid";

                /// <summary>
                /// The name of the fee column. This column holds the amount of any fee associated with the submission.
                /// </summary>
                public const string Fee = "fee";

                /// <summary>
                /// The name of the award column. This column holds the amount of any award associated with the submission.
                /// </summary>
                public const string Award = "award";

                /// <summary>
                /// The name of the online column. This column holds whether this submission is an online submission.
                /// </summary>
                public const string Online = "online";

                /// <summary>
                /// The name of the contest column. This column holds whether this submission is for a contest.
                /// </summary>
                public const string Contest = "contest";

                /// <summary>
                /// The name of the locked column. This column holds whether a submssion is locked.
                /// When a submission is locked, only certain operations are allowed.
                /// </summary>
                public const string Locked = "locked";

                /// <summary>
                /// The name of the submitted column. This column holds the date of the submission.
                /// </summary>
                public const string Submitted = "submitted";

                /// <summary>
                /// The name of the response column. This column holds the date of the response to the submission,
                /// or null if the submission hasn't yet received a response.
                /// </summary>
                public const string Response = "response";

                /// <summary>
                /// The name of the response type column. This column holds the type of response received.
                /// </summary>
                public const string ResponseType = "responsetype";

                /// <summary>
                /// The name of the notes column. This column holds any notes associated with the submission.
                /// </summary>
                public const string Notes = "notes";

                /// <summary>
                /// The name of the cover letter column.
                /// </summary>
                public const string Cover = "cover";

                /// <summary>
                /// Provides static column names for columns that get their value fron another table.
                /// </summary>
                public static class Joined
                {
                    /// <summary>
                    /// The name of the publisher column. This column gets its value from the <see cref="PublisherTable"/>.
                    /// </summary>
                    public const string Publisher = "JoinPubName";

                    /// <summary>
                    /// The name of the publisher url column. This column gets its value from the <see cref="PublisherTable"/>.
                    /// </summary>
                    public const string PublisherUrl = "JoinPubUrl";

                    /// <summary>
                    /// The name of the publisher exclusive column. This column gets its value from the <see cref="PublisherTable"/>.
                    /// </summary>
                    public const string PublisherExclusive = "JoinPubExclusive";

                    /// <summary>
                    /// The name of the publisher paying column. This column gets its value from the <see cref="PublisherTable"/>.
                    /// </summary>
                    public const string PublisherPaying = "JoinPubPaying";

                    /// <summary>
                    /// The name of the response type column. This column gets its value from the <see cref="ResponseTable"/>.
                    /// </summary>
                    public const string ResponseTypeName = "JoinRespTypeName";
                }

                ///// <summary>
                ///// Provides static column names for columns that are calculated from other values.
                ///// </summary>
                //public static class Calculated
                //{
                //    /// <summary>
                //    /// Gets the name of the calculated submitted column.
                //    /// This is a special use column that is used for sorting.
                //    /// </summary>
                //    public const string Submitted = "CalcSubmitted";
                //}
            }

            /// <summary>
            /// Provides static relation names.
            /// </summary>
            public static class Relations
            {
                /// <summary>
                /// The name of the relation that relates the <see cref="SubmissionBatchTable"/> to the <see cref="SubmissionTable"/>.
                /// </summary>
                public const string ToSubmission = "SubBatchToSub";

                /// <summary>
                /// The name of the relation that relates the <see cref="SubmissionBatchTable"/> to the <see cref="SubmissionDocumentTable"/>.
                /// </summary>
                public const string ToSubmissionDoc = "SubBatchToSubDoc";

                /// <summary>
                /// The name of the relation that relates the <see cref="SubmissionBatchTable"/> to the <see cref="SubmissionMessageTable"/>.
                /// </summary>
                public const string ToSubmissionMessage = "SubBatchToSubMessage";
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionBatchTable"/> class.
        /// </summary>
        public SubmissionBatchTable() : base(Defs.TableName)
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
            Load(null, $"{Defs.Columns.Submitted} DESC");
        }

        /// <summary>
        /// Provides an enumerable that enumerates all records.
        /// </summary>
        /// <returns>An enumerable</returns>
        public IEnumerable<SubmissionBatchRow> EnumerateAll()
        {
            foreach (DataRow row in EnumerateRows())
            {
                yield return SubmissionBatchRow.Create(row);
            }
        }

        /// <summary>
        /// Gets a <see cref="SubmissionBatchRow"/> object for the specified batch id
        /// </summary>
        /// <param name="id">The batch id</param>
        /// <returns>The batch row or null</returns>
        public SubmissionBatchRow GetSubmissionBatch(long id)
        {
            DataRow[] rows = Select($"{Defs.Columns.Id}={id}");
            return SubmissionBatchRow.Create(GetUniqueRow(rows));
        }

        /// <summary>
        /// Gets the count of submissions to the specified publisher that are open
        /// </summary>
        /// <param name="publisherId">The publisher id</param>
        /// <returns>The number of open submission</returns>
        public int OpenSubmissionCount(long publisherId)
        {
            return Select($"{Defs.Columns.PublisherId}={publisherId} AND {Defs.Columns.Response} IS NULL").Length;
        }

        /// <summary>
        /// Gets the number of times the specified title has been submitted to the specified publisher
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="publisherId">The publisher id</param>
        /// <param name="excludedBatchId">The submission batch id to exclude when calculating the count.</param>
        /// <returns>The count of times the title has been submitted to the publisher, excluding those of the specified submission batch.</returns>
        public int GetTitleToPublisherCount(long titleId, long publisherId, long excludedBatchId)
        {
            int count = 0;
            DataRow[] batchRows = Select($"{Defs.Columns.PublisherId}={publisherId} AND {Defs.Columns.Id} <> {excludedBatchId}");
            foreach (DataRow batchRow in batchRows)
            {
                DataRow[] submissionRows = batchRow.GetChildRows(Defs.Relations.ToSubmission);
                foreach (DataRow submissionRow in submissionRows)
                {
                    if ((long)submissionRow[SubmissionTable.Defs.Columns.TitleId] == titleId)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Gets the number of times that the specified title is currently submitted to a publisher flagged as exclusive (does not accept simultaneous submissions)
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="excludePublisherId">The publisher id to exclude. This is usually the publication the title is potentially being submitted to.</param>
        /// <returns>The count. Normally (unless the user decides to submit a title that is currently out to an exclusive publisher), this is zero or one.</returns>
        public int GetExclusiveCount(long titleId, long excludePublisherId)
        {
            SubmissionTable sub = DatabaseController.Instance.GetTable<SubmissionTable>();
            int count = 0;
            DataRow[] active = Select($"{Defs.Columns.PublisherId}<>{excludePublisherId} AND {Defs.Columns.Response} IS NULL AND {Defs.Columns.Joined.PublisherExclusive}=1");
            foreach(DataRow row in active)
            {
                long batchId = (long)row[Defs.Columns.Id];
                if (sub.SubmissionExists(batchId, titleId))
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Creates a submission
        /// </summary>
        /// <param name="publisherId">The publisher id</param>
        public void CreateSubmission(long publisherId)
        {
            SubmissionBatchRow created = SubmissionBatchRow.Create(NewRow(), publisherId);
            Rows.Add(created.Row);
            DataRow parentRow = created.Row.GetParentRow(PublisherTable.Defs.Relations.ToSubmissionBatch);
            Controller.GetTable<PublisherTable>().UpdateHaveActive(parentRow);
            Save();
        }

        /// <summary>
        /// Deletes the specified submission.
        /// </summary>
        /// <param name="batch">The batch object</param>
        /// <remarks>
        /// This method deletes the specified row and updates the parent publisher table
        /// to reflect the changed status of active submissions. You should call this method
        /// rather than deleting the row directly in order to update the parent.
        /// </remarks>
        public void DeleteSubmission(SubmissionBatchRow batch)
        {
            if (batch != null)
            {
                DataRow parentRow = batch.Row.GetParentRow(PublisherTable.Defs.Relations.ToSubmissionBatch);
                batch.Row.Delete();
                /* Update the parent publisher record */
                Controller.GetTable<PublisherTable>().UpdateHaveActive(parentRow);
                /* Save all. A submission delete affects other tables */
                Controller.Save();
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
                { Defs.Columns.PublisherId, ColumnType.Integer },
                { Defs.Columns.Fee, ColumnType.Numeric },
                { Defs.Columns.Award, ColumnType.Numeric },
                { Defs.Columns.Online, ColumnType.Boolean, false, false, 0 },
                { Defs.Columns.Contest, ColumnType.Boolean, false, false, 0 },
                { Defs.Columns.Locked, ColumnType.Boolean, false, false, 0 },
                { Defs.Columns.Submitted, ColumnType.Timestamp },
                { Defs.Columns.Response, ColumnType.Timestamp, false, true },
                { Defs.Columns.ResponseType, ColumnType.Integer, false, false, 0 },
                { Defs.Columns.Notes, ColumnType.Text, false, true },
                { Defs.Columns.Cover, ColumnType.Text, false, true }
            };
        }

        /// <summary>
        /// Establishes parent / child relationships with other tables.
        /// </summary>
        protected override void SetDataRelations()
        {
            CreateParentChildRelation<SubmissionTable>(Defs.Relations.ToSubmission, Defs.Columns.Id, SubmissionTable.Defs.Columns.BatchId);
            CreateParentChildRelation<SubmissionDocumentTable>(Defs.Relations.ToSubmissionDoc, Defs.Columns.Id, SubmissionDocumentTable.Defs.Columns.BatchId);
            CreateParentChildRelation<SubmissionMessageTable>(Defs.Relations.ToSubmissionMessage, Defs.Columns.Id, SubmissionMessageTable.Defs.Columns.BatchId);
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Joined"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            CreateChildToParentColumn(Defs.Columns.Joined.Publisher, PublisherTable.Defs.Relations.ToSubmissionBatch, PublisherTable.Defs.Columns.Name);
            CreateChildToParentColumn(Defs.Columns.Joined.PublisherUrl, PublisherTable.Defs.Relations.ToSubmissionBatch, PublisherTable.Defs.Columns.Url);
            CreateChildToParentColumn<bool>(Defs.Columns.Joined.PublisherExclusive, PublisherTable.Defs.Relations.ToSubmissionBatch, PublisherTable.Defs.Columns.Exclusive);
            CreateChildToParentColumn<bool>(Defs.Columns.Joined.PublisherPaying, PublisherTable.Defs.Relations.ToSubmissionBatch, PublisherTable.Defs.Columns.Paying);
            CreateChildToParentColumn(Defs.Columns.Joined.ResponseTypeName, ResponseTable.Defs.Relations.ToSubmissionBatch, ResponseTable.Defs.Columns.Name);
        }

        /// <summary>
        /// Called when a data column is changing its value.
        /// </summary>
        /// <param name="e">The event args.</param>
        /// <remarks>
        /// This method checks for changes to the <see cref="Defs.Columns.Response"/> column
        /// in order to auto manage the corresponding <see cref="Defs.Columns.ResponseType"/> column.
        /// </remarks>
        protected override void OnColumnChanging(DataColumnChangeEventArgs e)
        {
            base.OnColumnChanging(e);
            if (e.Column.ColumnName == Defs.Columns.Response)
            {
                if (e.ProposedValue is DateTime)
                {
                    // If current response date is null, set response type to not specified
                    if (e.Row[e.Column] == DBNull.Value)
                    {
                        e.Row[Defs.Columns.ResponseType] = ResponseTable.Defs.Values.ResponseNotSpecified;
                    }
                }
                else
                {
                    e.Row[Defs.Columns.ResponseType] = ResponseTable.Defs.Values.NoResponse;
                }
            }
        }

        /// <summary>
        /// Called when a data column has changed its value.
        /// </summary>
        /// <param name="e"></param>
        /// <remarks>
        /// This method checks for changes to the <see cref="Defs.Columns.Response"/> column
        /// in order to sync the <see cref="PublisherTable.Defs.Columns.Calculated.HaveActiveSubmission"/>
        /// column of the associated publisher.
        /// </remarks>
        protected override void OnColumnChanged(DataColumnChangeEventArgs e)
        {
            base.OnColumnChanged(e);
            if (e.Column.ColumnName == Defs.Columns.Response)
            {
                /* Get the publisher parent row and update it */
                DataRow parentRow = e.Row.GetParentRow(PublisherTable.Defs.Relations.ToSubmissionBatch);
                Controller.GetTable<PublisherTable>().UpdateHaveActive(parentRow);
            }
        }
        #endregion

        /************************************************************************/

        #region Update (Internal)
        internal override void PerformDataUpdate()
        {
            PerformUpdateFor(2, UpdateDates, "Set dates to zeroed time");
            PerformUpdateFor(3, AddCoverLetterColumn, "Add column for cover letter");
        }

        private void UpdateDates()
        {
            foreach (SubmissionBatchRow item in EnumerateAll())
            {
                item.Submitted = item.Submitted.ToZero();
                if (item.HasResponseDate)
                {
                    item.Response = item.Response.Value.ToZero();
                }
            }
        }

        private void AddCoverLetterColumn()
        {
            AddColumnIf(Defs.Columns.Cover, "text", typeof(string));
        }

        #endregion
    }
}