/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Controls;
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Panama.View;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Threading;
using TableColumns = Restless.Panama.Database.Tables.PublisherTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage publisher records.
    /// </summary>
    public class PublisherViewModel : DataRowViewModel<PublisherTable>
    {
        #region Private
        private int selectedEditSection;
        private PublisherRow selectedPublisher;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

        /// <inheritdoc/>
        public override bool ClearFilterCommandEnabled => Filters.IsAnyFilterActive;

        /// <inheritdoc/>
        public override bool OpenRowCommandEnabled => SelectedPublisher?.HasUrl() ?? false;

        /// <summary>
        /// Gets or sets the selected edit section
        /// </summary>
        public int SelectedEditSection
        {
            get => selectedEditSection;
            set => SetProperty(ref selectedEditSection, value);
        }

        /// <summary>
        /// Gets the currently selected publisher row
        /// </summary>
        public PublisherRow SelectedPublisher
        {
            get => selectedPublisher;
            private set => SetProperty(ref selectedPublisher, value);
        }

        /// <summary>
        /// Gets the submission period controller.
        /// </summary>
        public PublisherPeriodController Periods
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the submission controller for this VM.
        /// </summary>
        public PublisherSubmissionController Submissions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the submission titles controller for this VM.
        /// </summary>
        public PublisherSubmissionTitleController Titles
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the filters
        /// </summary>
        public PublisherRowFilter Filters => Config.PublisherFilter;

        /// <summary>
        /// Gets an enumerable of <see cref="CredentialRow"/> items.
        /// </summary>
        public IEnumerable<CredentialRow> Credentials => DatabaseController.Instance.GetTable<CredentialTable>().EnumerateAll();

        /// <summary>
        /// Gets or sets the selected credential item.
        /// </summary>
        public CredentialRow SelectedCredential
        {
            get;
            set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherViewModel"/> class.
        /// </summary>
        public PublisherViewModel()
        {
            Columns.Create("Id", TableColumns.Id).MakeFixedWidth(FixedWidth.W042);

            Columns.Add(CreateFlagsColumn("Flags", GetFlagGridColumns())
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W076)
                .AddToolTip(PublisherFlagsToolTip.Create(this)));

            Columns.Create("Name", TableColumns.Name);
            Columns.Create("Url", TableColumns.Url);

            Columns.Create("Added", TableColumns.Added)
                .MakeDate()
                .AddToolTip(Strings.ToolTipPublisherAdded)
                .MakeInitialSortDescending();

            Columns.Create("Last Sub", TableColumns.Calculated.LastSub)
                .MakeDate()
                .AddToolTip(Strings.TooltipPublisherLastSubmission)
                .AddSort(null, TableColumns.Name, DataGridColumnSortBehavior.AlwaysAscending)
                .SetSelectorName("Last Submission");

            Columns.Create("SC", TableColumns.Calculated.SubCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.ToolTipPublisherSubmissionCount)
                .AddSort(null, TableColumns.Name, DataGridColumnSortBehavior.AlwaysAscending)
                .SetSelectorName("Submission Count");

            Columns.RestoreColumnState(Config.PublisherGridColumnState);
            
            Commands.Add("ActiveFilter", p => Filters.SetToActive());
            Commands.Add("HaveSubFilter", p => Filters.SetToOpenSubmission());
            Commands.Add("InPeriodFilter", p => Filters.SetToInPeriod());
            Commands.Add("PayingFilter", p => Filters.SetToPaying());
            Commands.Add("FollowupFilter", p => Filters.SetToFollowup());
            Commands.Add("CopyLoginId", (o) => { CopyCredentialPart(CredentialTable.Defs.Columns.LoginId); }, CanCopyCredential);
            Commands.Add("CopyPassword", (o) => { CopyCredentialPart(CredentialTable.Defs.Columns.Password); }, CanCopyCredential);

            Periods = new PublisherPeriodController(this);
            Submissions = new PublisherSubmissionController(this);
            Titles = new PublisherSubmissionTitleController(this);

            // TODO
            // Credentials = DatabaseController.Instance.GetTable<CredentialTable>().GetCredentialList();

            /* Context menu items */
            MenuItems.AddItem(Strings.MenuItemCreatePublisher, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemBrowseToPublisherUrlOrClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandCopyLoginId, Commands["CopyLoginId"]);
            MenuItems.AddItem(Strings.CommandCopyPassword, Commands["CopyPassword"]);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemDeletePublisher, DeleteCommand).AddIconResource(ResourceKeys.Icon.XRedIconKey);

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                SelectedEditSection = 1;
                Filters.SetListView(ListView);
                Filters.ApplyFilter();
            }));
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
            SelectedPublisher = PublisherRow.Create(SelectedRow);
            Periods.Update();
            Submissions.Update();
            Titles.Update();
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return Filters?.OnDataRowFilter(item) ?? false;
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.Added);
        }

        /// <inheritdoc/>
        protected override void RunClearFilterCommand()
        {
            Filters.ClearAll();
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
                Filters.ClearAll();
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
                int childRowCount = SelectedRow.GetChildRows(PublisherTable.Defs.Relations.ToSubmissionBatch).Length;
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
            Config.PublisherGridColumnState = Columns.GetColumnState();
        }

        /// <inheritdoc/>
        protected override void OnClosing()
        {
            base.OnClosing();
            SignalSave();
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private bool CanCopyCredential(object o)
        {
            return SelectedCredential != null && SelectedCredential.Id != 0;
        }

        private void CopyCredentialPart(string columnName)
        {
            if (SelectedCredential != null && SelectedCredential.Id != 0)
            {
                Clipboard.SetText(SelectedCredential.Row[columnName].ToString());
                MainWindowViewModel.Instance.CreateNotificationMessage($"{columnName} copied to clipboard");
            }
        }

        private FlagGridColumnCollection GetFlagGridColumns()
        {
            return new FlagGridColumnCollection(this)
            {
                { TableColumns.Exclusive, Config.Colors.PublisherExclusive.ToBindingPath() },
                { TableColumns.Paying, Config.Colors.PublisherPaying.ToBindingPath() },
                { TableColumns.Goner, Config.Colors.PublisherGoner.ToBindingPath() },
                { TableColumns.Calculated.HaveActiveSubmission, Config.Colors.PublisherActiveSubmission.ToBindingPath() },
                { TableColumns.Calculated.InSubmissionPeriod, Config.Colors.PublisherPeriod.ToBindingPath() },
            };
        }
        #endregion
    }
}