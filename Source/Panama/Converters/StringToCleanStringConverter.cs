using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides a converter that accepts a string value and returns a string value that has been cleaned up.
    /// </summary>
    public class StringToCleanStringConverter : IValueConverter
    {
        #region Public methods
        /// <summary>
        /// Cleans up a string according to various options. At minumum, leading and ending white space is removed.
        /// </summary>
        /// <param name="value">The string to be cleaned.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Optional bitwise combination of <see cref="StringToCleanStringOptions"/> values.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>The cleaned string</returns>
        /// <remarks>
        /// Without <see cref="StringToCleanStringOptions"/> in the <paramref name="parameter"/> parm, 
        /// this method converts new line characters into a dot. This is useful in a data grid where
        /// we don't want new lines to expand the row display. Generally, this method is used in binding situations
        /// and the overloaded Convert method that accepts a <see cref="StringToCleanStringOptions"/> parm directly is better suited
        /// for direct, programatic string cleaning.
        /// </remarks>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                value = ((string)value).Trim();
                if (parameter is StringToCleanStringOptions)
                {
                    return Convert((string)value, (StringToCleanStringOptions)parameter);
                }
                return ((string)value).Replace(Environment.NewLine, ".");
            }
            return value;
        }

        /// <summary>
        /// Cleans up the string according to the specified options.
        /// </summary>
        /// <param name="value">The string value</param>
        /// <param name="options">Options</param>
        /// <returns>The cleaned string</returns>
        /// <remarks>
        /// This method is more convienent for calling directly from code
        /// </remarks>
        public string Convert(string value, StringToCleanStringOptions options)
        {

            if (options.HasFlag(StringToCleanStringOptions.RemoveHtml))
            {
                int startStyle = value.IndexOf("<style");
                int endStyle = value.IndexOf("</style>");
                if (startStyle != -1 && endStyle != -1)
                {
                    value = value.Substring(0, startStyle) + value.Substring(endStyle + 8);
                }

                value = Regex.Replace(value, "<.*?>", string.Empty);
            }

            if (options.HasFlag(StringToCleanStringOptions.TrimInterior))
            {

                // this replaces all double white space with a single space
                // str = Regex.Replace(str, @"\s+", " ");
                // this replaces are double white space with the same type of white space.
                value = Regex.Replace(value, @"(\s)\s+", "$1");
            }

            return value;
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
