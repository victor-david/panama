using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Provides enumeration values that describe the status of a single <see cref="TitleExportCandidate"/>.
    /// </summary>
    public enum TitleExportStatus
    {
        /// <summary>
        /// No status has been assigned.
        /// </summary>
        None,
        /// <summary>
        /// The original file is newer than the exported file.
        /// </summary>
        OriginalIsNewer,
        /// <summary>
        /// The original file is older than the exported file.
        /// </summary>
        OriginalIsOlder,
        /// <summary>
        /// The original file and the exported file are the same date/time.
        /// </summary>
        SameDateTime,
        /// <summary>
        /// The original file does not exist.
        /// </summary>
        OriginalFileDoesNotExist,
        /// <summary>
        /// The export file does not exist.
        /// </summary>
        ExportFileDoesNotExist
    }
}
