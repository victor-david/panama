using System;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.QueueTitleTable.Defs.Columns;

namespace Restless.Panama.Core
{
    public class TitleQueueRowFilter : RowFilter
    {
        private long queueStatus;
        private const long QueueStatusAny = Config.Other.DefaultQueueTitleFilterValue;

        protected override bool IsTextFilterSupported => true;
        public override bool IsAnyFilterActive => base.IsAnyFilterActive || QueueStatus != QueueStatusAny;

        public long QueueStatus
        {
            get => queueStatus;
            set
            {
                SetProperty(ref queueStatus, value);
                ApplyFilter();
            }
        }

        public TitleQueueRowFilter()
        {
            QueueStatus = QueueStatusAny;
        }

        public void SetQueueStatus(long value)
        {
            QueueStatus = value;
        }

        public override void ClearAll()
        {
            IncreaseSuspendLevel();
            base.ClearAll();
            QueueStatus = QueueStatusAny;
            DecreaseSuspendLevel();
        }


        public override bool OnDataRowFilter(DataRow item)
        {
            return EvaluateStatus(item) && EvaluateText(item);
        }

        private bool EvaluateStatus(DataRow item)
        {
            return QueueStatus == QueueStatusAny || (long)item[TableColumns.Status] == QueueStatus;
        }

        private bool EvaluateText(DataRow item)
        {
            return
                string.IsNullOrWhiteSpace(Text) ||
                item[TableColumns.Joined.Title].ToString().Contains(Text, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}