using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;

namespace Restless.App.Panama.ViewModel
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
        #pragma warning disable 1591
        public UserNoteViewModel()
        {
            DisplayName = Strings.CommandUserNote;
            MaxCreatable = 1;
            Columns.Create("Id", UserNoteTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Created", UserNoteTable.Defs.Columns.Created).MakeDate();
            Columns.SetDefaultSort(Columns.Create("Title", UserNoteTable.Defs.Columns.Title), ListSortDirection.Ascending);
            AddViewSourceSortDescriptions();
            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandAddUserNote, Strings.CommandAddUserNoteTooltip, AddCommand, ResourceHelper.Get("ImageAdd"), VisualCommandImageSize, VisualCommandFontSize));

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandDeleteUserNote, DeleteCommand, "ImageDeleteMenu");

            FilterPrompt = Strings.FilterPromptUserNote;
        }
        #pragma warning restore 1591


        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called when the filter text has changed to set the filter on the underlying data.
        /// </summary>
        /// <param name="text">The filter text.</param>
        protected override void OnFilterTextChanged(string text)
        {
            DataView.RowFilter = string.Format("{0} LIKE '%{1}%'", UserNoteTable.Defs.Columns.Title, text);
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
            if (SelectedRow != null && Messages.ShowYesNo(Strings.ConfirmationDeleteUserNote))
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
            return CanRunCommandIfRowSelected(null);
        }
        #endregion

        /************************************************************************/
        
        #region Private Methods
        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(UserNoteTable.Defs.Columns.Title, ListSortDirection.Ascending));
        }
        #endregion
    }
}