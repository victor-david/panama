using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Restless.Tools.Utility;
using Restless.App.Panama.Configuration;

namespace Restless.App.Panama.Tools
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
        /// <param name="title">The title</param>
        /// <param name="version">The version number.</param>
        /// <param name="originalPath">The full path to the original file</param>
        /// <param name="exportPath">The full path to the proposed export file.</param>
        /// <remarks>
        /// The <see cref="TitleExportCandidate"/> constructor examines the two paths to determine
        /// the status of the export candidate. It sets the <see cref="Status"/> property to one of the
        /// values from the <see cref="TitleExportStatus"/> enumeration.
        /// </remarks>
        public TitleExportCandidate(string title, long version, string originalPath, string exportPath)
            :base(title, version, Paths.Title.WithoutRoot(originalPath))
        {
            Validations.ValidateNullEmpty(originalPath, "ExportCandidate.OriginalPath");
            Validations.ValidateNullEmpty(exportPath, "ExportCandidate.ExportPath");

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
