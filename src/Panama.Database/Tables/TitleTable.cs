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
    public class TitleTable : Core.ApplicationTableBase
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
                public const string Id = DefaultPrimaryKeyName;

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
                    public const string LatestVersionDate = "CalcVerDate";

                    /// <summary>
                    /// The name of the latest version word count column. This calculated column
                    /// holds the word count of the latest title version per the <see cref="TitleVersionTable"/>.
                    /// </summary>
                    public const string LatestVersionWordCount = "CalcVerWordCount";

                    /// <summary>
                    /// The name of the latest version path column. This calculated column
                    /// holds the path of the latest title version per the <see cref="TitleVersionTable"/>.
                    /// </summary>
                    public const string LatestVersionPath = "CalcVerPath";

                    /// <summary>
                    /// The name of the tag count column. This calculated column
                    /// holds the number of related records from the <see cref="TitleTagTable"/>.
                    /// </summary>
                    public const string TagCount = "CalcTagCount";

                    /// <summary>
                    /// The name of the related count column. This claculated column
                    /// holds the number of related records from the <see cref="TitleRelatedTable"/>
                    /// </summary>
                    public const string RelatedCount = "CalcRelatedCount";

                    /// <summary>
                    /// Calculated column that holds the number of queues this title belongs to.
                    /// </summary>
                    public const string QueuedCount = "CalcQueuedCount";

                    /// <summary>
                    /// Calculated column that indicates if this title belongs to at least one queue.
                    /// </summary>
                    public const string IsQueued = "CalcIsQueued";

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
                /// The name of the relation that relates the <see cref="TitleTable"/> to the <see cref="TitleRelatedTable"/>.
                /// </summary>
                public const string ToTitleRelated = "TitleToTitleRelated";

                /// <summary>
                /// The name of the relation that relates the <see cref="TitleTable"/> to the <see cref="PublishedTable"/>.
                /// </summary>
                public const string ToPublished = "TitleToPublished";

                /// <summary>
                /// The name of the relation that relates the <see cref="TitleTable"/> to the <see cref="SelfPublishedTable"/>.
                /// </summary>
                public const string ToSelfPublished = "TitleToSelfPublished";

                /// <summary>
                /// The name of the relation that relates the <see cref="TitleTable"/> to the <see cref="QueueTitleTable"/>
                /// </summary>
                public const string ToQueueTitle = "TitleToQueueTitle";

            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleTable"/> class.
        /// </summary>
        public TitleTable() : base(Defs.TableName)
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
        /// Provides an enumerable that gets all titles in order of written desc, id desc.
        /// </summary>
        /// <returns>A <see cref="TitleRow"/></returns>
        public IEnumerable<TitleRow> EnumerateTitles()
        {
            DataRow[] rows = Select(null, $"{Defs.Columns.Written} desc, {Defs.Columns.Id} desc");
            foreach (DataRow row in rows)
            {
                yield return new TitleRow(row);
            }
            yield break;
        }

        /// <summary>
        /// Gets a single <see cref="RowObject"/> as specified by its id.
        /// </summary>
        /// <param name="id">The id of the record to get.</param>
        /// <returns>A <see cref="TitleRow"/> for <paramref name="id"/>, or null if not found</returns>
        public TitleRow GetSingleRecord(long id)
        {
            DataRow[] rows = Select($"{Defs.Columns.Id}={id}");
            if (rows.Length==1)
            {
                return new TitleRow(rows[0]);
            }
            return null;
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
                { Defs.Columns.Title, ColumnType.Text },
                { Defs.Columns.Created, ColumnType.Timestamp },
                { Defs.Columns.Written, ColumnType.Timestamp },
                { Defs.Columns.AuthorId, ColumnType.Integer, false, false, 1 },
                { Defs.Columns.Ready, ColumnType.Boolean, false, false, 0 },
                { Defs.Columns.QuickFlag, ColumnType.Boolean, false, false, 0 },
                { Defs.Columns.Notes, ColumnType.Text, false, true },
            };
        }

        /// <summary>
        /// Establishes parent / child relationships with other tables.
        /// </summary>
        protected override void SetDataRelations()
        {
            CreateParentChildRelation<SubmissionTable>(Defs.Relations.ToSubmission, Defs.Columns.Id, SubmissionTable.Defs.Columns.TitleId);
            CreateParentChildRelation<TitleVersionTable>(Defs.Relations.ToVersion, Defs.Columns.Id, TitleVersionTable.Defs.Columns.TitleId);
            CreateParentChildRelation<TitleTagTable>(Defs.Relations.ToTitleTag, Defs.Columns.Id, TitleTagTable.Defs.Columns.TitleId);
            CreateParentChildRelation<TitleRelatedTable>(Defs.Relations.ToTitleRelated, Defs.Columns.Id, TitleRelatedTable.Defs.Columns.RelatedId);
            CreateParentChildRelation<PublishedTable>(Defs.Relations.ToPublished, Defs.Columns.Id, PublishedTable.Defs.Columns.TitleId);
            CreateParentChildRelation<SelfPublishedTable>(Defs.Relations.ToSelfPublished, Defs.Columns.Id, SelfPublishedTable.Defs.Columns.TitleId);
            CreateParentChildRelation<QueueTitleTable>(Defs.Relations.ToQueueTitle, Defs.Columns.Id, QueueTitleTable.Defs.Columns.TitleId);
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

            expr = string.Format("Count(Child({0}).{1})", Defs.Relations.ToTitleRelated, TitleRelatedTable.Defs.Columns.TitleId);
            CreateExpressionColumn<long>(Defs.Columns.Calculated.RelatedCount, expr);

            expr = string.Format("Count(Child({0}).{1})", Defs.Relations.ToQueueTitle, QueueTitleTable.Defs.Columns.TitleId);
            CreateExpressionColumn<long>(Defs.Columns.Calculated.QueuedCount, expr);

            expr = string.Format("Count(Child({0}).{1}) > 0", Defs.Relations.ToQueueTitle, QueueTitleTable.Defs.Columns.TitleId);
            CreateExpressionColumn<bool>(Defs.Columns.Calculated.IsQueued, expr);

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
                    Defs.Columns.Calculated.LatestVersionDate,
                    Controller.GetTable<TitleVersionTable>(),
                    UpdateLatestVersionDate,
                    TitleVersionTable.Defs.Columns.Updated,
                    TitleVersionTable.Defs.Columns.Version
                );

            CreateActionExpressionColumn<long>
                (
                    Defs.Columns.Calculated.LatestVersionWordCount,
                    Controller.GetTable<TitleVersionTable>(),
                    UpdateLatestVersionWordCount,
                    TitleVersionTable.Defs.Columns.WordCount,
                    TitleVersionTable.Defs.Columns.Version
                );

            CreateActionExpressionColumn<string>
                (
                    Defs.Columns.Calculated.LatestVersionPath,
                    Controller.GetTable<TitleVersionTable>(),
                    UpdateLatestVersionPath,
                    TitleVersionTable.Defs.Columns.FileName
                );
        }

        /// <summary>
        /// Called when database initialization is complete to populate the <see cref="Defs.Columns.Calculated.LatestVersionWordCount"/> column.
        /// </summary>
        protected override void OnInitializationComplete()
        {
            foreach (DataRow row in Rows)
            {
                long titleId = (long)row[Defs.Columns.Id];
                TitleVersionController verController = TitleVersionTable.GetVersionController(titleId);
                if (verController.Versions.Count > 0)
                {
                    row[Defs.Columns.Calculated.LatestVersionWordCount] = verController.Versions[0].WordCount;
                    row[Defs.Columns.Calculated.LatestVersionDate] = verController.Versions[0].Updated;
                    row[Defs.Columns.Calculated.LatestVersionPath] = verController.Versions[0].FileName;
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
            row[Defs.Columns.Written] = Utility.GetUtcNowZero();
            row[Defs.Columns.AuthorId] = Controller.GetTable<AuthorTable>().GetDefaultAuthorId();
            row[Defs.Columns.Ready] = false;
            row[Defs.Columns.QuickFlag] = false;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void UpdateLatestVersionDate(ActionDataColumn col, DataRowChangeEventArgs e)
        {
            UpdateFromLatestVersionCalculated(e, Defs.Columns.Calculated.LatestVersionDate, TitleVersionTable.Defs.Columns.Updated);
        }

        private void UpdateLatestVersionWordCount(ActionDataColumn col, DataRowChangeEventArgs e)
        {
            UpdateFromLatestVersionCalculated(e, Defs.Columns.Calculated.LatestVersionWordCount, TitleVersionTable.Defs.Columns.WordCount);
        }

        private void UpdateLatestVersionPath(ActionDataColumn col, DataRowChangeEventArgs e)
        {
            UpdateFromLatestVersionCalculated(e, Defs.Columns.Calculated.LatestVersionPath, TitleVersionTable.Defs.Columns.FileName);
        }

        private void UpdateFromLatestVersionCalculated(DataRowChangeEventArgs e, string titleColumn, string titleVersionColumn)
        {
            long titleId = (long)e.Row[TitleVersionTable.Defs.Columns.TitleId];
            DataRow[] titleRows = Select($"{Defs.Columns.Id}={titleId}");
            if (titleRows.Length == 1)
            {
                TitleVersionController verController = TitleVersionTable.GetVersionController(titleId);
                if (verController.Versions.Count > 0)
                {
                    titleRows[0][titleColumn] = verController.Versions[0].Row[titleVersionColumn];
                }
            }
        }
        #endregion
    }
}