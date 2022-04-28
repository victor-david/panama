using System;
using System.Windows.Media;

namespace Restless.Panama.Controls
{
    public class FlagGridColumn
    {
        public string ColumnName { get; }

        public Brush Brush { get; }

        public FlagGridColumn(string columName, Brush brush)
        {
            if (string.IsNullOrWhiteSpace(columName))
            {
                throw new ArgumentNullException(nameof(columName));
            }

            ColumnName = columName;
            Brush = brush ?? throw new ArgumentNullException(nameof(brush));
        }
    }
}