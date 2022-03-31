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
using Restless.Toolkit.Core.Utility;
using System.ComponentModel;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.LinkTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage links in the <see cref="LinkTable"/>.
    /// </summary>
    public class LinkViewModel : DataRowViewModel<LinkTable>
    {
        #region Private
        private LinkRow selectedLink;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <summary>
        /// Gets the selected link
        /// </summary>
        public LinkRow SelectedLink
        {
            get => selectedLink;
            private set => SetProperty(ref selectedLink, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkViewModel"/> class.
        /// </summary>
        public LinkViewModel()
        {
            DisplayName = Strings.CommandLink;
            Columns.Create("Id", TableColumns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.Create("Added", TableColumns.Added).MakeDate();
            Columns.SetDefaultSort(Columns.Create("Name", TableColumns.Name), ListSortDirection.Ascending);
            Columns.Create("Url", TableColumns.Url).MakeFlexWidth(2.5);
            Columns.Create("Note", TableColumns.Notes).MakeSingleLine();

            /* Context menu items */
            MenuItems.AddItem(Strings.MenuItemAddLink, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemBrowseToUrlOrClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemDeleteLink, DeleteCommand).AddIconResource(ResourceKeys.Icon.XRedIconKey);
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedLink = LinkRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareString(item1, item2, TableColumns.Name);
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
            if (IsSelectedRowAccessible && MessageWindow.ShowYesNo(Strings.ConfirmationDeleteLink))
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

        /// <summary>
        /// Runs the open row command to browse to the row's url.
        /// </summary>
        protected override void RunOpenRowCommand()
        {
            OpenHelper.OpenWebSite(null, SelectedLink.Url);
        }

        /// <summary>
        /// Gets a boolean value that indicates if the <see cref=" DataRowViewModel{T}.OpenRowCommand"/> can run.
        /// </summary>
        /// <returns>true if the command can execute (row selected and has a url); otherwise, false.</returns>
        protected override bool CanRunOpenRowCommand()
        {
            return !string.IsNullOrWhiteSpace(SelectedLink?.Url);
        }
        #endregion
    }
}