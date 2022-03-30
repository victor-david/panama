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
using System.ComponentModel;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.UserNoteTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for managing user notes.
    /// </summary>
    public class UserNoteViewModel : DataGridViewModel<UserNoteTable>
    {
        #region Private
        private UserNoteRow selectedNote;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <summary>
        /// Gets the selected note
        /// </summary>
        public UserNoteRow SelectedNote
        {
            get => selectedNote;
            private set => SetProperty(ref selectedNote, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNoteViewModel"/> class.
        /// </summary>
        public UserNoteViewModel()
        {
            Columns.Create("Id", TableColumns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.Create("Created", TableColumns.Created).MakeDate();
            Columns.SetDefaultSort(Columns.Create("Title", TableColumns.Title), ListSortDirection.Ascending);

            /* Context menu items */
            MenuItems.AddItem(Strings.MenuItemAddUserNote, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemDeleteNote, DeleteCommand).AddIconResource(ResourceKeys.Icon.XRedIconKey);
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedNote = UserNoteRow.Create(SelectedRow);
        }

        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareString(item1, item2, TableColumns.Title);
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            Table.AddDefaultRow();
            Table.Save();
            Columns.RestoreDefaultSort();
            ForceListViewSort();
        }

        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
        protected override void RunDeleteCommand()
        {
            if (IsSelectedRowAccessible && MessageWindow.ShowYesNo(Strings.ConfirmationDeleteUserNote))
            {
                DeleteSelectedRow();
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
    }
}