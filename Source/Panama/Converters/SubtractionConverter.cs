using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using System.Globalization;

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides a converter that accepts a double value and returns that value minus the parameter value.
    /// </summary>
    /// <remarks>
    /// See:
    /// http://stackoverflow.com/questions/2625477/wpf-unwanted-grid-splitter-behaviour
    /// </remarks>
    [ValueConversion(typeof(double), typeof(double))]
    public class SubtractionConverter : MarkupExtension, IValueConverter
    {
        #region Constructor
        #pragma warning disable 1591
        public SubtractionConverter()
        {
            // prevents the designer that's referencing this converter directly from going stupid every time you type a character
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Converts a double value to its value minus the value given in <paramref name="parameter"/>.
        /// </summary>
        /// <param name="value">The double value.</param>
        /// <param name="targetType">The target type of the binding.</param>
        /// <param name="parameter">The value to subtract from <paramref name="value"/>.</param>
        /// <param name="culture">Not used.</param>
        /// <returns><paramref name="value"/> minus <paramref name="parameter"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double && targetType == typeof(double))
            {
                double dParameter;
                if (double.TryParse((string)parameter, NumberStyles.Any, CultureInfo.InvariantCulture, out dParameter))
                {
                    return (double)value - dParameter;
                }
            }
            return Binding.DoNothing;
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

        /// <summary>
        /// Gets the object that is set as the value of the target property for this markup extension. 
        /// </summary>
        /// <param name="serviceProvider">Object that can provide services for the markup extension.</param>
        /// <returns>This object.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
        #endregion
    }
}
