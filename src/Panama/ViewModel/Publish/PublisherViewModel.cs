using Restless.Panama.Controls;
using Restless.Panama.Core;
using Restless.Panama.Core.Filter;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Panama.View;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core;
using Restless.Toolkit.Core.Utility;
using System;
using System.Data;
using System.Globalization;
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
        public override bool OpenRowCommandEnabled => SelectedPublisher?.HasUrl ?? false;

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
        public PublisherPeriodController Periods { get; }

        /// <summary>
        /// Gets the submission controller for this VM.
        /// </summary>
        public PublisherSubmissionController Submissions { get; }

        /// <summary>
        /// Gets the filters
        /// </summary>
        public PublisherRowFilter Filters => Config.PublisherFilter;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherViewModel"/> class.
        /// </summary>
        public PublisherViewModel()
        {
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Add(CreateFlagsColumn(Header.Flags, GetFlagGridColumns())
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W076)
                .AddToolTip(PublisherFlagsToolTip.Create(this)));

            Columns.Create(Header.Name, TableColumns.Name);
            Columns.Create(Header.Url, TableColumns.Url);

            Columns.Create(Header.Added, TableColumns.Added)
                .MakeDate()
                .AddToolTip(ToolTip.PublisherAdded)
                .MakeInitialSortDescending();

            Columns.Create(Header.LastSubmissionShort, TableColumns.Calculated.LastSub)
                .MakeDate()
                .AddToolTip(ToolTip.PublisherLastSubmission)
                .SetSelectorName(Header.LastSubmission);

            Columns.Create(Header.SubmissionTotalCountShort, TableColumns.Calculated.SubCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(ToolTip.PublisherSubmissionCount)
                .SetPrimarySort(SortType.Long)
                .SetSecondarySort(TableColumns.Name, SortType.String, true)
                .SetSelectorName(Header.SubmissionTotalCount);

            Columns.RestoreColumnState(Config.PublisherGridColumnState);

            Commands.Add("ActiveFilter", p => Filters.SetToActive());
            Commands.Add("HaveSubFilter", p => Filters.SetToOpenSubmission());
            Commands.Add("InPeriodFilter", p => Filters.SetToInPeriod());
            Commands.Add("PayingFilter", p => Filters.SetToPaying());
            //Commands.Add("FollowupFilter", p => Filters.SetToFollowup());
            Commands.Add("NeverFilter", p => Filters.SetToNever());

            Periods = new PublisherPeriodController(this);
            Submissions = new PublisherSubmissionController(this);

            /* Context menu items */
            MenuItems.AddItem(Menu.CreatePublisher, AddCommand).AddIconResource(ResourceKeys.Icon.IconAdd);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Menu.BrowseToPublisherUrlOrClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.IconOpenWebSite);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Menu.DeletePublisher, DeleteCommand).AddIconResource(ResourceKeys.Icon.IconDelete);

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
            if (MessageWindow.ShowContinueCancel(Confirm.AddPublisher))
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
            if (SelectedPublisher?.HasUrl ?? false)
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
                    MessageWindow.ShowError(string.Format(CultureInfo.InvariantCulture, Error.CannotDeletePublisher, childRowCount));
                    return;
                }

                if (MessageWindow.ShowYesNo(Confirm.DeletePublisher))
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