/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Tools;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the operation logic for <see cref="View.CommandToolsWindow"/>.
    /// </summary>
    public class StartupToolWindowViewModel : ApplicationViewModel
    {
        #region Private
        private const int VersionIdx = 0;
        private const int SubmissionIdx = 1;
        private const int ExportIdx = 2;
        private const int ListIdx = 3;

        private readonly VersionUpdater versionUpdater;
        private readonly SubmissionUpdater submissionUpdater;
        private readonly TitleExporter titleExporter;
        private readonly TitleLister titleLister;

        private bool isCompleted;
        private int secondsToClose;
        #endregion

        /************************************************************************/

        #region Public properties
        public ObservableCollection<bool> RequestedOps
        {
            get;
        }

        public ObservableCollection<bool> InProgressOps
        {
            get;
        }

        public ObservableCollection<bool> CompletedOps
        {
            get;
        }

        /// <summary>
        /// Gets a boolean value that indicates if all operations are completed.
        /// </summary>
        public bool IsCompleted
        {
            get => isCompleted;
            private set => SetProperty(ref isCompleted, value);
        }

        /// <summary>
        /// Gets the number of seconds until automatic close
        /// </summary>
        public int SecondsToClose
        {
            get => secondsToClose;
            private set => SetProperty(ref secondsToClose, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandToolsWindowViewModel"/> class.
        /// </summary>
        public StartupToolWindowViewModel()
        {
            DisplayName = $"{AppInfo.Title} {AppInfo.VersionMajor} Command Tools";

            RequestedOps = new ObservableCollection<bool>()
            {
                false, false, false, false
            };

            InProgressOps = new ObservableCollection<bool>()
            {
                false, false, false, false
            };

            CompletedOps = new ObservableCollection<bool>()
            {
                false, false, false, false
            };


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

            Application.Current.MainWindow.Closing += MainWindowClosing;
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
                SetInProgress(VersionIdx);
                await versionUpdater.ExecuteAsync();
                SetCompleted(VersionIdx);
                
                SetInProgress(SubmissionIdx);
                await submissionUpdater.ExecuteAsync();
                SetCompleted(SubmissionIdx);
            }

            if (ops.IsExportRequested)
            {
                SetInProgress(ExportIdx);
                await titleExporter.ExecuteAsync();
                SetCompleted(ExportIdx);
            }

            if (ops.IsTitleListRequested)
            {
                SetInProgress(ListIdx);
                await titleLister.ExecuteAsync();
                SetCompleted(ListIdx);
            }

            IsCompleted = true;

            await WaitForCloseAsync();
            Application.Current.MainWindow.Close();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void SetInProgress(int index)
        {
            RequestedOps[index] = true;
            InProgressOps[index] = true;
        }

        private void SetCompleted(int index)
        {
            InProgressOps[index] = false;
            CompletedOps[index] = true;
        }

        private async Task WaitForCloseAsync()
        {
            SecondsToClose = 5;
            for (int k = 1; k <= 5; k++)
            {
                await Task.Delay(1000);
                SecondsToClose--;
            }
        }

        private void MainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !IsCompleted;
        }
        #endregion
    }
}