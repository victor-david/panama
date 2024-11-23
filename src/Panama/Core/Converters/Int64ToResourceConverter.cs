using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides a converter that accepts a long integer value and returns a resource
    /// </summary>
    public class Int64ToResourceConverter : IValueConverter
    {
        #region Public methods
        /// <summary>
        /// Converts a long integer value to a resource
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">A Dictionary<long, string> that maps values to resource ids</param>
        /// <param name="culture">Not used.</param>
        /// <returns>A path resource</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is long key && parameter is Dictionary<long, string> map && map.ContainsKey(key)
                ? LocalResources.Get(map[key])
                : null;
        }

        /// <summary>
        /// This method is not used. It throws a <see cref="NotImplementedException"/>
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}