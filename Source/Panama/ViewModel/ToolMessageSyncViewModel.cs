using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the submission message sync tool.
    /// </summary>
    public class ToolMessageSyncViewModel : WorkspaceViewModel
    {
        #region Private
        private bool inProgress;
        private int scanCount;
        private int processCount;
        private int errorCount;
        private string output;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets a boolean value that indicates if the operation is in progress.
        /// </summary>
        public bool InProgress
        {
            get => inProgress;
            private set => SetProperty(ref inProgress, value);
        }
        /// <summary>
        /// Gets the scan count.
        /// </summary>
        public int ScanCount
        {
            get => scanCount;
            private set => SetProperty(ref scanCount, value);
        }

        /// <summary>
        /// Gets the process count.
        /// </summary>
        public int ProcessCount
        {
            get => processCount;
            private set => SetProperty(ref processCount, value);
        }

        /// <summary>
        /// Gets the error count.
        /// </summary>
        public int ErrorCount
        {
            get => errorCount;
            private set => SetProperty(ref errorCount, value);
        }

        /// <summary>
        /// Gets the output string
        /// </summary>
        public string Output
        {
            get => output;
            private set => SetProperty(ref output, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolMessageSyncViewModel"/> class.
        /// </summary>
        public ToolMessageSyncViewModel()
        {
            DisplayName = Strings.CommandToolMessageSync;
            MaxCreatable = 1;
            Commands.Add("Begin", PerformSync, (o) => !InProgress);
            ResetCounters();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected Methods
        #endregion

        /************************************************************************/
        
        #region Private Methods
        private void PerformSync(object o)
        {
            InProgress = true;
            ResetCounters();

            var table = DatabaseController.Instance.GetTable<SubmissionMessageTable>();
            foreach (DataRow row in table.Rows)
            {
                string protocol = row[SubmissionMessageTable.Defs.Columns.Protocol].ToString();
                string entryId = row[SubmissionMessageTable.Defs.Columns.EntryId].ToString();
                if (protocol == SubmissionMessageTable.Defs.Values.Protocol.FileSystem)
                {
                    ScanCount++;
                    string fileName = Path.Combine(Config.FolderSubmissionMessage, entryId);
                    var msg = new MimeKitMessage(fileName);
                    if (!msg.IsError)
                    {
                        // set the subject. Normally it's already okay, but there's some entries where 
                        // it came from an Outlook extraction and the subject was edited.
                        row[SubmissionMessageTable.Defs.Columns.Subject] = msg.Subject;

                        string cleanSubject = GetCleanStr(msg.Subject);
                        string cleanSender = GetCleanStr(msg.FromName, msg.FromName == msg.FromEmail);

                        // MessageDate_Subject.ext
                        // 2018-10-22_17.21.09_Subject.eml
                        string newFileName =
                            string.Format("{0}_{1}_{2}{3}",
                                msg.MessageDateUtc.ToString("yyyy-MM-dd_HH.mm.ss"),
                                Format.ValidFileName(cleanSender),
                                Format.ValidFileName(cleanSubject),
                                Path.GetExtension(fileName));

                        if (entryId != newFileName)
                        {
                            AppendOutput($"Update {entryId} to {newFileName}");
                            string newFileNameFull = Path.Combine(Config.FolderSubmissionMessage, newFileName);
                            try
                            {
                                File.Move(fileName, newFileNameFull);
                                row[SubmissionMessageTable.Defs.Columns.EntryId] = newFileName;
                                ProcessCount++;
                            }
                            catch (Exception ex)
                            {
                                AppendOutput(ex.Message);
                                ErrorCount++;
                            }
                        }
                    }
                    else
                    {
                        AppendOutput(msg.ParseException.Message);
                        ErrorCount++;
                    }
                }
            }
            table.Save();
            InProgress = false;
        }

        private void ResetCounters()
        {
            ScanCount = ProcessCount = ErrorCount = 0;
            Output = string.Empty;
        }

        private void AppendOutput(string str)
        {
            Output += str + Environment.NewLine;
        }

        private string GetCleanStr(string str, bool allowDot = false)
        {
            str = str.Replace(":", "-").Replace(" ", "").Replace(",", "").Replace("'", "");
            if (!allowDot) str = str.Replace(".", "");
            return Format.ValidFileName(str);
        }
        #endregion
    }
}