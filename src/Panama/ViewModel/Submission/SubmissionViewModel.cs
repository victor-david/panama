/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Controls;
using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Panama.View;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Mvvm;
using System;
using System.Data;
using System.Globalization;
using System.Windows.Threading;
using TableColumns = Restless.Panama.Database.Tables.SubmissionBatchTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the submission view.
    /// </summary>
    public class SubmissionViewModel : DataRowViewModel<SubmissionBatchTable>
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

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => !(SelectedBatch?.IsLocked ?? true);

        /// <inheritdoc/>
        public override bool ClearFilterCommandEnabled => Filters.IsAnyFilterActive;

        /// <inheritdoc/>
        public override bool OpenRowCommandEnabled => SelectedBatch?.HasPublisherUrl ?? false;

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

        /// <summary>
        /// Gets the filters
        /// </summary>
        public SubmissionRowFilter Filters => Config.SubmissionFilter;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionViewModel"/> class.
        /// </summary>
        public SubmissionViewModel()
        {
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Add(CreateFlagsColumn(Header.Flags, GetFlagGridColumns())
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W076)
                .AddToolTip(SubmissionFlagsToolTip.Create(this)));

            Columns.Create(Header.Submitted, TableColumns.Submitted)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create(Header.Response, TableColumns.Response)
                .MakeDate()
                .SetSelectorName(Header.ResponseDate);

            Columns.Create(Header.Type, TableColumns.Joined.ResponseTypeName)
                .MakeFixedWidth(FixedWidth.W096)
                .SetSelectorName(Header.ResponseType);

            // string.Empty because VS gets confused and tries to connect to the wrong overload
            Columns.Create<DatesToDayDiffConverter>(Header.Days, TableColumns.Submitted, TableColumns.Response, string.Empty)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W052);

            Columns.Create(Header.Publisher, TableColumns.Joined.Publisher);

            Columns.Create(Header.Fee, TableColumns.Fee)
                .MakeNumeric("N2", FixedWidth.W058)
                .MakeRightAligned();

            Columns.Create(Header.Award, TableColumns.Award)
                .MakeNumeric("N2", FixedWidth.W058)
                .MakeRightAligned();

            Columns.Create(Header.Note, TableColumns.Notes)
                .MakeSingleLine();

            Columns.RestoreColumnState(Config.SubmissionGridColumnState);

            Commands.Add("ActiveFilter", p => Filters.SetToActive());
            Commands.Add("TryAgainFilter", p => Filters.SetToTryAgain());
            Commands.Add("PersonalFilter", p => Filters.SetToPersonal());
            Commands.Add("AcceptedFilter", p => Filters.SetToAccepted());

            /* Context menu items */
            MenuItems.AddItem(Menu.CreateSubmission, AddCommand)
                .AddIconResource(ResourceKeys.Icon.IconAdd);

            MenuItems.AddSeparator();

            MenuItems.AddItem(Menu.BrowseToPublisherUrl, OpenRowCommand)
                .AddIconResource(ResourceKeys.Icon.IconOpenWebSite);

            MenuItems.AddItem(
                Menu.FilterToPublisher,
                RelayCommand.Create(RunFilterToPublisherCommand, p => SelectedBatch != null))
                .AddIconResource(ResourceKeys.Icon.IconFilter);

            MenuItems.AddSeparator();

            MenuItems.AddItem(Menu.DeleteSubmission, DeleteCommand)
                .AddIconResource(ResourceKeys.Icon.IconDelete);

            Titles = new SubmissionTitleController(this);
            Documents = new SubmissionDocumentController(this);
            Messages = new SubmissionMessageController(this);
            Dates = new SubmissionDateController(this);

            ListView.IsLiveSorting = true;
            ListView.LiveSortingProperties.Add(TableColumns.Submitted);

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                SelectedEditSection = 1;
                Filters.SetListView(ListView);
                Filters.ApplyFilter();
            }));
        }
        #endregion

        /************************************************************************/

        #region Public methods
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
            if (SelectedBatch?.Submitted is DateTime date)
            {
                string dateStr = date.ToLocalTime().ToString(Config.Instance.DateFormat, CultureInfo.InvariantCulture);
                header = $"{dateStr} to {SelectedBatch.PublisherName}";
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
        protected override bool OnDataRowFilter(DataRow item)
        {
            return Filters?.OnDataRowFilter(item) ?? false;
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.Submitted);
        }

        /// <inheritdoc/>
        protected override void RunClearFilterCommand()
        {
            Filters.ClearAll();
        }

        /// <inheritdoc/>
        protected override void RunOpenRowCommand()
        {
            if (OpenRowCommandEnabled)
            {
                OpenHelper.OpenWebSite(null, SelectedBatch.PublisherUrl);
            }
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            if (WindowFactory.PublisherSelect.Create().GetPublisher() is PublisherRow publisher)
            {
                int openCount = Table.OpenSubmissionCount(publisher.Id);

                if (MessageWindow.ShowYesNo(StringHelper.GetSubmissionConfirmation(openCount, publisher.Name)))
                {
                    Table.CreateSubmission(publisher.Id);
                    MainWindowViewModel.Instance.CreateNotificationMessage(Text.SubmissionCreated);
                    ForceListViewSort();
                }
            }
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (DeleteCommandEnabled && MessageWindow.ShowYesNo(Confirm.DeleteSubmission))
            {
                // Call the DeleteSubmission() method to delete and perform other cleanup.
                Table.DeleteSubmission(SelectedBatch);
                ListView.Refresh();
            }
        }

        /// <inheritdoc/>
        protected override void OnSave()
        {
            Config.SubmissionGridColumnState = Columns.GetColumnState();
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
        private void RunFilterToPublisherCommand(object parm)
        {
            Filters.SetIdFilter(SelectedBatch.PublisherId);
        }

        private FlagGridColumnCollection GetFlagGridColumns()
        {
            return new FlagGridColumnCollection(this)
            {
                { TableColumns.Online, Config.Colors.SubmissionOnline.ToBindingPath() },
                { TableColumns.Contest, Config.Colors.SubmissionContest.ToBindingPath() },
                { TableColumns.Locked, Config.Colors.SubmissionLocked.ToBindingPath() },
                { TableColumns.Joined.PublisherExclusive, Config.Colors.PublisherExclusive.ToBindingPath() },
            };
        }
        #endregion
    }
}