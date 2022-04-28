using System.Collections.Generic;
using System.Windows.Media;

namespace Restless.Panama.Controls
{
    public class FlagGridColumnCollection : List<FlagGridColumn>
    {
        public FlagGridColumnCollection()
        {
        }

        public void Add(string columnName, Brush brush)
        {
            Add(new FlagGridColumn(columnName, brush));
        }
    }
}