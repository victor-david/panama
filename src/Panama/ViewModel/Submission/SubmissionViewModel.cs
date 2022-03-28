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
using System;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the submission view.
    /// </summary>
    public class SubmissionViewModel : DataGridViewModel<SubmissionBatchTable>
    {
        #region Private
        private int selectedEditSection;
        private SubmissionBatchRow selectedBatch;
        private string submissionHeader;
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
        /// Gets the selected submission batch
        /// </summary>
        public SubmissionBatchRow SelectedBatch
        {
            get => selectedBatch;
            private set => SetProperty(ref selectedBatch, value);
        }

        /// <summary>
        /// Gets the header text for the submission.
        /// </summary>
        public string SubmissionHeader
        {
            get => submissionHeader;
            private set => SetProperty(ref submissionHeader, value);
        }

        /// <summary>
        /// Gets the controller that handles submission titles.
        /// </summary>
        public SubmissionTitleController Titles
        {
            get;
        }

        /// <summary>
        /// Gets the controller that handles submission documents.
        /// </summary>
        public SubmissionDocumentController Documents
        {
            get;
        }

        /// <summary>
        /// Gets the controller that handles submission messages.
        /// </summary>
        public SubmissionMessageController Messages
        {
            get;
        }

        /// <summary>
        /// Gets the controller that handles submission dates.
        /// </summary>
        public SubmissionDateController Dates
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionViewModel"/> class.
        /// </summary>
        public SubmissionViewModel()
        {
            Columns.Create("Id", SubmissionBatchTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.W042);

            Columns.CreateResource<BooleanToPathConverter>("R", SubmissionBatchTable.Defs.Columns.Online, ResourceKeys.Icon.SquareSmallGreenIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipSubmissionOnline);

            Columns.CreateResource<BooleanToPathConverter>("C", SubmissionBatchTable.Defs.Columns.Contest, ResourceKeys.Icon.SquareSmallGrayIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipSubmissionContest);

            Columns.CreateResource<BooleanToPathConverter>("L", SubmissionBatchTable.Defs.Columns.Locked, ResourceKeys.Icon.SquareSmallRedIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipSubmissionLocked);

            DataGridColumn col = Columns.Create("Submitted", SubmissionBatchTable.Defs.Columns.Submitted)
                .MakeDate();
            //.AddSort(SubmissionBatchTable.Defs.Columns.Calculated.Submitted, SubmissionBatchTable.Defs.Columns.Submitted, DataGridColumnSortBehavior.FollowPrimary);

            Columns.SetDefaultSort(col, ListSortDirection.Descending);

            Columns.Create("Response", SubmissionBatchTable.Defs.Columns.Response).MakeDate();
            Columns.Create("Type", SubmissionBatchTable.Defs.Columns.Joined.ResponseTypeName).MakeFixedWidth(FixedWidth.W096);

            // string.Empty because VS gets confused and tries to connect to the wrong overload
            Columns.Create<DatesToDayDiffConverter>("Days", SubmissionBatchTable.Defs.Columns.Submitted, SubmissionBatchTable.Defs.Columns.Response, string.Empty)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W052);

            Columns.Create("Publisher", SubmissionBatchTable.Defs.Columns.Joined.Publisher);

            Columns.Create("Fee", SubmissionBatchTable.Defs.Columns.Fee)
                .MakeNumeric("N2", FixedWidth.W052);

            Columns.Create("Award", SubmissionBatchTable.Defs.Columns.Award)
                .MakeNumeric("N0", FixedWidth.W052);

            Columns.Create("Note", SubmissionBatchTable.Defs.Columns.Notes)
                .MakeSingleLine();

            //Commands.Add("FilterToPublisher", RunFilterToPublisherCommand, (o) => IsSelectedRowAccessible);
            //Commands.Add("ActiveFilter", (o) => { FilterText = "--"; });
            //Commands.Add("TryAgainFilter", (o) => { FilterText = "Try Again"; });
            //Commands.Add("PersonalNoteFilter", (o) => { FilterText = "Personal Note"; });
            //Commands.Add("ClearFilter", (o) => { FilterText = null; });

            // TODO
            //double minWidth = 80.0;
            //double imgSize = 20.0;
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandSubmissionFilterActive, Strings.CommandSubmissionFilterActiveTooltip, Commands["ActiveFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandSubmissionFilterTryAgain, Strings.CommandSubmissionFilterTryAgainTooltip, Commands["TryAgainFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandSubmissionFilterPersonalNote, Strings.CommandSubmissionFilterPersonalNoteTooltip, Commands["PersonalNoteFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandClearFilter, Strings.CommandClearFilterTooltip, Commands["ClearFilter"], null, imgSize, VisualCommandFontSize, minWidth));

            /* Context menu items */
            MenuItems.AddItem(Strings.MenuItemCreateSubmission, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemBrowseToPublisherUrl, OpenRowCommand).AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);
            MenuItems.AddItem(Strings.MenuItemFilterToPublisher, Commands["FilterToPublisher"]).AddIconResource(ResourceKeys.Icon.FilterIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemDeleteSubmission, DeleteCommand).AddIconResource(ResourceKeys.Icon.XRedIconKey);

            Titles = new SubmissionTitleController(this);
            Documents = new SubmissionDocumentController(this);
            Messages = new SubmissionMessageController(this);
            Dates = new SubmissionDateController(this);

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                SelectedEditSection = 1;
            }));
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Receives notification when a data record has been added.
        /// A new submission record is added from the PublisherViewModel
        /// </summary>
        public override void OnRecordAdded()
        {
            Columns.RestoreDefaultSort();
            ForceListViewSort();
        }

        /// <summary>
        /// Sets the <see cref="SubmissionHeader"/> property.
        /// </summary>
        /// <remarks>
        /// This method is called when the selected row changes or when the <see cref="Dates"/> controller
        /// updates its submitted date.
        /// </remarks>
        public void SetSubmissionHeader()
        {
            string header = null;
            if (SelectedRow != null && SelectedRow[SubmissionBatchTable.Defs.Columns.Submitted] is DateTime dt)
            {
                string dateStr = dt.ToLocalTime().ToString(Config.Instance.DateFormat);
                header = $"{dateStr} to {SelectedRow[SubmissionBatchTable.Defs.Columns.Joined.Publisher]}";
            }
            SubmissionHeader = header;
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedBatch = SubmissionBatchRow.Create(SelectedRow);
            SetSubmissionHeader();
            Titles.Update();
            Documents.Update();
            Messages.Update();
            Dates.Update();
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, SubmissionBatchTable.Defs.Columns.Submitted);
        }

        /// <inheritdoc/>
        protected override void RunOpenRowCommand()
        {
            if (CanRunOpenRowCommand())
            {
                OpenHelper.OpenWebSite(null, SelectedBatch.PublisherUrl);
            }
        }

        /// <inheritdoc/>
        protected override bool CanRunOpenRowCommand()
        {
            return base.CanRunOpenRowCommand() && (SelectedBatch?.HasPublisherUrl ?? false);
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            if (WindowFactory.PublisherSelect.Create().GetPublisher() is PublisherRow publisher)
            {
                int openCount = Table.OpenSubmissionCount(publisher.Id);
                string msg = openCount == 0 ?
                    string.Format(CultureInfo.InvariantCulture, Strings.FormatStringCreateSubmission, publisher.Name) :
                    string.Format(CultureInfo.InvariantCulture, Strings.FormatStringCreateSubmissionOpen, publisher.Name);

                if (MessageWindow.ShowYesNo(msg))
                {
                    Table.CreateSubmission(publisher.Id);
                    MainWindowViewModel.Instance.CreateNotificationMessage(Strings.ResultSubmissionCreated);
                    Columns.RestoreDefaultSort();
                    ForceListViewSort();
                }
            }
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (CanRunDeleteCommand() && MessageWindow.ShowYesNo(Strings.ConfirmationDeleteSubmission))
            {
                // Call the DeleteSubmission() method to delete and perform other cleanup.
                Table.DeleteSubmission(SelectedBatch);
                ListView.Refresh();
            }
        }

        /// <inheritdoc/>
        protected override bool CanRunDeleteCommand()
        {
            return IsSelectedRowAccessible && !(SelectedBatch?.IsLocked ?? true);
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void RunFilterToPublisherCommand(object o)
        {
            // TODO
        }
        #endregion
    }
}