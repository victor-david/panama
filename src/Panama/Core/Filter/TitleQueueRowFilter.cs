using System;
using System.Collections.Generic;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.QueueTitleTable.Defs.Columns;
using QueueStatusValues = Restless.Panama.Database.Tables.QueueTitleStatusTable.Defs.Values;

namespace Restless.Panama.Core
{
    public class TitleQueueRowFilter : RowFilter
    {
        private const int MaxQueueStatus = 3;

        protected override bool IsTextFilterSupported => true;
        public override bool IsAnyFilterActive => base.IsAnyFilterActive || QueueStatus.Count < MaxQueueStatus;

        public List<long> QueueStatus { get; }

        public TitleQueueRowFilter()
        {
            QueueStatus = new List<long>();
        }

        public void SetQueueStatus(long value, bool enabled)
        {
            if (enabled && !QueueStatus.Contains(value))
            {
                QueueStatus.Add(value);
            }

            if (!enabled && QueueStatus.Contains(value))
            {
                QueueStatus.Remove(value);
            }
            ApplyFilter();
        }

        public override void ClearAll()
        {
            IncreaseSuspendLevel();
            base.ClearAll();
            ResetQueueStatus();
            DecreaseSuspendLevel();
        }


        public override bool OnDataRowFilter(DataRow item)
        {
            return EvaluateStatus(item) && EvaluateText(item);
        }

        private bool EvaluateStatus(DataRow item)
        {
            return QueueStatus.Contains((long)item[TableColumns.Status]);
        }

        private bool EvaluateText(DataRow item)
        {
            return
                string.IsNullOrWhiteSpace(Text) ||
                item[TableColumns.Joined.Title].ToString().Contains(Text, StringComparison.InvariantCultureIgnoreCase);
        }

        private void ResetQueueStatus()
        {
            QueueStatus.Clear();
            QueueStatus.Add(QueueStatusValues.StatusIdle);
            QueueStatus.Add(QueueStatusValues.StatusPending);
            QueueStatus.Add(QueueStatusValues.StatusPublished);
        }
    }
}