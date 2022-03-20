/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Utility;
using Restless.Toolkit.Core.Utility;
using System;
using System.Globalization;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents a single candidate for the export operation.
    /// </summary>
    public class TitleExportCandidate : FileScanItem
    {
        #region Public Properties
        /// <summary>
        /// Gets the full export path associated with this instance.
        /// </summary>
        public string ExportFullName
        {
            get;
        }

        /// <summary>
        /// Gets the status of this instance.
        /// </summary>
        public TitleExportStatus Status
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleExportCandidate"/> class
        /// </summary>
        /// <param name="title">The title row</param>
        /// <param name="version">The title version row</param>
        /// <param name="exportDirectory">The export directory</param>
        public TitleExportCandidate(TitleRow title, TitleVersionRow version, string exportDirectory) : base(title, version)
        {
            Throw.IfEmpty(exportDirectory);

            FullName = Paths.Title.WithRoot(version.FileName);
            ExportFullName = System.IO.Path.Combine(exportDirectory, GetExportName(title, version));

            Status = TitleExportStatus.None;

            if (!System.IO.File.Exists(FullName))
            {
                Status = TitleExportStatus.OriginalFileDoesNotExist;
            }
            else if (!System.IO.File.Exists(ExportFullName))
            {
                Status = TitleExportStatus.ExportFileDoesNotExist;
            }
            else /* compare dates */
            {
                DateTime dateOrig = System.IO.File.GetLastWriteTime(FullName);
                DateTime dateExport = System.IO.File.GetLastWriteTime(ExportFullName);
                /*
                 * less than zero = t1 is earlier
                 * zero = same
                 * greater than zero = t1 is later
                 */
                int diff = DateTime.Compare(dateOrig, dateExport);
                Status = diff == 0 ? TitleExportStatus.SameDateTime : diff > 0 ? TitleExportStatus.OriginalIsNewer : TitleExportStatus.OriginalIsOlder;
            }
        }
        #endregion

        private string GetExportName(TitleRow title, TitleVersionRow ver)
        {
            // DateWritten_Title_vVer.Rev.Lang.ext
            // Ex: 2011-05-24_Title_v1.A.en-us.docx
            return
                string.Format(CultureInfo.InvariantCulture, "{0}_{1}_v{2}.{3}.{4}{5}",
                    title.Written.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    Format.ValidFileName(title.Title),
                    ver.Version, (char)ver.Revision,
                    ver.LanguageId,
                    System.IO.Path.GetExtension(ver.FileName));
        }
    }
}