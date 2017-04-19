using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System.Windows.Media;
using Restless.App.Panama.Converters;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage publisher records.
    /// </summary>
    public class PublisherViewModel : DataGridViewModel<PublisherTable>
    {
        #region Private
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
        /// Gets a list of of <see cref="CredentialTable.RowObject"/> items. The UI binds to this list.
        /// </summary>
        public List<CredentialTable.RowObject> Credentials
        {
            get;
            private set;
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
        #pragma warning disable 1591
        public PublisherViewModel()
        {
            DisplayName = Strings.CommandPublisher;
            MaxCreatable = 1;
            Columns.Create("Id", PublishedTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.CreateImage<BooleanToImageConverter>("P", PublisherTable.Defs.Columns.Calculated.InSubmissionPeriod);
            Columns.CreateImage<BooleanToImageConverter>("P", PublisherTable.Defs.Columns.Paying, "ImageMoney");
            Columns.Create("Name", PublisherTable.Defs.Columns.Name);
            Columns.Create("Url", PublisherTable.Defs.Columns.Url);
            Columns.SetDefaultSort(Columns.Create("Added", PublisherTable.Defs.Columns.Added).MakeDate()
                .AddToolTip(Strings.TooltipPublisherAdded), ListSortDirection.Descending);
            Columns.Create("Last Sub", PublisherTable.Defs.Columns.Calculated.LastSub).MakeDate()
                .AddToolTip(Strings.TooltipPublisherLastSubmission)
                .AddSort(null, PublisherTable.Defs.Columns.Name, DataGridColumnSortBehavior.AlwaysAscending);
            Columns.Create("SC", PublisherTable.Defs.Columns.Calculated.SubCount).MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipPublisherSubmissionCount)
                .AddSort(null, PublisherTable.Defs.Columns.Name, DataGridColumnSortBehavior.AlwaysAscending);
            Columns.Create("PC", PublisherTable.Defs.Columns.Calculated.SubPeriodCount).MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipPublisherSubmissionPeriodCount)
                .AddSort(null, PublisherTable.Defs.Columns.Name, DataGridColumnSortBehavior.AlwaysAscending);
            AddViewSourceSortDescriptions();
            
            RawCommands.Add("AddSubmission", RunAddSubmissionCommand, CanRunCommandIfRowSelected);
            /* This command is used from this model and from the Filters controller */
            RawCommands.Add("ClearFilter", (o) => { Filters.ClearAll(); }, (o) => { return Config.PublisherFilter.IsAnyFilterActive; });

            RawCommands.Add("CopyLoginId", (o) => { CopyCredentialPart(CredentialTable.Defs.Columns.LoginId); }, CanCopyCredential);
            RawCommands.Add("CopyPassword", (o) => { CopyCredentialPart(CredentialTable.Defs.Columns.Password); }, CanCopyCredential);

            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandAddPublisher, Strings.CommandAddPublisherTooltip, AddCommand, ResourceHelper.Get("ImageAdd"), VisualCommandImageSize, VisualCommandFontSize));
            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandClearFilter, Strings.CommandClearFilterTooltip, RawCommands["ClearFilter"], ResourceHelper.Get("ImageFilter"), VisualCommandImageSize, VisualCommandFontSize));

            Periods = new PublisherPeriodController(this);
            Submissions = new PublisherSubmissionController(this);
            Titles = new PublisherSubmissionTitleController(this);

            Credentials = DatabaseController.Instance.GetTable<CredentialTable>().GetCredentialList();

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandCreateSubmission, RawCommands["AddSubmission"], "ImageSubmissionMenu");
            MenuItems.AddItem(Strings.CommandBrowseToPublisherUrlOrClick, OpenRowCommand, "ImageBrowseToUrlMenu");
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandCopyLoginId, RawCommands["CopyLoginId"]);
            MenuItems.AddItem(Strings.CommandCopyPassword, RawCommands["CopyPassword"]);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandDeletePublisher, DeleteCommand, "ImageDeleteMenu");

            //FilterPrompt = Strings.FilterPromptPublisher;
            Filters = new PublisherFilterController(this);
            Filters.Apply();
        }
        #pragma warning restore 1591
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
            DataView.RowFilter = String.Format("{0} LIKE '%{1}%'", PublisherTable.Defs.Columns.Name, text);
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            Table.AddDefaultRow();
            Table.Save();
            FilterText = null;
            AddViewSourceSortDescriptions();
            Columns.RestoreDefaultSort();
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
            return 
                (
                    base.CanRunOpenRowCommand(item) && 
                    !String.IsNullOrEmpty(SelectedRow[PublisherTable.Defs.Columns.Url].ToString())
                );
        }


        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
        protected override void RunDeleteCommand()
        {
            int childRowCount = SelectedRow.GetChildRows(PublisherTable.Defs.Relations.ToSubmissionBatch).Length;
            if (childRowCount > 0)
            {
                Messages.ShowError(String.Format(Strings.InvalidOpCannotDeletePublisher, childRowCount));
                return;
            }
            if (Messages.ShowYesNo(Strings.ConfirmationDeletePublisher))
            {
                SelectedRow.Delete();
                Table.Save();
            }
        }

        /// <summary>
        /// Called when the framework checks to see if Delete command can execute
        /// </summary>
        /// <returns>true if a row is selected; otherwise, false.</returns> 
        protected override bool CanRunDeleteCommand()
        {
            return CanRunCommandIfRowSelected(null);
        }
        #endregion

        /************************************************************************/
        
        #region Private Methods
        private void RunAddSubmissionCommand(object o)
        {
            if (SelectedRow != null)
            {
                Int64 pubId = (Int64)SelectedRow[PublisherTable.Defs.Columns.Id];
                string pubName = SelectedRow[PublisherTable.Defs.Columns.Name].ToString();
                int openCount = DatabaseController.Instance.GetTable<SubmissionBatchTable>().OpenSubmissionCount(pubId);
                string msg = (openCount == 0) ? String.Format(Strings.FormatStringCreateSubmission, pubName) : String.Format(Strings.FormatStringCreateSubmissionOpen, pubName);
                if (Messages.ShowYesNo(msg))
                {
                    DatabaseController.Instance.GetTable<SubmissionBatchTable>().CreateSubmission(pubId);
                    MainViewModel.CreateNotificationMessage(Strings.ResultSubmissionCreated);
                    MainViewModel.NotifyWorkspaceOnRecordAdded<SubmissionViewModel>();
                    if (Config.Instance.AutoSwitchToSubmission)
                    {
                        MainViewModel.SwitchToWorkspace<SubmissionViewModel>();
                    }
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
                MainViewModel.CreateNotificationMessage(String.Format("{0} copied to clipboard", columnName));
            }
        }
        #endregion
    }
}