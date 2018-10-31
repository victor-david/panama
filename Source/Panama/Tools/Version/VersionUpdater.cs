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
            DatabaseController.Instance.Execution.NonQuery("VACUUM");

            var titleEnumerator = DatabaseController.Instance.GetTable<TitleTable>().EnumerateTitles();
            TotalCount = titleEnumerator.Count();

            foreach (var title in titleEnumerator)
            {
                ScanCount++;

                foreach (var ver in DatabaseController.Instance.GetTable<TitleVersionTable>().EnumerateVersions(title.Id))
                {
                    // var verObj = new TitleVersionTable.RowObject(ver);
                    ver.SetFileInfo(Paths.Title.WithRoot(ver.FileName));

                    if (ver.Info.Exists)
                    {
                        long foundWordCount = 0;
                        if (ver.DocType == DocumentTypeTable.Defs.Values.WordOpenXmlFileType)
                        {
                            foundWordCount = OpenXmlDocument.Reader.TryGetWordCount(ver.Info.FullName);
                            if (Config.Instance.SyncDocumentInternalDates)
                            {
                                var props = OpenXmlDocument.Reader.GetProperties(ver.Info.FullName);
                                // we can't use LastWriteTimeUtc here because props.Core.Modified converts it
                                // back to a local time. If we use Utc, it means that docs would update every time we run update
                                // because props.Core.Modified is never equal to verObj.Info.LastWriteTimeUtc.
                                //
                                // titleObj.WrittenUtc comes from the database. The DateTime.Kind property is not stored, therefore
                                // we don't know for sure if it's local or utc. With new entries / changes to Written, we store Utc. 
                                if (props.Core.Created != title.Written.ToLocalTime() || props.Core.Modified != ver.Info.LastWriteTime)
                                {
                                    props.Core.Created = title.Written.ToLocalTime();
                                    // we need to add the same number of seconds that props.Save() does.
                                    props.Core.Modified = ver.Info.LastWriteTime.AddSeconds(OpenXmlDocument.SecondsToAdd);
                                    props.Save();
                                    // we need to obtain the file info again to reflect the new modified date
                                    ver.SetFileInfo(Paths.Title.WithRoot(ver.FileName));
                                }

                            }
                        }
                        if (ver.Updated != ver.Info.LastWriteTimeUtc || ver.Size != ver.Info.Length || ver.WordCount != foundWordCount)
                        {
                            ver.Updated = ver.Info.LastWriteTimeUtc; ;
                            ver.Size = ver.Info.Length; ;
                            ver.WordCount = foundWordCount;
                            DatabaseController.Instance.GetTable<TitleVersionTable>().Save();
                            var item = new FileScanDisplayObject(ver.Version, ver.Revision, title.Title, ver.FileName);
                            OnUpdated(item);
                        }
                    }
                    else
                    {
                        var item = new FileScanDisplayObject(ver.Version, ver.Revision, title.Title, ver.FileName);
                        OnNotFound(item);
                    }
                }
            }
            
        }
        #endregion
    }
}
