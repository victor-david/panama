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
using Restless.Toolkit.Mvvm;
using System.Data;
using System.Globalization;
using TableColumns = Restless.Panama.Database.Tables.TagTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to display and manage tags in the <see cref="TagTable"/>.
    /// </summary>
    public class TagViewModel : DataRowViewModel<TagTable>
    {
        #region Private
        private TagRow selectedTag;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

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
            Columns.Create("Id", TableColumns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.Create("Tag", TableColumns.Tag).MakeInitialSortAscending();
            Columns.Create("Description", TableColumns.Description).MakeFlexWidth(2.5);
            Columns.Create("Usage", TableColumns.Calculated.UsageCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W076);

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandAddTag, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemDeleteTag, DeleteCommand).AddIconResource(ResourceKeys.Icon.XRedIconKey);

            AddCommand.Supported = CommandSupported.Yes;

            ListView.IsLiveSorting = true;
            ListView.LiveSortingProperties.Add(TableColumns.Tag);
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareString(item1, item2, TableColumns.Tag);
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
            // Filters.ClearAll();
            ForceListViewSort();
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
        #endregion
    }
}