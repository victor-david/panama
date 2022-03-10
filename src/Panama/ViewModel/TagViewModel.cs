/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System.ComponentModel;
using System.Data;
using System.Globalization;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to display and manage tags in the <see cref="TagTable"/>.
    /// </summary>
    public class TagViewModel : DataGridViewModel<TagTable>
    {
        #region Private
        private TagRow selectedTag;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the currently selected tag
        /// </summary>
        public TagRow SelectedTag
        {
            get => selectedTag;
            private set => SetProperty(ref selectedTag, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TagViewModel"/> class.
        /// </summary>
        public TagViewModel()
        {
            DisplayName = Strings.MenuItemTags;
            Columns.Create("Id", TagTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.Create("Tag", TagTable.Defs.Columns.Tag);
            Columns.Create("Description", TagTable.Defs.Columns.Description).MakeFlexWidth(2.5);
            Columns.Create("Usage", TagTable.Defs.Columns.Calculated.UsageCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W052);

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandAddTag, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandDeleteTag, DeleteCommand).AddIconResource(ResourceKeys.Icon.XRedIconKey);
            FilterPrompt = Strings.FilterPromptTag;

            AddCommand.Supported = CommandSupported.Yes;

            ListView.IsLiveSorting = true;
            ListView.LiveSortingProperties.Add(TagTable.Defs.Columns.Tag);
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareString(item1, item2, TagTable.Defs.Columns.Tag);
        }

        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedTag = TagRow.Create(SelectedRow);
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            Table.AddDefaultRow();
            Table.Save();
            FilterText = null;
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
                MessageWindow.ShowError(string.Format(CultureInfo.InvariantCulture, Strings.InvalidOpCannotDeleteTag, childRowCount));
                return;
            }
            if (MessageWindow.ShowYesNo(Strings.ConfirmationDeleteTag))
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
    }
}