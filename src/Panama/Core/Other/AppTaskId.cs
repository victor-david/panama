/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides unique task ids for identifying and managing application background tasks.
    /// </summary>
    public static class AppTaskId
    {
        /// <summary>
        /// Identifies the task that performs the title version scan.
        /// </summary>
        public const int TitleVersionScan = 100;

        /// <summary>
        /// Identifies the task that performs the submission document scan.
        /// </summary>
        public const int SubmissionDocumentScan = 101;

        /// <summary>
        /// Identifies the task that performs the title export scan.
        /// </summary>
        public const int TitleExportScan = 102;

        /// <summary>
        /// Identifies the task that performs the orphan finder scan.
        /// </summary>
        public const int OrphanFind = 103;

        /// <summary>
        /// Identifies the task that performs the title list creation.
        /// </summary>
        public const int TitleList = 104;

        /// <summary>
        /// Identifies the task that performs a workaround task used when selecting a DataGrid item from another VM.
        /// </summary>
        public const int SelectedItemFunkyWorkaround = 105;

        /// <summary>
        /// Identifies the task that performs document conversion.
        /// </summary>
        public const int Convert = 106;

        /// <summary>
        /// Identifies the task that performs the folder statistics scan.
        /// </summary>
        public const int FolderStatScan = 107;

        /// <summary>
        /// Identifies the task that performs selected updates as a startup option.
        /// </summary>
        public const int CommandTools = 108;
    }
}