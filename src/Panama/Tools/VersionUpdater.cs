/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Core.OpenXml;
using System.Linq;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Provides functionality to update title version meta data
    /// </summary>
    public class VersionUpdater : FileScanBase
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionUpdater"/> class.
        /// </summary>
        public VersionUpdater()
        {
        }
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the name of this file scanner tool.
        /// </summary>
        public override string ScannerName => "Title Version Updater";
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

                foreach (var ver in DatabaseController.Instance.GetTable<TitleVersionTable>().EnumerateVersions(title.Id, SortDirection.Ascending))
                {
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