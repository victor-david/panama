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
            var submissionEnumerator = DatabaseController.Instance.GetTable<SubmissionDocumentTable>().EnumerateSubmissionDocuments();

            TotalCount = submissionEnumerator.Count();

            foreach (var row in submissionEnumerator)
            {
                ScanCount++;
                if (DatabaseController.Instance.GetTable<DocumentTypeTable>().IsDocTypeSupported(row.DocType))
                {
                    string filename = Paths.SubmissionDocument.WithRoot(row.DocumentId);
                    var info = new FileInfo(filename);
                    if (info.Exists)
                    {
                        if (row.Updated != info.LastWriteTimeUtc || row.Size != info.Length)
                        {
                            row.Updated = info.LastWriteTimeUtc;
                            row.Size = info.Length;
                            DatabaseController.Instance.GetTable<SubmissionDocumentTable>().Save();
                            var item = new FileScanDisplayObject(row.Title, row.DocumentId);
                            OnUpdated(item);
                        }
                    }
                    else
                    {
                        var item = new FileScanDisplayObject(row.Title, row.DocumentId);
                        OnNotFound(item);
                    }
                }
            }
        }
        #endregion
    }
}
