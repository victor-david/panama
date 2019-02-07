/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Database.SQLite;
using System.Data;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Provides statistics for the <see cref="TitleVersionTable"/>.
    /// </summary>
    public class TitleVersionTableStats : TableStatisticBase
    {
        #region Public properties
        /// <summary>
        /// Gets the count of html version documents.
        /// </summary>
        public int HtmlCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of pdf version documents.
        /// </summary>
        public int PdfCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of text version documents.
        /// </summary>
        public int TextCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of version documents with an unknown type.
        /// </summary>
        public int UnknownCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of Word (older, .doc and .rtf) version documents.
        /// </summary>
        public int Word2007Count
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the count of Word Open Xml version documents.
        /// </summary>
        public int WordOpenXmlCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the number of particular version files that are shared between titles.
        /// This is normally zero, but there might be a reason why the user wants to do this.
        /// </summary>
        public int SharedCount
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleVersionTableStats"/> class.
        /// </summary>
        /// <param name="table">The title version table.</param>
        public TitleVersionTableStats(TitleVersionTable table)
            : base(table)
        {
            // the base constructor calls the Refresh method.
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Populates the statistics
        /// </summary>
        protected override void Refresh()
        {
            base.Refresh();
            HtmlCount = 0;
            PdfCount = 0;
            TextCount = 0;
            UnknownCount = 0;
            Word2007Count = 0;
            WordOpenXmlCount = 0;
            SharedCount = 0;
            Dictionary<string, int> files = new Dictionary<string, int>();
            foreach (DataRow row in Table.Rows)
            {
                long docType = (long)row[TitleVersionTable.Defs.Columns.DocType];
                if (docType == DocumentTypeTable.Defs.Values.HtmlFileType) HtmlCount++;
                if (docType == DocumentTypeTable.Defs.Values.PdfFileType) PdfCount++;
                if (docType == DocumentTypeTable.Defs.Values.TextFileType) TextCount++ ;
                if (docType == DocumentTypeTable.Defs.Values.UnknownFileType) UnknownCount++;
                if (docType == DocumentTypeTable.Defs.Values.WordOlderFileType) Word2007Count++;
                if (docType == DocumentTypeTable.Defs.Values.WordOpenXmlFileType) WordOpenXmlCount++;
                long titleId = (long)row[TitleVersionTable.Defs.Columns.TitleId];
                string fileName = row[TitleVersionTable.Defs.Columns.FileName].ToString();
                if (!files.ContainsKey(fileName))
                {
                    files.Add(fileName, 1);
                }
                else
                {
                    files[fileName]++;
                }
            }

            foreach (int count in files.Values)
            {
                if (count > 1) SharedCount++;
            }

            files = null;
        }
        #endregion
    }
}