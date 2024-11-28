using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using System.Data;
using System.Globalization;
using TableColumns = Restless.Panama.Database.Tables.SelfPublisherTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage self publisher records.
    /// </summary>
    public class SelfPublisherViewModel : DataRowViewModel<SelfPublisherTable>
    {
        #region Private
        private SelfPublisherRow selectedPublisher;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

        /// <inheritdoc/>
        public override bool OpenRowCommandEnabled => SelectedPublisher?.HasUrl ?? false;

        /// <summary>
        /// Gets the currently selected publisher row
        /// </summary>
        public SelfPublisherRow SelectedPublisher
        {
            get => selectedPublisher;
            private set => SetProperty(ref selectedPublisher, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherViewModel"/> class.
        /// </summary>
        public SelfPublisherViewModel()
        {
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create(Header.Name, TableColumns.Name);

            Columns.Create(Header.Url, TableColumns.Url);

            Columns.Create(Header.Added, TableColumns.Added)
                .MakeDate()
                .AddToolTip(ToolTip.PublisherAdded)
                .MakeInitialSortDescending();

            Columns.Create(Header.PublishedCountShort, TableColumns.Calculated.PubCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .SetSelectorName(Header.PublishedCount)
                .AddToolTip(ToolTip.SelfPublisherPublishedCount);

            Columns.RestoreColumnState(Config.SelfPublisherGridColumnState);

            /* Context menu items */
            MenuItems.AddItem(Menu.CreatePublisher, AddCommand).AddIconResource(ResourceKeys.Icon.IconAdd);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Menu.BrowseToPublisherUrlOrClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.IconOpenWebSite);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Menu.DeletePublisher, DeleteCommand).AddIconResource(ResourceKeys.Icon.IconDelete);
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
            SelectedPublisher = SelfPublisherRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return true;
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.Added);
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            if (MessageWindow.ShowYesNo(Confirm.AddPublisher))
            {
                Table.AddDefaultRow();
                Table.Save();
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
                int childRowCount = SelectedRow.GetChildRows(SelfPublisherTable.Defs.Relations.ToPublished).Length;
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
            Config.SelfPublisherGridColumnState = Columns.GetColumnState();
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