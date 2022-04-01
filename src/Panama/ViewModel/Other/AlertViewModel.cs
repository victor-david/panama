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

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for managing user notes.
    /// </summary>
    public class AlertViewModel : DataRowViewModel<AlertTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AlertViewModel"/> class
        /// </summary>
        public AlertViewModel()
        {
            Columns.Create("Id", AlertTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.CreateImage<BooleanToImageConverter>("E", AlertTable.Defs.Columns.Enabled);
            Columns.SetDefaultSort(Columns.Create("Date", AlertTable.Defs.Columns.Date).MakeDate(), ListSortDirection.Ascending);
            Columns.Create("Title", AlertTable.Defs.Columns.Title);
            AddViewSourceSortDescriptions();

            Commands.Add("Browse", RunBrowseCommand, CanRunBrowseCommand);

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandDeleteAlert, DeleteCommand).AddImageResource("ImageDeleteMenu");
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            Table.AddDefaultRow();
            Table.Save();
            AddViewSourceSortDescriptions();
            Columns.RestoreDefaultSort();
        }

        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
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
        private void AddViewSourceSortDescriptions()
        {
            // TODO
            //MainSource.SortDescriptions.Clear();
            //MainSource.SortDescriptions.Add(new SortDescription(AlertTable.Defs.Columns.Date, ListSortDirection.Ascending));
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