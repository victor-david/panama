/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Core;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using System.IO;
using System.Linq;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Updates the submission document file database entries.
    /// </summary>
    public class SubmissionUpdater : FileScanBase
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionUpdater"/> class.
        /// </summary>
        public SubmissionUpdater()
        {
        }
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the name of this file scanner tool.
        /// </summary>
        public override string ScannerName => "Submission Document Updater";
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