using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Utility;

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
        /// Gets the version
        /// </summary>
        public Int64 Version
        {
            get;
            private set;
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
        /// <param name="title">The title</param>
        /// <param name="version">The version number</param>
        /// <param name="filename">The file name</param>
        public FileScanDisplayObject(string title, Int64 version, string filename)
        {
            Validations.ValidateNullEmpty(title, "FileScanDisplayObject.Title");
            Validations.ValidateNullEmpty(filename, "FileScanDisplayObject.FileName");

            Title = title;
            Version = version;
            FileName = filename;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileScanDisplayObject"/> class, setting the version number to zero.
        /// </summary>
        /// <param name="title">The title</param>
        /// <param name="filename">The file name</param>
        public FileScanDisplayObject(string title, string filename)
            : this(title, 0, filename)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileScanDisplayObject"/> class, setting title to 'n/a', and version number to zero.
        /// </summary>
        /// <param name="filename">The file name</param>
        /// <param name="lastModified">The last modified of the file</param>
        public FileScanDisplayObject(string filename, DateTime lastModified)
            : this("n/a", 0, filename)
        {
            LastModified = lastModified;
        }
        #endregion
    }
}
