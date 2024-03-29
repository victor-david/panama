﻿using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using QueueStatusValues = Restless.Panama.Database.Tables.QueueTitleStatusTable.Defs.Values;
using TableColumns = Restless.Panama.Database.Tables.QueueTitleTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic used for title queue management
    /// </summary>
    public class TitleQueueViewModel : DataRowViewModel<QueueTitleTable>
    {
        #region Private
        private QueueTable QueueTable => DatabaseController.Instance.GetTable<QueueTable>();
        private QueueTitleStatusTable QueueTitleStatusTable => DatabaseController.Instance.GetTable<QueueTitleStatusTable>();
        private readonly ObservableCollection<QueueRow> queues;
        private QueueRow selectedQueue;
        private bool queueEditMode;
        private QueueTitleRow selectedTitle;
        private bool isIdleFilterChecked;
        private bool isScheduledFilterChecked;
        private bool isPublishedFilterChecked;
        private bool isFilterSuspended;
        #endregion

        /************************************************************************/

        #region Properties
        public override bool AddCommandEnabled => true;
        public override bool ClearFilterCommandEnabled => Filters.IsAnyFilterActive;
        public override bool DeleteCommandEnabled => SelectedQueue != null && SelectedTitle != null;

        /// <summary>
        /// Gets the queues.
        /// </summary>
        public ListCollectionView Queues { get; }

        /// <summary>
        /// Gets or sets the selected queue.
        /// </summary>
        public QueueRow SelectedQueue
        {
            get => selectedQueue;
            set
            {
                SetProperty(ref selectedQueue, value);
                Config.SelectedQueueId = selectedQueue?.Id ?? 0;
                QueueEditMode = false;
                ListView.Refresh();
            }
        }

        /// <summary>
        /// Gets a boolean value that determines if queue editing mode is enabled.
        /// </summary>
        public bool QueueEditMode
        {
            get => queueEditMode;
            private set => SetProperty(ref queueEditMode, value);
        }

        /// <summary>
        /// Gets the collection of menu items for the queue list.
        /// </summary>
        public MenuItemCollection QueueMenuItems { get; }

        public QueueTitleRow SelectedTitle
        {
            get => selectedTitle;
            private set => SetProperty(ref selectedTitle, value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if the idle filter is checked
        /// </summary>
        public bool IsIdleFilterChecked
        {
            get => isIdleFilterChecked;
            set
            {
                SetProperty(ref isIdleFilterChecked, value);
                if (!isFilterSuspended)
                {
                    Filters?.SetQueueStatus(QueueStatusValues.StatusIdle, isIdleFilterChecked);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the scheduled filter is checked
        /// </summary>
        public bool IsScheduledFilterChecked
        {
            get => isScheduledFilterChecked;
            set
            {
                SetProperty(ref isScheduledFilterChecked, value);
                if (!isFilterSuspended)
                {
                    Filters?.SetQueueStatus(QueueStatusValues.StatusPending, isScheduledFilterChecked);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if the published filter is checked
        /// </summary>
        public bool IsPublishedFilterChecked
        {
            get => isPublishedFilterChecked;
            set
            {
                SetProperty(ref isPublishedFilterChecked, value);
                if (!isFilterSuspended)
                {
                    Filters?.SetQueueStatus(QueueStatusValues.StatusPublished, isPublishedFilterChecked);
                }
            }
        }

        /// <summary>
        /// Gets the filter object
        /// </summary>
        public TitleQueueRowFilter Filters => Config.TitleQueueFilter;

        public DataView TitleStatus => QueueTitleStatusTable.DefaultView;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of <see cref="TitleQueueViewModel"/>
        /// </summary>
        public TitleQueueViewModel()
        {
            Columns.Create("Id", TableColumns.TitleId)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .CanUserSort = false;

            Columns.Create("Title", TableColumns.Joined.Title);

            Columns.Create("Written", TableColumns.Joined.Written).MakeDate();
            Columns.Create("Updated", TableColumns.Joined.Updated)
                .MakeDate()
                .AddToolTip(Strings.TooltipTitleUpdated);

            Columns.Create("WC", TableColumns.Joined.WordCount)
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitleWordCount)
                .SetSelectorName("Word Count");

            Columns.Create("Status", TableColumns.Joined.Status).CanUserSort = false;

            Columns.Create("Date", TableColumns.Date)
                .MakeDate()
                .AddCustomSort(null, TableColumns.Joined.Written, DataGridColumnSortBehavior.AlwaysDescending)
                .MakeInitialSortDescending();

            Columns.RestoreColumnState(Config.QueueTitleGridColumnState);

            Commands.Add("CloseQueueEdit", p => QueueEditMode = false);
            QueueEditMode = false;

            Commands.Add("ClearDate", p => SelectedTitle.ClearDate());

            SyncQueueFilterChecked();

            QueueMenuItems = new MenuItemCollection();

            QueueMenuItems.AddItem(Strings.MenuItemAddQueue, RelayCommand.Create(RunAddQueueCommand))
                .AddIconResource(ResourceKeys.Icon.PlusIconKey);

            QueueMenuItems.AddItem(Strings.MenuItemRenameQueue, RelayCommand.Create(p => QueueEditMode = true, p => SelectedQueue != null));
            QueueMenuItems.AddSeparator();

            QueueMenuItems.AddItem(Strings.MenuItemRemoveQueue, RelayCommand.Create(RunRemoveQueueCommand, p => SelectedQueue != null))
                .AddIconResource(ResourceKeys.Icon.XRedIconKey);

            MenuItems.AddItem(Strings.MenuItemAddTitle, AddCommand)
                .AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddItem(Strings.MenuItemCopyTitle, RelayCommand.Create(RunCopyTitleCommand))
                .AddIconResource(ResourceKeys.Icon.CircleSmallIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemOpenTitleOrDoubleClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemRemoveQueueTitle, DeleteCommand).AddIconResource(ResourceKeys.Icon.XIconKey);

            queues = new ObservableCollection<QueueRow>();
            PopulateQueues();

            Queues = new ListCollectionView(queues);
            using (Queues.DeferRefresh())
            {
                Queues.CustomSort = new GenericComparer<QueueRow>((x, y) => OnQueueDataRowCompare(x, y));
                Queues.IsLiveSorting = true;
                Queues.LiveSortingProperties.Add(nameof(QueueRow.Name));
            }

            SetSelectedQueue(Config.SelectedQueueId);

            ListView.IsLiveSorting = true;
            ListView.LiveSortingProperties.Add(TableColumns.Date);

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                Filters.SetListView(ListView);
                Filters.ApplyFilter();
            }));
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();
            QueueEditMode = false;
        }

        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedTitle = QueueTitleRow.Create(SelectedRow);
            SelectedTitle?.SetDateFormat(Config.DateFormat);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return
                (long)item[TableColumns.QueueId] == (SelectedQueue?.Id ?? 0) &&
                (Filters?.OnDataRowFilter(item) ?? false);
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            if (SelectedQueue != null && WindowFactory.TitleSelect.Create().GetTitles() is List<TitleRow> titles)
            {
                Table.AddTitles(SelectedQueue.Id, titles);
                ListView.Refresh();
            }
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (DeleteCommandEnabled && MessageWindow.ShowContinueCancel(Strings.ConfirmationRemoveQueueTitle))
            {
                SelectedTitle.Row.Delete();
                Table.Save();
                ListView.Refresh();
            }
        }

        /// <inheritdoc/>
        protected override void RunOpenRowCommand()
        {
            if (SelectedTitle != null)
            {
                Database.Tables.TitleVersionController verController = TitleVersionTable.GetVersionController(SelectedTitle.TitleId);
                if (verController.Versions.Count > 0)
                {
                    Open.TitleVersionFile(verController.Versions[0].FileName);
                }
            }
        }

        /// <inheritdoc/>
        protected override void RunClearFilterCommand()
        {
            Filters.ClearAll();
            SyncQueueFilterChecked();
        }

        /// <inheritdoc/>
        protected override void OnSave()
        {
            Config.QueueTitleGridColumnState = Columns.GetColumnState();
        }

        /// <inheritdoc/>
        protected override void OnClosing()
        {
            base.OnClosing();
            SignalSave();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void PopulateQueues()
        {
            queues.Clear();
            foreach (QueueRow row in QueueTable.EnumerateAll())
            {
                queues.Add(row);
            }
        }

        private void SetSelectedQueue(long id)
        {
            foreach (QueueRow queue in queues)
            {
                if (queue.Id == id)
                {
                    SelectedQueue = queue;
                }
            }
        }

        private int OnQueueDataRowCompare(QueueRow item1, QueueRow item2)
        {
            return DataRowCompareString(item1.Row, item2.Row, QueueTable.Defs.Columns.Name);
        }

        private void RunAddQueueCommand(object parm)
        {
            long temp = Config.SelectedQueueId;
            QueueTable.AddDefaultRow();
            PopulateQueues();
            SetSelectedQueue(temp);
            Queues.Refresh();
            MainWindowViewModel.Instance.SynchronizeTitleQueue();
        }

        private void RunRemoveQueueCommand(object parm)
        {
            if (SelectedQueue != null && MessageWindow.ShowContinueCancel(Strings.ConfirmationRemoveQueue))
            {
                QueueTable.RemoveQueue(SelectedQueue.Id);
                PopulateQueues();
                Queues.Refresh();
                MainWindowViewModel.Instance.SynchronizeTitleQueue();
            }
        }

        private void RunCopyTitleCommand(object parm)
        {
            if (SelectedTitle != null)
            {
                try
                {
                    Clipboard.SetText(SelectedTitle.Title);
                }
                catch
                {
                }
            }
        }

        private void SyncQueueFilterChecked()
        {
            isFilterSuspended = true;
            IsIdleFilterChecked = Filters.QueueStatus.Contains(QueueStatusValues.StatusIdle);
            IsScheduledFilterChecked = Filters.QueueStatus.Contains(QueueStatusValues.StatusPending);
            IsPublishedFilterChecked = Filters.QueueStatus.Contains(QueueStatusValues.StatusPublished);
            isFilterSuspended = false;
        }
        #endregion
    }
}