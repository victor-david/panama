using Restless.Tools.Database.SQLite;
using System;
using System.Data;
using System.IO;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains information about documents associated with a submission batch.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class SubmissionDocumentTable : TableBase
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
                public const string Id = "id";

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
        public SubmissionDocumentTable() : base(DatabaseController.Instance, Defs.TableName)
        {
        }
        #pragma warning restore 1591
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
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets the DDL needed to create this table.
        /// </summary>
        /// <returns>A SQL string that describes how to create this table.</returns>
        protected override string GetDdl()
        {
            return Resources.Create.SubmissionDocument;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate, DataColumnPropertyKey.ReceiveInsertedId);
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

        #region ITableImport and IColumnRowImporter implementation (commented out)
        //public bool PerformImport()
        //{
        //    return DatabaseImporter.Instance.ImportTable(this, this, "submission_document");
        //}

        //public string GetColumnName(string origColName)
        //{
        //    switch (origColName)
        //    {
        //        case "document_id": return Defs.Columns.DocId;
        //        case "document_type": return Defs.Columns.DocType;
        //        default: return origColName;
        //    }
        //}

        //public bool GetRowConfirmation(System.Data.DataRow row)
        //{
        //    //string docId = row["document_id"].ToString();
        //    //string hard = @"d:\writing\title_manager\documents\";
        //    //if (docId.ToLower().StartsWith(hard))
        //    //{
        //    //    row["document_id"] = docId.Substring(hard.Length);
        //    //}
        //    return true;
        //}
        //// Tor House Foundation_2008-03-02-19-04-10-421.doc

        //public bool IncludeColumn(string origColName)
        //{
        //    return true;
        //}

        #endregion



    }
}
