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
using Restless.Toolkit.Mvvm;
using System;
using System.Data;
using System.Windows.Input;
using TableColumns = Restless.Panama.Database.Tables.AlertTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="View.AboutWindow"/>.
    /// </summary>
    public class AlertWindowViewModel : WindowViewModel<AlertTable>
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
            Columns.CreateResource<BooleanToPathConverter>("E", TableColumns.Enabled, ResourceKeys.Icon.SquareSmallGreenIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipAlertEnabled);

            Columns.Create("Date", TableColumns.Date)
                .MakeDate()
                .MakeInitialSortAscending();

            Columns.Create("Title", TableColumns.Title);
            Columns.Create("Url", TableColumns.Url);

            PostponeCommand = RelayCommand.Create(RunPostponeCommand, p => SelectedAlert != null);
            DismissCommand = RelayCommand.Create(RunDismissCommand, p => SelectedAlert != null);

            MenuItems.AddItem(Strings.MenuItemOpenItemOrDoubleClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);
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
            return DataRowCompareDateTime(item1, item2, TableColumns.Date);
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

        private void RunDismissCommand(object parm)
        {
            SelectedAlert?.Dismiss();
        }
        #endregion
    }
}