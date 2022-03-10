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
    public class TitleExporter : FileScanBase
    {
        #region Private
        private readonly string exportDirectory;
        private readonly TitleExportTitleList candidates;
        private int updated;
        private int removed;
        #endregion

        /************************************************************************/

        #region Public Fields
        /// <summary>
        /// Gets the name of the read-me file that is created in the export directory during an export operation.
        /// </summary>
        public const string ReadMe = "_ReadMeExport.txt";
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the name of this file scanner tool.
        /// </summary>
        public override string ScannerName => "Title Exporter";
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleExporter"/> class.
        /// </summary>
        /// <param name="exportDirectory">The directory for the exported files.</param>
        public TitleExporter(string exportDirectory)
        {
            this.exportDirectory = exportDirectory;
            candidates = new TitleExportTitleList();
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Performs the export operation. This method is called from the base class on a background task.
        /// </summary>
        protected override void ExecuteTask()
        {
            // This is checked by the caller, but we need to make sure.
            if (!Directory.Exists(exportDirectory))
            {
                throw new InvalidOperationException(Strings.InvalidOpExportFolderNotSet);
            }
            updated = removed = 0;
            PopulateCandidates();
            PerformExport();
            RemoveExtraFromExportDirectory();
            WriteReadMeFileIf();
        }
        #endregion

        /************************************************************************/

        #region Private Methods

        private void PopulateCandidates()
        {
            candidates.Clear();

            foreach (TitleRow title in DatabaseController.Instance.GetTable<TitleTable>().EnumerateTitles())
            {
                foreach (TitleVersionRow ver in DatabaseController.Instance.GetTable<TitleVersionTable>().EnumerateVersions(title.Id))
                {
                    // DateWritten_Title_vVer.Rev.Lang.ext
                    // Ex: 2011-05-24_Title_v1.A.en-us.docx
                    string exportFileName =
                        string.Format("{0}_{1}_v{2}.{3}.{4}{5}",
                            title.Written.ToString("yyyy-MM-dd"),
                            Format.ValidFileName(title.Title),
                            ver.Version, (char)ver.Revision,
                            ver.LanguageId,
                            Path.GetExtension(ver.FileName));
                    candidates.Add(new TitleExportCandidate(ver.Version, ver.Revision, title.Title, Paths.Title.WithRoot(ver.FileName), Path.Combine(exportDirectory, exportFileName)));
                }

            }
            TotalCount = candidates.Count;
        }

        private void PerformExport()
        {
            foreach (TitleExportCandidate candidate in candidates)
            {
                ScanCount++;
                if (candidate.Status == TitleExportStatus.OriginalIsNewer || candidate.Status == TitleExportStatus.ExportFileDoesNotExist)
                {
                    File.Copy(candidate.OriginalPath, candidate.ExportPath, true);
                    var item = new FileScanDisplayObject(candidate.Version, candidate.Revision, candidate.Title, Paths.Export.WithoutRoot(candidate.ExportPath));
                    OnUpdated(item);
                    updated++;
                }
            }
        }

        private void RemoveExtraFromExportDirectory()
        {
            string[] files = Directory.GetFiles(exportDirectory);
            foreach (string path in files)
            {
                string fileName = Path.GetFileName(path);
                if (fileName != ReadMe && !candidates.HasCandidateWithExportPath(path))
                {
                    File.Delete(path);
                    var item = new FileScanDisplayObject("(unknown)", fileName);
                    OnNotFound(item);
                    removed++;
                }
            }
        }

        /// <summary>
        /// Writes the readme file if any updates or removals occured, or if the file doesn't exist.
        /// </summary>
        private void WriteReadMeFileIf()
        {
            string readMeFile = Path.Combine(exportDirectory, ReadMe);
            if (updated > 0 || removed > 0 || !File.Exists(readMeFile))
            {
                AssemblyInfo a = new(AssemblyInfoType.Entry);
                File.WriteAllText(readMeFile, string.Format(Strings.FormatTextExport, a.Title, DateTime.UtcNow.ToString("R")));
            }
        }
        #endregion
    }


}