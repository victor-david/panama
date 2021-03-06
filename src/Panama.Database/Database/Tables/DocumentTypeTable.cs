/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Tools.Database.SQLite;
using System;
using System.Data;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that hold dcoument type information. Used for title version documents
    /// and submssion documents. This is a lookup table.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class DocumentTypeTable : TableBase
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
                public const string Id = "id";

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
        /// Initializes a new instance of the <see cref="DocumentTypeTable"/> class.
        /// </summary>
        public DocumentTypeTable() : base(DatabaseController.Instance, Defs.TableName)
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
        /// Gets a DataRow array that contains all supported doc types
        /// </summary>
        /// <returns>An array of <see cref="DataRow"/> objects that contains the supported document types.</returns>
        public DataRow[] GetSupportedDocTypes()
        {
            return Select(string.Format("{0}=1", Defs.Columns.Supported), Defs.Columns.Ordering);
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
                string fileExt = System.IO.Path.GetExtension(filename).ToLower();
                if (!string.IsNullOrEmpty(fileExt))
                {
                    foreach (DataRow row in GetSupportedDocTypes())
                    {
                        string[] extensions = row[Defs.Columns.Extensions].ToString().Split(';');
                        foreach (string ext in extensions)
                        {
                            if (ext == fileExt)
                            {
                                return (long)row[Defs.Columns.Id];
                            }
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
        /// Gets the DDL needed to create this table.
        /// </summary>
        /// <returns>A SQL string that describes how to create this table.</returns>
        protected override string GetDdl()
        {
            return Resources.Create.DocumentType;
        }

        /// <summary>
        /// Gets the SQL needed to populate this table with its default values.
        /// </summary>
        /// <returns>A SQL string to insert the default data.</returns>
        protected override string GetPopulateSql()
        {
            return Resources.Data.DocumentType;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate, DataColumnPropertyKey.ReceiveInsertedId);
        }
        #endregion

        /************************************************************************/



    }
}