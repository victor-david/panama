using Restless.Tools.Database.SQLite;
using System;
using System.Data;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that holds the basic information about each title.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class TitleTable : TableBase
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
            public const string TableName = "title";

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
                /// The name of the title column.
                /// </summary>
                public const string Title = "title";

                /// <summary>
                /// The name of the written column. Holds the date that the title was written.
                /// </summary>
                public const string Written = "written";

                /// <summary>
                /// The name of the author id column.
                /// </summary>
                public const string AuthorId = "authorid";

                /// <summary>
                /// The name of the ready column. 
                /// </summary>
                public const string Ready = "ready";

                /// <summary>
                /// Provides static column names for columns that are calculated from other values.
                /// </summary>
                public static class Calculated
                {
                    /// <summary>
                    /// The name of the version count column. This calculated column
                    /// holds the number of related records from the <see cref="TitleVersionTable"/>.
                    /// </summary>
                    public const string VersionCount = "CalcVerCount";

                    /// <summary>
                    /// The name of the latest version date column. This calculated column
                    /// holds the modified date of the latest title version per the <see cref="TitleVersionTable"/>.
                    /// </summary>
                    public const string LastestVersionDate = "CalcVerDate";

                    /// <summary>
                    /// The name of the latest version word count column. This calculated column
                    /// holds the word count of the latest title version per the <see cref="TitleVersionTable"/>.
                    /// </summary>
                    public const string LastestVersionWordCount = "CalcVerWordCount";

                    /// <summary>
                    /// The name of the latest version path column. This calculated column
                    /// holds the path of the latest title version per the <see cref="TitleVersionTable"/>.
                    /// </summary>
                    public const string LastestVersionPath = "CalcVerPath";

                    /// <summary>
                    /// The name of the tag count column. This calculated column
                    /// holds the number of related records from the <see cref="TitleTagTable"/>.
                    /// </summary>
                    public const string TagCount = "CalcTagCount";

                    /// <summary>
                    /// The name of the submission count column. This calculated column
                    /// holds the number of related records from the <see cref="SubmissionTable"/>.
                    /// </summary>
                    public const string SubCount = "CalcSubCount";

                    /// <summary>
                    /// The name of the current submission count column. This calculated column
                    /// holds the number of submission that are currently active.
                    /// </summary>
                    public const string CurrentSubCount = "CalcCurrSubCount";

                    /// <summary>
                    /// The name of the published count column. This calculated column
                    /// holds the number of times a title has been published, as per the <see cref="PublishedTable"/>.
                    /// </summary>
                    public const string PublishedCount = "CalcPubCount";

                    /// <summary>
                    /// The name of the is-published column. This calculated column
                    /// holds a boolean value that indicates if a title has aby related records in the <see cref="PublishedTable"/>.
                    /// </summary>
                    public const string IsPublished = "CalcIsPublished";
                }
            }

            /// <summary>
            /// Provides static relation names.
            /// </summary>
            public static class Relations
            {
                /// <summary>
                /// The name of the relation that relates the <see cref="TitleTable"/> to the <see cref="SubmissionTable"/>.
                /// </summary>
                public const string ToSubmission = "TitleToSubmission";

                /// <summary>
                /// The name of the relation that relates the <see cref="TitleTable"/> to the <see cref="TitleVersionTable"/>.
                /// </summary>
                public const string ToVersion = "TitleToVersion";

                /// <summary>
                /// The name of the relation that relates the <see cref="TitleTable"/> to the <see cref="TitleTagTable"/>.
                /// </summary>
                public const string ToTitleTag = "TitleToTitleTag";

                /// <summary>
                /// The name of the relation that relates the <see cref="TitleTable"/> to the <see cref="PublishedTable"/>.
                /// </summary>
                public const string ToPublished = "TitleToPublished";
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
        public TitleTable() : base(DatabaseController.Instance, Defs.TableName)
        {
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
            Load(null, String.Format("{0} DESC",Defs. Columns.Written));
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
            return Resources.Create.Title;
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
            CreateParentChildRelation<SubmissionTable>(Defs.Relations.ToSubmission, Defs.Columns.Id, SubmissionTable.Defs.Columns.TitleId);
            CreateParentChildRelation<TitleVersionTable>(Defs.Relations.ToVersion, Defs.Columns.Id, TitleVersionTable.Defs.Columns.TitleId);
            CreateParentChildRelation<TitleTagTable>(Defs.Relations.ToTitleTag, Defs.Columns.Id, TitleTagTable.Defs.Columns.TitleId);
            CreateParentChildRelation<PublishedTable>(Defs.Relations.ToPublished, Defs.Columns.Id, PublishedTable.Defs.Columns.TitleId);
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Calculated"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            string expr = String.Format("Count(Child({0}).{1})", Defs.Relations.ToVersion, TitleVersionTable.Defs.Columns.Id);
            CreateExpressionColumn<Int64>(Defs.Columns.Calculated.VersionCount, expr);

            expr = String.Format("Count(Child({0}).{1})", Defs.Relations.ToTitleTag, TitleTagTable.Defs.Columns.TagId);
            CreateExpressionColumn<Int64>(Defs.Columns.Calculated.TagCount, expr);

            expr = String.Format("Count(Child({0}).{1})", Defs.Relations.ToSubmission, SubmissionTable.Defs.Columns.Id);
            CreateExpressionColumn<Int64>(Defs.Columns.Calculated.SubCount, expr);

            expr = String.Format("Count(Child({0}).{1})", Defs.Relations.ToPublished, PublishedTable.Defs.Columns.Id);
            CreateExpressionColumn<Int64>(Defs.Columns.Calculated.PublishedCount, expr);

            expr = String.Format("Count(Child({0}).{1}) > 0", Defs.Relations.ToPublished, PublishedTable.Defs.Columns.Id);
            CreateExpressionColumn<bool>(Defs.Columns.Calculated.IsPublished, expr);

            expr = String.Format("Sum(Child({0}).{1})", Defs.Relations.ToSubmission, SubmissionTable.Defs.Columns.Calculated.CurrentSubCount);
            CreateExpressionColumn<Int64>(Defs.Columns.Calculated.CurrentSubCount, expr);

            CreateActionExpressionColumn<DateTime>
                (
                    Defs.Columns.Calculated.LastestVersionDate,
                    Controller.GetTable<TitleVersionTable>(),
                    UpdateLatestVersionDate,
                    TitleVersionTable.Defs.Columns.Updated,
                    TitleVersionTable.Defs.Columns.Version
                );

            CreateActionExpressionColumn<Int64>
                (
                    Defs.Columns.Calculated.LastestVersionWordCount,
                    Controller.GetTable<TitleVersionTable>(),
                    UpdateLatestVersionWordCount,
                    TitleVersionTable.Defs.Columns.WordCount,
                    TitleVersionTable.Defs.Columns.Version
                );

            CreateActionExpressionColumn<string>
                (
                    Defs.Columns.Calculated.LastestVersionPath,
                    Controller.GetTable<TitleVersionTable>(),
                    UpdateLatestVersionPath,
                    TitleVersionTable.Defs.Columns.FileName
                );
        }

        /// <summary>
        /// Called when database initialization is complete to populate the <see cref="Defs.Columns.Calculated.LastestVersionWordCount"/> column.
        /// </summary>
        protected override void OnInitializationComplete()
        {
            var versions = Controller.GetTable<TitleVersionTable>();
            foreach (DataRow row in Rows)
            {
                Int64 titleId = (Int64)row[Defs.Columns.Id];
                DataRow versionRow = versions.GetLastVersion(titleId);
                if (versionRow != null)
                {
                    row[Defs.Columns.Calculated.LastestVersionWordCount] = versionRow[TitleVersionTable.Defs.Columns.WordCount];
                    row[Defs.Columns.Calculated.LastestVersionDate] = versionRow[TitleVersionTable.Defs.Columns.Updated];
                    row[Defs.Columns.Calculated.LastestVersionPath] = versionRow[TitleVersionTable.Defs.Columns.FileName];

                }
            }
            AcceptChanges();
        }


        /// <summary>
        /// Populates a new row with default (starter) values
        /// </summary>
        /// <param name="row">The freshly created DataRow to poulate</param>
        protected override void PopulateDefaultRow(System.Data.DataRow row)
        {
            row[Defs.Columns.Title] = "(new title)";
            row[Defs.Columns.Written] = DateTime.UtcNow;
            row[Defs.Columns.AuthorId] = 1;
            row[Defs.Columns.Ready] = 0;
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void UpdateLatestVersionDate(ActionDataColumn col, DataRowChangeEventArgs e)
        {
            UpdateFromLatestVersionCalculated(e, Defs.Columns.Calculated.LastestVersionDate, TitleVersionTable.Defs.Columns.Updated);
        }

        private void UpdateLatestVersionWordCount(ActionDataColumn col, DataRowChangeEventArgs e)
        {
            UpdateFromLatestVersionCalculated(e, Defs.Columns.Calculated.LastestVersionWordCount, TitleVersionTable.Defs.Columns.WordCount);
        }
        
        private void UpdateLatestVersionPath(ActionDataColumn col, DataRowChangeEventArgs e)
        {
            UpdateFromLatestVersionCalculated(e, Defs.Columns.Calculated.LastestVersionPath, TitleVersionTable.Defs.Columns.FileName);
        }

        private void UpdateFromLatestVersionCalculated(DataRowChangeEventArgs e, string titleColumn, string titleVersionColumn)
        {
            Int64 titleId = (Int64)e.Row[TitleVersionTable.Defs.Columns.TitleId];
            DataRow[] titleRows = Select(String.Format("{0}={1}", Defs.Columns.Id, titleId));
            if (titleRows.Length == 1)
            {
                DataRow versionRow = Controller.GetTable<TitleVersionTable>().GetLastVersion(titleId);
                if (versionRow != null)
                {
                    titleRows[0][titleColumn] = versionRow[titleVersionColumn];
                }
            }
        }
        #endregion

        /************************************************************************/

        #region ITableImport and IColumnRowImporter implementation (commented out)
        //public bool PerformImport()
        //{
        //    //Controller.
        //    return DatabaseImporter.Instance.ImportTable(this, this);
        //}

        //public string GetColumnName(string origColName)
        //{
        //    switch (origColName)
        //    {
        //        case "titleid": return Defs.Columns.Id;
        //        case "date_write": return Defs.Columns.Written;
        //        default: return origColName;
        //    }
        //}

        //public bool IncludeColumn(string origColName)
        //{
        //    return (origColName != "classid" && origColName != "submission_count");
        //}

        //public bool GetRowConfirmation(System.Data.DataRow row)
        //{
        //    return true;
        //}
        #endregion

        /************************************************************************/

        #region Row Object
        /// <summary>
        /// Encapsulates a single row from the <see cref="TitleTable"/>.
        /// </summary>
        public class RowObject : RowObjectBase<TitleTable>
        {
            #region Public properties
            /// <summary>
            /// Gets the id for this row object.
            /// </summary>
            public Int64 Id
            {
                get { return GetInt64(Defs.Columns.Id); }
            }

            /// <summary>
            /// Gets or sets the title for this row object.
            /// </summary>
            public string Title
            {
                get { return GetString(Defs.Columns.Title); }
                set { SetValue(Defs.Columns.Title, value); }
            }
            /// <summary>
            /// Gets or sets the written date/time value for this row object.
            /// The return value is expressed as UTC, but since this comes from
            /// the database, it's not certain to be so. Earlier data was stored with local value.
            /// When setting this field, use Utc.
            /// </summary>
            public DateTime Written
            {
                get { return GetDateTime(Defs.Columns.Written); }
                set { SetValue(Defs.Columns.Written, value); }
            }

            /// <summary>
            /// Gets or sets the author id for this row object.
            /// </summary>
            public Int64 AuthorId
            {
                get { return GetInt64(Defs.Columns.AuthorId); }
                set { SetValue(Defs.Columns.AuthorId, value); }
            }
            #endregion

            /************************************************************************/

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="RowObject"/> class.
            /// </summary>
            /// <param name="row">The data row</param>
            public RowObject(DataRow row)
                : base(row)
            {
            }
            #endregion
        }
        #endregion

    }
}
