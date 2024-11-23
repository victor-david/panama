using System;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents the optional startup options that the application can accept.
    /// </summary>
    public class StartupOptions
    {
        #region Public fields
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
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if any operation was specifed in the startup arguments.
        /// </summary>
        public bool IsAnyOperationRequested => IsUpdateRequested || IsExportRequested || IsTitleListRequested;

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
            IsUpdateRequested = IsUpdateRequested || string.Compare(arg, Update, StringComparison.OrdinalIgnoreCase) == 0;
            IsExportRequested = IsExportRequested || string.Compare(arg, Export, StringComparison.OrdinalIgnoreCase) == 0;
            IsTitleListRequested = IsTitleListRequested || string.Compare(arg, TitleList, StringComparison.OrdinalIgnoreCase) == 0;
        }
        #endregion
    }
}