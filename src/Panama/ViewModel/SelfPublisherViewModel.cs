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
using System.Globalization;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage self publisher records.
    /// </summary>
    public class SelfPublisherViewModel : DataGridViewModel<SelfPublisherTable>
    {
        #region Private
        private SelfPublisherRow selectedPublisher;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the currently selected publisher row
        /// </summary>
        public SelfPublisherRow SelectedPublisher
        {
            get => selectedPublisher;
            private set => SetProperty(ref selectedPublisher, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherViewModel"/> class.
        /// </summary>
        public SelfPublisherViewModel()
        {
            DisplayName = Strings.CommandSelfPublisher;
            Columns.Create("Id", SelfPublisherTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.Create("Name", SelfPublisherTable.Defs.Columns.Name);
            Columns.Create("Url", SelfPublisherTable.Defs.Columns.Url);
            Columns.SetDefaultSort(Columns.Create("Added", SelfPublisherTable.Defs.Columns.Added).MakeDate().AddToolTip(Strings.ToolTipPublisherAdded), ListSortDirection.Descending);

            Columns.Create("PC", SelfPublisherTable.Defs.Columns.Calculated.PubCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipSelfPublisherPublishedCount)
                .AddSort(null, SelfPublisherTable.Defs.Columns.Name, DataGridColumnSortBehavior.AlwaysAscending);

            /* Context menu items */
            MenuItems.AddItem(Strings.MenuItemCreatePublisher, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemBrowseToPublisherUrlOrClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemDeletePublisher, DeleteCommand).AddIconResource(ResourceKeys.Icon.XRedIconKey);

        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called when the selected item on the associated data grid has changed.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedPublisher = SelfPublisherRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return true;
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, SelfPublisherTable.Defs.Columns.Added);
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            if (MessageWindow.ShowYesNo(Strings.ConfirmationAddPublisher))
            {
                Table.AddDefaultRow();
                Table.Save();
                // Filters.ClearAll();
                Columns.RestoreDefaultSort();
                ForceListViewSort();
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
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> command.
        /// This command opens the publisher's web site.
        /// </summary>
        protected override void RunOpenRowCommand()
        {
            if (SelectedPublisher?.HasUrl() ?? false)
            {
                OpenHelper.OpenWebSite(null, SelectedPublisher.Url);
            }
        }

        /// <summary>
        /// Gets a value that indicates if the <see cref="DataGridViewModel{T}.OpenRowCommand"/> can run.
        /// </summary>
        /// <returns>true if the <see cref="DataGridViewModel{T}.OpenRowCommand"/> can run; otherwise, false.</returns>
        protected override bool CanRunOpenRowCommand()
        {
            return base.CanRunOpenRowCommand() && (SelectedPublisher?.HasUrl() ?? false);
        }

        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
        protected override void RunDeleteCommand()
        {
            if (IsSelectedRowAccessible)
            {
                int childRowCount = SelectedRow.GetChildRows(SelfPublisherTable.Defs.Relations.ToPublished).Length;
                if (childRowCount > 0)
                {
                    MessageWindow.ShowError(string.Format(CultureInfo.InvariantCulture, Strings.InvalidOpCannotDeletePublisher, childRowCount));
                    return;
                }

                if (MessageWindow.ShowYesNo(Strings.ConfirmationDeletePublisher))
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
            return IsSelectedRowAccessible;
        }
        #endregion
    }
}