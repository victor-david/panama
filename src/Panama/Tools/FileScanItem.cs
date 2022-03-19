/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
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
        public string Title { get; private set; }
        public string Path { get; private set; }
        public string File { get; private set; }
        public long Version { get; private set; }
        public long Revision { get; private set; }
        public string VersionRevision { get; private set; }
        public long Size { get; }
        public DateTime LastModified { get; }
        #endregion

        /************************************************************************/

        #region Constructor
        public static FileScanItem Create(string title, string path, long version, long revision)
        {
            return new FileScanItem()
            {
                Title = title,
                Path = path,
                File = System.IO.Path.GetFileName(path),
                Version = version,
                Revision = revision,
                VersionRevision = $"{version}{(char)revision}"
            };
        }
        ///// <summary>
        ///// Initializes a new instance of the <see cref="FileScanResult"/> class.
        ///// </summary>
        ///// <param name="version">The version number</param>
        ///// <param name="revision">The revision number.</param>
        ///// <param name="size">The size of the file</param>
        ///// <param name="title">The title</param>
        ///// <param name="filename">The file name</param>
        //public FileScanItem(long version, long revision, long size, string title, string filename)
        //{
        //    Throw.IfEmpty(title);
        //    Throw.IfEmpty(filename);
        //    Version = version;
        //    Revision = revision;
        //    Size = size;
        //    Title = title;
        //    FileName = filename;
        //}

        ///// <summary>
        ///// Initializes a new instance of the <see cref="FileScanResult"/> class.
        ///// </summary>
        ///// <param name="version">The version number</param>
        ///// <param name="revision">The revision number.</param>
        ///// <param name="title">The title</param>
        ///// <param name="filename">The file name</param>
        //public FileScanItem(long version, long revision, string title, string filename)
        //    : this(version, revision, 0, title, filename)
        //{
        //}

        ///// <summary>
        ///// Initializes a new instance of the <see cref="FileScanResult"/> class,
        ///// setting version number and revision number to zero.
        ///// </summary>
        ///// <param name="title">The title</param>
        ///// <param name="filename">The file name</param>
        //public FileScanItem(string title, string filename)
        //    : this(0, 0, title, filename)
        //{
        //}

        ///// <summary>
        ///// Initializes a new instance of the <see cref="FileScanResult"/> class,
        ///// setting title to 'n/a',  and version / revision numbers to zero.
        ///// </summary>
        ///// <param name="filename">The file name</param>
        ///// <param name="size">The file size</param>
        ///// <param name="lastModified">The last modified of the file</param>
        //public FileScanItem(string filename, long size, DateTime lastModified)
        //    : this(0, 0, size, "n/a", filename)
        //{
        //    LastModified = lastModified;
        //}
        #endregion

        public override string ToString()
        {
            return Path;
        }
    }

}