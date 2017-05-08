using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Utility;
using Restless.App.Panama.Database;
using System.IO;

namespace Restless.App.Panama
{
    /// <summary>
    /// A singleton class that provides information about the application.
    /// </summary>
    public sealed class ApplicationInfo
    {
        #region Private
        private static ApplicationInfo instance = new ApplicationInfo();
        private const string DefaultRootFileName = "DevelopmentRoot.txt";
        private const string DefaultRootEntry = "DevelopmentRoot=";
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the singleton instance of this class.
        /// </summary>
        public static ApplicationInfo Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Gets the assembly information object.
        /// </summary>
        public AssemblyInfo Assembly
        {
            get;
            private set;
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
        public string ReferenceFileName
        {
            get
            {
                return Path.Combine(RootFolder, "Panama.Reference.chm");
            }
        }

        /// <summary>
        /// Gets the database file name
        /// </summary>
        public string DatabaseFileName
        {
            get { return DatabaseController.Instance.DatabaseFileName; }
        }

        /// <summary>
        /// Gets a boolean value that indicates if the database importer is enabled
        /// </summary>
        public bool DatabaseImportEnabled
        {
            get { return DatabaseImporter.Instance.IsEnabled; }
        }

        /// <summary>
        /// Gets a boolean value that indicates if the current process is a 64 bit process.
        /// </summary>
        public bool Is64Bit
        {
            get { return Environment.Is64BitProcess; }
        }

        #endregion

        /************************************************************************/

        #region Constructor (private)

        private ApplicationInfo()
        {
            Assembly = new AssemblyInfo(AssemblyInfoType.Entry);
            RootFolder = Path.GetDirectoryName(Assembly.Location);
            string loc = RootFolder.ToLower();
            if (
                loc.Contains(@"bin\debug") || loc.Contains(@"bin\release") ||
                loc.Contains(@"bin\x86\debug") || loc.Contains(@"bin\x86\release") ||
                loc.Contains(@"bin\x64\debug") || loc.Contains(@"bin\x64\release")
               )
            {
                string devName = Path.Combine(RootFolder, DefaultRootFileName);
                if (File.Exists(devName))
                {
                    string[] lines = File.ReadAllLines(devName);
                    foreach (string line in lines)
                    {
                        if (line.StartsWith(DefaultRootEntry))
                        {
                            RootFolder = line.Substring(DefaultRootEntry.Length);
                        }
                    }
                }
            }

        }
        #endregion
    }
}
