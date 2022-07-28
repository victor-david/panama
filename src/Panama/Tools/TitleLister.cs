/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using System.Globalization;
using System.IO;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Creates a list of all titles and their corresponding versions.
    /// </summary>
    public class TitleLister : TitleScanner
    {
        #region Public properties and fields
        /// <summary>
        /// Gets the name of the output file (file name only, no path) that holds the list of titles.
        /// </summary>
        public const string ListFile = "TitleList.txt";
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleLister"/> class.
        /// </summary>
        public TitleLister()
        {
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Performs the title list operation.
        /// </summary>
        protected override FileScanResult ExecuteTask()
        {
            ThrowIfOutputDirectoryNotSet();
            FileScanResult result = new();

            string separator = string.Empty.PadLeft(60, '-');

            foreach (TitleRow title in TitleTable.EnumerateTitles())
            {
                result.AppendOutputText(string.Format(CultureInfo.InvariantCulture, "{0} - {1}", title.Written.ToString(Config.Instance.DateFormat, CultureInfo.InvariantCulture), title.Title));

                foreach (TitleVersionRow ver in TitleVersionTable.EnumerateVersions(title.Id, SortDirection.Ascending))
                {
                    result.ScanCount++;
                    string note = !string.IsNullOrEmpty(ver.Note) ? $"[{ver.Note}]" : string.Empty;
                    result.AppendOutputText($"  v{ver.Version}.{(char)ver.Revision} {ver.LanguageId} {ver.FileName} {note}".TrimEnd());
                }

                result.AppendOutputText(separator);
            }

            File.WriteAllText(Path.Combine(OutputDirectory, ListFile), result.OutputText.ToString());
            return result;
        }
        #endregion
    }
}