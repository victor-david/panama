/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using System;
using System.Collections.Generic;
using System.IO;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Provides a tool that locates files beneath <see cref="Config.FolderTitleRoot"/>
    /// that are not included in title version record.
    /// </summary>
    public class OrphanFinder : Scanner
    {
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
        protected override FileScanResult ExecuteTask()
        {
            FileScanResult result = new();
            //    var files = new List<string>();
            //    var versions = DatabaseController.Instance.GetTable<TitleVersionTable>();

            //    string appDir = Path.GetDirectoryName(ApplicationInfo.Instance.Assembly.Location);

            //    foreach (string dir in Directory.EnumerateDirectories(Config.Instance.FolderTitleRoot, "*", SearchOption.AllDirectories))
            //    {
            //        if (dir != Config.Instance.FolderSubmissionDocument &&
            //            dir != Config.Instance.FolderExport &&
            //            dir != Config.Instance.FolderSubmissionMessage &&
            //            dir != Config.Instance.FolderSubmissionMessageAttachment &&
            //            !dir.StartsWith(appDir))
            //        {
            //            files.AddRange(Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly));
            //        }
            //    }

            //    TotalCount = files.Count;
            //    string[] exclusions = Config.Instance.OrphanExclusions.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            //    foreach (string file in files)
            //    {
            //        ScanCount++;
            //        int excludeCount = 0;
            //        string path = Path.GetDirectoryName(file).ToLower();

            //        foreach (string ex in exclusions)
            //        {
            //            if (path.Contains(ex.Trim().ToLower()))
            //            {
            //                excludeCount++;
            //            }
            //        }

            //        if (excludeCount == 0)
            //        {
            //            string searchFile = Paths.Title.WithoutRoot(file);

            //            if (!versions.VersionWithFileExists(searchFile))
            //            {
            //                var info = new FileInfo(file);
            //                var item = new FileScanDisplayObject(searchFile, info.Length, info.LastWriteTimeUtc);
            //                OnNotFound(item);
            //            }
            //        }
            //    }

            return result;
        }
        #endregion
    }
}