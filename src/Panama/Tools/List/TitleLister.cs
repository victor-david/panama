/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Creates a list of all titles and their corresponding versions.
    /// </summary>
    public class TitleLister : FileScanBase
    {
        #region Private
        private readonly string outputDirectory;
        private readonly TitleTable titleTable;
        private readonly TitleVersionTable titleVersionTable;
        #endregion

        /************************************************************************/

        #region Public properties and fields
        /// <summary>
        /// Gets the name of the output file (file name only, no path) that holds the list of titles.
        /// </summary>
        public const string ListFile = "TitleList.txt";

        /// <summary>
        /// Gets the name of this file scanner tool.
        /// </summary>
        public override string ScannerName => "Title Lister";

        /// <summary>
        /// Gets the full path to the file that holds the list of titles.
        /// </summary>
        public string TitleListFileName
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleLister"/> class.
        /// </summary>
        /// <param name="outputDirectory">The directory in which to write the title list.</param>
        public TitleLister(string outputDirectory)
        {
            this.outputDirectory = outputDirectory;
            titleTable = DatabaseController.Instance.GetTable<TitleTable>();
            titleVersionTable = DatabaseController.Instance.GetTable<TitleVersionTable>();
            TitleListFileName = Path.Combine(outputDirectory, ListFile);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Performs the title list operation. This method is called from the base class on a background task.
        /// </summary>
        protected override void ExecuteTask()
        {
            // This is checked by the caller, but we need to make sure.
            if (!Directory.Exists(outputDirectory))
            {
                throw new InvalidOperationException(Strings.InvalidOpTitleListOutputFolder);
            }

            List<string> lines = new();

            var titleEnumerator = titleTable.EnumerateTitles();
            TotalCount = titleEnumerator.Count();

            foreach (TitleTable.RowObject title in titleEnumerator)
            {
                lines.Add(string.Format("{0} - {1}", title.Written.ToString(Config.Instance.DateFormat), title.Title));

                foreach (var ver in titleVersionTable.EnumerateVersions(title.Id))
                {
                    string note = !string.IsNullOrEmpty(ver.Note) ? $"[{ver.Note}]" : string.Empty;
                    lines.Add($"  v{ver.Version}.{(char)ver.Revision} {ver.LanguageId} {ver.FileName}   {note}".Trim());
                }

                lines.Add("----------------------------------------------------------------------------------------------------");
                ScanCount++;
            }

            File.WriteAllLines(TitleListFileName, lines);
        }
        #endregion
    }
}