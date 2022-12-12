using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Controls;
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
        private QueueTable QueueTable => DatabaseController.Instance.GetTable<QueueTable>();
        private long selectedQueueId;
        private string selectedQueueName;
        private QueueTitleRow selectedTitle;

        public override bool OpenRowCommandEnabled => false;

        public ListCollectionView Queues
        {
            get;
        }

        public long SelectedQueueId
        {
            get => selectedQueueId;
            set
            {
                SetProperty(ref selectedQueueId, value);
                Config.SelectedQueueId = selectedQueueId;
                SetQueueName();
                ListView.Refresh();
            }
        }

        public string SelectedQueueName
        {
            get => selectedQueueName;
            private set => SetProperty(ref selectedQueueName, value);
        }

        public QueueTitleRow SelectedTitle
        {
            get => selectedTitle;
            private set => SetProperty(ref selectedTitle, value);
        }

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

            Queues = new ListCollectionView(new DataView(QueueTable));

            SetQueueId();
        }

        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedTitle = QueueTitleRow.Create(SelectedRow);

        }

        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.QueueId] == SelectedQueueId;
        }


        private void SetQueueId()
        {
            foreach (QueueRow row in QueueTable.EnumerateAll())
            {
                if (row.Id == Config.SelectedQueueId)
                {
                    SelectedQueueId = row.Id;
                }
            }
        }

        private void SetQueueName()
        {
            foreach (QueueRow row in QueueTable.EnumerateAll())
            {
                if (row.Id == SelectedQueueId)
                {
                    SelectedQueueName = row.Name;
                }
            }
        }
    }
}