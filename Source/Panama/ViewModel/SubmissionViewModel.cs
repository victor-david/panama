using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System.Windows.Controls;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the submission view.
    /// </summary>
    public class SubmissionViewModel : DataGridViewModel<SubmissionBatchTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
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
            Columns.Create<DatesToDayDiffConverter>("Days", SubmissionBatchTable.Defs.Columns.Submitted, SubmissionBatchTable.Defs.Columns.Response).MakeCentered().MakeFixedWidth(FixedWidth.MediumNumeric);
            Columns.Create("Publisher", SubmissionBatchTable.Defs.Columns.Joined.Publisher);
            Columns.Create("Fee", SubmissionBatchTable.Defs.Columns.Fee).MakeNumeric("N2", FixedWidth.MediumNumeric);
            Columns.Create("Award", SubmissionBatchTable.Defs.Columns.Award).MakeNumeric("N0", FixedWidth.MediumNumeric);
            Columns.Create("Note", SubmissionBatchTable.Defs.Columns.Notes).MakeSingleLine();

            AddViewSourceSortDescriptions();

            Commands.Add("FilterToPublisher", RunFilterToPublisherCommand, CanRunCommandIfRowSelected);
            Commands.Add("GridSorting", (o) => { SortItemSource(o as DataGridBoundColumn); });

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandBrowseToPublisherUrl, OpenRowCommand, "ImageBrowseToUrlMenu");
            MenuItems.AddItem(Strings.CommandFilterToPublisher, Commands["FilterToPublisher"], "ImageFilterMenu");

            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandDeleteSubmission, DeleteCommand, "ImageDeleteMenu");

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
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called when the filter text has changed to set the filter on the underlying data.
        /// </summary>
        /// <param name="text">The filter text.</param>
        protected override void OnFilterTextChanged(string text)
        {
            string filter = String.Empty;
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
                (
                    base.CanRunOpenRowCommand(item) &&
                    !String.IsNullOrEmpty(SelectedRow[SubmissionBatchTable.Defs.Columns.Joined.PublisherUrl].ToString())
                );
        }


        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
        protected override void RunDeleteCommand()
        {
            if (SelectedRow != null && Restless.Tools.Utility.Messages.ShowYesNo(Strings.ConfirmationDeleteSubmission))
            {
                SelectedRow.Delete();
                DatabaseController.Instance.Save();
            }
        }

        /// <summary>
        /// Called when the framework checks to see if Delete command can execute
        /// </summary>
        /// <returns>true if a row is selected and the submission is unlocked; otherwise, false.</returns>
        protected override bool CanRunDeleteCommand()
        {
            return CanRunCommandIfRowSelected(null) && !(bool)SelectedRow[SubmissionBatchTable.Defs.Columns.Locked];
        }
        #endregion

        /************************************************************************/
        
        #region Private Methods

        private void SortItemSource(DataGridBoundColumn col)
        {
            //if (col
        }

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