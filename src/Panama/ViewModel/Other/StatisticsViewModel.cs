/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Controls;
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Tools;
using Restless.Toolkit.Core.Database.SQLite;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to display application statistcs.
    /// </summary>
    public class StatisticsViewModel : ApplicationViewModel
    {
        #region Private
        private const int HeaderColumnWidth = 216;
        private const int ValueColumnWidth = 68;
        private const int MagicFactor = 20;
        private bool haveTitleRoot;
        private bool isFolderOperationInProgress;
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
        }

        /// <summary>
        /// Gets the title version statistics object.
        /// </summary>
        public TitleVersionTableStats Version
        {
            get;
        }

        /// <summary>
        /// Gets the submission statistics object.
        /// </summary>
        public SubmissionBatchTableStats Submission
        {
            get;
        }

        /// <summary>
        /// Gets the publisher statistics object.
        /// </summary>
        public PublisherTableStats Publisher
        {
            get;
        }

        /// <summary>
        /// Gets the folder view that displays folder statistics.
        /// </summary>
        public ObservableCollection<TreeViewItem> FolderView
        {
            get;
        }

        /// <summary>
        /// Gets a boolean value that indicates if the title root folder 
        /// is set and points to a valid path.
        /// </summary>
        public bool HaveTitleRoot
        {
            get => haveTitleRoot;
            private set => SetProperty(ref haveTitleRoot, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if the folder operation is in progress
        /// </summary>
        public bool IsFolderOperationInProgress
        {
            get => isFolderOperationInProgress;
            private set => SetProperty(ref isFolderOperationInProgress, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="StatisticsViewModel"/> class.
        /// </summary>
        public StatisticsViewModel()
        {
            Title = new TableStatisticBase(DatabaseController.Instance.GetTable<TitleTable>());
            Version = new TitleVersionTableStats(DatabaseController.Instance.GetTable<TitleVersionTable>());
            Submission = new SubmissionBatchTableStats(DatabaseController.Instance.GetTable<SubmissionBatchTable>());
            Publisher = new PublisherTableStats(DatabaseController.Instance.GetTable<PublisherTable>());
            FolderView = new ObservableCollection<TreeViewItem>();
            HaveTitleRoot = !string.IsNullOrEmpty(Config.FolderTitleRoot) && Directory.Exists(Config.FolderTitleRoot);
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <inheritdoc/>
        protected override void OnActivated()
        {
            if (HaveTitleRoot)
            {
                InitFolderView();
            }
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        /// <summary>
        /// Scan folders on a background thread and creates the folder statistics.
        /// </summary>
        private async void InitFolderView()
        {
            IsFolderOperationInProgress = true;
            FolderView.Clear();
            rootStat = new FolderStatisticItem(Config.FolderTitleRoot);
            await rootStat.PopulateAsync();
            CreateTreeViewItems();
            IsFolderOperationInProgress = false;
        }

        /// <summary>
        /// When folder statistics are created, creates the TreeViewItem objects. Runs on the UI thread.
        /// </summary>
        private void CreateTreeViewItems()
        {
            DisplayGrid rootDisplay = new()
            {
                HeaderFontSize = 14,
                HeaderForeground = Brushes.DimGray,
                Header = $"Title root: {Config.FolderTitleRoot}",
                HeaderColumnWidth = HeaderColumnWidth + MagicFactor - 1,
                ValueColumnWidth = ValueColumnWidth,
                ValueFontSize = 13,
                ValueForeground = Brushes.MediumBlue
            };

            rootDisplay.SetValues("Total", ".docx", ".doc", ".pdf", ".txt", "Other");

            TreeViewItem rootTreeItem = new()
            {
                Header = rootDisplay,
                IsExpanded = true
            };

            /* recursively adds tree items */
            AddTreeViewItem(rootTreeItem, rootStat);
            FolderView.Add(rootTreeItem);
        }

        /// <summary>
        /// Adds the items recursively.
        /// </summary>
        /// <param name="treeItem"></param>
        /// <param name="statItem"></param>
        private void AddTreeViewItem(TreeViewItem treeItem, FolderStatisticItem statItem)
        {
            foreach (FolderStatisticItem child in statItem.Children)
            {
                DisplayGrid display = new()
                {
                    Header = child.FolderDisplay,
                    HeaderColumnWidth = HeaderColumnWidth - ((child.Depth - 1) * MagicFactor),
                    ValueColumnWidth = ValueColumnWidth
                };

                display.SetValues(child.Total, child.Docx, child.Doc, child.Pdf, child.Txt, child.Other);
                TreeViewItem item = new()
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