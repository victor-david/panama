using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Resources;

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides a <see cref="SolidColorBrush"/> object according to the passed parameter.
    /// </summary>
    public class TriggerToBrushConverter : MarkupExtension, IValueConverter
    {
        #region Constructor
        #pragma warning disable 1591
        public TriggerToBrushConverter()
        {
            // prevents the designer that's referencing this converter directly from going stupid every time you type a character
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Provides a <see cref="SolidColorBrush"/> object according to the passed parameter.
        /// </summary>
        /// <param name="value">Not used.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">A value from the <see cref="TriggerToBrushOptions"/> enumeration.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>A <see cref="SolidColorBrush"/> object.</returns>
        /// <remarks>
        /// This is not a converter in the usual sense. It is used from inside a trigger; we already know the value that
        /// causes the trigger to activate. Based on the parameter passed here, we supply a brush which is based on user preference.
        /// </remarks>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter is TriggerToBrushOptions key)
            {
                switch (key)
                {
                    case TriggerToBrushOptions.Published:
                        return Config.Instance.Colors.TitlePublishedBack.GetBrush();
                    case TriggerToBrushOptions.Submitted:
                        return Config.Instance.Colors.TitleSubmittedBack.GetBrush();
                    case TriggerToBrushOptions.Period:
                        return Config.Instance.Colors.PublisherPeriodBack.GetBrush();
                    case TriggerToBrushOptions.Goner:
                        return Config.Instance.Colors.PublisherGonerFore.GetBrush();
                    default:
                        return new SolidColorBrush(Colors.LightGray);
                }
            }
            return new SolidColorBrush(Colors.Black);
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