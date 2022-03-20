using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using System;
using System.IO;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Extends <see cref="Scanner"/> scanner tools that deal with titles
    /// and an output directory
    /// </summary>
    public abstract class TitleScanner : Scanner
    {
        protected TitleTable TitleTable => DatabaseController.Instance.GetTable<TitleTable>();
        protected TitleVersionTable TitleVersionTable => DatabaseController.Instance.GetTable<TitleVersionTable>();

        /// <summary>
        /// Gets or sets the output directory
        /// </summary>
        public string OutputDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Throws an <see cref="InvalidOperationException"/> if <see cref="OutputDirectory"/>
        /// isn't set or doesn't exist.
        /// </summary>
        protected void ThrowIfOutputDirectoryNotSet()
        {
            if (string.IsNullOrWhiteSpace(OutputDirectory) || !Directory.Exists(OutputDirectory))
            {
                throw new InvalidOperationException(Strings.InvalidOpOutputFolderNotSet);
            }
        }
    }
}
