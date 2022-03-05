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
    /// Provides the logic that is used for authors management.
    /// </summary>
    public class AuthorViewModel : DataGridViewModel<AuthorTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorViewModel"/> class.
        /// </summary>
        public AuthorViewModel()
        {
            DisplayName = Strings.CommandAuthor;
            MaxCreatable = 1;
            Columns.SetDefaultSort(Columns.Create("Id", AuthorTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard), ListSortDirection.Ascending);
            Columns.CreateImage<BooleanToImageConverter>("D", AuthorTable.Defs.Columns.IsDefault);
            Columns.Create("Name", AuthorTable.Defs.Columns.Name);
            AddViewSourceSortDescriptions();

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandDeleteAuthor, DeleteCommand).AddImageResource("ImageDeleteMenu");

            FilterPrompt = Strings.FilterPromptAuthor;
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
            DataView.RowFilter = $"{AuthorTable.Defs.Columns.Name} LIKE '%{text}%'";
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            if (Messages.ShowYesNo(Strings.ConfirmationAddAuthor))
            {
                Table.AddDefaultRow();
                Table.Save();
                FilterText = null;
                AddViewSourceSortDescriptions();
                Columns.RestoreDefaultSort();
            }
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
            if (CanRunDeleteCommand())
            {
                int childRowCount = SelectedRow.GetChildRows(AuthorTable.Defs.Relations.ToTitle).Length;
                if (childRowCount > 0)
                {
                    Messages.ShowError(string.Format(Strings.InvalidOpCannotDeleteAuthor, childRowCount));
                    return;
                }
                if (Messages.ShowYesNo(Strings.ConfirmationDeleteAuthor))
                {
                    SelectedRow.Delete();
                    Table.Save();
                }
            }
        }

        /// <summary>
        /// Called when the framework checks to see if Delete command can execute
        /// </summary>
        /// <returns>true if a row is selected; otherwise, false.</returns>
        protected override bool CanRunDeleteCommand()
        {
            // Can delete if the row is accesible and its not the system inserted author.
            return IsSelectedRowAccessible && (long)SelectedPrimaryKey != 1;
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(AuthorTable.Defs.Columns.Id, ListSortDirection.Ascending));
        }
        #endregion
    }
}