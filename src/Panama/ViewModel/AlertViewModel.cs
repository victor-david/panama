/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Converters;
using Restless.App.Panama.Core;
using Restless.App.Panama.Resources;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Utility;
using System.ComponentModel;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for managing user notes.
    /// </summary>
    public class AlertViewModel : DataGridViewModel<AlertTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AlertViewModel"/> class
        /// </summary>
        /// <param name="owner">The VM that owns this view model.</param>
        public AlertViewModel(ApplicationViewModel owner) : base(owner)
        {
            DisplayName = Strings.CommandAlert;
            MaxCreatable = 1;
            Columns.Create("Id", AlertTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.CreateImage<BooleanToImageConverter>("E", AlertTable.Defs.Columns.Enabled);
            Columns.SetDefaultSort(Columns.Create("Date", AlertTable.Defs.Columns.Date).MakeDate(), ListSortDirection.Ascending);
            Columns.Create("Title", AlertTable.Defs.Columns.Title);
            AddViewSourceSortDescriptions();
            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandAddAlert, Strings.CommandAddAlertTooltip, AddCommand, ResourceHelper.Get("ImageAdd"), VisualCommandImageSize, VisualCommandFontSize));

            Commands.Add("Browse", RunBrowseCommand, CanRunBrowseCommand);

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandDeleteAlert, DeleteCommand).AddImageResource("ImageDeleteMenu");

            FilterPrompt = Strings.FilterPromptAlert;
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called when the filter text has changed to set the filter on the underlying data.
        /// </summary>
        /// <param name="text">The filter text.</param>
        protected override void OnFilterTextChanged(string text)
        {
            DataView.RowFilter = string.Format("{0} LIKE '%{1}%'", AlertTable.Defs.Columns.Title, text);
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            Table.AddDefaultRow();
            Table.Save();
            FilterText = null;
            AddViewSourceSortDescriptions();
            Columns.RestoreDefaultSort();
        }

        /// <summary>
        /// Called when the framework checks to see if Add command can execute
        /// </summary>
        /// <returns>This method always returns true.</returns>
        protected override bool CanRunAddCommand()
        {
            return true;
        }

        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
        protected override void RunDeleteCommand()
        {
            if (IsSelectedRowAccessible && Messages.ShowYesNo(Strings.ConfirmationDeleteAlert))
            {
                SelectedRow.Delete();
                Table.Save();
            }
        }

        /// <summary>
        /// Called when the framework checks to see if Delete command can execute
        /// </summary>
        /// <returns>true if a row is selected; otherwise, false.</returns>
        protected override bool CanRunDeleteCommand()
        {
            return IsSelectedRowAccessible;
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(AlertTable.Defs.Columns.Date, ListSortDirection.Ascending));
        }

        private void RunBrowseCommand(object parm)
        {
            if (CanRunBrowseCommand(parm))
            {
                OpenHelper.OpenWebSite(null, SelectedRow[AlertTable.Defs.Columns.Url].ToString());
            }
        }

        private bool CanRunBrowseCommand(object parm)
        {
            return
                IsSelectedRowAccessible &&
                !string.IsNullOrEmpty(SelectedRow[AlertTable.Defs.Columns.Url].ToString());
        }
        #endregion
    }
}