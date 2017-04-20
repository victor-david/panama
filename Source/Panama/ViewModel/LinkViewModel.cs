using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage links in the <see cref="LinkTable"/>.
    /// </summary>
    public class LinkViewModel : DataGridViewModel<LinkTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public LinkViewModel()
        {
            DisplayName = Strings.CommandLink;
            MaxCreatable = 1;
            Columns.Create("Id", LinkTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Added", LinkTable.Defs.Columns.Added).MakeDate();
            Columns.SetDefaultSort(Columns.Create("Name", LinkTable.Defs.Columns.Name), ListSortDirection.Ascending);
            Columns.Create("Url", LinkTable.Defs.Columns.Url).MakeFlexWidth(2.5);
            Columns.Create("Note", LinkTable.Defs.Columns.Notes).MakeSingleLine();
            AddViewSourceSortDescriptions();
            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandAddLink, Strings.CommandAddLinkTooltip, AddCommand, ResourceHelper.Get("ImageAdd"), VisualCommandImageSize, VisualCommandFontSize));

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandBrowseToUrlOrClick, OpenRowCommand, "ImageBrowseToUrlMenu");
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandDeleteLink, DeleteCommand, "ImageDeleteMenu");
            FilterPrompt = Strings.FilterPromptLink;
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called when the filter text has changed to set the filter on the underlying data.
        /// </summary>
        /// <param name="text">The filter text.</param>
        protected override void OnFilterTextChanged(string text)
        {
            DataView.RowFilter = String.Format("{0} LIKE '%{1}%' OR {2} LIKE '%{3}%'", LinkTable.Defs.Columns.Name, text, LinkTable.Defs.Columns.Notes, text);
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
            if (Messages.ShowYesNo(Strings.ConfirmationDeleteLink))
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

        /// <summary>
        /// Runs the open row command to browse to the row's url.
        /// </summary>
        /// <param name="item">The command parameter (not used)</param>
        protected override void RunOpenRowCommand(object item)
        {
            OpenHelper.OpenWebSite(null, SelectedRow[LinkTable.Defs.Columns.Url].ToString());
        }

        /// <summary>
        /// Gets a boolean value that indicates if the <see cref=" DataGridViewModel{T}.OpenRowCommand"/> can run.
        /// </summary>
        /// <param name="item">The command parameter (not used)</param>
        /// <returns>true if the command can execute (row selected and has a url); otherwise, false.</returns>
        protected override bool CanRunOpenRowCommand(object item)
        {
            return 
                base.CanRunOpenRowCommand(item) &&
                !String.IsNullOrWhiteSpace(SelectedRow[LinkTable.Defs.Columns.Url].ToString());
        }
        #endregion

        /************************************************************************/
        
        #region Private Methods

        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(LinkTable.Defs.Columns.Name, ListSortDirection.Ascending));
        }
        #endregion
    }
}