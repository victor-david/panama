using Restless.App.Panama.Configuration;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Database;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
        private TabItem tornItem;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Returns the collection of available workspaces to display.
        /// A 'workspace' is a ViewModel that can request to be closed.
        /// </summary>
        public ObservableCollection<WorkspaceViewModel> Workspaces
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

        /// <summary>
        /// Gets or sets the torm item.
        /// </summary>
        public TabItem TornItem
        {
            get { return tornItem; }
            set
            {
                tornItem = value;
                TearWorkspace(tornItem);
                
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
                
            Workspaces = new ObservableCollection<WorkspaceViewModel>();
            Workspaces.CollectionChanged += OnWorkspacesChanged;
            AppInfo = ApplicationInfo.Instance;
            DisplayName = TabDisplayName = string.Format("{0} {1}", AppInfo.Assembly.Title, AppInfo.Assembly.VersionMajor);
#if DEBUG
            DisplayName = TabDisplayName = string.Format("{0} {1} (DEBUG)", AppInfo.Assembly.Title, AppInfo.Assembly.VersionMajor);
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
        public void NotifyWorkspaceOnRecordAdded<T>() where T : WorkspaceViewModel, new()
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
        public T SwitchToWorkspace<T>() where T : WorkspaceViewModel, new()
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

        #region Private Helpers

        private void OnWorkspacesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.NewItems)
                    workspace.Closing += OnWorkspaceClosing;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (WorkspaceViewModel workspace in e.OldItems)
                    workspace.Closing -= OnWorkspaceClosing;
        }

        private void OnWorkspaceClosing(object sender, CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                WorkspaceViewModel workspace = sender as WorkspaceViewModel;
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
            List<WorkspaceViewModel> temp = new List<WorkspaceViewModel>(Workspaces);
            foreach (WorkspaceViewModel workspace in temp)
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

        private void CreateIf<T>() where T : WorkspaceViewModel, new()
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
                    var t = Workspaces.OfType<T>().First<T>();
                    SetActiveWorkspace(t);
                }
            }
        }

        private void SetActiveWorkspace(WorkspaceViewModel workspace)
        {
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(Workspaces);
            if (collectionView != null)
            {
                collectionView.MoveCurrentTo(workspace);
            }
        }

        /// <summary>
        /// This is no longer used. TabControlExtended can detect ItemsSource as an ObservableCollection and move the items.
        /// If needed (for instance, for additional functionality), you can bind to the TabControlExtended.ReorderTabsCommand instead.
        /// That takes priority and the command execution passes an instance of TabItemDragDrop.
        /// </summary>
        /// <param name="dropItems">The drag / drop items.</param>
        private void MoveWorkspaces(TabItemDragDrop dropItems)
        {
            if (dropItems != null)
            {
                var source = dropItems.Source.Content as WorkspaceViewModel;
                var target = dropItems.Target.Content as WorkspaceViewModel;
                if (source != null && target != null)
                {
                    int sourceIdx = Workspaces.IndexOf(source);
                    int targetIdx = Workspaces.IndexOf(target);
                    if (sourceIdx != -1 && targetIdx != -1)
                    {
                        Workspaces.Move(sourceIdx, targetIdx);
                    }
                }
            }
        }

        private void TearWorkspace(TabItem tornItem)
        {
            if (tornItem != null)
            {
                var workspace = tornItem.Content as WorkspaceViewModel;
                if (workspace != null)
                {
                    var w = WindowFactory.Workspace.Create(workspace.TabDisplayName, workspace);
                    w.Show();
                    workspace.CloseCommand.Execute(null);
                }
            }
        }

        private bool CanCreate<T>() where T : WorkspaceViewModel
        {
            var it = Workspaces.OfType<T>();
            int count = it.Count<T>();
            if (count > 0)
            {
                T viewModel = it.First<T>();
                return (viewModel.MaxCreatable == -1 || count < viewModel.MaxCreatable);
            }
            return true;
        }

        private void Create<T>() where T : WorkspaceViewModel, new()
        {
            Execution.TryCatch(() =>
                {
                    //object dc = this.da
                    T workspace = new T();
                    workspace.MainViewModel = this;
                    Workspaces.Add(workspace);
                    SetActiveWorkspace(workspace);
                });
        }

        /// <summary>
        /// Gets the first workspace of type T
        /// </summary>
        /// <typeparam name="T">The workspace type</typeparam>
        /// <returns>The workspace, or null if there isn't one.</returns>
        private T GetFirstWorkspace<T>() where T : WorkspaceViewModel
        {
            var it = Workspaces.OfType<T>();
            int count = it.Count<T>();
            if (count > 0)
            {
                return it.First<T>();
            }
            return null;
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