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
using System.IO;
using System.Linq;
using OrphanValues = Restless.Panama.Database.Tables.OrphanExclusionTable.Defs.Values;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Provides a tool that locates files beneath <see cref="Config.FolderTitleRoot"/>
    /// that are not included in title version record.
    /// </summary>
    public class OrphanFinder : TitleScanner
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
        /// Performs the update
        /// </summary>
        protected override FileScanResult ExecuteTask()
        {
            FileScanResult result = new();
            List<string> files = new();

            foreach (string dir in Directory.EnumerateDirectories(Config.Instance.FolderTitleRoot, "*", SearchOption.AllDirectories))
            {
                if (!IsDirectoryAutoExcluded(dir) && !IsDirectoryExcluded(dir))
                {
                    files.AddRange(Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly));
                }
            }

            foreach (string file in files)
            {
                result.ScanCount++;
                if (!IsFileExtensionExcluded(Path.GetExtension(file)) && !IsFileExcluded(Path.GetFileName(file)))
                {
                    if (!TitleVersionTable.VersionWithFileExists(Paths.Title.WithoutRoot(file)))
                    {
                        result.Updated.Add(FileScanItem.Create(file));
                    }

                }
            }
            return result;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private bool IsDirectoryAutoExcluded(string value)
        {
            return
                value == Config.Instance.FolderSubmissionDocument ||
                value == Config.Instance.FolderExport ||
                value == Config.Instance.FolderSubmissionMessage ||
                value == Config.Instance.FolderSubmissionMessageAttachment;
        }

        private bool IsDirectoryExcluded(string value)
        {
            foreach (string directory in OrphanExclusionTable.EnumerateExclusion(OrphanValues.DirectoryType).Select(p => Paths.Title.WithRoot(p.Value)))
            {
                if (value.StartsWith(directory, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsFileExtensionExcluded(string value)
        {
            foreach (OrphanExclusionRow item in OrphanExclusionTable.EnumerateExclusion(OrphanValues.FileExtensionType))
            {
                if (value.Equals(item.Value, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsFileExcluded(string value)
        {
            foreach (string file in OrphanExclusionTable.EnumerateExclusion(OrphanValues.FileType).Select(p => Paths.Title.WithRoot(p.Value)))
            {
                if (value.Equals(file, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}