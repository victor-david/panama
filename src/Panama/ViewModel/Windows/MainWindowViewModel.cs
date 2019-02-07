/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Database;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// The ViewModel for the application's main window.
    /// </summary>
    public class MainWindowViewModel : WindowViewModel
    {
        #region Private
        private string notificationMessage;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Returns the collection of available workspaces to display.
        /// A 'workspace' is a ViewModel that can request to be closed.
        /// </summary>
        public ObservableCollection<ApplicationViewModel> Workspaces
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the application information object.
        /// </summary>
        public ApplicationInfo AppInfo
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a notification message
        /// </summary>
        public string NotificationMessage
        {
            get
            {
                return notificationMessage;
            }
            private set
            {
                SetProperty(ref notificationMessage, value);
                System.Media.SystemSounds.Asterisk.Play();
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        /// <param name="owner">The window object that wons this VM.</param>
        public MainWindowViewModel(Window owner)
            :base(owner)
        {
            Owner.Closing += new CancelEventHandler(MainWindowClosing);
            Commands.Add("About", OpenAbout);
            Commands.Add("Author", (o) => CreateIf<AuthorViewModel>());
            Commands.Add("Close", CloseMainWindow);
            Commands.Add("CloseAll", CloseAllWorkspaces, CloseAllWorkspacesCanExecute);
            Commands.Add("Config", (o) => CreateIf<ConfigViewModel>());
            Commands.Add("Credential", (o) => CreateIf<CredentialViewModel>());
            Commands.Add("Alert", (o) => CreateIf<AlertViewModel>());
            Commands.Add("Link", (o) => CreateIf<LinkViewModel>());
            Commands.Add("Note", (o) => CreateIf<UserNoteViewModel>());
            Commands.Add("Publisher", (o) => CreateIf<PublisherViewModel>());
            Commands.Add("Reference", (o) => OpenHelper.OpenFile(AppInfo.ReferenceFileName));
            Commands.Add("ResetWindow", RunResetWindowCommand);
            Commands.Add("Submission", (o) => CreateIf<SubmissionViewModel>());
            Commands.Add("Save", Save);
            Commands.Add("Statistics", (o) => CreateIf<StatisticsViewModel>());
            Commands.Add("Table", (o) => CreateIf<TableViewModel>());
            Commands.Add("Tag", (o) => CreateIf<TagViewModel>());
            Commands.Add("Title", (o) => CreateIf<TitleViewModel>());
            Commands.Add("ToolConvert", (o) => CreateIf<ToolConvertViewModel>(), CanRunToolConvertCommand);
            Commands.Add("ToolExport", (o) => CreateIf<ToolExportViewModel>());
            Commands.Add("ToolMessageSync", (o) => CreateIf<ToolMessageSyncViewModel>());
            Commands.Add("ToolMeta", (o) => CreateIf<ToolMetaUpdateViewModel>());
            Commands.Add("ToolOrphan", (o) => CreateIf<ToolOrphanViewModel>());
            Commands.Add("ToolScramble", (o) => CreateIf<ToolScrambleViewModel>());
            Commands.Add("ToolSearch", (o) => CreateIf<ToolSearchViewModel>());
            Commands.Add("ToolTitleList", (o) => CreateIf<ToolTitleListViewModel>());

            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandTitle, Strings.CommandTitleTooltip, Commands["Title"], ResourceHelper.Get("ImageTitle")));
            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandPublisher, Strings.CommandPublisherTooltip, Commands["Publisher"], ResourceHelper.Get("ImagePublisher")));
            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandSubmission, Strings.CommandSubmissionTooltip, Commands["Submission"], ResourceHelper.Get("ImageSubmission")));
                
            Workspaces = new ObservableCollection<ApplicationViewModel>();
            Workspaces.CollectionChanged += OnWorkspacesChanged;
            AppInfo = ApplicationInfo.Instance;
            // 
            DisplayName = string.Format("{0} {1}", AppInfo.Assembly.Title, AppInfo.Assembly.VersionMajor);
#if DEBUG
            DisplayName = string.Format("{0} {1} (DEBUG)", AppInfo.Assembly.Title, AppInfo.Assembly.VersionMajor);
