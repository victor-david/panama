using System;
using System.Windows.Data;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides a converter that accepts a string value and returns a masked string value.
    /// </summary>
    public class StringToMaskedStringConverter : IValueConverter
    {
        #region Public methods
        /// <summary>
        /// Masks a string.
        /// </summary>
        /// <param name="value">The string to be masked.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">If an integer, indicates the number of asterisks to return as the masked string. Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>The masked string</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int count = 12;
            if (parameter is int) count = (int)parameter;
            return new string('*', count);
        }


        /// <summary>
        /// This method is not used. It throws a <see cref="NotImplementedException"/>
        /// </summary>
        /// <param name="value">n/a</param>
        /// <param name="targetType">n/a</param>
        /// <param name="parameter">n/a</param>
        /// <param name="culture">n/a</param>
        /// <returns>n/a</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}