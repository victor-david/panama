using Restless.Tools.Database.SQLite;
using System;
using System.Data;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains submission period information for publishers.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class SubmissionPeriodTable : TableBase
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
            public const string TableName = "submissionperiod";

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
                /// The name of the start column. Holds the date that a submission period starts.
                /// </summary>
                public const string Start = "start";

                /// <summary>
                /// The name of the end column. Holds the date that a submission period ends.
                /// </summary>
                public const string End = "end";

                /// <summary>
                /// The name of the notes column.
                /// </summary>
                public const string Notes = "notes";

                /// <summary>
                /// Provides static column names for columns that get their value fron another table.
                /// </summary>
                public static class Joined
                {
                    /// <summary>
                    /// The name of the publisre name column. This column gets its value from the <see cref="PublisherTable"/>.
                    /// </summary>
                    public const string Publisher = "JoinPubName";
                }
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
        public SubmissionPeriodTable() : base(DatabaseController.Instance, Defs.TableName)
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
            Load(null, Defs.Columns.Id);
        }

        /// <summary>
        /// Adds a record to the submission period table
        /// </summary>
        /// <param name="publisherId">The publisher id</param>
        /// <param name="start">The start date of the submission period</param>
        /// <param name="end">The end date of the submission period</param>
        public void AddSubmissionPeriod(long publisherId, DateTime start, DateTime end)
        {
            DataRow row = NewRow();
            row[Defs.Columns.PublisherId] = publisherId;
            row[Defs.Columns.Start] = start;
            row[Defs.Columns.End] = end;
            Rows.Add(row);
            Save();
            /* Get the publisher parent row and update it */
            DataRow parentRow = row.GetParentRow(PublisherTable.Defs.Relations.ToSubmissionPeriod);
            Controller.GetTable<PublisherTable>().UpdateInPeriod(parentRow);
        }

        /// <summary>
        /// Removes the submission period specified by its data row and updates the parent publisher record
        /// </summary>
        /// <param name="row">The data row</param>
        /// <remarks>
        /// This method deletes the specified row and updates the parent publisher table
        /// to reflect the changed set of submission periods. You should call this method
        /// rather than deleting the row directly in order to update the parent.
        /// </remarks>
        public void RemoveSubmissionPeriod(DataRow row)
        {
            if (row != null && row.Table.TableName == TableName)
            {
                DataRow parentRow = row.GetParentRow(PublisherTable.Defs.Relations.ToSubmissionPeriod);
                row.Delete();
                /* Update the parent publisher record */
                //Controller.GetTable<PublisherTable>().UpdateInPeriod(parentRow);
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods

        
        //protected override void OnRowDeleting(DataRowChangeEventArgs e)
        //{
        //    base.OnRowDeleting(e);
        //    DataRow parentRow = e.Row.GetParentRow(PublisherTable.Defs.Relations.ToSubmissionPeriod);
        //    if (parentRow != null)
        //    {
        //    }

        //}

        //protected override void OnRowDeleted(DataRowChangeEventArgs e)
        //{
        //    base.OnRowDeleted(e);
        //    DataRow parentRow = e.Row.GetParentRow(PublisherTable.Defs.Relations.ToSubmissionPeriod);
        //    if (parentRow != null)
        //    {
        //    }
        //}

        /// <summary>
        /// Gets the DDL needed to create this table.
        /// </summary>
        /// <returns>A SQL string that describes how to create this table.</returns>
        protected override string GetDdl()
        {
            return Resources.Create.SubmissionPeriod;
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
            CreateChildToParentColumn(Defs.Columns.Joined.Publisher, PublisherTable.Defs.Relations.ToSubmissionPeriod, PublisherTable.Defs.Columns.Name);
        }
        #endregion

        /************************************************************************/
    }
}
