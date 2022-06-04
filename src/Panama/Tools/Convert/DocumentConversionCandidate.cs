/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
#if DOCX
using Restless.Panama.Core;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.Tools.Mvvm;
using Restless.Tools.OfficeAutomation;
using Restless.Tools.Utility;
using System.Collections.Generic;
using System.IO;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents a single candidate for the file conversion operation.
    /// </summary>
    public class DocumentConversionCandidate  : ObservableObject
    {
        #region Private
        private readonly List<TitleVersionTable.RowObject> versions;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the file info for this conversion candidate.
        /// </summary>
        public FileInfo Info
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if this candidate belongs to a file version or versions.
        /// </summary>
        public bool IsVersion
        {
            get => versions.Count > 0;
        }

        /// <summary>
        /// Gets a boolean value that indicates if this candidate belongs to a submission document.
        /// </summary>
        public bool IsSubmissionDoc
        {
            get;
            private set;
        }

        ///// <summary>
        ///// Gets the title id associated with the version, if <see cref="IsVersion"/> is true; otherwise, null;
        ///// </summary>
        //public long? TitleId
        //{
        //    get
        //    {
        //        if (versionRow != null)
        //        {
        //            return (long)versionRow[TitleVersionTable.Defs.Columns.TitleId];
        //        }
        //        return null;
        //    }
        //}

        /// <summary>
        /// Gets a boolean value that indicates if this item has been converted.
        /// </summary>
        public bool IsConverted
        {
            get => Result != null && Result.Success;
        }

        /// <summary>
        /// Gets the result of the conversion.
        /// This property is null until <see cref="Convert"/> is called.
        /// </summary>
        public OfficeConversionResult Result
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentConversionCandidate"/> class.
        /// </summary>
        /// <param name="fileName">The file name</param>
        public DocumentConversionCandidate(string fileName)
        {
            Validations.ValidateNullEmpty(fileName, "FileConversionCandidate.FileName");
            Info = new FileInfo(fileName);
            versions = DatabaseController.Instance.GetTable<TitleVersionTable>().GetVersionsWithFile(Paths.Title.WithoutRoot(fileName));
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Performs the document conversion.
        /// </summary>
        public void Convert()
        {
            if (!IsConverted)
            {
                Result = OfficeOperation.Instance.ConvertToXmlDocument(Info.FullName);
                if (Result.Success)
                {
                    if (IsVersion)
                    {
                        foreach (var ver in versions)
                        {
                            ver.FileName = Paths.Title.WithoutRoot(Result.ConvertedInfo.FullName);
                        }
                        DatabaseController.Instance.GetTable<TitleVersionTable>().Save();
                    }
                    Info = Result.ConvertedInfo;
                }
            }
        }

        /// <summary>
        /// Signals the property updates that may be bound to the UI.
        /// This method should be called on the UI thread.
        /// </summary>
        public void SignalPropertyUpdates()
        {
            OnPropertyChanged(nameof(Info));
            OnPropertyChanged(nameof(Result));
            OnPropertyChanged(nameof(IsConverted));
        }
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion
    }
}
#endif