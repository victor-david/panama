/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Core;
using Restless.App.Panama.Tools;
using System;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the operation logic for <see cref="View.CommandToolsWindow"/>.
    /// </summary>
    public class CommandToolsWindowViewModel : WindowViewModel
    {
        #region Private
        private readonly StartupOptions ops;
        private bool isCompleted;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a string that describes the status of the operations.
        /// </summary>
        public string Status
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if all operations are completed.
        /// </summary>
        public bool IsCompleted
        {
            get { return isCompleted; }
            private set
            {
                SetProperty(ref isCompleted, value);
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandToolsWindowViewModel"/> class.
        /// </summary>
        /// <param name="ops">The starup options that describe which operations to perform.</param>
        public CommandToolsWindowViewModel(StartupOptions ops)
        {
            this.ops = ops;
            DisplayName = string.Format("{0} {1} Command Tools", ApplicationInfo.Instance.Assembly.Title, ApplicationInfo.Instance.Assembly.VersionMajor);
            Status = string.Empty;
            AddToStatus("Performing requested operations", true);
            IsCompleted = false;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Performs the operations that were specified upon startup.
        /// </summary>
        public void PerformOperations()
        {
            // TODO
            //IsCompleted = false;
            //TaskManager.Instance.ExecuteTask(AppTaskId.CommandTools, (token) =>
            //    {
            //        if (ops.IsUpdateRequested)
            //        {
            //            AddToStatus("  Updating title version meta data...", false);
            //            var versionUpdater = new VersionUpdater();

            //            versionUpdater.Execute();
            //            AddToStatus("..done", true);

            //            AddToStatus("  Updating submission document meta data...", false);
            //            var submissionUpdater = new SubmissionUpdater();
            //            submissionUpdater.Execute();
            //            AddToStatus("..done", true);
            //        }

            //        if (ops.IsExportRequested)
            //        {
            //            AddToStatus("  Exporting title version documents...", false);
            //            var titleExporter = new TitleExporter(Config.FolderExport);
            //            titleExporter.Execute();
            //            AddToStatus("..done", true);
            //        }

            //        if (ops.IsTitleListRequested)
            //        {
            //            AddToStatus("  Creating title list... ", false);
            //            var titleLister = new TitleLister(Config.FolderTitleRoot);
            //            titleLister.Execute();
            //            AddToStatus("done", true);
            //            OpenHelper.OpenFile(titleLister.TitleListFileName);
            //        }
            //        IsCompleted = true;
            //    }, null, null, false);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void AddToStatus(string text, bool newLine)
        {
            Status += text;
            if (newLine) Status += Environment.NewLine;
            OnPropertyChanged(nameof(Status));
        }
        #endregion
    }
}