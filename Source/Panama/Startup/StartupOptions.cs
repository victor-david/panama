using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restless.App.Panama
{
    /// <summary>
    /// Represents the optional startup options that the application can accept.
    /// </summary>
    public class StartupOptions
    {
        #region Publc fields
        /// <summary>
        /// The startup option that specifies the update option. This option performs
        /// an update of the title version and submission document meta data.
        /// </summary>
        public const string Update = "/update";

        /// <summary>
        /// The startup that specifies the export option. This option performs
        /// a syncronization export of all title version documents to the export folder.
        /// </summary>
        public const string Export = "/export";

        /// <summary>
        /// The startup option that specifies the title list creation option. This option creates
        /// a text file that contains the titles and corresponding file names of all title versions.
        /// The list is created in the title root folder.
        /// </summary>
        public const string TitleList = "/list";

        /// <summary>
        /// The startup option that the specifes the database file to use.
        /// It must be in the form of /db:pathToDatabaseFile
        /// </summary>
        public const string Database = "/db:";

        #endregion

        /************************************************************************/
        
        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if any operation was specifed in the startup arguments.
        /// </summary>
        public bool IsAnyOperationRequested
        {
            get
            {
                return IsUpdateRequested || IsExportRequested || IsTitleListRequested;
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates if an update of title version and submission documents
        /// was specifed in the startup arguments. 
        /// This corresponds to the <see cref="Update"/> argument.
        /// </summary>
        public bool IsUpdateRequested
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if an export operation was specifed in the startup arguments.
        /// This corresponds to the <see cref="Export"/> argument.
        /// </summary>
        public bool IsExportRequested
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if a title list operation was specifed in the startup arguments.
        /// This corresponds to the <see cref="TitleList"/> argument.
        /// </summary>
        public bool IsTitleListRequested
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the database file that was specified, or null if none.
        /// This property is set via the <see cref="Database"/> argument.
        /// </summary>
        public string DatabaseFileName
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="StartupOptions"/> class.
        /// </summary>
        /// <param name="args">The array of string arguments (may be empty)</param>
        public StartupOptions(string[] args)
        {
            IsUpdateRequested = false;
            IsExportRequested = false;
            IsTitleListRequested = false;
            if (args != null && args.Length > 0)
            {
                foreach (string arg in args)
                {
                    EvaluateArgument(arg);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void EvaluateArgument(string arg)
        {
            if (arg.ToLower() == Update) IsUpdateRequested = true;
            if (arg.ToLower() == Export) IsExportRequested = true;
            if (arg.ToLower() == TitleList) IsTitleListRequested = true;
            if (arg.ToLower().StartsWith(Database) && arg.Length > Database.Length)
            {
                DatabaseFileName = arg.Substring(Database.Length).Trim();
            }
        }
        #endregion
    }
}
