using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Database;
using Restless.Tools.Threading;
using System.Data;
using System.IO;
using Restless.App.Panama.Configuration;
using Restless.Tools.OpenXml;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Provides functionality to update title version meta data
    /// </summary>
    public class VersionUpdater : FileScanBase
    {
        #region Constructor
        #pragma warning disable 1591
        public VersionUpdater()
        {
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Performs the update. This method is called from the base class on a background task.
        /// </summary>
        protected override void ExecuteTask()
        {
            //HashSet<string> processedPaths = new HashSet<string>();
            
            DatabaseController.Instance.Execution.NonQuery("VACUUM");

            DataRow[] titleRows = DatabaseController.Instance.GetTable<TitleTable>().Select(null, String.Format("{0} DESC", TitleTable.Defs.Columns.Written));
            TotalCount = titleRows.Length;

            foreach (DataRow titleRow in titleRows)
            {
                ScanCount++;
                var titleObj = new TitleTable.RowObject(titleRow);

                DataRow[] versionRows = DatabaseController.Instance.GetTable<TitleVersionTable>().GetAllVersions(titleObj.Id);
                foreach (DataRow versionRow in versionRows)
                {
                    var verObj = new TitleVersionTable.RowObject(versionRow);
                    verObj.SetFileInfo(Paths.Title.WithRoot(verObj.FileName));

                    if (verObj.Info.Exists)
                    {
                        Int64 foundWordCount = 0;
                        if (verObj.DocType == DocumentTypeTable.Defs.Values.WordOpenXmlFileType)
                        {
                            foundWordCount = OpenXmlDocument.Reader.TryGetWordCount(verObj.Info.FullName);
                            if (Config.Instance.SyncDocumentInternalDates)
                            {
                                var props = OpenXmlDocument.Reader.GetProperties(verObj.Info.FullName);
                                // we can't use LastWriteTimeUtc here because props.Core.Modified converts it
                                // back to a local time. If we use Utc, it means that docs would update every time we run update
                                // because props.Core.Modified is never equal to verObj.Info.LastWriteTimeUtc.
                                //
                                // titleObj.WrittenUtc comes from the database. The DateTime.Kind property is not stored, therefore
                                // we don't know for sure if it's local or utc. With new entries / changes to Written, we store Utc. 
                                if (props.Core.Created != titleObj.Written.ToLocalTime() || props.Core.Modified != verObj.Info.LastWriteTime)
                                {
                                    props.Core.Created = titleObj.Written.ToLocalTime();
                                    // we need to add the same number of seconds that props.Save() does.
                                    props.Core.Modified = verObj.Info.LastWriteTime.AddSeconds(OpenXmlDocument.SecondsToAdd);
                                    props.Save();
                                    // we need to obtain the file info again to reflect the new modified date
                                    verObj.SetFileInfo(Paths.Title.WithRoot(verObj.FileName));
                                }

                            }
                        }
                        if (verObj.Updated != verObj.Info.LastWriteTimeUtc || verObj.Size != verObj.Info.Length || verObj.WordCount != foundWordCount)
                        {
                            verObj.Updated = verObj.Info.LastWriteTimeUtc; ;
                            verObj.Size = verObj.Info.Length; ;
                            verObj.WordCount = foundWordCount;
                            DatabaseController.Instance.GetTable<TitleVersionTable>().Save();
                            var item = new FileScanDisplayObject(titleObj.Title, verObj.Version, verObj.FileName);
                            OnUpdated(item);
                        }
                    }
                    else
                    {
                        var item = new FileScanDisplayObject(titleObj.Title, verObj.Version, verObj.FileName);
                        OnNotFound(item);
                    }
                }
            }
        }
        #endregion
    }
}
