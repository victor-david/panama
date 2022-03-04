/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Toolkit.Core.Utility;
using System;
using System.IO;

namespace Restless.App.Panama.Core
{
    /// <summary>
    /// A singleton class that provides information about the application.
    /// </summary>
    public sealed class ApplicationInfo
    {
        #region Public properties
        /// <summary>
        /// Gets the singleton instance of this class.
        /// </summary>
        public static ApplicationInfo Instance { get; } = new ApplicationInfo();

        /// <summary>
        /// Gets the assembly information object.
        /// </summary>
        public AssemblyInfo Assembly
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the build date for the assembly.
        /// </summary>
        public DateTime BuildDate
        {
            get
            {
                var version = Assembly.VersionRaw;
                DateTime buildDate = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
                return buildDate;
            }
        }

        /// <summary>
        /// Gets the root folder for the application.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This property is used to locate the database file. The sub-directory "db" is always appended.
        /// </para>
        /// <para>
        /// During normal execution, this property returns the location of the application executable.
        /// During development, you can specify a folder to be used by modifying DevelopmentRoot.txt.
        /// That way, you can use a database located outside of the development environment.
        /// </para>
        /// </remarks>
        public string RootFolder
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the reference help file.
        /// </summary>
        public string ReferenceFileName => Path.Combine(RootFolder, "Panama.Reference.chm");

        /// <summary>
        /// Gets the database file name
        /// </summary>
        public string DatabaseFileName => DatabaseController.Instance.DatabaseFileName;

        /// <summary>
        /// Gets a boolean value that indicates if the current process is a 64 bit process.
        /// </summary>
        public bool Is64Bit => Environment.Is64BitProcess;
        #endregion

        /************************************************************************/

        #region Constructor (private)

        private ApplicationInfo()
        {
            Assembly = new AssemblyInfo(AssemblyInfoType.Entry);
            RootFolder = Path.GetDirectoryName(Assembly.Location);
        }
        #endregion
    }
}