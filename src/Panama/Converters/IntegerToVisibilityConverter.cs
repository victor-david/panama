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
using System.Windows;
using System.Windows.Data;
using System.Data;
using System.Windows.Markup;

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides a converter that accepts an integer value and returns a <see cref="Visibility"/> value.
    /// </summary>
    public class IntegerToVisibilityConverter : MarkupExtension, IValueConverter
    {
        #region Public methods
        /// <summary>
        /// Converts an integer value to a <see cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The integer value.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">An optional parameter that reverses the evaluation.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>
        /// <para>
        /// If <paramref name="parameter"/> is null, returns <see cref="Visibility.Visible"/> for any <paramref name="value"/> greater than zero; otherwise, <see cref="Visibility.Collapsed"/>.
        /// </para>
        /// <para>
        /// If <paramref name="parameter"/> is not null, returns <see cref="Visibility.Visible"/> when <paramref name="value"/> equals zero; otherwise, <see cref="Visibility.Collapsed"/>.
        /// </para>
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                if (parameter != null)
                {
                    return ((int)value == 0) ? Visibility.Visible : Visibility.Collapsed;
                }
                return ((int)value > 0) ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Visible;
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