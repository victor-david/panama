/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Tables;
using Restless.Panama.Utility;
using System;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents a single file scan item
    /// </summary>
    public class FileScanItem
    {
        #region Public properties
        /// <summary>
        /// Gets the title associated with the scan item
        /// </summary>
        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the file name associated with the scan item
        /// </summary>
        public string File
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or (from a devired class) sets the path
        /// </summary>
        public string Path
        {
            get;
            protected set;
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

        public string VersionRevision => $"{Version}{(char)Revision}";

        /// <summary>
        /// Gets the size associated with this scan item
        /// </summary>
        public long Size { get; }

        /// <summary>
        /// Gets the last modified date associated with this scan item
        /// </summary>
        public DateTime LastModified { get; }
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
            File = version.FileName;
            Version = version.Version;
            Revision = version.Revision;
        }

        public static FileScanItem Create(string title, string file, long version, long revision)
        {
            return new FileScanItem()
            {
                Title = title,
                File = System.IO.Path.GetFileName(file),
                Version = version,
                Revision = revision,
            };
        }
        #endregion

        public override string ToString()
        {
            return Path;
        }
    }

}