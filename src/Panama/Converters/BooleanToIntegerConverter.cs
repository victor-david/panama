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
using System.Windows.Markup;

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides a converter that accepts a boolean value and returns an integer.
    /// </summary>
    public class BooleanToIntegerConverter : MarkupExtension, IValueConverter
    {
        #region Constructor
        #pragma warning disable 1591
        public BooleanToIntegerConverter()
        {
            // prevents the designer that's referencing this converter directly from going stupid every time you type a character
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Converts a boolean value to an integer.
        /// </summary>
        /// <param name="value">The boolean value</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">An integer that specifies the value to return if <paramref name="value"/> is true. If not passed, the value zero will be used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>
        /// The integer value specified by <paramref name="parameter"/> if <paramref name="value"/> is true,
        /// or <paramref name="parameter"/> + 1 if <paramref name="value"/> is false.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is bool)
            {
                int trueValue = 0;
                if (parameter is int)
                {
                    trueValue = (int)parameter;
                }
                return (bool)value ? trueValue : trueValue + 1;
            }
            return 0;
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