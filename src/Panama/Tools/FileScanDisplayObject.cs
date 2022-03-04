/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Represents the result of a file scan and provides an object for visual components to bind to.
    /// </summary>
    public class FileScanDisplayObject
    {
        #region Public properties
        /// <summary>
        /// Gets the title
        /// </summary>
        public string Title
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the version number. 
        /// Some consumers of this class set this to zero.
        /// </summary>
        public long Version
        {
            get;
        }

        /// <summary>
        /// Gets the revision number.
        /// Some consumers of this class set this to zero.
        /// </summary>
        public long Revision
        {
            get;
        }

        /// <summary>
        /// Gets the size of the file in bytes.
        /// </summary>
        public long Size
        {
            get;
        }

        /// <summary>
        /// Gets the file name. This is typically an unrooted name, but not required.
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the last modified date. Not all consumers of this class use this property
        /// </summary>
        public DateTime LastModified
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FileScanDisplayObject"/> class.
        /// </summary>
        /// <param name="version">The version number</param>
        /// <param name="revision">The revision number.</param>
        /// <param name="size">The size of the file</param>
        /// <param name="title">The title</param>
        /// <param name="filename">The file name</param>
        public FileScanDisplayObject(long version, long revision, long size, string title, string filename)
        {
            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException(nameof(filename));
            }

            Version = version;
            Revision = revision;
            Size = size;
            Title = title;
            FileName = filename;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileScanDisplayObject"/> class.
        /// </summary>
        /// <param name="version">The version number</param>
        /// <param name="revision">The revision number.</param>
        /// <param name="title">The title</param>
        /// <param name="filename">The file name</param>
        public FileScanDisplayObject(long version, long revision, string title, string filename)
            :this(version, revision, 0, title, filename)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileScanDisplayObject"/> class,
        /// setting version number and revision number to zero.
        /// </summary>
        /// <param name="title">The title</param>
        /// <param name="filename">The file name</param>
        public FileScanDisplayObject(string title, string filename)
            : this(0, 0, title, filename)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileScanDisplayObject"/> class,
        /// setting title to 'n/a',  and version / revision numbers to zero.
        /// </summary>
        /// <param name="filename">The file name</param>
        /// <param name="size">The file size</param>
        /// <param name="lastModified">The last modified of the file</param>
        public FileScanDisplayObject(string filename, long size, DateTime lastModified)
            : this(0, 0, size, "n/a", filename)
        {
            LastModified = lastModified;
        }
        #endregion
    }
}