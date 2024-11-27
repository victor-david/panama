using Restless.Panama.Resources;
using System;
using System.Windows.Data;
using Values = Restless.Panama.Database.Tables.PublishedAllTable.Defs.Values;

namespace Restless.Panama.Core.Converters
{
    /// <summary>
    /// Provides a converter that accepts a published type and returns a string
    /// </summary>
    public class PublishedTypeToStringConverter : IValueConverter
    {
        #region Public methods
        /// <summary>
        /// Converts a boolean value to a specified resource.
        /// </summary>
        /// <param name="value">The boolean value</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">The resource name to use when value is true</param>
        /// <param name="culture">Not used.</param>
        /// <returns>The resource, if <paramref name="value"/> is true; otherwise, null.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is long lValue)
            {
                return lValue switch
                {
                    Values.TypePublisher => Header.Published,
                    Values.TypeSelfPublisher => Header.SelfPublished,
                    _ => Text.Unknown
                };
            }
            return Text.Unknown;
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