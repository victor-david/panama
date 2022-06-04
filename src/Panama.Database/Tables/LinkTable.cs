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
    /// Represents the table that hold links to web sites.
    /// </summary>
    public class LinkTable : Core.ApplicationTableBase
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
            public const string TableName = "link";

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
                /// The name of the name column. This column holds the descriptive name of the link.
                /// </summary>
                public const string Name = "name";

                /// <summary>
                /// The name of the url column.
                /// </summary>
                public const string Url = "url";

                /// <summary>
                /// The name of the notes column. This column holds any notes associated with the link.
                /// </summary>
                public const string Notes = "notes";

                /// <summary>
                /// The name of the credential id column.
                /// </summary>
                public const string CredentialId = "credentialid";

                /// <summary>
                /// The name of the added column. This column holds the date that the link was added.
                /// </summary>
                public const string Added = "added";

            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkTable"/> class.
        /// </summary>
        public LinkTable() : base(Defs.TableName)
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
            Load(null, Defs.Columns.Name);
        }

        /// <summary>
        /// Provides an enumerable that gets all records in order of id.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<LinkRow> EnumerateAll()
        {
            foreach (DataRow row in EnumerateRows(null, Defs.Columns.Id))
            {
                yield return LinkRow.Create(row);
            }
        }

        /// <summary>
        /// Clears the credential id of all links with the specified credential id.
        /// </summary>
        /// <param name="id">The credential id.</param>
        public void ClearCredential(long id)
        {
            foreach (DataRow row in EnumerateRows($"{Defs.Columns.CredentialId}={id}"))
            {
                row[Defs.Columns.CredentialId] = 0;
            }
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
                { Defs.Columns.Name, ColumnType.Text },
                { Defs.Columns.Url, ColumnType.Text },
                { Defs.Columns.Notes, ColumnType.Text, false, true },
                { Defs.Columns.CredentialId, ColumnType.Integer, false, false, 0, IndexType.Index },
                { Defs.Columns.Added, ColumnType.Timestamp },
            };
        }

        /// <summary>
        /// Populates a new row with default (starter) values
        /// </summary>
        /// <param name="row">The freshly created DataRow to poulate</param>
        protected override void PopulateDefaultRow(DataRow row)
        {
            row[Defs.Columns.Name] = LinkRow.DefaultValue;
            row[Defs.Columns.Url] = LinkRow.DefaultValue;
            row[Defs.Columns.Notes] = DBNull.Value;
            row[Defs.Columns.CredentialId] = 0;
            row[Defs.Columns.Added] = DateTime.UtcNow;
        }
        #endregion
    }
}