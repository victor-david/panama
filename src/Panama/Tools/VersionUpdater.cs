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

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Provides functionality to update title version meta data
    /// </summary>
    public class VersionUpdater : Scanner
    {
        private TitleTable TitleTable => DatabaseController.Instance.GetTable<TitleTable>();
        private TitleVersionTable TitleVersionTable => DatabaseController.Instance.GetTable<TitleVersionTable>();

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionUpdater"/> class.
        /// </summary>
        public VersionUpdater()
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
                        // checks only last updated date and size
                        if (version.RequireSynchonization())
                        {
                            long foundWordCount = 0;
                            if (version.DocType == DocumentTypeTable.Defs.Values.WordOpenXmlFileType)
                            {
                                foundWordCount = OpenXmlDocument.Reader.TryGetWordCount(version.Info.FullName);
                                if (Config.Instance.SyncDocumentInternalDates)
                                {
                                    PropertiesAdapter props = OpenXmlDocument.Reader.GetProperties(version.Info.FullName);
                                    // we can't use LastWriteTimeUtc here because props.Core.Modified converts it
                                    // back to a local time. If we use Utc, it means that docs would update every time we run update
                                    // because props.Core.Modified is never equal to verObj.Info.LastWriteTimeUtc.
                                    //
                                    // titleObj.WrittenUtc comes from the database. The DateTime.Kind property is not stored, therefore
                                    // we don't know for sure if it's local or utc. With new entries / changes to Written, we store Utc.
                                    if (props.Core.Created != title.Written.ToLocalTime() || props.Core.Modified != version.Info.LastWriteTime)
                                    {
                                        props.Core.Created = title.Written.ToLocalTime();
                                        // we need to add the same number of seconds that props.Save() does.
                                        props.Core.Modified = version.Info.LastWriteTime.AddSeconds(OpenXmlDocument.SecondsToAdd);
                                        props.Save();
                                        // we need to obtain the file info again to reflect the new modified date
                                        version.SetFileInfo(Paths.Title.WithRoot(version.FileName));
                                    }
                                }
                            }

                            // checks last updated date, size, and word count
                            // this code path has already determined that last updated and/or size has changed
                            if (version.RequireSynchonization(foundWordCount))
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