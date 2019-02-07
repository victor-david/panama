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
    /// Provides a converter that accepts two boolean values and returns a <see cref="Visibility"/> value.
    /// </summary>
    /// <remarks>
    /// See:
    /// https://code.msdn.microsoft.com/windowsapps/How-to-add-a-hint-text-to-ed66a3c6
    /// </remarks>
    public class BooleanToVisibilityMultiConverter : MarkupExtension, IMultiValueConverter
    {
        #region Constructor
        #pragma warning disable 1591
        public BooleanToVisibilityMultiConverter()
        {
            // prevents the designer that's referencing this converter directly from going stupid every time you type a character
        }
        #pragma warning restore 1591
        #endregion
        
        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Converts two boolean values to a <see cref="Visibility"/> value.
        /// </summary>
        /// <param name="values">The boolean values</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">
        /// A value from the <see cref="BooleanToVisibilityMultiConverterOptions"/> enumeration that describes how to treat the two boolean values.
        /// If not passed, the default is <see cref="BooleanToVisibilityMultiConverterOptions.OneTrueOrTwoTrue"/>.
        /// </param>
        /// <param name="culture">Not used.</param>
        /// <returns>A <see cref="Visibility"/> value.</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] is bool && values[1] is bool)
            {
                bool b1 = (bool)values[0];
                bool b2 = (bool)values[1];
                BooleanToVisibilityMultiConverterOptions op = BooleanToVisibilityMultiConverterOptions.OneTrueOrTwoTrue;
                if (parameter is BooleanToVisibilityMultiConverterOptions)
                {
                    op = (BooleanToVisibilityMultiConverterOptions)parameter;
                }
                switch (op)
                {
                    case BooleanToVisibilityMultiConverterOptions.OneFalseOrTwoTrue:
                        if (!b1 || b2) return Visibility.Collapsed;
                        break;

                    case BooleanToVisibilityMultiConverterOptions.OneTrueAndTwoTrue:
                        if (b1 && b2) return Visibility.Collapsed;
                        break;

                    case BooleanToVisibilityMultiConverterOptions.OneTrueOrTwoTrue:
                        if (b1 || b2) return Visibility.Collapsed;
                        break;
                }
            }
            return Visibility.Visible;
        }

        /// <summary>
        /// This method is not used. It throws a <see cref="NotImplementedException"/>
        /// </summary>
        /// <param name="value">n/a</param>
        /// <param name="targetTypes">n/a</param>
        /// <param name="parameter">n/a</param>
        /// <param name="culture">n/a</param>
        /// <returns>n/a</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
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