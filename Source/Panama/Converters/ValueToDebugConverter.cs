using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Diagnostics;

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides a debugging converter. When this converter is used, breaks into the debugger
    /// </summary>
    public class ValueToDebugConverter : IValueConverter
    {
        /// <summary>
        /// Breaks into the debugger.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns><paramref name="value"/> unchanged.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        /// <summary>
        /// Breaks into the debugger.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns><paramref name="value"/> unchanged.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}
