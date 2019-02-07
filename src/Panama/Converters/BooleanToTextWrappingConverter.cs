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
    /// Provides a converter that accepts a boolean value and returns a <see cref="TextWrapping"/> value.
    /// </summary>
    public class BooleanToTextWrappingConverter : IValueConverter
    {
        #region Public methods
        /// <summary>
        /// Converts a boolean value to a <see cref="TextWrapping"/> value.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>TextWrapping.Wrap if <paramref name="value"/> is true; otherwise, TextWrapping.NoWrap.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {

                return (bool)value ? TextWrapping.Wrap : TextWrapping.NoWrap;
            }
            return TextWrapping.NoWrap;
        }

        /// <summary>
        /// Converts a <see cref="TextWrapping"/> value to a boolean value.
        /// </summary>
        /// <param name="value">The <see cref="TextWrapping"/> value.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>True if <paramref name="value"/> is  TextWrapping.Wrap; otherwise, false.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is TextWrapping)
            {
                return (TextWrapping)value ==  TextWrapping.Wrap;
            }
            return false;
        }
        #endregion
    }
}