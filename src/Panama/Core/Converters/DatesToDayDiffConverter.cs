using System;
using System.Windows.Data;

namespace Restless.Panama.Core.Converters
{
    /// <summary>
    /// Provides a converter that accepts two <see cref="DateTime"/> objects and returns the number of days between them.
    /// </summary>
    public class DatesToDayDiffConverter : IMultiValueConverter
    {
        #region Public methods
        /// <summary>
        /// Receives two <see cref="DateTime"/> objects and returns the number of days between them.
        /// </summary>
        /// <param name="values">The array of <see cref="DateTime"/> objects.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>A string that contains the number of days between the two dates.</returns>
        /// <remarks>
        /// If there is not at least two elements in <paramref name="values"/>, this method returns null.
        /// If only the first element is a <see cref="DateTime"/>, the difference is calculated between today and the supplied date.
        /// </remarks>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length > 1)
            {
                DateTime date1 = Zeroed(DateTime.Now);
                DateTime date2 = Zeroed(DateTime.Now);

                if (values[0] is DateTime d1) date1 = Zeroed(d1);
                if (values[1] is DateTime d2) date2 = Zeroed(d2);
                return (date2 - date1).Days.ToString();
            }
            return null;
        }

        private DateTime Zeroed(DateTime date) => new(date.Year, date.Month, date.Day);

        /// <summary>
        /// This method is not used. It throws a <see cref="NotImplementedException"/>
        /// </summary>
        /// <param name="value">n/a</param>
        /// <param name="targetTypes">n/a</param>
        /// <param name="parameter">n/a</param>
        /// <param name="culture">n/a</param>
        /// <returns>n/a</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}