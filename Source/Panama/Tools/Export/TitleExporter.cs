using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Utility;
using System.IO;
using System.Data;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Resources;
using Restless.Tools.Threading;

namespace Restless.App.Panama.Tools
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
        #region Private Vars
        private string exportDirectory;
        private TitleExportTitleList candidates;
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

        #region Public Properties
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
            Validations.ValidateInvalidOperation(!Directory.Exists(exportDirectory), Strings.InvalidOpExportFolderNotSet);
            updated = removed = 0;
            PopulateCandidates();
            PerformExport();
            RemoveExtraFromExportDirectory();
            WriteReadMeFileIf();
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        //private void CreateExportDirectory()
        //{
        //    if (!Directory.Exists(exportDirectory))
        //    {
        //        Directory.CreateDirectory(exportDirectory);
        //    }
        //}

        private void PopulateCandidates()
        {
            candidates.Clear();

            DataRow[] titleRows = DatabaseController.Instance.GetTable<TitleTable>().Select(null, String.Format("{0} DESC", TitleTable.Defs.Columns.Written));

            foreach (DataRow titleRow in titleRows)
            {
                Int64 titleId = (Int64)titleRow[TitleTable.Defs.Columns.Id];
                string title = titleRow[TitleTable.Defs.Columns.Title].ToString();
                DateTime written = (DateTime)titleRow[TitleTable.Defs.Columns.Written];

                DataRow[] versionRows = DatabaseController.Instance.GetTable<TitleVersionTable>().GetAllVersions(titleId);
                foreach (DataRow versionRow in versionRows)
                {
                    DateTime versionDate = (DateTime)versionRow[TitleVersionTable.Defs.Columns.Updated];
                    Int64 versionSize = (Int64)versionRow[TitleVersionTable.Defs.Columns.Size];
                    Int64 version = (Int64)versionRow[TitleVersionTable.Defs.Columns.Version];
                    string language = versionRow[TitleVersionTable.Defs.Columns.LangId].ToString();
                    string versionFile = versionRow[TitleVersionTable.Defs.Columns.FileName].ToString();

                    // DateWritten_Title_Ver.Lang.ext
                    // yyyy-MM-dd_Title_Ver.Lang.ext
                    // 2011-05-24_Title_v1.en-us.docx
                    string exportFileName =
                        String.Format("{0}_{1}_v{2}.{3}{4}",
                            written.ToString("yyyy-MM-dd"),
                            Format.ValidFileName(title),
                            version, language,
                            Path.GetExtension(versionFile));
                    candidates.Add(new TitleExportCandidate(title, version, Paths.Title.WithRoot(versionFile), Path.Combine(exportDirectory, exportFileName)));
                }
            }
            TotalCount = candidates.Count;
        }

        private void PerformExport()
        {
            foreach (var candidate in candidates)
            {
                ScanCount++;
                if (candidate.Status == TitleExportStatus.OriginalIsNewer || candidate.Status == TitleExportStatus.ExportFileDoesNotExist)
                {
                    File.Copy(candidate.OriginalPath, candidate.ExportPath, true);
                    var item = new FileScanDisplayObject(candidate.Title, candidate.Version, Paths.Export.WithoutRoot(candidate.ExportPath));
                    OnUpdated(item);
                    updated++;
                }
                //System.Threading.Thread.Sleep(1);
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
                AssemblyInfo a = new AssemblyInfo(AssemblyInfoType.Entry);
                File.WriteAllText(readMeFile, String.Format(Strings.FormatTextExport, a.Title, DateTime.UtcNow.ToString("R")));
            }
        }
        #endregion
    }


}
