/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Core.Utility;
using System;
using System.Globalization;
using System.IO;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Exports title versions to a specified export directory.
    /// </summary>
    /// <remarks>
    /// The export operation scans all files represented in the <see cref="TitleVersionTable"/>,
    /// and creates a copy in the folder specified by <see cref="Config.FolderExport"/>. It only copys
    /// those files that don't exist in the export folder, or have been updated since the last export operation.
    /// Files in the export folder that have no corresponding record in the <see cref="TitleVersionTable"/>
    /// (for instance, due to a file rename, or the change/removal of a version) are removed.
    /// </remarks>
    public class TitleExporter : TitleScanner
    {
        #region Private
        private readonly TitleExportTitleList candidates;
        #endregion

        /************************************************************************/

        #region Public
        /// <summary>
        /// Gets the name of the readme file that is created in the export directory during an export operation.
        /// </summary>
        public const string ReadMe = "_ReadMeExport.txt";
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleExporter"/> class.
        /// </summary>
        public TitleExporter()
        {
            candidates = new TitleExportTitleList();
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Performs the export operation.
        /// </summary>
        protected override FileScanResult ExecuteTask()
        {
            ThrowIfOutputDirectoryNotSet();
            FileScanResult result = new();
            PopulateCandidates();
            PerformExport(result);
            RemoveExtraFromExportDirectory(result);
            WriteReadMeFileIf(result);
            return result;
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void PopulateCandidates()
        {
            candidates.Clear();

            foreach (TitleRow title in TitleTable.EnumerateTitles())
            {
                foreach (TitleVersionRow version in TitleVersionTable.EnumerateVersions(title.Id, SortDirection.Ascending))
                {
                    candidates.Add(new TitleExportCandidate(title, version, OutputDirectory));
                }
            }
        }

        private void PerformExport(FileScanResult result)
        {
            foreach (TitleExportCandidate candidate in candidates)
            {
                result.ScanCount++;

                if (candidate.Status is TitleExportStatus.OriginalIsNewer or TitleExportStatus.ExportFileDoesNotExist)
                {
                    File.Copy(candidate.Path, candidate.ExportPath, true);
                    result.Updated.Add(FileScanItem.Create(candidate.Title, candidate.ExportName, candidate.Version, candidate.Revision));
                }
            }
        }

        private void RemoveExtraFromExportDirectory(FileScanResult result)
        {
            foreach (string path in Directory.EnumerateFiles(OutputDirectory))
            {
                string fileName = Path.GetFileName(path);
                if (fileName != ReadMe && !candidates.HasCandidateWithExportPath(path))
                {
                    File.Delete(path);
                    result.NotFound.Add(FileScanItem.Create("(Unknown)", fileName, 0, TitleVersionTable.Defs.Values.RevisionA));
                }
            }
        }

        /// <summary>
        /// Writes the readme file if any updates or removals occured, or if the file doesn't exist.
        /// </summary>
        private void WriteReadMeFileIf(FileScanResult result)
        {
            string readMeFile = Path.Combine(OutputDirectory, ReadMe);
            if (result.Updated.Count > 0 || result.NotFound.Count > 0 || !File.Exists(readMeFile))
            {
                AssemblyInfo a = new(AssemblyInfoType.Entry);
                File.WriteAllText(readMeFile, string.Format(CultureInfo.InvariantCulture, Strings.FormatTextExport, a.Title, DateTime.UtcNow.ToString("R")));
            }
        }
        #endregion
    }
}