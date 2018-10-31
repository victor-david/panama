using Restless.App.Panama.Configuration;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Creates a list of all titles and their corresponding versions.
    /// </summary>
    public class TitleLister : FileScanBase
    {
        #region Private
        private string outputDirectory;
        private TitleTable titleTable;
        private TitleVersionTable titleVersionTable;
        #endregion

        /************************************************************************/

        #region Public properties and fields
        /// <summary>
        /// Gets the name of the output file (file name only, no path) that holds the list of titles.
        /// </summary>
        public const string ListFile = "TitleList.txt";

        /// <summary>
        /// Gets the full path to the file that holds the list of titles.
        /// </summary>
        public string TitleListFileName
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleLister"/> class.
        /// </summary>
        /// <param name="outputDirectory">The directory in which to write the title list.</param>
        public TitleLister(string outputDirectory)
        {
            this.outputDirectory = outputDirectory;
            titleTable = DatabaseController.Instance.GetTable<TitleTable>();
            titleVersionTable = DatabaseController.Instance.GetTable<TitleVersionTable>();
            TitleListFileName = Path.Combine(outputDirectory, ListFile);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Performs the title list operation. This method is called from the base class on a background task.
        /// </summary>
        protected override void ExecuteTask()
        {
            // This is checked by the caller, but we need to make sure.
            Validations.ValidateInvalidOperation(!Directory.Exists(outputDirectory), Strings.InvalidOpTitleListOutputFolder);

            List<string> lines = new List<string>();

            var titleEnumerator = titleTable.EnumerateTitles();
            TotalCount = titleEnumerator.Count();

            foreach (var title in titleEnumerator)
            {
                lines.Add(string.Format("{0} - {1}", title.Written.ToString(Config.Instance.DateFormat), title.Title));

                foreach (var ver in titleVersionTable.EnumerateVersions(title.Id))
                {
                    string note = !string.IsNullOrEmpty(ver.Note) ? $"[{ver.Note}]" : string.Empty;
                    lines.Add($"  v{ver.Version}.{(char)ver.Revision} {ver.LanguageId} {ver.FileName}   {note}".Trim());
                }

                lines.Add("----------------------------------------------------------------------------------------------------");
                ScanCount++;
            }

            File.WriteAllLines(TitleListFileName, lines);
        }
        #endregion
    }
}