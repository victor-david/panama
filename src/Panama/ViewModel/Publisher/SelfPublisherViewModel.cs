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
using System.Data;
using System.Globalization;
using TableColumns = Restless.Panama.Database.Tables.SelfPublisherTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage self publisher records.
    /// </summary>
    public class SelfPublisherViewModel : DataRowViewModel<SelfPublisherTable>
    {
        #region Private
        private SelfPublisherRow selectedPublisher;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

        /// <inheritdoc/>
        public override bool OpenRowCommandEnabled => SelectedPublisher?.HasUrl() ?? false;

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
            Columns.Create("Id", TableColumns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.Create("Name", TableColumns.Name);
            Columns.Create("Url", TableColumns.Url);
            Columns.Create("Added", TableColumns.Added)
                .MakeDate()
                .AddToolTip(Strings.ToolTipPublisherAdded)
                .MakeInitialSortDescending();

            Columns.Create("PC", TableColumns.Calculated.PubCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipSelfPublisherPublishedCount)
                .AddSort(null, TableColumns.Name, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.RestoreColumnState(Config.SelfPublisherGridColumnState);

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
            return DataRowCompareDateTime(item2, item1, TableColumns.Added);
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
                ForceListViewSort();
            }
        }

        /// <summary>
        /// Runs the <see cref="DataRowViewModel{T}.OpenRowCommand"/> command.
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

        /// <inheritdoc/>
        protected override void OnSave()
        {
            Config.SelfPublisherGridColumnState = Columns.GetColumnState();
        }

        /// <inheritdoc/>
        protected override void OnClosing()
        {
            base.OnClosing();
            SignalSave();
        }
        #endregion
    }
}