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
using System.Globalization;
using TableColumns = Restless.Panama.Database.Tables.AuthorTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for authors management.
    /// </summary>
    public class AuthorViewModel : DataRowViewModel<AuthorTable>
    {
        #region Private
        private AuthorRow selectedAuthor;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <summary>
        /// Gets the currently selected author
        /// </summary>
        public AuthorRow SelectedAuthor
        {
            get => selectedAuthor;
            private set => SetProperty(ref selectedAuthor, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorViewModel"/> class.
        /// </summary>
        public AuthorViewModel()
        {
            Columns.SetDefaultSort(Columns.Create("Id", TableColumns.Id).MakeFixedWidth(FixedWidth.W042), ListSortDirection.Ascending);
            Columns.CreateResource<BooleanToPathConverter>("R", TableColumns.IsDefault, ResourceKeys.Icon.SquareSmallGreenIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipAuthorDefault);

            Columns.Create("Name", TableColumns.Name);

            /* Context menu items */
            MenuItems.AddItem(Strings.MenuItemAddAuthor, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemDeleteAuthor, DeleteCommand).AddIconResource(ResourceKeys.Icon.XRedIconKey);
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedAuthor = AuthorRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareLong(item1, item2, TableColumns.Id);
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            if (MessageWindow.ShowYesNo(Strings.ConfirmationAddAuthor))
            {
                Table.AddDefaultRow();
                Table.Save();
                // Filters.ClearAll();
                Columns.RestoreDefaultSort();
                ForceListViewSort();
            }
        }

        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
        protected override void RunDeleteCommand()
        {
            if (CanRunDeleteCommand())
            {
                int childRowCount = SelectedRow.GetChildRows(AuthorTable.Defs.Relations.ToTitle).Length;
                if (childRowCount > 0)
                {
                    MessageWindow.ShowError(string.Format(CultureInfo.InvariantCulture, Strings.InvalidOpCannotDeleteAuthor, childRowCount));
                    return;
                }

                if (MessageWindow.ShowYesNo(Strings.ConfirmationDeleteAuthor))
                {
                    DeleteSelectedRow();
                }
            }
        }

        /// <summary>
        /// Called when the framework checks to see if Delete command can execute
        /// </summary>
        /// <returns>true if a row is selected; otherwise, false.</returns>
        protected override bool CanRunDeleteCommand()
        {
            /* if selected and not the system generated author id */
            return (SelectedAuthor?.Id ?? AuthorTable.Defs.Values.SystemAuthorId) != AuthorTable.Defs.Values.SystemAuthorId;
        }
        #endregion
    }
}