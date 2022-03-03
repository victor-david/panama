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
                /// The name of the created column. Holds the date that the title record was created.
                /// </summary>
                public const string Created = "created";

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
                /// The name of the quick flag column. 
                /// </summary>
                public const string QuickFlag = "qflag";

                /// <summary>
                /// The name of the notes column. 
                /// </summary>
                public const string Notes = "notes";

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
                    /// The name of the is-submitted. This calculated column
                    /// holds a boolean value that indicates if the title is currently submitted.
                    /// </summary>
                    public const string IsSubmitted = "CalcIsSubmitted";

                    /// <summary>
                    /// The name of the published count column. This calculated column
                    /// holds the number of times a title has been published, as per the <see cref="PublishedTable"/>.
                    /// </summary>
                    public const string PublishedCount = "CalcPubCount";

                    /// <summary>
                    /// The name of the self published count column. This calculated column
                    /// holds the number of times a title has been published, as per the <see cref="SelfPublishedTable"/>.
                    /// </summary>
                    public const string SelfPublishedCount = "CalcSelfPubCount";

                    /// <summary>
                    /// The name of the is-published column. This calculated column
                    /// holds a boolean value that indicates if a title has any related records in the <see cref="PublishedTable"/>.
                    /// </summary>
                    public const string IsPublished = "CalcIsPublished";

                    /// <summary>
                    /// The name of the is-self-published column. This calculated column
                    /// holds a boolean value that indicates if a title has any related records in the <see cref="SelfPublishedTable"/>.
                    /// </summary>
                    public const string IsSelfPublished = "CalcIsSelfPublished";
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

                /// <summary>
                /// The name of the relation that relates the <see cref="TitleTable"/> to the <see cref="SelfPublishedTable"/>.
                /// </summary>
                public const string ToSelfPublished = "TitleToSelfPublished";

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
        /// Initializes a new instance of the <see cref="TitleTable"/> class.
        /// </summary>
        public TitleTable() : base(DatabaseController.Instance, Defs.TableName)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Rows collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, string.Format("{0} DESC",Defs. Columns.Written));
        }

        /// <summary>
        /// Provides an enumerable that gets all titles in order of written DESC.
        /// </summary>
        /// <returns>A <see cref="RowObject"/></returns>
        public IEnumerable<RowObject> EnumerateTitles()
        {
            DataRow[] rows = Select(null, $"{Defs.Columns.Written} DESC");
            foreach (DataRow row in rows)
            {
                yield return new RowObject(row);
            }
            yield break;
        }

        /// <summary>
        /// Gets a single <see cref="RowObject"/> as specified by its id.
        /// </summary>
        /// <param name="id">The id of the record to get.</param>
        /// <returns>A <see cref="RowObject"/> for <paramref name="id"/>, or null if not found</returns>
        public RowObject GetSingleRecord(long id)
        {
            DataRow[] rows = Select($"{Defs.Columns.Id}={id}");
            if (rows.Length==1)
            {
                return new RowObject(rows[0]);
            }
            return null;
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
            CreateParentChildRelation<SelfPublishedTable>(Defs.Relations.ToSelfPublished, Defs.Columns.Id, SelfPublishedTable.Defs.Columns.TitleId);
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Calculated"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            string expr = string.Format("Count(Child({0}).{1})", Defs.Relations.ToVersion, TitleVersionTable.Defs.Columns.Id);
            CreateExpressionColumn<long>(Defs.Columns.Calculated.VersionCount, expr);

            expr = string.Format("Count(Child({0}).{1})", Defs.Relations.ToTitleTag, TitleTagTable.Defs.Columns.TagId);
            CreateExpressionColumn<long>(Defs.Columns.Calculated.TagCount, expr);

            expr = string.Format("Count(Child({0}).{1})", Defs.Relations.ToSubmission, SubmissionTable.Defs.Columns.Id);
            CreateExpressionColumn<long>(Defs.Columns.Calculated.SubCount, expr);

            expr = string.Format("Count(Child({0}).{1})", Defs.Relations.ToPublished, PublishedTable.Defs.Columns.Id);
            CreateExpressionColumn<long>(Defs.Columns.Calculated.PublishedCount, expr);

            expr = string.Format("Count(Child({0}).{1})", Defs.Relations.ToSelfPublished, SelfPublishedTable.Defs.Columns.Id);
            CreateExpressionColumn<long>(Defs.Columns.Calculated.SelfPublishedCount, expr);

            expr = string.Format("Count(Child({0}).{1}) > 0", Defs.Relations.ToPublished, PublishedTable.Defs.Columns.Id);
            CreateExpressionColumn<bool>(Defs.Columns.Calculated.IsPublished, expr);

            expr = string.Format("Count(Child({0}).{1}) > 0", Defs.Relations.ToSelfPublished, SelfPublishedTable.Defs.Columns.Id);
            CreateExpressionColumn<bool>(Defs.Columns.Calculated.IsSelfPublished, expr);

            expr = string.Format("Sum(Child({0}).{1})", Defs.Relations.ToSubmission, SubmissionTable.Defs.Columns.Calculated.CurrentSubCount);
            CreateExpressionColumn<long>(Defs.Columns.Calculated.CurrentSubCount, expr);

            expr = string.Format("Sum(Child({0}).{1}) > 0", Defs.Relations.ToSubmission, SubmissionTable.Defs.Columns.Calculated.CurrentSubCount);
            CreateExpressionColumn<bool>(Defs.Columns.Calculated.IsSubmitted, expr);

            CreateActionExpressionColumn<DateTime>
                (
                    Defs.Columns.Calculated.LastestVersionDate,
                    Controller.GetTable<TitleVersionTable>(),
                    UpdateLatestVersionDate,
                    TitleVersionTable.Defs.Columns.Updated,
                    TitleVersionTable.Defs.Columns.Version
                );

            CreateActionExpressionColumn<long>
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
                long titleId = (long)row[Defs.Columns.Id];
                var verController = versions.GetVersionController(titleId);
                if (verController.Versions.Count > 0)
                {
                    row[Defs.Columns.Calculated.LastestVersionWordCount] = verController.Versions[0].WordCount;
                    row[Defs.Columns.Calculated.LastestVersionDate] = verController.Versions[0].Updated;
                    row[Defs.Columns.Calculated.LastestVersionPath] = verController.Versions[0].FileName;
                }
            }
            AcceptChanges();
        }

        /// <summary>
        /// Populates a new row with default (starter) values
        /// </summary>
        /// <param name="row">The freshly created DataRow to poulate</param>
        protected override void PopulateDefaultRow(DataRow row)
        {
            row[Defs.Columns.Title] = "(new title)";
            row[Defs.Columns.Created] = DateTime.UtcNow;
            row[Defs.Columns.Written] = DateTime.UtcNow;
            row[Defs.Columns.AuthorId] = Controller.GetTable<AuthorTable>().GetDefaultAuthorId();
            row[Defs.Columns.Ready] = false;
            row[Defs.Columns.QuickFlag] = false;
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
            long titleId = (long)e.Row[TitleVersionTable.Defs.Columns.TitleId];
            DataRow[] titleRows = Select($"{Defs.Columns.Id}={titleId}");
            if (titleRows.Length == 1)
            {
                var verController = Controller.GetTable<TitleVersionTable>().GetVersionController(titleId);
                if (verController.Versions.Count > 0)
                {
                    titleRows[0][titleColumn] = verController.Versions[0].Row[titleVersionColumn];
                }
            }
        }
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
            public long Id
            {
                get => GetInt64(Defs.Columns.Id);
            }

            /// <summary>
            /// Gets or sets the title for this row object.
            /// </summary>
            public string Title
            {
                get => GetString(Defs.Columns.Title);
                set => SetValue(Defs.Columns.Title, value);
            }

            /// <summary>
            /// Gets the created date/time value for this row object.
            /// </summary>
            public DateTime Created
            {
                get => GetDateTime(Defs.Columns.Created);
            }

            /// <summary>
            /// Gets or sets the written date/time value for this row object.
            /// The return value is expressed as UTC, but since this comes from
            /// the database, it's not certain to be so. Earlier data was stored with local value.
            /// When setting this field, use Utc.
            /// </summary>
            public DateTime Written
            {
                get => GetDateTime(Defs.Columns.Written);
                set => SetValue(Defs.Columns.Written, value);
            }

            /// <summary>
            /// Gets or sets the author id for this row object.
            /// </summary>
            public long AuthorId
            {
                get => GetInt64(Defs.Columns.AuthorId);
                set => SetValue(Defs.Columns.AuthorId, value);
            }

            /// <summary>
            /// Gets or sets the ready flag.
            /// </summary>
            public bool Ready
            {
                get => GetBoolean(Defs.Columns.Ready);
                set => SetValue(Defs.Columns.Ready, value);
            }

            /// <summary>
            /// Gets or sets the quick tag flag.
            /// </summary>
            public bool QuickFlag
            {
                get => GetBoolean(Defs.Columns.QuickFlag);
                set => SetValue(Defs.Columns.QuickFlag, value);
            }

            /// <summary>
            /// Gets or sets the notes for this row object.
            /// </summary>
            public string Notes
            {
                get => GetString(Defs.Columns.Notes);
                set => SetValue(Defs.Columns.Notes, value);
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