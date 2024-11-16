using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core;
using Restless.Toolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
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
        private bool isQueueEditActive;
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
                IsQueueEditActive = false;
                ListView.Refresh();
            }
        }

        /// <summary>
        /// Gets a boolean value that determines if queue editing mode is enabled.
        /// </summary>
        public bool IsQueueEditActive
        {
            get => isQueueEditActive;
            set => SetProperty(ref isQueueEditActive, value);
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

        public ICommand ClearDateCommand { get; }
        public ICommand CloseQueueEditCommand { get; }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of <see cref="TitleQueueViewModel"/>
        /// </summary>
        public TitleQueueViewModel()
        {
            InitColumns();
            //InitColumnComparerMap();

            IsQueueEditActive = false;
            ClearDateCommand = RelayCommand.Create(p => RunClearDateCommand());
            CloseQueueEditCommand = RelayCommand.Create(p => IsQueueEditActive = false);

            SyncQueueFilterChecked();

            QueueMenuItems = new MenuItemCollection();
            InitQueueMenuItems();
            InitMenuItems();

            queues = new ObservableCollection<QueueRow>();
            PopulateQueues();

            Queues = new ListCollectionView(queues);
            using (Queues.DeferRefresh())
            {
                Queues.CustomSort = new GenericComparer<QueueRow>(OnQueueDataRowCompare);
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
            IsQueueEditActive = false;
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
            if (DeleteCommandEnabled && MessageWindow.ShowContinueCancel(Confirm.RemoveQueueTitle))
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
        private void InitColumns()
        {
            Columns.Create(Header.Id, TableColumns.TitleId)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .CanUserSort = false;

            Columns.Create(Header.Title, TableColumns.Joined.Title);

            Columns.Create(Header.Written, TableColumns.Joined.Written).MakeDate();
            Columns.Create(Header.Updated, TableColumns.Joined.Updated)
                .MakeDate()
                .AddToolTip(ToolTip.TitleUpdated);

            Columns.Create(Header.WordCountShort, TableColumns.Joined.WordCount)
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(ToolTip.TitleWordCount)
                .MakeNonSortable()
                .SetSelectorName(Header.WordCount);

            Columns.Create(Header.Status, TableColumns.Joined.Status).CanUserSort = false;

            Columns.Create(Header.Date, TableColumns.Date)
                .MakeDate()
                .SetPrimarySort(SortType.NullableDateTime)
                .SetSecondarySort(TableColumns.Joined.Written, SortType.DateTime, false)
                .MakeInitialSortDescending();

            Columns.RestoreColumnState(Config.QueueTitleGridColumnState);
        }

        //private void InitColumnComparerMap()
        //{
        //    ColumnComparerMap.Add(TableColumns.Date, (item1, item2, asc) =>
        //        SortHelper.CompareNullableDateTime(item1, item2, TableColumns.Date, asc)
        //        .ThenDateTime(item1, item2, TableColumns.Joined.Written, false));
        //}

        private void InitMenuItems()
        {
            MenuItems.AddItem(Menu.AddTitle, AddCommand)
                .AddIconResource(ResourceKeys.Icon.IconAdd);

            MenuItems.AddItem(Menu.CopyTitle, RelayCommand.Create(p => RunCopyTitleCommand()))
                .AddIconResource(ResourceKeys.Icon.IconCopy);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Menu.OpenTitleOrDoubleClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.IconOpenWebSite);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Menu.RemoveQueueTitle, DeleteCommand).AddIconResource(ResourceKeys.Icon.IconTrayRemove);
        }

        private void InitQueueMenuItems()
        {
            QueueMenuItems.AddItem(Menu.AddQueue, RelayCommand.Create(p => RunAddQueueCommand()))
                .AddIconResource(ResourceKeys.Icon.IconAdd);

            QueueMenuItems.AddItem(Menu.RenameQueue, RelayCommand.Create(p => IsQueueEditActive = true, p => SelectedQueue != null))
                .AddIconResource(ResourceKeys.Icon.IconFileReplace);

            QueueMenuItems.AddSeparator();

            QueueMenuItems.AddItem(Menu.RemoveQueue, RelayCommand.Create(p => RunRemoveQueueCommand(), p => SelectedQueue != null))
                .AddIconResource(ResourceKeys.Icon.IconDelete);
        }

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

        private void RunAddQueueCommand()
        {
            long temp = Config.SelectedQueueId;
            QueueTable.AddDefaultRow();
            PopulateQueues();
            SetSelectedQueue(temp);
            Queues.Refresh();
            MainWindowViewModel.Instance.SynchronizeTitleQueue();
        }

        private void RunRemoveQueueCommand()
        {
            if (SelectedQueue != null && MessageWindow.ShowContinueCancel(Confirm.RemoveQueue))
            {
                QueueTable.RemoveQueue(SelectedQueue.Id);
                PopulateQueues();
                Queues.Refresh();
                MainWindowViewModel.Instance.SynchronizeTitleQueue();
            }
        }

        private void RunCopyTitleCommand()
        {
            if (SelectedTitle != null)
            {
                Toolkit.Core.Utility.Execution.TryCatchSwallow(() => Clipboard.SetText(SelectedTitle.Title));
            }
        }

        private void RunClearDateCommand()
        {
            SelectedTitle?.ClearDate();
            OnPropertyChanged(nameof(SelectedTitle));
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