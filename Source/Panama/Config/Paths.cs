using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Restless.App.Panama.Configuration
{
    /// <summary>
    /// Provides static methods for handling rooted and non-rooted paths for title and submission documents.
    /// </summary>
    public static class Paths
    {
        #region Title paths
        /// <summary>
        /// Provides static methods for handling rooted and non-rooted paths for title documents.
        /// </summary>
        public static class Title
        {
            /// <summary>
            /// Returns the file name without the portion that is the title root.
            /// </summary>
            /// <param name="fileName">The file name</param>
            /// <returns>The file name without the title root portion.</returns>
            public static string WithoutRoot(string fileName)
            {
                return Handlers.WithoutRoot(fileName, Config.Instance.FolderTitleRoot);
            }

            /// <summary>
            /// Returns the file name combined with the title root.
            /// </summary>
            /// <param name="fileName">The file name.</param>
            /// <returns>The combined path, or <paramref name="fileName"/> unaltered if it is already rooted.</returns>
            public static string WithRoot(string fileName)
            {
                return Handlers.WithRoot(fileName, Config.Instance.FolderTitleRoot);
            }
        }
        #endregion
        
        /************************************************************************/
        
        #region Submission document paths
        /// <summary>
        /// Provides static methods for handling rooted and non-rooted paths for submission documents.
        /// </summary>
        public static class SubmissionDocument
        {
            /// <summary>
            /// Returns the file name without the portion that is the submission document root.
            /// </summary>
            /// <param name="fileName">The file name</param>
            /// <returns>The file name without the submission document root portion.</returns>
            public static string WithoutRoot(string fileName)
            {
                return Handlers.WithoutRoot(fileName, Config.Instance.FolderSubmissionDocument);
            }

            /// <summary>
            /// Returns the file name combined with the submission document root.
            /// </summary>
            /// <param name="fileName">The file name.</param>
            /// <returns>The combined path, or <paramref name="fileName"/> unaltered if it is already rooted.</returns>
            public static string WithRoot(string fileName)
            {
                return Handlers.WithRoot(fileName, Config.Instance.FolderSubmissionDocument);
            }
        }
        #endregion

        /************************************************************************/

        #region Export document paths
        /// <summary>
        /// Provides static methods for handling rooted and non-rooted paths for submission documents.
        /// </summary>
        public static class Export
        {
            /// <summary>
            /// Returns the file name without the portion that is the submission document root.
            /// </summary>
            /// <param name="fileName">The file name</param>
            /// <returns>The file name without the submission document root portion.</returns>
            public static string WithoutRoot(string fileName)
            {
                return Handlers.WithoutRoot(fileName, Config.Instance.FolderExport);
            }

            /// <summary>
            /// Returns the file name combined with the submission document root.
            /// </summary>
            /// <param name="fileName">The file name.</param>
            /// <returns>The combined path, or <paramref name="fileName"/> unaltered if it is already rooted.</returns>
            public static string WithRoot(string fileName)
            {
                return Handlers.WithRoot(fileName, Config.Instance.FolderExport);
            }
        }
        #endregion
        /************************************************************************/
        
        #region Common handler (private static class)
        
        private static class Handlers
        {
            public static string WithoutRoot(string fileName, string root)
            {
                if (fileName.ToLower().IndexOf(root.ToLower()) == 0)
                {
                    fileName = fileName.Substring(root.Length);
                    while (Path.IsPathRooted(fileName) && fileName.Length > 1)
                    {
                        fileName = fileName.Substring(1);
                    }
                }
                return fileName;
            }

            public static string WithRoot(string fileName, string root)
            {
                if (Path.IsPathRooted(fileName))
                {
                    return fileName;
                }
                return Path.Combine(root, fileName);
            }
        }
        #endregion
    }
}