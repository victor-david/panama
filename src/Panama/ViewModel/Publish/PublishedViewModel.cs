using Restless.Panama.Core;
using Restless.Panama.Core.Filter;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using System;
using System.Data;
using System.Windows.Threading;
using TableColumns = Restless.Panama.Database.Tables.PublishedAllTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage published records.
    /// </summary>
    public class PublishedViewModel : DataRowViewModel<PublishedAllTable>
    {
        #region Private
        private int selectedEditSection;
        private PublishedAllRow selectedPublished;
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
        public override bool OpenRowCommandEnabled => SelectedPublished?.HasUrl ?? false;

        /// <summary>
        /// Gets or sets the selected edit section
        /// </summary>
        public int SelectedEditSection
        {
            get => selectedEditSection;
            set => SetProperty(ref selectedEditSection, value);
        }

        /// <summary>
        /// Gets the currently selected published row
        /// </summary>
        public PublishedAllRow SelectedPublished
        {
            get => selectedPublished;
            private set => SetProperty(ref selectedPublished, value);
        }

        /// <summary>
        /// Gets the filters
        /// </summary>
        public PublisherRowFilter Filters => Config.PublisherFilter;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublishedViewModel"/> class.
        /// </summary>
        public PublishedViewModel()
        {
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create(Header.Type, TableColumns.TypeId).MakeFixedWidth(FixedWidth.W058);

            Columns.Create(Header.Added, TableColumns.Added)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create(Header.Published, TableColumns.Published)
                .MakeDate();

            Columns.Create(Header.Title, TableColumns.Title);

            Columns.Create(Header.Publisher, TableColumns.Publisher);

            //Columns.RestoreColumnState(Config.PublisherGridColumnState);

            //Commands.Add("ActiveFilter", p => Filters.SetToActive());
            //Commands.Add("HaveSubFilter", p => Filters.SetToOpenSubmission());
            //Commands.Add("InPeriodFilter", p => Filters.SetToInPeriod());
            //Commands.Add("PayingFilter", p => Filters.SetToPaying());
            //Commands.Add("FollowupFilter", p => Filters.SetToFollowup());

            /* Context menu items */
            //MenuItems.AddItem(Menu.CreatePublisher, AddCommand).AddIconResource(ResourceKeys.Icon.IconAdd);
            //MenuItems.AddSeparator();
            //MenuItems.AddItem(Menu.BrowseToPublisherUrlOrClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.IconOpenWebSite);
            //MenuItems.AddSeparator();
            //MenuItems.AddItem(Menu.DeletePublisher, DeleteCommand).AddIconResource(ResourceKeys.Icon.IconDelete);

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
            SelectedPublished = PublishedAllRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        //protected override bool OnDataRowFilter(DataRow item)
        //{
        //    return Filters?.OnDataRowFilter(item) ?? false;
        //}

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
            if (SelectedPublished?.HasUrl ?? false)
            {
                OpenHelper.OpenWebSite(null, SelectedPublished.Url);
            }
        }

        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
        protected override void RunDeleteCommand()
        {
            //if (IsSelectedRowAccessible)
            //{
            //    int childRowCount = SelectedRow.GetChildRows(PublisherTable.Defs.Relations.ToSubmissionBatch).Length;
            //    if (childRowCount > 0)
            //    {
            //        MessageWindow.ShowError(string.Format(CultureInfo.InvariantCulture, Error.CannotDeletePublisher, childRowCount));
            //        return;
            //    }

            //    if (MessageWindow.ShowYesNo(Confirm.DeletePublisher))
            //    {
            //        DeleteSelectedRow();
            //    }
            //}
        }

        /// <inheritdoc/>
        protected override void OnSave()
        {
            //Config.PublisherGridColumnState = Columns.GetColumnState();
        }

        /// <inheritdoc/>
        protected override void OnClosing()
        {
            base.OnClosing();
            SignalSave();
        }
        #endregion
    }
}