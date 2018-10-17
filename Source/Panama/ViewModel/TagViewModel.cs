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
    /// Provides the logic that is used to display and manage tags in the <see cref="TagTable"/>.
    /// </summary>
    public class TagViewModel : DataGridViewModel<TagTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TagViewModel"/> class.
        /// </summary>
        public TagViewModel()
        {
            DisplayName = Strings.CommandTag;
            MaxCreatable = 1;
            Columns.Create("Id", TagTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.SetDefaultSort(Columns.Create("Tag", TagTable.Defs.Columns.Tag), ListSortDirection.Ascending);
            Columns.Create("Description", TagTable.Defs.Columns.Description).MakeFlexWidth(2.5);
            Columns.Create("Usage", TagTable.Defs.Columns.UsageCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.MediumNumeric);
            AddViewSourceSortDescriptions();
            Commands.Add("RefreshTagUsage", (o) =>
                {
                    DatabaseController.Instance.GetTable<TagTable>().RefreshTagUsage();
                });
            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandAddTag, Strings.CommandAddTagTooltip, AddCommand, ResourceHelper.Get("ImageAdd"), VisualCommandImageSize, VisualCommandFontSize));
            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandRefreshTagUsage, Strings.CommandRefreshTagUsageTooltip, Commands["RefreshTagUsage"], ResourceHelper.Get("ImageRefresh"), VisualCommandImageSize, VisualCommandFontSize));

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandDeleteTag, DeleteCommand, "ImageDeleteMenu");
            FilterPrompt = Strings.FilterPromptTag;

            AddCommand.Supported = CommandSupported.Yes;
        }
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
            DataView.RowFilter = string.Format("{0} LIKE '%{1}%'", TagTable.Defs.Columns.Tag, text);
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
            int childRowCount = SelectedRow.GetChildRows(TagTable.Defs.Relations.ToTitleTag).Length;
            if (childRowCount > 0)
            {
                Messages.ShowError(string.Format(Strings.InvalidOpCannotDeleteTag, childRowCount));
                return;
            }
            if (Messages.ShowYesNo(Strings.ConfirmationDeleteTag))
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
            MainSource.SortDescriptions.Add(new SortDescription(TagTable.Defs.Columns.Tag, ListSortDirection.Ascending));
        }
        #endregion
    }
}