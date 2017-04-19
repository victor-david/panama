using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Threading;
using System.Data;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Configuration;
using System.IO;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Updates the submission document file database entries.
    /// </summary>
    public class SubmissionUpdater : FileScanBase
    {
        #region Constructor
        #pragma warning disable 1591
        public SubmissionUpdater()
        {
        }
        #pragma warning restore 1591

        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Performs the update. This method is called from the base class on a background task.
        /// </summary>
        protected override void ExecuteTask()
        {
            DataRow[] rows = DatabaseController.Instance.GetTable<SubmissionDocumentTable>().Select(null, SubmissionDocumentTable.Defs.Columns.Id);
            TotalCount = rows.Length;

            foreach (DataRow row in rows)
            {
                ScanCount++;
                Int64 docType = (Int64)row[SubmissionDocumentTable.Defs.Columns.DocType];
                if (DatabaseController.Instance.GetTable<DocumentTypeTable>().IsDocTypeSupported(docType))
                {
                    string title = row[SubmissionDocumentTable.Defs.Columns.Title].ToString();
                    DateTime subDocDate = (DateTime)row[SubmissionDocumentTable.Defs.Columns.Updated];
                    Int64 subDocSize = (Int64)row[SubmissionDocumentTable.Defs.Columns.Size];
                    string rowName = row[SubmissionDocumentTable.Defs.Columns.DocId].ToString();
                    string filename = Paths.SubmissionDocument.WithRoot(rowName);
                    var info = new FileInfo(filename);
                    if (info.Exists)
                    {
                        if (subDocDate != info.LastWriteTimeUtc || subDocSize != info.Length)
                        {
                            row[SubmissionDocumentTable.Defs.Columns.Updated] = info.LastWriteTimeUtc;
                            row[SubmissionDocumentTable.Defs.Columns.Size] = info.Length;
                            DatabaseController.Instance.GetTable<SubmissionDocumentTable>().Save();
                            var item = new FileScanDisplayObject(title, rowName);
                            OnUpdated(item);
                        }
                    }
                    else
                    {
                        var item = new FileScanDisplayObject(title, rowName);
                        OnNotFound(item);
                    }
                }
            }
        }
        #endregion
    }
}
