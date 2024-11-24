using Restless.Panama.Core;
using Restless.Panama.Core.Converters;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.AlertTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for managing user notes.
    /// </summary>
    public class AlertViewModel : DataRowViewModel<AlertTable>
    {
        #region Private
        private AlertRow selectedAlert;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

        /// <summary>
        /// Gets the selected alert
        /// </summary>
        public AlertRow SelectedAlert
        {
            get => selectedAlert;
            private set => SetProperty(ref selectedAlert, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AlertViewModel"/> class
        /// </summary>
        public AlertViewModel()
        {
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.CreateResource<BooleanToResourceConverter>(Header.EnabledShort, TableColumns.Enabled, ResourceKeys.Icon.IconSquare)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(ToolTip.AlertEnabled);

            Columns.Create(Header.Date, TableColumns.Date)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create(Header.Title, TableColumns.Title);

            Commands.Add("Browse", RunBrowseCommand, CanRunBrowseCommand);

            /* Context menu items */
            MenuItems.AddItem(Menu.AddAlert, AddCommand).AddIconResource(ResourceKeys.Icon.IconAdd);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Menu.DeleteAlert, DeleteCommand).AddIconResource(ResourceKeys.Icon.IconDelete);

            ListView.IsLiveSorting = true;
            ListView.LiveSortingProperties.Add(TableColumns.Date);
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedAlert = AlertRow.Create(SelectedRow);
            SelectedAlert?.SetDateFormat(Config.DateFormat);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.Date);
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            if (MessageWindow.ShowContinueCancel(Confirm.AddAlert))
            {
                Table.AddDefaultRow();
                Table.Save();
                // Filters.ClearAll();
                ForceListViewSort();
            }
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (IsSelectedRowAccessible && MessageWindow.ShowYesNo(Confirm.DeleteAlert))
            {
                DeleteSelectedRow();
            }
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void RunBrowseCommand(object parm)
        {
            if (CanRunBrowseCommand(parm))
            {
                OpenHelper.OpenWebSite(null, SelectedRow[TableColumns.Url].ToString());
            }
        }

        private bool CanRunBrowseCommand(object parm)
        {
            return
                IsSelectedRowAccessible &&
                !string.IsNullOrEmpty(SelectedRow[TableColumns.Url].ToString());
        }
        #endregion
    }
}