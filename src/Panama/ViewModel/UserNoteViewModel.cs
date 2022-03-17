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
using Restless.Toolkit.Utility;
using System.ComponentModel;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for managing user notes.
    /// </summary>
    public class UserNoteViewModel : DataGridViewModel<UserNoteTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNoteViewModel"/> class.
        /// </summary>
        public UserNoteViewModel()
        {
            DisplayName = Strings.TextNotes;
            Columns.Create("Id", UserNoteTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.Create("Created", UserNoteTable.Defs.Columns.Created).MakeDate();
            Columns.SetDefaultSort(Columns.Create("Title", UserNoteTable.Defs.Columns.Title), ListSortDirection.Ascending);
            AddViewSourceSortDescriptions();

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandDeleteUserNote, DeleteCommand).AddImageResource("ImageDeleteMenu");
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
            if (IsSelectedRowAccessible && Messages.ShowYesNo(Strings.ConfirmationDeleteUserNote))
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
            // TODO
            //MainSource.SortDescriptions.Clear();
            //MainSource.SortDescriptions.Add(new SortDescription(UserNoteTable.Defs.Columns.Title, ListSortDirection.Ascending));
        }
        #endregion
    }
}