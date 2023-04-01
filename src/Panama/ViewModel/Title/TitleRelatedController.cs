using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System.Collections.Generic;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.TitleRelatedTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    public class TitleRelatedController : BaseController<TitleViewModel, TitleRelatedTable>
    {
        private TitleRelatedRow selectedRelated;

        #region Public properties
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

        /// <inheritdoc/>
        public override bool OpenRowCommandEnabled => IsSelectedRowAccessible;

        /// <summary>
        /// Gets the selected related row
        /// </summary>
        public TitleRelatedRow SelectedRelated
        {
            get => selectedRelated;
            private set => SetProperty(ref selectedRelated, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleRelatedController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleRelatedController(TitleViewModel owner) : base(owner)
        {
            Columns.Create("Id", TableColumns.RelatedId)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Title", TableColumns.Joined.Title)
                .MakeInitialSortAscending();

            Columns.Create("Written", TableColumns.Joined.Written).MakeDate();

            Columns.Create("Updated", TableColumns.Joined.Updated)
                .MakeDate()
                .AddToolTip(Strings.TooltipTitleUpdated);

            Columns.Create("WC", TableColumns.Joined.LatestVersionWordCount)
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitleWordCount);

            MenuItems.AddItem(Strings.MenuItemAddRelated, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddItem(Strings.MenuItemOpenTitleOrDoubleClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemFilterTitleListToRelated, RelayCommand.Create(RunFilterToRelatedCommand, p => !ListView.IsEmpty))
                .AddIconResource(ResourceKeys.Icon.FilterIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemRemoveRelated, DeleteCommand).AddIconResource(ResourceKeys.Icon.XMediumIconKey);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedRelated = TitleRelatedRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareString(item1, item2, TableColumns.Joined.Title);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.TitleId] == (Owner?.SelectedTitle?.Id ?? 0);
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            long titleId = Owner?.SelectedTitle?.Id ?? 0;
            if (titleId > 0 && WindowFactory.TitleSelect.Create().GetTitles() is List<TitleRow> titles)
            {
                Table.AddIfNotExist(titleId, titles);
                ListView.Refresh();
            }
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (IsSelectedRowAccessible && MessageWindow.ShowContinueCancel(Strings.ConfirmationRemoveRelatedTitle))
            {
                Table.RemoveIfExist(Owner?.SelectedTitle?.Id ?? 0, SelectedRelated.RelatedId);
                ListView.Refresh();
            }
        }

        /// <inheritdoc/>
        protected override void RunOpenRowCommand()
        {
            if (!string.IsNullOrWhiteSpace(SelectedRelated?.LatestVersionPath))
            {
                Open.TitleVersionFile(SelectedRelated.LatestVersionPath);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunFilterToRelatedCommand(object parm)
        {
            List<long> ids = new();
            foreach (DataRowView view in ListView)
            {
                long id = (long)view.Row[TableColumns.RelatedId];
                ids.Add(id);
            }
            ids.Add(Owner.SelectedTitle?.Id ?? 0);
            Owner.Filters.SetMultipleIdFilter(ids);
        }
        #endregion
    }
}