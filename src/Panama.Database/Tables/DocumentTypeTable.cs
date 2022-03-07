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
    /// Represents the table that hold dcoument type information. Used for title version documents
    /// and submssion documents. This is a lookup table.
    /// </summary>
    public class DocumentTypeTable : Core.ApplicationTableBase
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
            public const string TableName = "documenttype";

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
                /// The name of the name column.
                /// </summary>
                public const string Name = "name";

                /// <summary>
                /// The name of the extensions column.
                /// </summary>
                public const string Extensions = "extensions";

                /// <summary>
                /// The name of the ordering column.
                /// </summary>
                public const string Ordering = "ordering";

                /// <summary>
                /// The name of the supported column. Document types that are unsupported cannot
                /// be used, but are retained for backward compatibility with existing data.
                /// </summary>
                public const string Supported = "supported";
            }

            /// <summary>
            /// Provides static values for the <see cref="Columns.Id"/>  column.
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// The value of the <see cref="Columns.Id"/> column that represents an unknown document file type.
                /// </summary>
                public const long UnknownFileType = 0;

                /// <summary>
                /// The value of the <see cref="Columns.Id"/> column that represents a Word OpenXml document.
                /// </summary>
                public const long WordOpenXmlFileType = 1;

                /// <summary>
                /// The value of the <see cref="Columns.Id"/> column that represents an older Word document (.doc or .rtf)
                /// </summary>
                public const long WordOlderFileType = 2;

                /// <summary>
                /// The value of the <see cref="Columns.Id"/> column that represents a PDF document.
                /// </summary>
                public const long PdfFileType = 3;

                /// <summary>
                /// The value of the <see cref="Columns.Id"/> column that represents a plain text document.
                /// </summary>
                public const long TextFileType = 4;

                /// <summary>
                /// The value of the <see cref="Columns.Id"/> column that represents an Html document.
                /// </summary>
                public const long HtmlFileType = 9;

                /// <summary>
                /// The value of the <see cref="Columns.Id"/> column that represents an image document.
                /// </summary>
                public const long ImageFileType = 10;

                /// <summary>
                /// The value of the <see cref="Columns.Id"/> column that represents an executable file.
                /// </summary>
                public const long ExecutableFileType = 11;
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentTypeTable"/> class.
        /// </summary>
        public DocumentTypeTable() : base(Defs.TableName)
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
        /// Gets a boolean value that indicates if the specified doc type is supported
        /// </summary>
        /// <param name="id">The doc type id</param>
        /// <returns>true if supported; otherwise, false.</returns>
        public bool IsDocTypeSupported(long id)
        {
            DataRow[] rows = Select(string.Format("{0}={1}", Defs.Columns.Id, id));
            if (rows.Length == 1)
            {
                return (bool)rows[0][Defs.Columns.Supported];
            }
            return false;
        }

        /// <summary>
        /// Provides an enumerable that enumerates all supported document types
        /// </summary>
        /// <returns></returns>
        public IEnumerable<DocumentTypeRow> EnumerateSupported()
        {
            foreach (DataRow row in EnumerateRows($"{Defs.Columns.Supported}=1", Defs.Columns.Ordering))
            {
                yield return new DocumentTypeRow(row);
            }
        }

        /// <summary>
        /// Gets the document type id from the specified file name
        /// </summary>
        /// <param name="filename">The file name</param>
        /// <returns>The corresponding document type id, or Defs.Values.UnknownFileType if unable to determine the type</returns>
        public long GetDocTypeFromFileName(string filename)
        {
            if (!string.IsNullOrEmpty(filename))
            {
                /* The leading dot is stripped because extensions are stored without one */
                string extension = System.IO.Path.GetExtension(filename)?.ToLower().Substring(1);
                if (!string.IsNullOrEmpty(extension))
                {
                    foreach (DocumentTypeRow docType in EnumerateSupported())
                    {
                        if (docType.ContainsExtension(extension))
                        {
                            return docType.Id;
                        }
                    }
                }
            }
            return Defs.Values.UnknownFileType;
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
                { Defs.Columns.Extensions, ColumnType.Text, false, true },
                { Defs.Columns.Ordering, ColumnType.Integer },
                { Defs.Columns.Supported, ColumnType.Boolean, false, false, 1 },
            };
        }

        /// <summary>
        /// Gets a list of column names to use in subsequent initial insert operations.
        /// These are used only when the table is empty, i.e. upon first creation.
        /// </summary>
        /// <returns>A list of column names</returns>
        protected override List<string> GetPopulateColumnList()
        {
            return new List<string>() { Defs.Columns.Id, Defs.Columns.Name, Defs.Columns.Extensions, Defs.Columns.Ordering, Defs.Columns.Supported };
        }

        /// <summary>
        /// Provides an enumerable that returns values for each row to be populated.
        /// </summary>
        /// <returns>An IEnumerable</returns>
        protected override IEnumerable<object[]> EnumeratePopulateValues()
        {
            yield return new object[] { 1, "Word Document (OpenXml)", "docx;dotm", 1, true };
            yield return new object[] { 2, "Word Document (Old)", "doc;rtf", 2, true };
            yield return new object[] { 3, "Pdf Document", "pdf", 3, true };
            yield return new object[] { 4, "Text Document", "txt", 4, true };
            yield return new object[] { 5, "Outlook Message", "msg", 5, true };
            yield return new object[] { 6, "Audio", "mp3", 6, true };
            yield return new object[] { 7, "Html", "html;htm", 7, true };
            yield return new object[] { 8, "Images", "jpg;jpeg;png", 8, true };
            yield return new object[] { 9, "Executable", "exe", 9, true };
            
            yield return new object[] { 0, "Unknown", null, 100, false };
            yield return new object[] { 10, "Outlook Message (Direct Reference)", null, 101, false };
            yield return new object[] { 11, "Outlook Folder (Direct Reference)", null, 102, false };
        }
        #endregion
    }
}