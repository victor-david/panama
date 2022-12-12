using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Data;
using TableColumns = Restless.Panama.Database.Tables.QueueTitleTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic used for title queue management
    /// </summary>
    public class TitleQueueViewModel : DataRowViewModel<QueueTitleTable>
    {
        #region Private
        private QueueTable QueueTable => DatabaseController.Instance.GetTable<QueueTable>();
        private ObservableCollection<QueueRow> queues;
        private QueueRow selectedQueue;
        private QueueTitleRow selectedTitle;
        #endregion

        /************************************************************************/

        #region Properties
        public override bool OpenRowCommandEnabled => false;
        public override bool AddCommandEnabled => true;

        public ListCollectionView Queues { get; }

        public QueueRow SelectedQueue
        {
            get => selectedQueue;
            set
            {
                SetProperty(ref selectedQueue, value);
                Config.SelectedQueueId = selectedQueue?.Id ?? 0;
                ListView.Refresh();
            }
        }

        /// <summary>
        /// Gets the collection of menu items for the queue list.
        /// </summary>
        public MenuItemCollection QueueMenuItems { get; }

        public QueueTitleRow SelectedTitle
        {
            get => selectedTitle;
            private set => SetProperty(ref selectedTitle, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of <see cref="TitleQueueViewModel"/>
        /// </summary>
        public TitleQueueViewModel()
        {
            Columns.Create("Id", TableColumns.TitleId)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Written", TableColumns.Joined.Written)
                .MakeDate();

            Columns.Create("Title", TableColumns.Joined.Title);

            Columns.Create("Status", TableColumns.Status);

            Columns.Create("Published", TableColumns.Date)
                .MakeDate();

            QueueMenuItems = new MenuItemCollection();
            QueueMenuItems.AddItem(Strings.MenuItemAddQueue, RelayCommand.Create(RunAddQueueCommand))
                .AddIconResource(ResourceKeys.Icon.PlusIconKey);

            QueueMenuItems.AddSeparator();

            QueueMenuItems.AddItem(Strings.MenuItemRemoveQueue, RelayCommand.Create(RunRemoveQueueCommand, p => SelectedQueue != null))
                .AddIconResource(ResourceKeys.Icon.XRedIconKey);

            MenuItems.AddItem(Strings.MenuItemAddTitle, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);

            queues = new ObservableCollection<QueueRow>();
            PopulateQueues();

            Queues = new ListCollectionView(queues);
            using (Queues.DeferRefresh())
            {
                Queues.CustomSort = new GenericComparer<QueueRow>((x, y) => OnQueueDataRowCompare(x, y));
            }

            SetSelectedQueue(Config.SelectedQueueId);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedTitle = QueueTitleRow.Create(SelectedRow);

        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.QueueId] == (SelectedQueue?.Id ?? 0);
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void PopulateQueues()
        {
            queues.Clear();
            foreach (QueueRow row in QueueTable.EnumerateAll())
            {
                queues.Add(row);
            }
        }

        private void SetSelectedQueue(long id)
        {
            foreach (QueueRow queue in queues)
            {
                if (queue.Id == id)
                {
                    SelectedQueue = queue;
                }
            }
        }

        private int OnQueueDataRowCompare(QueueRow item1, QueueRow item2)
        {
            return DataRowCompareString(item1.Row, item2.Row, QueueTable.Defs.Columns.Name);
        }

        private void RunAddQueueCommand(object parm)
        {
            long temp = Config.SelectedQueueId;
            QueueTable.AddDefaultRow();
            PopulateQueues();
            SetSelectedQueue(temp);
            Queues.Refresh();
        }

        private void RunRemoveQueueCommand(object parm)
        {
            if (SelectedQueue != null && MessageWindow.ShowContinueCancel(Strings.ConfirmationRemoveQueue))
            {
                QueueTable.RemoveQueue(SelectedQueue.Id);
                PopulateQueues();
                Queues.Refresh();
            }
        }
        #endregion
    }
}