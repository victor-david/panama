using Restless.Toolkit.Core.Database.SQLite;
using System.Collections;
using System.Collections.Generic;

namespace Restless.Panama.Core
{
    public abstract class StatisticBase : TableStatisticBase, IEnumerable<Statistic>
    {
        protected StatisticBase(TableBase table) : base(table)
        {
        }

        public abstract IEnumerator<Statistic> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}