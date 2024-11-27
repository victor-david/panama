using Restless.Panama.Core;
using Restless.Panama.Core.Converters;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Mvvm;
using System;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Menu = Restless.Panama.Resources.Menu;
using TableColumns = Restless.Panama.Database.Tables.PublishedAllTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage published records.
    /// </summary>
    public class PublishedViewModel : DataRowViewModel<PublishedAllTable>
    {
        #region Private
        private PublishedAllRow selectedPublished;
        private readonly bool[] groups = { false, false, false, false };
        private string searchText;
        private readonly PropertyGroupDescription typeGroup;
        private readonly PropertyGroupDescription publisherGroup;
        private readonly PropertyGroupDescription titleGroup;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool ClearFilterCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

        /// <inheritdoc/>
        public override bool OpenRowCommandEnabled => SelectedPublished?.HasUrl ?? false;

        /// <summary>
        /// Gets the currently selected published row
        /// </summary>
        public PublishedAllRow SelectedPublished
        {
            get => selectedPublished;
            private set => SetProperty(ref selectedPublished, value);
        }

        public bool GroupByNone
        {
            get => groups[0];
            set => ApplyListViewGrouping(0);
        }

        public bool GroupByType
        {
            get => groups[1];
            set => ApplyListViewGrouping(1);
        }

        public bool GroupByTitle
        {
            get => groups[2];
            set => ApplyListViewGrouping(2);
        }

        public bool GroupByPublisher
        {
            get => groups[3];
            set => ApplyListViewGrouping(3);
        }

        public string SearchText
        {
            get => searchText;
            set => SetProperty(ref searchText, value, SearchTextUpdated);
        }

        public ICommand ClearPublishedDateCommand { get; }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublishedViewModel"/> class.
        /// </summary>
        public PublishedViewModel()
        {
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create<PublishedTypeToStringConverter>(Header.Type, TableColumns.TypeId)
                .MakeFixedWidth(FixedWidth.W112);

            Columns.Create(Header.Added, TableColumns.Added)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create(Header.Published, TableColumns.Published)
                .MakeDate();

            Columns.Create(Header.Title, TableColumns.Title);

            Columns.Create(Header.Publisher, TableColumns.Publisher);

            Columns.RestoreColumnState(Config.PublishedGridColumnState);

            MenuItems.AddItem(Menu.RemovePublished, DeleteCommand).AddIconResource(ResourceKeys.Icon.IconDelete);

            typeGroup = new PropertyGroupDescription(TableColumns.TypeId);
            publisherGroup = new PropertyGroupDescription(TableColumns.Publisher);
            titleGroup = new PropertyGroupDescription(TableColumns.Title);

            ClearPublishedDateCommand = RelayCommand.Create(p => RunClearPublishedDateCommand(), p => SelectedPublished?.HasPublishedDate ?? false);

            SearchText = Config.PublishedSearchText;
            ApplyListViewGrouping(Config.PublishedGroupIndex, true);
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <inheritdoc/>
        protected override void OnActivated()
        {
            Table.Initialize();
        }

        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedPublished = PublishedAllRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                return
                    item[TableColumns.Title].ToString().Contains(SearchText, StringComparison.InvariantCultureIgnoreCase) ||
                    item[TableColumns.Publisher].ToString().Contains(SearchText, StringComparison.InvariantCultureIgnoreCase);
            }
            return true;
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.Added);
        }

        /// <inheritdoc/>
        protected override void RunClearFilterCommand()
        {
            SearchText = null;
        }

        /// <inheritdoc/>
        protected override void RunOpenRowCommand()
        {
            if (SelectedPublished?.HasUrl ?? false)
            {
                OpenHelper.OpenWebSite(null, SelectedPublished.Url);
            }
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (IsSelectedRowAccessible && MessageWindow.ShowContinueCancel(Confirm.RemoveTitlePublished))
            {
                SelectedPublished?.Delete();
                ListView.Refresh();
            }
        }

        /// <inheritdoc/>
        protected override void OnSave()
        {
            Config.PublishedGridColumnState = Columns.GetColumnState();
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
        private void RunClearPublishedDateCommand()
        {
            if (SelectedPublished is not null && MessageWindow.ShowContinueCancel(Confirm.ClearPublishedDate))
            {
                SelectedPublished.Published = null;
                OnPropertyChanged(nameof(SelectedPublished));
            }
        }

        private void SearchTextUpdated()
        {
            Config.PublishedSearchText = SearchText;
            ListView.Refresh();
        }

        private void ApplyListViewGrouping(int groupIndex, bool initializing = false)
        {
            if (Config.PublishedGroupIndex == groupIndex && !initializing)
            {
                return;
            }

            Config.PublishedGroupIndex = groupIndex;

            for (int k = 0; k < groups.Length; k++)
            {
                groups[k] = k == groupIndex;
            }

            OnPropertyChanged(nameof(GroupByNone));
            OnPropertyChanged(nameof(GroupByType));
            OnPropertyChanged(nameof(GroupByTitle));
            OnPropertyChanged(nameof(GroupByPublisher));

            using (ListView.DeferRefresh())
            {
                ListView.GroupDescriptions.Clear();
                switch (groupIndex)
                {
                    case 1:
                        ListView.GroupDescriptions.Add(typeGroup);
                        ListView.GroupDescriptions.Add(publisherGroup);
                        ListView.GroupDescriptions.Add(titleGroup);
                        break;
                    case 2:
                        ListView.GroupDescriptions.Add(titleGroup);
                        ListView.GroupDescriptions.Add(publisherGroup);
                        break;
                    case 3:
                        ListView.GroupDescriptions.Add(publisherGroup);
                        ListView.GroupDescriptions.Add(titleGroup);
                        break;
                }
            }

            bool canSort = groupIndex == 0;
            foreach (DataGridColumn col in Columns)
            {
                col.CanUserSort = canSort;
                if (!canSort)
                {
                    col.SortDirection = null;
                }
            }
        }
        #endregion
    }
}