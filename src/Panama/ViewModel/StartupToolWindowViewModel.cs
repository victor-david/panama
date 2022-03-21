/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Tools;
using System;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the operation logic for <see cref="View.CommandToolsWindow"/>.
    /// </summary>
    public class StartupToolWindowViewModel : ApplicationViewModel
    {
        #region Private
        private readonly VersionUpdater versionUpdater;
        private readonly SubmissionUpdater submissionUpdater;
        private readonly TitleExporter titleExporter;
        private readonly TitleLister titleLister;
        private readonly StartupOptions ops;
        private bool opTitleMetadata;
        private bool opSubmissionMetadata;
        private bool opExport;
        private bool opTitleList;
        private bool isCompleted;
        #endregion

        /************************************************************************/

        #region Public properties
        public bool OpTitleMetadata
        {
            get => opTitleMetadata;
            private set => SetProperty(ref opTitleMetadata, value);
        }

        public bool OpSubmissionMetadata
        {
            get => opSubmissionMetadata;
            private set => SetProperty(ref opSubmissionMetadata, value);
        }

        public bool OpExport
        {
            get => opExport;
            private set => SetProperty(ref opExport, value);
        }

        public bool OpTitleList
        {
            get => opTitleList;
            private set => SetProperty(ref opTitleList, value);
        }

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
            get => isCompleted;
            private set => SetProperty(ref isCompleted, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandToolsWindowViewModel"/> class.
        /// </summary>
        public StartupToolWindowViewModel()
        {
            DisplayName = $"{ApplicationInfo.Instance.Assembly.Title} {ApplicationInfo.Instance.Assembly.VersionMajor} Command Tools";
            Status = string.Empty;
            AddToStatus("Performing requested operations", true);
            IsCompleted = false;
            versionUpdater = new();
            submissionUpdater = new();
            titleExporter = new()
            {
                OutputDirectory = Config.FolderExport
            };

            titleLister = new()
            {
                OutputDirectory = Config.FolderTitleRoot
            };


        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Performs the specified operations.
        /// </summary>
        public async void PerformOperations(StartupOptions ops)
        {
            if (ops.IsUpdateRequested)
            {
                OpTitleMetadata = true;
                await versionUpdater.ExecuteAsync();

                OpSubmissionMetadata = true;
                await submissionUpdater.ExecuteAsync();
            }

            if (ops.IsExportRequested)
            {
                OpExport = true;
                await titleExporter.ExecuteAsync();
            }

            if (ops.IsTitleListRequested)
            {
                OpTitleList = true;
                await titleLister.ExecuteAsync();
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void AddToStatus(string text, bool newLine)
        {
            Status += text;
            if (newLine)
            {
                Status += Environment.NewLine;
            }

            OnPropertyChanged(nameof(Status));
        }
        #endregion
    }
}