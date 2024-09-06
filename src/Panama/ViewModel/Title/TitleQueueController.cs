using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.QueueTitleTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    public class TitleQueueController : BaseController<TitleViewModel, QueueTitleTable>
    {
        private QueueTitleRow selectedQueue;

        #region Public properties
        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

        /// <summary>
        /// Gets the selected queue row
        /// </summary>
        public QueueTitleRow SelectedQueue
        {
            get => selectedQueue;
            private set => SetProperty(ref selectedQueue, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleQueueController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleQueueController(TitleViewModel owner) : base(owner)
        {
            Columns.Create("Queue", TableColumns.Joined.QueueName)
                .MakeInitialSortAscending();

            Columns.Create("Status", TableColumns.Joined.Status);

            MenuItems.AddItem(Strings.MenuItemRemoveFromQueue, DeleteCommand).AddIconResource(ResourceKeys.Icon.XMediumIconKey);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedQueue = QueueTitleRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareString(item1, item2, TableColumns.Joined.QueueName);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.TitleId] == (Owner?.SelectedTitle?.Id ?? 0);
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (IsSelectedRowAccessible && MessageWindow.ShowContinueCancel(Strings.ConfirmationRemoveTitleFromQueue))
            {
                SelectedQueue.Row.Delete();
                Table.Save();
                ListView.Refresh();
            }
        }
        #endregion
    }
}