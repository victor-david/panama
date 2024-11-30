using Restless.Panama.Core;
using Restless.Panama.Core.Converters;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Mvvm;
using System;
using System.Data;
using System.Windows.Input;
using TableColumns = Restless.Panama.Database.Tables.AlertTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="View.AlertWindow"/>.
    /// </summary>
    public class AlertWindowViewModel : DataRowViewModel<AlertTable>
    {
        #region Private
        private AlertRow selectedAlert;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the selected alert object.
        /// </summary>
        public AlertRow SelectedAlert
        {
            get => selectedAlert;
            set => SetProperty(ref selectedAlert, value);
        }

        /// <summary>
        /// Gets the command used to postpone an alert
        /// </summary>
        public ICommand PostponeCommand
        {
            get;
        }

        /// <summary>
        /// Gets the command to dismiss an alert
        /// </summary>
        public ICommand DismissCommand
        {
            get;
        }

        /// <inheritdoc/>
        public override bool OpenRowCommandEnabled => SelectedAlert?.HasUrl ?? false;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AlertWindowViewModel"/> class.
        /// </summary>
        public AlertWindowViewModel()
        {
            DisplayName = Header.ActiveAlerts;

            Columns.CreateResource<BooleanToResourceConverter>(Header.EnabledShort, TableColumns.Enabled, ResourceKeys.Icon.IconSquare)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(ToolTip.AlertEnabled);

            Columns.Create(Header.Date, TableColumns.Date)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create(Header.Title, TableColumns.Title);
            Columns.Create(Header.Url, TableColumns.Url);

            PostponeCommand = RelayCommand.Create(RunPostponeCommand, p => SelectedAlert != null);
            DismissCommand = RelayCommand.Create(p => RunDismissCommand(), p => SelectedAlert != null);

            MenuItems.AddItem(Menu.OpenItemOrDoubleClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.IconOpenWebSite);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
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
        protected override bool OnDataRowFilter(DataRow item)
        {
            return
                (bool)item[TableColumns.Enabled] &&
                DateTime.Compare((DateTime)item[TableColumns.Date], DateTime.UtcNow) <= 0;
        }

        /// <inheritdoc/>
        protected override void RunOpenRowCommand()
        {
            if (SelectedAlert?.HasUrl ?? false)
            {
                OpenHelper.OpenWebSite(null, SelectedAlert.Url);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunPostponeCommand(object parm)
        {
            if (int.TryParse(parm?.ToString(), out int days))
            {
                SelectedAlert?.Postpone(days);
            }
        }

        private void RunDismissCommand()
        {
            SelectedAlert?.Dismiss();
        }
        #endregion
    }
}