/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Windows.Data;
using System.Windows.Media;
using Restless.Panama.Resources;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides a converter that accepts a boolean value and if true, returns a specified <see cref="ImageSource"/> object.
    /// </summary>
    public class BooleanToImageConverter : IValueConverter
    {
        #region Public methods
        /// <summary>
        /// Converts a boolean value to an <see cref="ImageSource"/> object.
        /// </summary>
        /// <param name="value">The boolean value</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">The resource name of an image source to use when value is true. If null, the default of "ImageCheck" will be used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>The image, if <paramref name="value"/> is true; otherwise, null.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ImageSource result = null;

            if (value is bool && (bool)value)
            {
                string resourceName = (parameter is string) ? parameter.ToString() : "ImageCheck";
                result = (ImageSource)LocalResources.Get(resourceName);
            }
            return result;
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