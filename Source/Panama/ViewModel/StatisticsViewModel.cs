using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Threading;
using Restless.Tools.Utility;
using Restless.App.Panama.Tools;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to display application statistcs.
    /// </summary>
    public class StatisticsViewModel : WorkspaceViewModel
    {
        #region Private
        private const int FileHeaderWidth = 216;
        private const int FileValueWidth = 68;
        private bool haveFolderView;
        private bool isFolderViewLoaded;
        private FolderStatisticItem rootStat;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the title statistics object.
        /// </summary>
        public TableStatisticBase Title
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the title version statistics object.
        /// </summary>
        public TitleVersionTableStats Version
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the submission statistics object.
        /// </summary>
        public SubmissionBatchTableStats Submission
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the publisher statistics object.
        /// </summary>
        public PublisherTableStats Publisher
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the folder view that displays folder statistics.
        /// </summary>
        public ObservableCollection<TreeViewItem> FolderView
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if the folder view is available.
        /// Folder view may be unavailable because the title root folder is not set
        /// or points to a non-existent path.
        /// </summary>
        public bool HaveFolderView
        {
            get { return haveFolderView; }
            private set
            {
                SetProperty(ref haveFolderView, value);
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates if the folder view has finished loading.
        /// </summary>
        public bool IsFolderViewLoaded
        {
            get { return isFolderViewLoaded; }
            private set
            {
                SetProperty(ref isFolderViewLoaded, value);
                if (isFolderViewLoaded) CreateTreeViewItems();
                OnPropertyChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsViewModel"/> class.
        /// </summary>
        public StatisticsViewModel()
        {
            DisplayName = Strings.CommandStatistics;
            MaxCreatable = 1;
            Title = new TableStatisticBase(DatabaseController.Instance.GetTable<TitleTable>());
            Version = new TitleVersionTableStats(DatabaseController.Instance.GetTable<TitleVersionTable>());
            Submission = new SubmissionBatchTableStats(DatabaseController.Instance.GetTable<SubmissionBatchTable>());
            Publisher = new PublisherTableStats(DatabaseController.Instance.GetTable<PublisherTable>());
            FolderView = new ObservableCollection<TreeViewItem>();
            IsFolderViewLoaded = false;
            HaveFolderView = (!String.IsNullOrEmpty(Config.FolderTitleRoot) && Directory.Exists(Config.FolderTitleRoot));
            if (HaveFolderView)
            {
                InitFolderView();
            }
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Raises the Closing event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = TaskManager.Instance.WaitForAllRegisteredTasks(() =>
            {
                MainViewModel.CreateNotificationMessage(Strings.NotificationCannotExitTasksAreRunning);
                System.Media.SystemSounds.Beep.Play();

            }, null);
            base.OnClosing(e);
        }
        #endregion

        /************************************************************************/
        
        #region Private Methods
        /// <summary>
        /// Scan folders on a background thread and creates the folder statistics.
        /// </summary>
        private void InitFolderView()
        {
            TaskManager.Instance.ExecuteTask(AppTaskId.FolderStatScan, (token) =>
                {
                    rootStat = new FolderStatisticItem(Config.FolderTitleRoot);
                    rootStat.Populate();
                    TaskManager.Instance.DispatchTask(() => { IsFolderViewLoaded = true; });
                }, null, null, false);
        }

        /// <summary>
        /// When folder statistics are created, creates the TreeViewItem objects. Runs on the UI thread.
        /// </summary>
        private void CreateTreeViewItems()
        {
            TreeViewItem rootItem = new TreeViewItem();
            rootItem.Header = String.Format("Title root: {0}", Config.FolderTitleRoot);
            rootItem.IsExpanded = true;

            // Make a header item that displays the file types
            TreeViewItem headerItem = new TreeViewItem();
            var headerItemDisplay = new GridRowDisplay()
            {
                Columns = 6,
                HeaderWidth = FileHeaderWidth,
                ValueWidth = FileValueWidth,
                ValueFontSize = 12,
                ValueForeground = new SolidColorBrush(Color.FromArgb(255, 204, 0, 0)),
            };
            headerItemDisplay.SetValues("Total", ".docx", ".doc", ".pdf", ".txt", "Other");
            headerItem.Header = headerItemDisplay;
            // add the header item
            rootItem.Items.Add(headerItem);

            // This can create recursive tree items.
            AddTreeViewItem(rootItem, rootStat);

            FolderView.Add(rootItem);
        }

        /// <summary>
        /// Adds the items recursively. 
        /// </summary>
        /// <param name="treeItem"></param>
        /// <param name="statItem"></param>
        private void AddTreeViewItem(TreeViewItem treeItem, FolderStatisticItem statItem)
        {

            foreach (var child in statItem.Children)
            {
                var display = new GridRowDisplay()
                {
                    Columns = 6,
                    Header = child.FolderDisplay,
                    HeaderWidth = FileHeaderWidth - ((child.Depth - 1) * 20),
                    ValueWidth = FileValueWidth,
                };


                display.SetValues(child.Total, child.Docx, child.Doc, child.Pdf, child.Txt, child.Other);
                var item = new TreeViewItem()
                {
                    Header = display
                };
                treeItem.Items.Add(item);
                AddTreeViewItem(item, child);
            }
        }
        #endregion
    }
}