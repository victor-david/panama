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
using Restless.Toolkit.Mvvm;
using System;
using System.ComponentModel;
using System.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the submission view.
    /// </summary>
    public class SubmissionViewModel : DataGridViewModel<SubmissionBatchTable>
    {
        #region Private
        private string submissionHeader;
        #endregion

        /************************************************************************/

        #region Properties
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
            private set;
        }

        /// <summary>
        /// Gets the controller that handles submission documents.
        /// </summary>
        public SubmissionDocumentController Documents
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the controller that handles submission messages.
        /// </summary>
        public SubmissionMessageController Messages
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the controller that handles submission dates.
        /// </summary>
        public SubmissionSubmittedController Submitted
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the controller that handles submission responses.
        /// </summary>
        public SubmissionResponseController Response
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionViewModel"/> class.
        /// </summary>
        public SubmissionViewModel()
        {
            DisplayName = Strings.CommandSubmission;
            MaxCreatable = 1;
            Columns.Create("Id", SubmissionBatchTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.CreateImage<BooleanToImageConverter>("O", SubmissionBatchTable.Defs.Columns.Online)
                .AddToolTip(Strings.TooltipSubmissionOnline);
            Columns.CreateImage<BooleanToImageConverter>("C", SubmissionBatchTable.Defs.Columns.Contest)
                .AddToolTip(Strings.TooltipSubmissionContest);
            Columns.CreateImage<BooleanToImageConverter>("L", SubmissionBatchTable.Defs.Columns.Locked)
                .AddToolTip(Strings.TooltipSubmissionLocked);
            var col = Columns.Create("Submitted", SubmissionBatchTable.Defs.Columns.Submitted)
                .MakeDate()
                .AddSort(SubmissionBatchTable.Defs.Columns.Calculated.Submitted, SubmissionBatchTable.Defs.Columns.Submitted, DataGridColumnSortBehavior.FollowPrimary);
            Columns.SetDefaultSort(col, ListSortDirection.Descending);
            Columns.Create("Response", SubmissionBatchTable.Defs.Columns.Response).MakeDate();
            Columns.Create("Type", SubmissionBatchTable.Defs.Columns.Joined.ResponseTypeName).MakeFixedWidth(FixedWidth.MediumString);
            // string.Empty because VS gets confused and tries to connect to the wromg overload
            Columns.Create<DatesToDayDiffConverter>("Days", SubmissionBatchTable.Defs.Columns.Submitted, SubmissionBatchTable.Defs.Columns.Response, string.Empty).MakeCentered().MakeFixedWidth(FixedWidth.MediumNumeric);
            Columns.Create("Publisher", SubmissionBatchTable.Defs.Columns.Joined.Publisher);
            Columns.Create("Fee", SubmissionBatchTable.Defs.Columns.Fee).MakeNumeric("N2", FixedWidth.MediumNumeric);
            Columns.Create("Award", SubmissionBatchTable.Defs.Columns.Award).MakeNumeric("N0", FixedWidth.MediumNumeric);
            Columns.Create("Note", SubmissionBatchTable.Defs.Columns.Notes).MakeSingleLine();

            AddViewSourceSortDescriptions();

            Commands.Add("FilterToPublisher", RunFilterToPublisherCommand, (o) => IsSelectedRowAccessible);
            Commands.Add("ActiveFilter", (o) => { FilterText = "--"; });
            Commands.Add("TryAgainFilter", (o) => { FilterText = "Try Again"; });
            Commands.Add("PersonalNoteFilter", (o) => { FilterText = "Personal Note"; });
            Commands.Add("ClearFilter", (o) => { FilterText = null; });

            // TODO
            //double minWidth = 80.0;
            //double imgSize = 20.0;
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandSubmissionFilterActive, Strings.CommandSubmissionFilterActiveTooltip, Commands["ActiveFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandSubmissionFilterTryAgain, Strings.CommandSubmissionFilterTryAgainTooltip, Commands["TryAgainFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandSubmissionFilterPersonalNote, Strings.CommandSubmissionFilterPersonalNoteTooltip, Commands["PersonalNoteFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandClearFilter, Strings.CommandClearFilterTooltip, Commands["ClearFilter"], null, imgSize, VisualCommandFontSize, minWidth));

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandBrowseToPublisherUrl, OpenRowCommand).AddImageResource("ImageBrowseToUrlMenu");
            MenuItems.AddItem(Strings.CommandFilterToPublisher, Commands["FilterToPublisher"]).AddImageResource("ImageFilterMenu");

            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandDeleteSubmission, DeleteCommand).AddImageResource("ImageDeleteMenu");

            Titles = new SubmissionTitleController(this);
            Documents = new SubmissionDocumentController(this);
            Messages = new SubmissionMessageController(this);
            Response = new SubmissionResponseController(this);
            Submitted = new SubmissionSubmittedController(this);
            FilterPrompt = Strings.FilterPromptSubmission;
            FilterText = Config.SubmissionFilter;
            AddCommand.Supported = CommandSupported.NoWithException;
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
            AddViewSourceSortDescriptions();
            Columns.RestoreDefaultSort();
        }

        /// <summary>
        /// Sets the <see cref="SubmissionHeader"/> property.
        /// </summary>
        /// <remarks>
        /// This method is called when the selected row changes or when the <see cref="Submitted"/> controller
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
        /// <summary>
        /// Called when the filter text has changed to set the filter on the underlying data.
        /// </summary>
        /// <param name="text">The filter text.</param>
        protected override void OnFilterTextChanged(string text)
        {
            string filter = string.Empty;
            if (text.StartsWith("-"))
            {
                filter = $"{SubmissionBatchTable.Defs.Columns.Joined.ResponseTypeName} IS NULL";
            }
            else
            {
                filter = $"{SubmissionBatchTable.Defs.Columns.Joined.Publisher} LIKE '%{text}%' OR {SubmissionBatchTable.Defs.Columns.Joined.ResponseTypeName} LIKE '%{text}%'";
            }
            DataView.RowFilter = filter;
            // Note: save FilterText, not text. When applied again, it will be sanitized.
            Config.SubmissionFilter = FilterText;
        }

        /// <summary>
        /// Called when the filter text is cleared.
        /// </summary>
        protected override void OnFilterTextCleared()
        {
            Config.SubmissionFilter = null;
        }

        /// <summary>
        /// Called when the selected item on the associated data grid has changed.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SetSubmissionHeader();
            Titles.Update();
            Documents.Update();
            Messages.Update();
            Response.Update();
            Submitted.Update();
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
                OpenHelper.OpenWebSite(null, SelectedRow[SubmissionBatchTable.Defs.Columns.Joined.PublisherUrl].ToString());
            }
        }

        /// <summary>
        /// Gets a value that indicates if the <see cref="DataGridViewModel{T}.OpenRowCommand"/> can run.
        /// </summary>
        /// <param name="item">Command parameter, not used</param>
        /// <returns>true if the <see cref="DataGridViewModel{T}.OpenRowCommand"/> can run; otherwise, false.</returns>
        protected override bool CanRunOpenRowCommand(object item)
        {
            return
                    base.CanRunOpenRowCommand(item) &&
                    !string.IsNullOrEmpty(SelectedRow[SubmissionBatchTable.Defs.Columns.Joined.PublisherUrl].ToString());
        }

        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
        protected override void RunDeleteCommand()
        {
            if (CanRunDeleteCommand() && Toolkit.Utility.Messages.ShowYesNo(Strings.ConfirmationDeleteSubmission))
            {
                // Call the DeleteSubmission() method to delete and perform other cleanup.
                DatabaseController.Instance.GetTable<SubmissionBatchTable>().DeleteSubmission(SelectedRow);
            }
        }

        /// <summary>
        /// Called when the framework checks to see if Delete command can execute
        /// </summary>
        /// <returns>true if a row is selected and the submission is unlocked; otherwise, false.</returns>
        protected override bool CanRunDeleteCommand()
        {
            return IsSelectedRowAccessible && !(bool)SelectedRow[SubmissionBatchTable.Defs.Columns.Locked];
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void RunFilterToPublisherCommand(object o)
        {
            FilterText = SelectedRow[SubmissionBatchTable.Defs.Columns.Joined.Publisher].ToString();
        }

        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(SubmissionBatchTable.Defs.Columns.Calculated.Submitted, ListSortDirection.Descending));
            MainSource.SortDescriptions.Add(new SortDescription(SubmissionBatchTable.Defs.Columns.Submitted, ListSortDirection.Descending));
        }
        #endregion
    }
}