/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the link verification table.
    /// </summary>
    public class LinkVerifyTable : Core.ApplicationTableBase
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
            public const string TableName = "linkverify";

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
                /// Id from other table
                /// </summary>
                public const string Xid = "xid";
                
                /// <summary>
                /// Source of the link (publisher, published, user links, etc.)
                /// </summary>
                public const string Source = "source";

                /// <summary>
                /// Url
                /// </summary>
                public const string Url = "url";

                /// <summary>
                /// Date / time of last scan
                /// </summary>
                public const string Scanned = "scanned";

                /// <summary>
                /// Status, ex: 200
                /// </summary>
                public const string Status = "status";

                /// <summary>
                /// Status text, ex: Ok
                /// </summary>
                public const string StatusText = "statustext";

                /// <summary>
                /// Response size in bytes
                /// </summary>
                public const string Size = "size";

                /// <summary>
                /// Error text if any
                /// </summary>
                public const string Error = "error";
            }

            /// <summary>
            /// Provides static values
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// Identifier for alert source
                /// </summary>
                public const string AlertSource = "Alert";

                /// <summary>
                /// Identifier for link source
                /// </summary>
                public const string LinkSource = "Link";

                /// <summary>
                /// Identifier for published source
                /// </summary>
                public const string PublishedSource = "Published";

                /// <summary>
                /// Identifier for publisher source
                /// </summary>
                public const string PublisherSource = "Publisher";

                /// <summary>
                /// Identifier for self published source
                /// </summary>
                public const string SelfPublishedSource = "Self-Published";

                /// <summary>
                /// Identifier for self publisher source
                /// </summary>
                public const string SelfPublisherSource = "Self-Publisher";
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkVerifyTable"/> class.
        /// </summary>
        public LinkVerifyTable() : base(Defs.TableName)
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
            Load(null, Defs.Columns.Id);
        }

        /// <summary>
        /// Provides an enumerable that gets all entries in order of id ASC.
        /// </summary>
        /// <returns>An enumerable that gets all entries</returns>
        public IEnumerable<LinkVerifyRow> EnumerateAll()
        {
            foreach (DataRow row in EnumerateRows(null, Defs.Columns.Id))
            {
                yield return new LinkVerifyRow(row);
            }
        }

        /// <summary>
        /// Refreshes all links by deleting all rows and re-populating.
        /// </summary>
        public void Refresh()
        {
            foreach (LinkVerifyRow link in EnumerateAll())
            {
                link.Row.Delete();
            }
            Save();
            Clear();
            PopulateLinks();
            Save();
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
                { Defs.Columns.Xid, ColumnType.Integer },
                { Defs.Columns.Source, ColumnType.Text },
                { Defs.Columns.Url, ColumnType.Text },
                { Defs.Columns.Scanned, ColumnType.Timestamp, false, true },
                { Defs.Columns.Status, ColumnType.Integer, false, false, 0 },
                { Defs.Columns.StatusText, ColumnType.Text, false, true },
                { Defs.Columns.Size, ColumnType.Integer, false, false, 0 },
                { Defs.Columns.Error, ColumnType.Text, false, true }
            };
        }

        /// <summary>
        /// Establishes parent / child relationships with other tables.
        /// </summary>
        protected override void SetDataRelations()
        {
        }

        /// <inheritdoc/>
        protected override void OnInitializationComplete()
        {
            base.OnInitializationComplete();
            PopulateLinks();
            Save();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void PopulateLinks()
        {
            foreach (AlertRow alert in Controller.GetTable<AlertTable>().EnumerateAll())
            {
                if (alert.HasUrl)
                {
                    AddRow(Defs.Values.AlertSource, alert.Id, alert.Url);
                }
            }

            foreach (LinkRow link in Controller.GetTable<LinkTable>().EnumerateAll())
            {
                if (link.HasUrl)
                {
                    AddRow(Defs.Values.LinkSource, link.Id, link.Url);
                }
            }

            foreach (PublishedRow published in Controller.GetTable<PublishedTable>().EnumerateAll())
            {
                if (published.HasUrl)
                {
                    AddRow(Defs.Values.PublishedSource, published.Id, published.Url);
                }
            }

            foreach (PublisherRow publisher in Controller.GetTable<PublisherTable>().EnumerateAll())
            {
                if (publisher.HasUrl)
                {
                    AddRow(Defs.Values.PublisherSource, publisher.Id, publisher.Url);
                }
            }

            foreach (SelfPublishedRow published in Controller.GetTable<SelfPublishedTable>().EnumerateAll())
            {
                if (published.HasUrl)
                {
                    AddRow(Defs.Values.SelfPublishedSource, published.Id, published.Url);
                }
            }

            foreach (SelfPublisherRow publisher in Controller.GetTable<SelfPublisherTable>().EnumerateAll())
            {
                if (publisher.HasUrl)
                {
                    AddRow(Defs.Values.SelfPublisherSource, publisher.Id, publisher.Url);
                }
            }
        }

        private void AddRow(string source, long xid, string url)
        {
            DataRow[] rows = Select($"{Defs.Columns.Source}='{source}' AND {Defs.Columns.Xid}={xid}");
            if (rows.Length == 0)
            {
                DataRow row = NewRow();
                row[Defs.Columns.Xid] = xid;
                row[Defs.Columns.Source] = source;
                row[Defs.Columns.Url] = url;
                row[Defs.Columns.Status] = 0;
                row[Defs.Columns.Size] = 0;
                Rows.Add(row);
            }
        }
        #endregion
    }
}