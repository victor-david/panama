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
using Restless.Toolkit.Core.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage publisher records.
    /// </summary>
    public class PublisherViewModel : DataGridViewModel<PublisherTable>
    {
        #region Private
        private int selectedEditSection;
        private PublisherRow selectedPublisher;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

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
        /// Gets an enumerable of <see cref="CredentialTable.RowObject"/> items. The UI binds to this list.
        /// </summary>
        public IEnumerable<CredentialTable.RowObject> Credentials
        {
            get => DatabaseController.Instance.GetTable<CredentialTable>().EnumerateCredentials();
        }

        /// <summary>
        /// Gets or sets the selected credential item.
        /// </summary>
        public CredentialTable.RowObject SelectedCredential
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
            Columns.Create("Id", PublisherTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.W042);

            Columns.CreateResource<BooleanToPathConverter>("P", PublisherTable.Defs.Columns.Calculated.InSubmissionPeriod, ResourceKeys.Icon.SquareSmallBlueIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipPublisherInPeriod);

            Columns.CreateResource<BooleanToPathConverter>("E", PublisherTable.Defs.Columns.Exclusive, ResourceKeys.Icon.SquareSmallRedIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipPublisherExclusive);

            Columns.CreateResource<BooleanToPathConverter>("P", PublisherTable.Defs.Columns.Paying, ResourceKeys.Icon.SquareSmallGreenIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipPublisherPay);

            Columns.Create("Name", PublisherTable.Defs.Columns.Name);
            Columns.Create("Url", PublisherTable.Defs.Columns.Url);

            DataGridColumn column = Columns.Create("Added", PublisherTable.Defs.Columns.Added)
                .MakeDate()
                .AddToolTip(Strings.ToolTipPublisherAdded);

            Columns.SetDefaultSort(column, ListSortDirection.Descending);

            Columns.CreateResource<BooleanToPathConverter>("A", PublisherTable.Defs.Columns.Calculated.HaveActiveSubmission, ResourceKeys.Icon.SquareSmallGrayIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipPublisherHasActive);

            Columns.Create("Last Sub", PublisherTable.Defs.Columns.Calculated.LastSub)
                .MakeDate()
                .AddToolTip(Strings.TooltipPublisherLastSubmission)
                .AddSort(null, PublisherTable.Defs.Columns.Name, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("SC", PublisherTable.Defs.Columns.Calculated.SubCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.ToolTipPublisherSubmissionCount)
                .AddSort(null, PublisherTable.Defs.Columns.Name, DataGridColumnSortBehavior.AlwaysAscending);

            Commands.Add("ActiveFilter", p => Filters.SetToActive());
            Commands.Add("HaveSubFilter", p => Filters.SetToOpenSubmission());
            Commands.Add("InPeriodFilter", p => Filters.SetToInPeriod());
            Commands.Add("PayingFilter", p => Filters.SetToPaying());
            Commands.Add("FollowupFilter", p => Filters.SetToFollowup());
            Commands.Add("ToggleCustomFilter", o => IsCustomFilterOpen = !IsCustomFilterOpen);
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
            return DataRowCompareDateTime(item2, item1, PublisherTable.Defs.Columns.Added);
        }

        /// <inheritdoc/>
        protected override void RunClearFilterCommand()
        {
            Filters.ClearAll();
        }

        /// <inheritdoc/>
        protected override bool CanRunClearFilterCommand()
        {
            return Filters.IsAnyFilterActive;
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
                Columns.RestoreDefaultSort();
                ForceListViewSort();
            }
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
        #endregion
    }
}