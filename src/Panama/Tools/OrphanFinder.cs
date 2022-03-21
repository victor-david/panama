/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Provides a tool that locates files beneath <see cref="Config.FolderTitleRoot"/>
    /// that are not included in title version record.
    /// </summary>
    public class OrphanFinder : TitleScanner
    {
        private string[] directoryExclusions;
        private string[] fileExclusions;

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
        /// Performs the update
        /// </summary>
        protected override FileScanResult ExecuteTask()
        {
            FileScanResult result = new();
            List<string> files = new();

            directoryExclusions = Config.Instance.OrphanDirectoryExclusions.ToLower(CultureInfo.InvariantCulture).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            fileExclusions = Config.Instance.OrphanFileExclusions.ToLower(CultureInfo.InvariantCulture).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            files.AddRange(Directory.EnumerateFiles(Config.Instance.FolderTitleRoot));

            foreach (string dir in Directory.EnumerateDirectories(Config.Instance.FolderTitleRoot, "*", SearchOption.AllDirectories))
            {
                if (!IsDirectoryAutoExcluded(dir))
                {
                    files.AddRange(Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly));
                }
            }

            foreach (string fullPath in files)
            {
                result.ScanCount++;

                if (!IsDirectoryUserExcluded(Path.GetDirectoryName(fullPath)) && !IsFileUserExcluded(Path.GetFileName(fullPath)))
                {
                    if (!TitleVersionTable.VersionWithFileExists(Paths.Title.WithoutRoot(fullPath)))
                    {
                        result.Updated.Add(FileScanItem.Create(fullPath));
                    }
                }
            }
            return result;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private bool IsDirectoryAutoExcluded(string directory)
        {
            return
                directory == Config.Instance.FolderSubmissionDocument ||
                directory == Config.Instance.FolderExport ||
                directory == Config.Instance.FolderSubmissionMessage ||
                directory == Config.Instance.FolderSubmissionMessageAttachment;
        }

        private bool IsDirectoryUserExcluded(string directory)
        {
            foreach (string ex in directoryExclusions)
            {
                if (directory.ToLower(CultureInfo.InvariantCulture).Contains(ex.ToLower(CultureInfo.InvariantCulture)))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsFileUserExcluded(string file)
        {
            foreach (string ex in fileExclusions)
            {
                if (file.ToLower(CultureInfo.InvariantCulture).Contains(ex.ToLower(CultureInfo.InvariantCulture)))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}