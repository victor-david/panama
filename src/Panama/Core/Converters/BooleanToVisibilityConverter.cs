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
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides a converter that accepts a boolean value and returns a <see cref="Visibility"/> value.
    /// </summary>
    public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter
    {
        #region Constructor
        #pragma warning disable 1591
        public BooleanToVisibilityConverter()
        {
            // prevents the designer that's referencing this converter directly from going stupid every time you type a character
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Converts a boolean value and to a <see cref="Visibility"/> value.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        /// <param name="targetType">The target type.</param>
        /// <param name="parameter">A value from the <see cref="BooleanToVisibilityConverterOptions"/>, or null to use the default.</param>
        /// <param name="culture">Not used</param>
        /// <returns>Either <see cref="Visibility.Visible"/> or <see cref="Visibility.Collapsed"/></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Visibility)) return value;

            if (value is bool)
            {
                BooleanToVisibilityConverterOptions op = BooleanToVisibilityConverterOptions.TrueToVisibility;
                if (parameter is BooleanToVisibilityConverterOptions)
                {
                    op = (BooleanToVisibilityConverterOptions)parameter;
                }
                switch (op)
                {
                    case BooleanToVisibilityConverterOptions.TrueToVisibility:
                        return (bool)value ? Visibility.Visible : Visibility.Collapsed;
                    case BooleanToVisibilityConverterOptions.FalseToVisibility:
                        return (bool)value ? Visibility.Collapsed : Visibility.Visible;
                }

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