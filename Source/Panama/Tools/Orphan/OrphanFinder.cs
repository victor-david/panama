using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.Tools.Threading;
using System.Diagnostics;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Provides a tool that locates files beneath <see cref="Config.FolderTitleRoot"/>
    /// that are not included in title version record.
    /// </summary>
    public class OrphanFinder : FileScanBase
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OrphanFinder"/> class.
        /// </summary>
        public OrphanFinder()
        {
        }

        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Performs the update. This method is called from the base class on a background task.
        /// </summary>
        protected override void ExecuteTask()
        {
            var files = new List<string>();
            var versions = DatabaseController.Instance.GetTable<TitleVersionTable>();

            string appDir = Path.GetDirectoryName(ApplicationInfo.Instance.Assembly.Location);

            foreach (string dir in Directory.EnumerateDirectories(Config.Instance.FolderTitleRoot, "*", SearchOption.AllDirectories))
            {
                if (dir != Config.Instance.FolderSubmissionDocument && 
                    dir != Config.Instance.FolderExport && 
                    dir != Config.Instance.FolderSubmissionMessage &&
                    dir != Config.Instance.FolderSubmissionMessageAttachment &&
                    !dir.StartsWith(appDir))
                {
                    files.AddRange(Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly));
                }
            }

            TotalCount = files.Count;
            string[] exclusions = Config.Instance.OrphanExclusions.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string file in files)
            {
                ScanCount++;
                int excludeCount = 0;
                string path = Path.GetDirectoryName(file).ToLower();
                
                foreach (string ex in exclusions)
                {
                    if (path.Contains(ex.Trim().ToLower()))
                    {
                        excludeCount++;
                    }
                }

                if (excludeCount == 0)
                {
                    string searchFile = Paths.Title.WithoutRoot(file);


                    if (!versions.VersionWithFileExists(searchFile))
                    {
                        var item = new FileScanDisplayObject(searchFile, File.GetLastWriteTimeUtc(file));
                        OnNotFound(item);
                    }
                }
            }
        }
        #endregion
    }
}
