/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using System;
using System.IO;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents a single candidate for the export operation.
    /// </summary>
    public class TitleExportCandidate : FileScanDisplayObject
    {
        #region Public Properties
        /// <summary>
        /// Gets the original path associated with this instance.
        /// </summary>
        public string OriginalPath
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the export path associated with this instance.
        /// </summary>
        public string ExportPath
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the status of this instance.
        /// </summary>
        public TitleExportStatus Status
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleExportCandidate"/> class
        /// </summary>
        /// <param name="version">The version number.</param>
        /// <param name="revision">The revision number,</param>
        /// <param name="title">The title</param>
        /// <param name="originalPath">The full path to the original file</param>
        /// <param name="exportPath">The full path to the proposed export file.</param>
        /// <remarks>
        /// The <see cref="TitleExportCandidate"/> constructor examines the two paths to determine
        /// the status of the export candidate. It sets the <see cref="Status"/> property to one of the
        /// values from the <see cref="TitleExportStatus"/> enumeration.
        /// </remarks>
        public TitleExportCandidate(long version, long revision, string title, string originalPath, string exportPath)
            :base(version, revision, title, Paths.Title.WithoutRoot(originalPath))
        {
            if (string.IsNullOrEmpty(originalPath))
            {
                throw new ArgumentNullException(nameof(originalPath));
            }
            if (string.IsNullOrEmpty(exportPath))
            {
                throw new ArgumentNullException(nameof(exportPath));
            }

            OriginalPath = originalPath;
            ExportPath = exportPath;
            Status = TitleExportStatus.None;
            if (!File.Exists(originalPath))
                Status = TitleExportStatus.OriginalFileDoesNotExist;
            else if (!File.Exists(exportPath))
                Status = TitleExportStatus.ExportFileDoesNotExist;
            else /* compare dates */
            {
                DateTime dtOrig = File.GetLastWriteTime(originalPath);
                DateTime dtExport = File.GetLastWriteTime(exportPath);
                /*
                 * less than zero = t1 is earlier
                 * zero = same
                 * greater than zero = t1 is later
                 */
                int diff = DateTime.Compare(dtOrig, dtExport);
                if (diff == 0)
                    Status = TitleExportStatus.SameDateTime;
                else if (diff > 0)
                    Status = TitleExportStatus.OriginalIsNewer;
                else
                    Status = TitleExportStatus.OriginalIsOlder;
            }
        }
        #endregion
    }
}