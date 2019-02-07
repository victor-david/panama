/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Text.RegularExpressions;
using System.Windows.Data;

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
            if (value is string str)
            {
                str = str.Trim();
                if (parameter is StringToCleanStringOptions ops)
                {
                    return Convert(str, ops);
                }
                return str.Replace(Environment.NewLine, ".");
            }
            return value;
        }

        /// <summary>
        /// Cleans up the string according to the specified options.
        /// </summary>
        /// <param name="str">The string value</param>
        /// <param name="options">Options</param>
        /// <returns>The cleaned string</returns>
        /// <remarks>
        /// This method is more convienent for calling directly from code
        /// </remarks>
        public string Convert(string str, StringToCleanStringOptions options)
        {

            if (options.HasFlag(StringToCleanStringOptions.RemoveHtml))
            {
                int startStyle = str.IndexOf("<style");
                int endStyle = str.IndexOf("</style>");
                if (startStyle != -1 && endStyle != -1)
                {
                    str = str.Substring(0, startStyle) + str.Substring(endStyle + 8);
                }

                str = Regex.Replace(str, "<.*?>", string.Empty);
                // str = StripTagsCharArray(str);
            }

            if (options.HasFlag(StringToCleanStringOptions.TrimInterior))
            {
                // this replaces all double white space with a single space
                // str = Regex.Replace(str, @"\s+", " ");
                // this replaces are double white space with the same type of white space.
                str = Regex.Replace(str, @"(\s)\s+", "$1");
            }

            return str.Trim();
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

        /************************************************************************/

        #region Private methods
        private string StripTagsCharArray(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;
            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }
        #endregion
    }
}