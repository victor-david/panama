/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Core.Database.SQLite;
using Restless.Toolkit.Core.OpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains information of versions of titles.
    /// </summary>
    public class TitleVersionTable : Core.ApplicationTableBase
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
            public const string TableName = "titleversion";

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
                /// The name of the title id column.
                /// </summary>
                public const string TitleId = "titleid";

                /// <summary>
                /// The name of the document type column.
                /// </summary>
                public const string DocType = "doctype";

                /// <summary>
                /// The name of the file name column.
                /// </summary>
                public const string FileName = "filename";

                /// <summary>
                /// The name of the note column.
                /// </summary>
                public const string Note = "note";

                /// <summary>
                /// The name of the updated column.
                /// </summary>
                public const string Updated = "updated";

                /// <summary>
                /// The name of the size column
                /// </summary>
                public const string Size = "size";

                /// <summary>
                /// The name of the version column
                /// </summary>
                public const string Version = "version";

                /// <summary>
                /// The name of the revision column
                /// </summary>
                public const string Revision = "revision";

                /// <summary>
                /// The name of the language id column
                /// </summary>
                public const string LangId = "langid";

                /// <summary>
                /// The name of the word count column
                /// </summary>
                public const string WordCount = "wordcount";
            }

            /// <summary>
            /// Provides static values associated with title versions.
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// Represents the numerical value for revision A.
                /// </summary>
                public const long RevisionA = 65;
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleVersionTable"/> class.
        /// </summary>
        public TitleVersionTable() : base(Defs.TableName)
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
            Load(null, $"{Defs.Columns.TitleId},{Defs.Columns.Version}");
        }
        
        /// <summary>
        /// Gets a <see cref="TitleVersionController"/> object that describes version information
        /// and provides version management for the specified title.
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <returns>
        /// A <see cref="TitleVersionController"/> object that describes version information
        /// and provides version management for <paramref name="titleId"/>.
        /// </returns>
        public TitleVersionController GetVersionController(long titleId)
        {
            return new TitleVersionController(this, titleId);
        }

        /// <summary>
        /// Provides an enumerable that enumerates all versions for the specified title
        /// in order of version ASC, revision ASC.
        /// </summary>
        /// <param name="titleId">The title id to get all versions for.</param>
        /// <returns>An enumerable</returns>
        public IEnumerable<TitleVersionRow> EnumerateVersions(long titleId)
        {
            foreach (DataRow row in EnumerateRows($"{Defs.Columns.TitleId}={titleId}", $"{Defs.Columns.Version} ASC, {Defs.Columns.Revision} ASC"))
            {
                yield return new TitleVersionRow(row);
            }
        }

        /// <summary>
        /// Provides an enumerable that enumerates all versions with the specified file name
        /// </summary>
        /// <param name="fileName">The file name (should be non-rooted)</param>
        /// <returns>An enumerable</returns>
        public IEnumerable<TitleVersionRow> EnumerateVersions(string fileName)
        {
            fileName = fileName.Replace("'", "''");
            foreach (DataRow row in EnumerateRows($"{Defs.Columns.FileName}='{fileName}'"))
            {
                yield return new TitleVersionRow(row);
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates if the specified file exists as a version record.
        /// </summary>
        /// <param name="fileName">The file name (should be non-rooted)</param>
        /// <returns>true if a record containing the title exists; otherwise, false.</returns>
        public bool VersionWithFileExists(string fileName)
        {
            return EnumerateVersions(fileName).Count() > 0;
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
            // TODO - check why 65 default
            return new ColumnDefinitionCollection()
            {
                { Defs.Columns.Id, ColumnType.Integer, true },
                { Defs.Columns.TitleId, ColumnType.Integer, false, false, 0, IndexType.Index },
                { Defs.Columns.DocType, ColumnType.Integer, false, false, 0 },
                { Defs.Columns.FileName, ColumnType.Text },
                { Defs.Columns.Note, ColumnType.Text, false, true },
                { Defs.Columns.Updated, ColumnType.Timestamp },
                { Defs.Columns.Size, ColumnType.Integer, false, false, 0 },
                { Defs.Columns.Version, ColumnType.Integer, false, false, 0 },
                { Defs.Columns.Revision, ColumnType.Integer, false, false, 65 },
                { Defs.Columns.LangId, ColumnType.Text, false, false, LanguageTable.Defs.Values.DefaultLanguageId, IndexType.Index },
                { Defs.Columns.WordCount, ColumnType.Integer, false, false, 0 },
            };
        }

        /// <summary>
        /// Called when a value of a column has changed, raises the <see cref="DataTable.ColumnChanged"/> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// This method watches for changes on the <see cref="Defs.Columns.FileName"/> property and updates the 
        /// <see cref="Defs.Columns.Size"/>,
        /// <see cref="Defs.Columns.Updated"/> and
        /// <see cref="Defs.Columns.WordCount"/> columns accordingly.
        /// </remarks>
        protected override void OnColumnChanged(DataColumnChangeEventArgs e)
        {
            base.OnColumnChanged(e);
            if (e.Column.ColumnName == Defs.Columns.FileName)
            {
                string fullPath = Path.Combine(Controller.GetTable<ConfigTable>().GetRowValue(ConfigTable.Defs.FieldIds.FolderTitleRoot), e.ProposedValue.ToString());
                var info = new FileInfo(fullPath);
                if (info.Exists)
                {
                    e.Row[Defs.Columns.Size] = info.Length;
                    e.Row[Defs.Columns.Updated] = info.LastWriteTimeUtc;
                    e.Row[Defs.Columns.WordCount] = OpenXmlDocument.Reader.TryGetWordCount(fullPath);
                }
                else
                {
                    e.Row[Defs.Columns.Size] = 0; ;
                    e.Row[Defs.Columns.Updated] = DateTime.UtcNow;
                    e.Row[Defs.Columns.WordCount] = 0;
                }
                e.Row[Defs.Columns.DocType] = Controller.GetTable<DocumentTypeTable>().GetDocTypeFromFileName(e.ProposedValue.ToString());
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion
    }
}