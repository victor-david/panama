using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using Restless.App.Panama.Database;
using Restless.Tools.Database.Generic;
using Restless.Tools.Database.SQLite;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains publisher information.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class PublisherTable : TableBase
    {
        #region Private
        private bool saveBeforeUpdateInPeriod;
        #endregion
        
        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Provides static definitions for table properties such as column names and relation names.
        /// </summary>
        public static class Defs
        {
            /// <summary>
            /// Specifies the name of this table.
            /// </summary>
            public const string TableName = "publisher";

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
                /// The name of the name column. This column holds the name of the publisher.
                /// </summary>
                public const string Name = "name";

                /// <summary>
                /// The name of the city column.
                /// </summary>
                public const string City = "city";

                /// <summary>
                /// The name of the state column.
                /// </summary>
                public const string State = "state";

                /// <summary>
                /// The name of the zip column.
                /// </summary>
                public const string Zip = "zip";

                /// <summary>
                /// The name of the url column. This column holds the url to the publisher's web site.
                /// </summary>
                public const string Url = "url";

                /// <summary>
                /// The name of the email column.
                /// </summary>
                public const string Email = "email";

                /// <summary>
                /// The name of the options column. This column is currently not used.
                /// </summary>
                public const string Options = "options";

                /// <summary>
                /// The name of the simultaneous column, whether the publisher accepts simultaneous submissions.
                /// </summary>
                public const string Simultaneous = "simultaneous";

                /// <summary>
                /// The name of the paying column, whether the publisher pays its contributors.
                /// </summary>
                public const string Paying = "paying";

                /// <summary>
                /// The name of the followup column. This column is used to flag a publisher for later followup.
                /// </summary>
                public const string Followup = "followup";

                /// <summary>
                /// The name of the goner column. This column is used to flag a publisher that has gone out of business, or is otherwise not of interest to the user.
                /// </summary>
                public const string Goner = "goner";

                /// <summary>
                /// The name of the credential id column.
                /// </summary>
                public const string CredentialId = "credentialid";

                /// <summary>
                /// The name of the notes column. This column holds any notes about the publisher.
                /// </summary>
                public const string Notes = "notes";

                /// <summary>
                /// The name of the added column. This column holds the date that the publisher was added.
                /// </summary>
                public const string Added = "added";

                /// <summary>
                /// Provides static column names for columns that are calculated from other values.
                /// </summary>
                public static class Calculated
                {
                    /// <summary>
                    /// The name of the submission count column. This calculated column
                    /// holds the total number of submissions made to the publisher.
                    /// </summary>
                    public const string SubCount = "CalcSubCount";

                    /// <summary>
                    /// The name of the last submission column. This calculated column
                    /// holds the date of the last submission to the publisher, or null
                    /// if there's never been a submission.
                    /// </summary>
                    public const string LastSub = "CalcLastSub";

                    /// <summary>
                    /// The name of the submission period count column. This calculated column
                    /// holds the number of records from the <see cref="SubmissionPeriodTable"/>
                    /// that belong to the publisher.
                    /// </summary>
                    public const string SubPeriodCount = "CalcSupPerCount";

                    /// <summary>
                    /// The name of the in-submission-period column. This calculated column
                    /// holds a boolean value that indicates if the current date falls within
                    /// any one of the defined submssion periods for the publisher.
                    /// </summary>
                    public const string InSubmissionPeriod = "CalcInPeriod";
                }
            }

            /// <summary>
            /// Provides static relation names.
            /// </summary>
            public static class Relations
            {
                /// <summary>
                /// The name of the relation that relates the <see cref="PublisherTable"/> to the <see cref="SubmissionBatchTable"/>.
                /// </summary>
                public const string ToSubmissionBatch = "PubToSubmissionBatch";

                /// <summary>
                /// The name of the relation that relates the <see cref="PublisherTable"/> to the <see cref="SubmissionPeriodTable"/>.
                /// </summary>
                public const string ToSubmissionPeriod = "PubToSubmissionPeriod";

                /// <summary>
                /// The name of the relation that relates the <see cref="PublisherTable"/> to the <see cref="PublishedTable"/>.
                /// </summary>
                public const string ToPublished = "PubToPublished";
            }
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get { return Defs.Columns.Id; }
        }
        #endregion

        /************************************************************************/
        
        #region Constructor
        #pragma warning disable 1591
        public PublisherTable() : base(DatabaseController.Instance, Defs.TableName)
        {
            saveBeforeUpdateInPeriod = true;
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Rows collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, String.Format("{0} DESC", Defs.Columns.Added));
        }

        /// <summary>
        /// Clears the credential id of all publishers with the specified credential id.
        /// </summary>
        /// <param name="id">The credential id.</param>
        public void ClearCredential(Int64 id)
        {
            DataRow[] rows = Select(String.Format("{0}={1}", Defs.Columns.CredentialId, id));
            foreach (DataRow row in rows)
            {
                row[Defs.Columns.CredentialId] = 0;
            }
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
            return Resources.Create.Publisher;
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
            CreateParentChildRelation<SubmissionBatchTable>(Defs.Relations.ToSubmissionBatch, Defs.Columns.Id, SubmissionBatchTable.Defs.Columns.PublisherId);
            CreateParentChildRelation<SubmissionPeriodTable>(Defs.Relations.ToSubmissionPeriod, Defs.Columns.Id, SubmissionPeriodTable.Defs.Columns.PublisherId);
            CreateParentChildRelation<PublishedTable>(Defs.Relations.ToPublished, Defs.Columns.Id, PublishedTable.Defs.Columns.PublisherId);
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Calculated"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            string expr = String.Format("Count(Child({0}).{1})", Defs.Relations.ToSubmissionBatch, SubmissionBatchTable.Defs.Columns.Id);
            CreateExpressionColumn<Int64>(Defs.Columns.Calculated.SubCount, expr);

            expr = String.Format("Max(Child({0}).{1})", Defs.Relations.ToSubmissionBatch, SubmissionBatchTable.Defs.Columns.Submitted);
            CreateExpressionColumn<DateTime>(Defs.Columns.Calculated.LastSub, expr);

            expr = String.Format("Count(Child({0}).{1})", Defs.Relations.ToSubmissionPeriod, SubmissionPeriodTable.Defs.Columns.Id);
            CreateExpressionColumn<Int64>(Defs.Columns.Calculated.SubPeriodCount, expr);
        }

        /// <summary>
        /// Called when database initialization is complete. Calculates the publisher in-period values.
        /// </summary>
        protected override void OnInitializationComplete()
        {
            DataColumn col = new DataColumn(Defs.Columns.Calculated.InSubmissionPeriod, typeof(bool));
            Columns.Add(col);
            SetColumnProperty(col, DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate);
            UpdateInPeriod();
        }

        /// <summary>
        /// Populates a new row with default (starter) values
        /// </summary>
        /// <param name="row">The freshly created DataRow to poulate</param>
        protected override void PopulateDefaultRow(System.Data.DataRow row)
        {
            row[Defs.Columns.Added] = DateTime.UtcNow;
            row[Defs.Columns.CredentialId] = 0;
            row[Defs.Columns.Followup] = false;
            row[Defs.Columns.Goner] = false;
            row[Defs.Columns.Name] = "(new publisher)";
            row[Defs.Columns.Options] = 0;
            row[Defs.Columns.Paying] = false;
            row[Defs.Columns.Simultaneous] = false;
        }
        #endregion

        /************************************************************************/
        
        #region Internal methods
        /// <summary>
        /// Updates the specified publisher data row, setting tis calculated InSubmissionPeriod column.
        /// </summary>
        /// <param name="row">The publisher data row</param>
        /// <remarks>
        /// This method is used by the SubmissionPeriodTable to sync the publisher table
        /// when a submission period record is added or deleted. It's also used by this
        /// class when performing the startup initialization.
        /// </remarks>
        internal void UpdateInPeriod(DataRow row)
        {
            if (row != null && row.Table.TableName == TableName)
            {
                // When initializing, this method is called from private method UpdateInPeriod() 
                // for every row in the table. At this time, saveBeforeUpdateInPeriod is false.
                // Later, this method is called from SubmissionPeriodTable to update when
                // a period is added or deleted. At that point, saveBeforeUpdateInPeriod is true
                // because we don't want any other pending changes on the row to be lost
                // with row.AcceptChanges()
                 
                if (saveBeforeUpdateInPeriod) Save();
                row[Defs.Columns.Calculated.InSubmissionPeriod] = false;
                DataRow[] childRows = row.GetChildRows(Defs.Relations.ToSubmissionPeriod);
                bool inPeriod = false;
                foreach (DataRow childRow in childRows)
                {
                    DateTime start = (DateTime)childRow[SubmissionPeriodTable.Defs.Columns.Start];
                    DateTime end = (DateTime)childRow[SubmissionPeriodTable.Defs.Columns.End];
                    inPeriod = inPeriod || IsInPeriod(start, end);
                }
                row[Defs.Columns.Calculated.InSubmissionPeriod] = inPeriod;
                /* This is a virtual calculated field. Don't want this change being seen as dirty. */
                row.AcceptChanges();
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Updates all rows
        /// </summary>
        private void UpdateInPeriod()
        {
            saveBeforeUpdateInPeriod = false;
            foreach (DataRow row in Rows)
            {
                UpdateInPeriod(row);
            }
            saveBeforeUpdateInPeriod = true;
        }

        private bool IsInPeriod(DateTime start, DateTime end)
        {
            DateTime now = DateTime.UtcNow;
            // Normalize both dates to the same year
            start = new DateTime(now.Year - 1, start.Month, start.Day);
            end = new DateTime(now.Year - 1, end.Month, end.Day);

            // If start date (day of year) comes later than end date (day of year), bump end date up one year.
            if (start.DayOfYear > end.DayOfYear)
            {
                end = end.AddYears(1);
            }

            // If both are behind today, bump both up one year.
            if (DateTime.Compare(start, now) < 0 && DateTime.Compare(end, now) < 0)
            {
                start = start.AddYears(1);
                end = end.AddYears(1);
            }
            return (DateTime.Compare(now, start) >= 0 && DateTime.Compare(now, end) <= 0);
        }
        #endregion

        /************************************************************************/

        #region ITableImport and IColumnRowImporter implementation (commented out)
        //public bool PerformImport()
        //{
        //    return DatabaseImporter.Instance.ImportTable(this, this, "publication");
        //}

        //public string GetColumnName(string origColName)
        //{
        //    switch (origColName)
        //    {
        //        case "publicationid": return Defs.Columns.Id;
        //        case "pubname": return Defs.Columns.Name;
        //        case "website": return Defs.Columns.Url;
        //        case "submission_count": return Defs.Columns.SubCount;
        //        case "last_submission_date": return Defs.Columns.LastSub;
        //        case "credential_id": return Defs.Columns.CredentialId;
        //        case "date_added": return Defs.Columns.Added;
        //        default: return origColName;
        //    }
        //}

        //public bool IncludeColumn(string origColName)
        //{
        //    return true;
        //}

        //public bool GetRowConfirmation(DataRow row)
        //{
        //    string noteStr = row[Defs.Columns.Notes].ToString();
        //    if (String.IsNullOrEmpty(noteStr) || noteStr.StartsWith(@"{\rtf1\"))
        //    {
        //        row[Defs.Columns.Notes] = DBNull.Value;
        //    }
        //    return true;
        //}
        #endregion
    }
}