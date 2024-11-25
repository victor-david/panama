using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Core.OpenXml;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Provides functionality to update title version meta data
    /// </summary>
    public class TitleVersionUpdater : Scanner
    {
        private TitleTable TitleTable => DatabaseController.Instance.GetTable<TitleTable>();
        private TitleVersionTable TitleVersionTable => DatabaseController.Instance.GetTable<TitleVersionTable>();

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleVersionUpdater"/> class.
        /// </summary>
        public TitleVersionUpdater()
        {
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Performs the update.
        /// </summary>
        protected override FileScanResult ExecuteTask()
        {
            FileScanResult result = new();
            DatabaseController.Instance.Execution.NonQuery("VACUUM");

            foreach (TitleRow title in TitleTable.EnumerateTitles())
            {
                title.Written = title.Written.ToUtcZero();

                foreach (TitleVersionRow version in TitleVersionTable.EnumerateVersions(title.Id, SortDirection.Ascending))
                {
                    result.ScanCount++;
                    version.SetFileInfo(Paths.Title.WithRoot(version.FileName));

                    if (version.Info.Exists)
                    {
                        // Checks last updated date and size
                        if (version.RequireSynchonization())
                        {
                            bool ignoreUpdate = false;
                            long foundWordCount = 0;
                            if (version.DocType == DocumentTypeTable.Defs.Values.WordOpenXmlFileType)
                            {
                                foundWordCount = OpenXmlDocument.Reader.TryGetWordCount(version.Info.FullName);
                                // don't update if zero. this means the file was open when TryGetWordCount() ran.
                                ignoreUpdate = foundWordCount == 0;
                            }

                            // Code path has already determined that last updated and/or size has changed.
                            // Proceed with update unless the word count failed.
                            if (!ignoreUpdate)
                            {
                                version.Synchronize(foundWordCount);
                                DatabaseController.Instance.GetTable<TitleVersionTable>().Save();
                                result.Updated.Add(FileScanItem.Create(title.Title, version.Info.FullName, version.Version, version.Revision));
                            }
                        }
                    }
                    else
                    {
                        result.NotFound.Add(FileScanItem.Create(title.Title, version.Info.FullName, version.Version, version.Revision));
                    }
                }
            }
            return result;
        }
        #endregion
    }
}