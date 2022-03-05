/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Core.Utility;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the submission message sync tool.
    /// </summary>
    public class ToolMessageSyncViewModel : ApplicationViewModel
    {
        #region Private
        private bool inProgress;
        private int totalCount;
        private int totalScanCount;
        private int messageScanCount;
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
        /// Gets the total count.
        /// </summary>
        public int TotalCount
        {
            get => totalCount;
            private set => SetProperty(ref totalCount, value);
        }

        /// <summary>
        /// Gets the total count of submission message rows scanned.
        /// </summary>
        public int TotalScanCount
        {
            get => totalScanCount;
            private set => SetProperty(ref totalScanCount, value);
        }

        /// <summary>
        /// Gets the message scan count, i.e. entries that are for an external .eml file
        /// </summary>
        public int MessageScanCount
        {
            get => messageScanCount;
            private set => SetProperty(ref messageScanCount, value);
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
            Commands.Add("Begin", PerformSync);
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
            TotalCount = table.Rows.Count;

            // TODO
            //TaskManager.Instance.ExecuteTask(1, (token) =>
            //{
            //    foreach (DataRow row in table.Rows)
            //    {
            //        TaskManager.Instance.DispatchTask(() => TotalScanCount++);
            //        string protocol = row[SubmissionMessageTable.Defs.Columns.Protocol].ToString();
            //        string entryId = row[SubmissionMessageTable.Defs.Columns.EntryId].ToString();
            //        if (protocol == SubmissionMessageTable.Defs.Values.Protocol.FileSystem)
            //        {
            //            TaskManager.Instance.DispatchTask(() => MessageScanCount++);

            //            string fileName = Path.Combine(Config.FolderSubmissionMessage, entryId);
            //            var msg = new MimeKitMessage(fileName);
            //            if (!msg.IsError)
            //            {
            //                // set the subject. Normally it's already okay, but there's some entries where
            //                // it came from an Outlook extraction and the subject was edited.
            //                row[SubmissionMessageTable.Defs.Columns.Subject] = msg.Subject;

            //                string cleanSubject = GetCleanStr(msg.Subject);
            //                string cleanSender = GetCleanStr(msg.FromName, msg.FromName == msg.FromEmail);

            //                // MessageDate_Subject.ext
            //                // 2018-10-22_17.21.09_Subject.eml
            //                string newFileName =
            //                    string.Format("{0}_{1}_{2}{3}",
            //                        msg.MessageDateUtc.ToString("yyyy-MM-dd_HH.mm.ss"),
            //                        Format.ValidFileName(cleanSender),
            //                        Format.ValidFileName(cleanSubject),
            //                        Path.GetExtension(fileName));

            //                if (entryId != newFileName)
            //                {
            //                    AppendOutput($"Update {entryId} to {newFileName}");
            //                    string newFileNameFull = Path.Combine(Config.FolderSubmissionMessage, newFileName);
            //                    try
            //                    {
            //                        File.Move(fileName, newFileNameFull);
            //                        row[SubmissionMessageTable.Defs.Columns.EntryId] = newFileName;
            //                        TaskManager.Instance.DispatchTask(() => ProcessCount++);
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        AppendOutput(ex.Message);
            //                        TaskManager.Instance.DispatchTask(() => ErrorCount++);
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                AppendOutput(msg.ParseException.Message);
            //                TaskManager.Instance.DispatchTask(() => ErrorCount++);
            //            }
            //        }
            //    }
            //    table.Save();
            //    TaskManager.Instance.DispatchTask(() => InProgress = false);

            //}, null, null, false);
        }

        private void ResetCounters()
        {
            TotalScanCount = MessageScanCount = ProcessCount = ErrorCount = 0;
            // TotalCount is set once the start button is clicked.
            // Before that, if it's set to zero (same as minimum), the progress bar displays as indeterminate.
            // Setting it to non-zero prevents that.
            TotalCount = 10;
            Output = string.Empty;
        }

        private void AppendOutput(string str)
        {
            // TODO
            //TaskManager.Instance.DispatchTask(() => Output += str + Environment.NewLine);
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