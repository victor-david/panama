/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Utility;
using System.ComponentModel;
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
            Columns.Create("Id", TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.CreateResource<BooleanToPathConverter>("E", TableColumns.Enabled, ResourceKeys.Icon.SquareSmallGreenIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipAlertEnabled);

            Columns.SetDefaultSort(
                Columns.Create("Date", TableColumns.Date)
                .MakeDate(),
                ListSortDirection.Ascending);

            Columns.Create("Title", TableColumns.Title);

            Commands.Add("Browse", RunBrowseCommand, CanRunBrowseCommand);

            /* Context menu items */
            MenuItems.AddItem(Strings.MenuItemAddAlert, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemDeleteAlert, DeleteCommand).AddIconResource(ResourceKeys.Icon.XMediumIconKey);
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
            return DataRowCompareDateTime(item1, item2, TableColumns.Date);
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            if (MessageWindow.ShowContinueCancel(Strings.ConfirmationAddAlert))
            {
                Table.AddDefaultRow();
                Table.Save();
                // Filters.ClearAll();
                Columns.RestoreDefaultSort();
                ForceListViewSort();
            }
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (IsSelectedRowAccessible && Messages.ShowYesNo(Strings.ConfirmationDeleteAlert))
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