using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Restless.Tools.Utility;
using Restless.App.Panama.Resources;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Configuration;
using Restless.Tools.Threading;

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
        /// Initializes a new instance of the <see cref="TitleExporter"/> class.
        /// </summary>
        /// <param name="outputDirectory">The directory in which to write the title list.</param>
        public TitleLister(string outputDirectory)
        {
            this.outputDirectory = outputDirectory;
            titleTable = DatabaseController.Instance.GetTable<TitleTable>();
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

            DataRow[] titleRows = titleTable.Select(null, string.Format("{0} DESC", TitleTable.Defs.Columns.Written));
            TotalCount = titleRows.Length;

            foreach (DataRow titleRow in titleRows)
            {
                DateTime written = (DateTime)titleRow[TitleTable.Defs.Columns.Written];
                lines.Add(string.Format("{0} - {1}", written.ToString(Config.Instance.DateFormat), titleRow[TitleTable.Defs.Columns.Title].ToString()));

                DataRow[] titleVersionRows = titleRow.GetChildRows(TitleTable.Defs.Relations.ToVersion);
                foreach (DataRow titleVersionRow in titleVersionRows)
                {
                    lines.Add(
                        string.Format("  v{0}. {1} {2} {3}", 
                            titleVersionRow[TitleVersionTable.Defs.Columns.Version], 
                            titleVersionRow[TitleVersionTable.Defs.Columns.LangId], 
                            titleVersionRow[TitleVersionTable.Defs.Columns.FileName],
                            titleVersionRow[TitleVersionTable.Defs.Columns.Note]));
                }

                lines.Add("----------------------------------------------------------------------------------------------------");
                ScanCount++;
                //System.Threading.Thread.Sleep(1);
            }

            File.WriteAllLines(TitleListFileName, lines);
        }
        #endregion
    }
}
