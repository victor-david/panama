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
using Restless.Toolkit.Utility;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage publisher records.
    /// </summary>
    public class PublisherViewModel : DataGridViewModel<PublisherTable>
    {
        #region Private
        private bool isFilterVisible;
        #endregion

        /************************************************************************/

        #region Properties
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
        /// Gets the filters controller for this VM
        /// </summary>
        public PublisherFilterController Filters
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a visibility value that determines if the title filter is visible.
        /// </summary>
        public Visibility FilterVisibility
        {
            get => (isFilterVisible) ? Visibility.Visible : Visibility.Collapsed;
        }

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
            DisplayName = Strings.CommandPublisher;
            Columns.Create("Id", PublisherTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.CreateImage<BooleanToImageConverter>("P", PublisherTable.Defs.Columns.Calculated.InSubmissionPeriod).AddToolTip(Strings.TooltipPublisherInPeriod);
            Columns.CreateImage<BooleanToImageConverter>("E", PublisherTable.Defs.Columns.Exclusive, "ImageExclamation").AddToolTip(Strings.TooltipPublisherExclusive);
            Columns.CreateImage<BooleanToImageConverter>("P", PublisherTable.Defs.Columns.Paying, "ImageMoney").AddToolTip(Strings.TooltipPublisherPay);

            Columns.Create("Name", PublisherTable.Defs.Columns.Name);
            Columns.Create("Url", PublisherTable.Defs.Columns.Url);

            var col = Columns.Create("Added", PublisherTable.Defs.Columns.Added)
                .MakeDate()
                .AddToolTip(Strings.TooltipPublisherAdded);

            Columns.SetDefaultSort(col, ListSortDirection.Descending);
            Columns.CreateImage<BooleanToImageConverter>("A", PublisherTable.Defs.Columns.Calculated.HaveActiveSubmission, "ImageExclamation")
                .AddToolTip(Strings.TooltipPublisherHasActive);

            Columns.Create("Last Sub", PublisherTable.Defs.Columns.Calculated.LastSub)
                .MakeDate()
                .AddToolTip(Strings.TooltipPublisherLastSubmission)
                .AddSort(null, PublisherTable.Defs.Columns.Name, DataGridColumnSortBehavior.AlwaysAscending);
            Columns.Create("SC", PublisherTable.Defs.Columns.Calculated.SubCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipPublisherSubmissionCount)
                .AddSort(null, PublisherTable.Defs.Columns.Name, DataGridColumnSortBehavior.AlwaysAscending);

            AddViewSourceSortDescriptions();

            /* This command is used from this model and from the Filters controller */
            Commands.Add("ClearFilter", (o) => Filters.ClearAll(), (o) => Config.PublisherFilter.IsAnyFilterActive);
            Commands.Add("ActiveFilter", (o) => Filters.SetToActive());
            Commands.Add("HaveSubFilter", (o) => Filters.SetToHaveSubmission());
            Commands.Add("InPeriodFilter", (o) => Filters.SetToInPeriod());
            Commands.Add("PayingFilter", (o) => Filters.SetToPaying());
            Commands.Add("FollowupFilter", (o) => Filters.SetToFollowup());
            // TODO
            //Commands.Add("AdvancedFilter", (o) =>
            //{
            //    isFilterVisible = !isFilterVisible;
            //    advFilter.Icon = (isFilterVisible) ? ResourceHelper.Get("ImageChevronUp") : ResourceHelper.Get("ImageChevronDown");
            //    OnPropertyChanged(nameof(FilterVisibility));
            //});

            Commands.Add("AddSubmission", RunAddSubmissionCommand, (o) => IsSelectedRowAccessible);
            Commands.Add("CopyLoginId", (o) => { CopyCredentialPart(CredentialTable.Defs.Columns.LoginId); }, CanCopyCredential);
            Commands.Add("CopyPassword", (o) => { CopyCredentialPart(CredentialTable.Defs.Columns.Password); }, CanCopyCredential);

            // TODO
            //double minWidth = 80.0;
            //double imgSize = 20.0;
            //advFilter = new VisualCommandViewModel(Strings.CommandFilterAdvanced, Strings.CommandFilterAdvancedTooltip, Commands["AdvancedFilter"], ResourceHelper.Get("ImageChevronDown"), imgSize, VisualCommandFontSize, 100.0);
            //FilterCommands.Add(advFilter);
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandPublisherFilterActive, Strings.CommandPublisherFilterActiveTooltip, Commands["ActiveFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandPublisherFilterHaveSub, Strings.CommandPublisherFilterHaveSubTooltip, Commands["HaveSubFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandPublisherFilterInPeriod, Strings.CommandPublisherFilterInPeriodTooltip, Commands["InPeriodFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandPublisherFilterPaying, Strings.CommandPublisherFilterPayingTooltip, Commands["PayingFilter"],null, imgSize, VisualCommandFontSize,  minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandPublisherFilterFollowup, Strings.CommandPublisherFilterFollowupTooltip, Commands["FollowupFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandClearFilter, Strings.CommandClearFilterTooltip, Commands["ClearFilter"], null, imgSize, VisualCommandFontSize, minWidth));

            Periods = new PublisherPeriodController(this);
            Submissions = new PublisherSubmissionController(this);
            Titles = new PublisherSubmissionTitleController(this);

            // Credentials = DatabaseController.Instance.GetTable<CredentialTable>().GetCredentialList();

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandCreateSubmission, Commands["AddSubmission"]).AddImageResource("ImageSubmissionMenu");
            MenuItems.AddItem(Strings.CommandBrowseToPublisherUrlOrClick, OpenRowCommand).AddImageResource("ImageBrowseToUrlMenu");
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandCopyLoginId, Commands["CopyLoginId"]);
            MenuItems.AddItem(Strings.CommandCopyPassword, Commands["CopyPassword"]);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandDeletePublisher, DeleteCommand).AddImageResource("ImageDeleteMenu");

            Filters = new PublisherFilterController(this);
            Filters.Apply();
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
            Periods.Update();
            Submissions.Update();
            Titles.Update();
        }

        /// <summary>
        /// Called when the filter text has changed to set the filter on the underlying data.
        /// </summary>
        /// <param name="text">The filter text.</param>
        protected override void OnFilterTextChanged(string text)
        {
            DataView.RowFilter = string.Format("{0} LIKE '%{1}%'", PublisherTable.Defs.Columns.Name, text);
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
                Filters.ClearAll();
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
                OpenHelper.OpenWebSite(null, SelectedRow[PublisherTable.Defs.Columns.Url].ToString());
            }
        }

        /// <summary>
        /// Gets a value that indicates if the <see cref="DataGridViewModel{T}.OpenRowCommand"/> can run.
        /// </summary>
        /// <param name="item">Command parameter, not used</param>
        /// <returns>true if the <see cref="DataGridViewModel{T}.OpenRowCommand"/> can run; otherwise, false.</returns>
        protected override bool CanRunOpenRowCommand(object item)
        {
            return base.CanRunOpenRowCommand(item) && !string.IsNullOrEmpty(SelectedRow[PublisherTable.Defs.Columns.Url].ToString());
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
                    Messages.ShowError(string.Format(Strings.InvalidOpCannotDeletePublisher, childRowCount));
                    return;
                }
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
        private void RunAddSubmissionCommand(object o)
        {
            if (SelectedRow != null)
            {
                long pubId = (long)SelectedRow[PublisherTable.Defs.Columns.Id];
                string pubName = SelectedRow[PublisherTable.Defs.Columns.Name].ToString();
                int openCount = DatabaseController.Instance.GetTable<SubmissionBatchTable>().OpenSubmissionCount(pubId);
                string msg = (openCount == 0) ? string.Format(Strings.FormatStringCreateSubmission, pubName) : string.Format(Strings.FormatStringCreateSubmissionOpen, pubName);
                if (Messages.ShowYesNo(msg))
                {
                    DatabaseController.Instance.GetTable<SubmissionBatchTable>().CreateSubmission(pubId);
                    MainWindowViewModel.Instance.CreateNotificationMessage(Strings.ResultSubmissionCreated);
                    MainWindowViewModel.Instance.NotifyWorkspaceOnRecordAdded<SubmissionViewModel>();
                    MainWindowViewModel.Instance.SwitchToWorkspace<SubmissionViewModel>();
                }
            }
        }

        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(PublishedTable.Defs.Columns.Added, ListSortDirection.Descending));
        }

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