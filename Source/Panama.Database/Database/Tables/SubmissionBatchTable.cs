using Restless.Tools.Database.SQLite;
using System;
using System.Data;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains information about a submission batch. A batch is associated 
    /// with a single publisher and may reference various titles, documents, and messages.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class SubmissionBatchTable : TableBase
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
                public const string Id = "id";

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
                    /// The name of the response type column. This column gets its value from the <see cref="ResponseTable"/>.
                    /// </summary>
                    public const string ResponseTypeName = "JoinRespTypeName";
                }

                /// <summary>
                /// Provides static column names for columns that are calculated from other values.
                /// </summary>
                public static class Calculated
                {
                    /// <summary>
                    /// Gets the name of the calculated submitted column.
                    /// This is a special use column that is used for sorting.
                    /// </summary>
                    public const string Submitted = "CalcSubmitted";
                }
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
        #pragma warning disable 1591
        public SubmissionBatchTable() : base(DatabaseController.Instance, Defs.TableName)
        {
        }
        #pragma warning restore 1591
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
                if (sub.TitleExistsInBatch(batchId, titleId))
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
            DataRow row = NewRow();
            row[Defs.Columns.PublisherId] = publisherId;
            row[Defs.Columns.Online] = false;
            row[Defs.Columns.Contest] = false;
            row[Defs.Columns.Locked] = false;
            row[Defs.Columns.Submitted] = DateTime.UtcNow;
            row[Defs.Columns.ResponseType] = ResponseTable.Defs.Values.NoResponse;
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
            return Resources.Create.SubmissionBatch;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate, DataColumnPropertyKey.ReceiveInsertedId);
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
            CreateChildToParentColumn(Defs.Columns.Joined.ResponseTypeName, ResponseTable.Defs.Relations.ToSubmissionBatch, ResponseTable.Defs.Columns.Name);
            CreateActionExpressionColumn<DateTime>
                (
                    Defs.Columns.Calculated.Submitted,
                    this,
                    UpdateCalculatedSubmitted,
                    Defs.Columns.Submitted,
                    Defs.Columns.Response
                );
        }

        /// <summary>
        /// Called when database initialization is complete to populate the <see cref="Defs.Columns.Calculated.Submitted"/> column.
        /// </summary>
        protected override void OnInitializationComplete()
        {
            foreach (DataRow row in Rows)
            {
                UpdateCalculatedSubmitted(row);
            }
            AcceptChanges();

            //foreach (DataRow row in Rows)
            //{
            //    DataRow[] subRows = row.GetChildRows(Defs.Relations.ToSubmission);
            //    Int64 ordering = 1;
            //    foreach (DataRow subRow in subRows)
            //    {
            //        subRow[SubmissionTable.Defs.Columns.Ordering] = ordering;
            //        ordering++;
            //    }
            //}


        }
        #endregion

        /************************************************************************/
        
        #region Private methods
        private void UpdateCalculatedSubmitted(ActionDataColumn col, DataRowChangeEventArgs e)
        {
            
            UpdateCalculatedSubmitted(e.Row);
        }

        private void UpdateCalculatedSubmitted(DataRow row)
        {
            DateTime baseDate = new DateTime(DateTime.UtcNow.Year + 10, 1, 1);
            if (row[Defs.Columns.Response] == DBNull.Value)
            {
                DateTime submitted = (DateTime)row[Defs.Columns.Submitted];
                row[Defs.Columns.Calculated.Submitted] = baseDate.AddTicks(submitted.Ticks);
            }
            else
            {
                row[Defs.Columns.Calculated.Submitted] = row[Defs.Columns.Submitted];
            }
        }
        #endregion

        /************************************************************************/

        #region ITableImport and IColumnRowImporter implementation (commented out)
        //public bool PerformImport()
        //{
        //    return DatabaseImporter.Instance.ImportTable(this, this, "submission_batch");
        //}

        //public string GetColumnName(string origColName)
        //{
        //    switch (origColName)
        //    {
        //        case "publicationid": return Defs.Columns.PublisherId;
        //        case "date_submitted": return Defs.Columns.Submitted;
        //        case "date_response": return Defs.Columns.Response;
        //        case "response_type": return Defs.Columns.ResponseType;
        //        default: return origColName;
        //    }
        //}

        //public bool IncludeColumn(string origColName)
        //{
        //    return (origColName != "idate_response");
        //}

        //public bool GetRowConfirmation(System.Data.DataRow row)
        //{
        //    return true;
        //}
        #endregion
    }
}
