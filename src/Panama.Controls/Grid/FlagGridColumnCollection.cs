using System;
using System.Collections.Generic;

namespace Restless.Panama.Controls
{
    public class FlagGridColumnCollection : List<FlagGridColumn>
    {
        internal object DataContext { get; }

        public FlagGridColumnCollection(object dataContext)
        {
            DataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public void Add(string columnName, string bindingPath)
        {
            Add(new FlagGridColumn(columnName, bindingPath));
        }
    }
}