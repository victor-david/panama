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
using System.IO;

namespace Restless.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains information about documents associated with a submission batch.
    /// </summary>
    public class SubmissionDocumentTable : Core.ApplicationTableBase
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
            public const string TableName = "submissiondocument";

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
                /// The name of the submission batch id column.
                /// </summary>
                public const string BatchId = "submissionbatchid";

                /// <summary>
                /// The name of the title column. Holds a user-entered descriptive title for the document.
                /// </summary>
                public const string Title = "title";

                /// <summary>
                /// The name of the document type column. Gets its value from <see cref="DocumentTypeTable"/>.
                /// </summary>
                public const string DocType = "doctype";

                /// <summary>
                /// The name of the document id column. This is normally the file name, but can be used for other references.
                /// </summary>
                public const string DocId = "docid";

                /// <summary>
                /// The name of the updated column, the date / time this document was updated.
                /// </summary>
                public const string Updated = "updated";

                /// <summary>
                /// The name of the size column, the size in bytes of this document.
                /// </summary>
                public const string Size = "size";
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionDocumentTable"/> class.
        /// </summary>
        public SubmissionDocumentTable() : base(Defs.TableName)
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
            Load(null, Defs.Columns.BatchId);
        }

        /// <summary>
        /// Adds a document entry.
        /// </summary>
        /// <param name="batchId">The batch id</param>
        /// <param name="docId">The doc id, may be null to create a placeholder row</param>
        public void AddEntry(long batchId, string docId)
        {
            DataRow row = NewRow();
            row[Defs.Columns.BatchId] = batchId;
            row[Defs.Columns.Title] = "(new document)";

            object rowDocId = DBNull.Value;
            if (!string.IsNullOrEmpty(docId))
            {
                rowDocId = docId;
            }

            // OnColumnChanged(e) method will update the associated fields: Updated, Size, and DocType
            row[Defs.Columns.DocId] = rowDocId;
            Rows.Add(row);
            Save();
        }

        /// <summary>
        /// Adds a document entry as a placeholder.
        /// </summary>
        /// <param name="batchId">The batch id.</param>
        public void AddEntry(long batchId)
        {
            AddEntry(batchId, null);
        }

        /// <summary>
        /// Provides an enumerable that gets all submission documents in order of id ASC.
        /// </summary>
        /// <returns>A <see cref="RowObject"/></returns>
        public IEnumerable<RowObject> EnumerateSubmissionDocuments()
        {
            DataRow[] rows = Select(null, $"{Defs.Columns.Id} ASC");
            foreach (DataRow row in rows)
            {
                yield return new RowObject(row);
            }
            yield break;
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
                { Defs.Columns.BatchId, ColumnType.Integer },
                { Defs.Columns.Title, ColumnType.Text },
                { Defs.Columns.DocType, ColumnType.Integer },
                { Defs.Columns.DocId, ColumnType.Text, false, true },
                { Defs.Columns.Updated, ColumnType.Timestamp },
                { Defs.Columns.Size, ColumnType.Integer, false, false, 0 },
            };
        }

        /// <summary>
        /// Occurs when data in a column is changed.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnColumnChanged(DataColumnChangeEventArgs e)
        {
            base.OnColumnChanged(e);
            if (e.Column.ColumnName == Defs.Columns.DocId)
            {
                string fullPath = Path.Combine(Controller.GetTable<ConfigTable>().GetRowValue(ConfigTable.Defs.FieldIds.FolderSubmissionDocument), e.ProposedValue.ToString());
                var info = new FileInfo(fullPath);
                if (info.Exists)
                {
                    e.Row[Defs.Columns.Size] = info.Length;
                    e.Row[Defs.Columns.Updated] = info.LastWriteTimeUtc;
                }
                else
                {
                    e.Row[Defs.Columns.Size] = 0; ;
                    e.Row[Defs.Columns.Updated] = DateTime.UtcNow;
                }
                e.Row[Defs.Columns.DocType] = Controller.GetTable<DocumentTypeTable>().GetDocTypeFromFileName(e.ProposedValue.ToString());
            }
        }
        #endregion

        /************************************************************************/

        #region Row Object
        /// <summary>
        /// Encapsulates a single row from the <see cref="SubmissionDocumentTable"/>.
        /// </summary>
        public class RowObject : RowObjectBase<SubmissionDocumentTable>
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
            /// Gets the batch id for this row object.
            /// </summary>
            public long BatchId
            {
                get => GetInt64(Defs.Columns.BatchId);
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
            /// Gets the document type for this row object.
            /// </summary>
            public long DocType
            {
                get => GetInt64(Defs.Columns.DocType);
            }

            /// <summary>
            /// Gets or sets the document id (usually a file name) for this row object.
            /// </summary>
            public string DocumentId
            {
                get => GetString(Defs.Columns.DocId);
                set => SetValue(Defs.Columns.DocId, value);
            }

            /// <summary>
            /// Gets or sets the updated date for this row object.
            /// </summary>
            public DateTime Updated
            {
                get => GetDateTime(Defs.Columns.Updated);
                set => SetValue(Defs.Columns.Updated, value);
            }

            /// <summary>
            /// Gets or sets the file size.
            /// </summary>
            public long Size
            {
                get => GetInt64(Defs.Columns.Size);
                set => SetValue(Defs.Columns.Size, value);
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