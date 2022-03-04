/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains publisher information.
    /// </summary>
    public class SelfPublisherTable : Core.ApplicationTableBase
    {
        #region Private
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
            public const string TableName = "selfpublisher";

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
                /// The name of the name column. This column holds the name of the publisher.
                /// </summary>
                public const string Name = "name";

                /// <summary>
                /// The name of the url column. This column holds the url to the publisher's web site.
                /// </summary>
                public const string Url = "url";

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
                    /// The name of the published count column. This calculated column
                    /// holds the total number of items published.
                    /// </summary>
                    public const string PubCount = "CalcPubCount";
                }
            }

            /// <summary>
            /// Provides static relation names.
            /// </summary>
            public static class Relations
            {
                /// <summary>
                /// The name of the relation that relates the <see cref="SelfPublisherTable"/> to the <see cref="PublishedTable"/>.
                /// </summary>
                public const string ToPublished = "SelfPubToPublished";
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfPublisherTable"/> class.
        /// </summary>
        public SelfPublisherTable() : base(Defs.TableName)
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
            Load(null, string.Format("{0} DESC", Defs.Columns.Added));
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
                { Defs.Columns.Url, ColumnType.Text, false, true },
                { Defs.Columns.Notes, ColumnType.Text, false, true },
                { Defs.Columns.Added, ColumnType.Timestamp },
            };
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
            CreateParentChildRelation<SelfPublishedTable>(Defs.Relations.ToPublished, Defs.Columns.Id, SelfPublishedTable.Defs.Columns.SelfPublisherId);
        }

        /// <summary>
        /// Creates the <see cref="Defs.Columns.Calculated"/> columns for this table.
        /// </summary>
        protected override void UseDataRelations()
        {
            string expr = string.Format("Count(Child({0}).{1})", Defs.Relations.ToPublished, SelfPublishedTable.Defs.Columns.Id);
            CreateExpressionColumn<long>(Defs.Columns.Calculated.PubCount, expr);

            //expr = string.Format("Max(Child({0}).{1})", Defs.Relations.ToSubmissionBatch, SubmissionBatchTable.Defs.Columns.Submitted);
            //CreateExpressionColumn<DateTime>(Defs.Columns.Calculated.LastSub, expr);

            //expr = string.Format("Count(Child({0}).{1})", Defs.Relations.ToSubmissionPeriod, SubmissionPeriodTable.Defs.Columns.Id);
            //CreateExpressionColumn<long>(Defs.Columns.Calculated.SubPeriodCount, expr);
        }

        /// <summary>
        /// Populates a new row with default (starter) values
        /// </summary>
        /// <param name="row">The freshly created DataRow to poulate</param>
        protected override void PopulateDefaultRow(System.Data.DataRow row)
        {
            row[Defs.Columns.Name] = "(new self publisher)";
            row[Defs.Columns.Added] = DateTime.UtcNow;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion
    }
}