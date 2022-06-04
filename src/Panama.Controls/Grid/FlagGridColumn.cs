using System;

namespace Restless.Panama.Controls
{
    public class FlagGridColumn
    {
        public string ColumnName { get; }

        public string BindingPath { get; }

        public FlagGridColumn(string columName, string bindingPath)
        {
            if (string.IsNullOrWhiteSpace(columName))
            {
                throw new ArgumentNullException(nameof(columName));
            }

            if (string.IsNullOrWhiteSpace(bindingPath))
            {
                throw new ArgumentNullException(nameof(bindingPath));
            }

            ColumnName = columName;
            BindingPath = bindingPath;
        }
    }
}