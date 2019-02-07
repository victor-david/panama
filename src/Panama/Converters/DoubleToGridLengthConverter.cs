/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides a converter that accepts a double value and returns a <see cref="GridLength"/>.
    /// </summary>
    public class DoubleToGridLengthConverter : IValueConverter
    {
        #region Public methods
        /// <summary>
        /// Converts a double value to a <see cref="GridLength"/>.
        /// </summary>
        /// <param name="value">The double value</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>A <see cref="GridLength"/> object with its Value property set to <paramref name="value"/></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double)
            {
                return new GridLength((double)value);
            }
            return new GridLength(46);
        }

        /// <summary>
        /// Converts a <see cref="GridLength"/> to a double value.
        /// </summary>
        /// <param name="value">The <see cref="GridLength"/> object.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>The Value property of <paramref name="value"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is GridLength)
            {
                return ((GridLength)value).Value;
            }
            return 46;
        }
        #endregion
    }
}