#endif
        }
        #endregion

        /************************************************************************/
        
        #region Public methods
        /// <summary>
        /// Creates a notification message that displays on the main status bar
        /// </summary>
        /// <param name="message">The message</param>
        public void CreateNotificationMessage(string message)
        {
            NotificationMessage = message;
        }
        /// <summary>
        /// Notifies the workspace that a new record has been added that affects it
        /// </summary>
        /// <typeparam name="T">The type of workspace to notify</typeparam>
        public void NotifyWorkspaceOnRecordAdded<T>() where T : ApplicationViewModel
        {
            var ws = GetFirstWorkspace<T>();
            if (ws != null)
            {
                ws.OnRecordAdded();
            }
        }

        /// <summary>
        /// Creates if necessary and switches to the workspace specified by its type.
        /// </summary>
        /// <typeparam name="T">The type of workspace to switch to.</typeparam>
        /// <returns>The workspace</returns>
        public T SwitchToWorkspace<T>() where T : ApplicationViewModel
        {
            // The reason we don't use CreateIf<T>() directly (which would work)
            // is to avoid the possibility that a VM allows more than one instance.
            // We want to go the first one. If there isn't a first one, we'll create.
             
            var ws = GetFirstWorkspace<T>();
            if (ws != null)
            {
                SetActiveWorkspace(ws);
            }
            else
            {
                Create<T>();
            }

            ICollectionView collectionView = CollectionViewSource.GetDefaultView(Workspaces);
            if (collectionView != null)
            {
                return collectionView.CurrentItem as T;
            }
            return null;

        }
        #endregion

        /************************************************************************/

        #region Private methods (VM creation / management)

        private void Create<T>() where T : ApplicationViewModel
        {
            Execution.TryCatch(() =>
                {
                    ApplicationViewModel vm = (T)Activator.CreateInstance(typeof(T), this);
                    vm.MainViewModel = this;
                    Workspaces.Add(vm);
                    SetActiveWorkspace(vm);
                });
        }

        private void CreateIf<T>() where T : ApplicationViewModel
        {
            if (CanCreate<T>())
            {
                Create<T>();
            }
            else
            {
                ICollectionView collectionView = CollectionViewSource.GetDefaultView(Workspaces);
                if (!(collectionView.CurrentItem is T))
                {
                    var vm = Workspaces.OfType<T>().First();
                    SetActiveWorkspace(vm);
                }
            }
        }

        private bool CanCreate<T>() where T : ApplicationViewModel
        {
            var it = Workspaces.OfType<T>();
            int count = it.Count();
            if (count > 0)
            {
                T viewModel = it.First();
                return viewModel.MaxCreatable == -1 || count < viewModel.MaxCreatable;
            }
            return true;
        }

        /// <summary>
        /// Gets the first workspace of type T
        /// </summary>
        /// <typeparam name="T">The workspace type</typeparam>
        /// <returns>The workspace, or null if there isn't one.</returns>
        private T GetFirstWorkspace<T>() where T : ApplicationViewModel
        {
            var it = Workspaces.OfType<T>();
            int count = it.Count();
            if (count > 0)
            {
                return it.First();
            }
            return null;
        }

        private void SetActiveWorkspace(ApplicationViewModel workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(Workspaces);
            if (collectionView != null)
            {
                collectionView.MoveCurrentTo(workspace);
            }
        }

        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var workspace in e.NewItems.OfType<ApplicationViewModel>())
                {
                    workspace.Closing += OnWorkspaceClosing;
                }
            }

            if (e.OldItems != null)
            {
                foreach (var workspace in e.OldItems.OfType<ApplicationViewModel>())
                {
                    workspace.Closing -= OnWorkspaceClosing;
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (other)

        private void OnWorkspaceClosing(object sender, CancelEventArgs e)
        {
            if (!e.Cancel && sender is ApplicationViewModel workspace)
            {
                workspace.Dispose();
                Workspaces.Remove(workspace);
            }
        }

        private bool CloseAllWorkspacesCanExecute(object o)
        {
            return Workspaces.Count > 0;
        }

        private void CloseAllWorkspaces(object o)
        {
            List<ApplicationViewModel> temp = new List<ApplicationViewModel>(Workspaces);
            foreach (ApplicationViewModel workspace in temp)
            {
                workspace.CloseCommand.Execute(null);
            }
        }

        private void OpenAbout(object o)
        {
            var about = WindowFactory.About.Create();
            about.ShowDialog();
        }

        private void CloseMainWindow(object o)
        {
            Owner.Close();
        }

        private void Save(object o)
        {
            Config.Instance.SaveFilterObjects();
            DatabaseController.Instance.Save();
            NotificationMessage = "All data successfully saved to the database";
        }

        private void MainWindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = TaskManager.Instance.WaitForAllRegisteredTasks(() =>
                {
                    NotificationMessage = Strings.NotificationCannotExitTasksAreRunning;
                    System.Media.SystemSounds.Beep.Play();
                }, null);

            if (!e.Cancel)
            {

                Config.Instance.MainWindowWidth = (int)Owner.Width;
                Config.Instance.MainWindowHeight = (int)Owner.Height;
                if (Owner.WindowState != WindowState.Minimized)
                {
                    Config.Instance.MainWindowState = Owner.WindowState;
                }
            }
        }

        private void RunResetWindowCommand(object o)
        {
            Owner.Width = Config.Default.MainWindow.Width;
            Owner.Height = Config.Default.MainWindow.Height;
            Owner.Top = (SystemParameters.WorkArea.Height / 2) - (Owner.Height / 2);
            Owner.Left = (SystemParameters.WorkArea.Width / 2) - (Owner.Width / 2);
            Owner.WindowState = WindowState.Normal;
        }

        private bool CanRunToolConvertCommand(object o)
        {
#if DOCX
            return true;
#else
            return false;
#endif
        }
        #endregion
    }
}