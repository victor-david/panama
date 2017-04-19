#if DOCX
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.Tools.Threading;
using Restless.Tools.Utility;
using Restless.Tools.OfficeAutomation;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Represents a single candidate for the file conversion operation.
    /// </summary>
    public class DocumentConversionCandidate  : NotifyPropertyChangedBase
    {
        #region Private
        private FileInfo info;
        private DataRow versionRow;
        #endregion

        /************************************************************************/
        
        #region Public properties
        /// <summary>
        /// Gets the file info for this conversion candidate.
        /// </summary>
        public FileInfo Info
        {
            get { return info; }
            private set
            {
                info = value;
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates if this candidate belongs to a file version.
        /// </summary>
        public bool IsVersion
        {
            get { return versionRow != null; }
        }

        /// <summary>
        /// Gets a boolean value that indicates if this candidate belongs to a submission document.
        /// </summary>
        public bool IsSubmissionDoc
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the title id associated with the version, if <see cref="IsVersion"/> is true; otherwise, null;
        /// </summary>
        public Int64? TitleId
        {
            get
            {
                if (versionRow != null)
                {
                    return (Int64)versionRow[TitleVersionTable.Defs.Columns.TitleId];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates if this item has been converted.
        /// </summary>
        public bool IsConverted
        {
            get
            {
                return Result != null && Result.Success;
            }
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
            versionRow = DatabaseController.Instance.GetTable<TitleVersionTable>().GetVersionWithFile(Paths.Title.WithoutRoot(fileName));
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
                        versionRow[TitleVersionTable.Defs.Columns.FileName] = Paths.Title.WithoutRoot(Result.ConvertedInfo.FullName);
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
            OnPropertyChanged("Info");
            OnPropertyChanged("Result");
            OnPropertyChanged("IsConverted");
        }
        #endregion

        /************************************************************************/
        
        #region Private methods
        #endregion
    }
}
#endif