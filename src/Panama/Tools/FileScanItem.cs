/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Tables;
using Restless.Panama.Utility;
using System;
using System.IO;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents a single file scan item
    /// </summary>
    public class FileScanItem
    {
        #region Public
        public const string TitleZero = "---";

        /// <summary>
        /// Gets the title associated with the scan item
        /// </summary>
        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the full path and file name for this scan item
        /// </summary>
        public string FullName
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the file name associated with the scan item
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the version associated with this scan item
        /// </summary>
        public long Version
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the revision associated with this scan item
        /// </summary>
        public long Revision
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a string that contains concatenated version and revision
        /// </summary>
        public string VersionRevision => $"{Version}{(char)Revision}";


        /// <summary>
        /// Gets the size associated with this scan item
        /// </summary>
        public long Size
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the last modified date associated with this scan item
        /// </summary>
        public DateTime LastWriteTimeUtc
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the file extension
        /// </summary>
        public string FileExtension => Path.GetExtension(FullName);

        /// <summary>
        /// Gest the directory name of the file
        /// </summary>
        public string DirectoryName => Path.GetDirectoryName(FullName);
        #endregion

        /************************************************************************/

        #region Constructors
        private FileScanItem()
        {
        }

        protected FileScanItem(TitleRow title, TitleVersionRow version)
        {
            Throw.IfNull(title);
            Throw.IfNull(version);
            Title = title.Title;
            FileName = version.FileName;
            Version = version.Version;
            Revision = version.Revision;
        }

        /// <summary>
        /// Returns a new instance of the <see cref="FileScanItem"/> class.
        /// </summary>
        /// <param name="title">The title associated with this scan item</param>
        /// <param name="fullName">The full path and file name</param>
        /// <param name="version">The version</param>
        /// <param name="revision">Revision. If zero, transforms to revision A</param>
        /// <returns></returns>
        public static FileScanItem Create(string title, string fullName, long version, long revision)
        {
            return new FileScanItem()
            {
                Title = string.IsNullOrWhiteSpace(title) ? TitleZero : title,
                FullName = fullName,
                FileName = Path.GetFileName(fullName),
                Version = version,
                Revision = revision == 0 ? TitleVersionTable.Defs.Values.RevisionA : revision,
            };
        }

        public static FileScanItem Create(string fullName)
        {
            FileScanItem result = new()
            {
                Title = TitleZero,
                FullName = fullName,
                FileName = Path.GetFileName(fullName),
                Version = 0,
                Revision = TitleVersionTable.Defs.Values.RevisionA
            };

            FileInfo fileInfo = new(fullName);
            if (fileInfo.Exists)
            {
                result.Size = fileInfo.Length;
                result.LastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
            }
            return result;
        }
        #endregion

        public override string ToString()
        {
            return string.IsNullOrEmpty(FullName) ? FileName : FullName;
        }
    }

}