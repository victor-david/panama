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
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.UserNoteTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for managing user notes.
    /// </summary>
    public class UserNoteViewModel : DataRowViewModel<UserNoteTable>
    {
        #region Private
        private UserNoteRow selectedNote;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

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
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);
            Columns.Create(Header.Created, TableColumns.Created).MakeDate().MakeInitialSortDescending();
            Columns.Create(Header.Title, TableColumns.Title);

            /* Context menu items */
            MenuItems.AddItem(Menu.AddUserNote, AddCommand).AddIconResource(ResourceKeys.Icon.IconAdd);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Menu.DeleteNote, DeleteCommand).AddIconResource(ResourceKeys.Icon.IconDelete);
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
            return DataRowCompareDateTime(item2, item1, TableColumns.Created);
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            Table.AddDefaultRow();
            Table.Save();
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
        #endregion
    }
}