/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.IO;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides static methods for handling rooted and non-rooted paths for title and submission documents.
    /// </summary>
    public static class Paths
    {
        #region Title
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
        
        #region Submission document
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

        #region Submission message
        /// <summary>
        /// Provides static methods for handling rooted and non-rooted paths for submission messages.
        /// </summary>
        public static class SubmissionMessage
        {
            /// <summary>
            /// Returns the file name without the portion that is the submission message root.
            /// </summary>
            /// <param name="fileName">The file name</param>
            /// <returns>The file name without the submission message root portion.</returns>
            public static string WithoutRoot(string fileName)
            {
                return Handlers.WithoutRoot(fileName, Config.Instance.FolderSubmissionMessage);
            }

            /// <summary>
            /// Returns the file name combined with the submission message root.
            /// </summary>
            /// <param name="fileName">The file name.</param>
            /// <returns>The combined path, or <paramref name="fileName"/> unaltered if it is already rooted.</returns>
            public static string WithRoot(string fileName)
            {
                return Handlers.WithRoot(fileName, Config.Instance.FolderSubmissionMessage);
            }
        }
        #endregion

        /************************************************************************/

        #region Export
        /// <summary>
        /// Provides static methods for handling rooted and non-rooted paths for exported documents.
        /// </summary>
        public static class Export
        {
            /// <summary>
            /// Returns the file name without the portion that is the export root.
            /// </summary>
            /// <param name="fileName">The file name</param>
            /// <returns>The file name without the export root portion.</returns>
            public static string WithoutRoot(string fileName)
            {
                return Handlers.WithoutRoot(fileName, Config.Instance.FolderExport);
            }

            /// <summary>
            /// Returns the file name combined with the export root.
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