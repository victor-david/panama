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
using Restless.Toolkit.Utility;
using System.ComponentModel;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage self publisher records.
    /// </summary>
    public class SelfPublisherViewModel : DataGridViewModel<SelfPublisherTable>
    {
        #region Properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherViewModel"/> class.
        /// </summary>
        /// <param name="owner">The VM that owns this view model.</param>
        public SelfPublisherViewModel(ApplicationViewModel owner) : base(owner)
        {
            DisplayName = Strings.CommandSelfPublisher;
            MaxCreatable = 1;
            Columns.Create("Id", SelfPublisherTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Name", SelfPublisherTable.Defs.Columns.Name);
            Columns.Create("Url", SelfPublisherTable.Defs.Columns.Url);

            var col = Columns.Create("Added", SelfPublisherTable.Defs.Columns.Added)
                .MakeDate()
                .AddToolTip(Strings.TooltipPublisherAdded);

            Columns.SetDefaultSort(col, ListSortDirection.Descending);
            Columns.Create("PC", SelfPublisherTable.Defs.Columns.Calculated.PubCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipSelfPublisherPublishedCount)
                .AddSort(null, SelfPublisherTable.Defs.Columns.Name, DataGridColumnSortBehavior.AlwaysAscending);

            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandAddPublisher, Strings.CommandAddPublisherTooltip, AddCommand, ResourceHelper.Get("ImageAdd"), VisualCommandImageSize, VisualCommandFontSize));

            AddViewSourceSortDescriptions();

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandBrowseToPublisherUrlOrClick, OpenRowCommand).AddImageResource("ImageBrowseToUrlMenu");
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
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            if (Messages.ShowYesNo(Strings.ConfirmationAddPublisher))
            {
                Table.AddDefaultRow();
                Table.Save();
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
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> command.
        /// This command opens the publisher's web site.
        /// </summary>
        /// <param name="item">The command parameter, not used.</param>
        protected override void RunOpenRowCommand(object item)
        {
            if (SelectedRow != null)
            {
                OpenHelper.OpenWebSite(null, SelectedRow[SelfPublisherTable.Defs.Columns.Url].ToString());
            }
        }

        /// <summary>
        /// Gets a value that indicates if the <see cref="DataGridViewModel{T}.OpenRowCommand"/> can run.
        /// </summary>
        /// <param name="item">Command parameter, not used</param>
        /// <returns>true if the <see cref="DataGridViewModel{T}.OpenRowCommand"/> can run; otherwise, false.</returns>
        protected override bool CanRunOpenRowCommand(object item)
        {
            return base.CanRunOpenRowCommand(item) && !string.IsNullOrEmpty(SelectedRow[SelfPublisherTable.Defs.Columns.Url].ToString());
        }

        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
        protected override void RunDeleteCommand()
        {
            if (IsSelectedRowAccessible)
            {
                if (Messages.ShowYesNo(Strings.ConfirmationDeletePublisher))
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
            return IsSelectedRowAccessible;
        }
        #endregion

        /************************************************************************/

        #region Private Methods

        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(PublishedTable.Defs.Columns.Added, ListSortDirection.Descending));
        }
        #endregion
    }
}