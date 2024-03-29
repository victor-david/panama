/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/

namespace Restless.Panama.Tools
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