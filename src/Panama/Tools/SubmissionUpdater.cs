/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Updates the submission document file database entries.
    /// </summary>
    public class SubmissionUpdater : Scanner
    {
        private SubmissionDocumentTable SubmissionDocumentTable => DatabaseController.Instance.GetTable<SubmissionDocumentTable>();
        private DocumentTypeTable DocumentTypeTable => DatabaseController.Instance.GetTable<DocumentTypeTable>();


        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionUpdater"/> class.
        /// </summary>
        public SubmissionUpdater()
        {
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Performs the update.
        /// </summary>
        protected override FileScanResult ExecuteTask()
        {
            FileScanResult result = new();

            foreach (SubmissionDocumentRow document in SubmissionDocumentTable.EnumerateSubmissionDocuments())
            {
                result.ScanCount++;

                if (DocumentTypeTable.IsDocTypeSupported(document.DocType))
                {
                    document.SetFileInfo(Paths.SubmissionDocument.WithRoot(document.DocumentId));

                    if (document.Info.Exists)
                    {
                        if (document.RequireSynchonization())
                        {
                            document.Synchronize();
                            SubmissionDocumentTable.Save();
                            result.Updated.Add(FileScanItem.Create(document.Title, document.DocumentId, 0, 0));
                        }
                    }
                    else
                    {
                        result.NotFound.Add(FileScanItem.Create(document.Title, document.DocumentId, 0, 0));
                    }
                }
            }
            return result;
        }
        #endregion
    }
}