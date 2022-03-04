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
    /// Provides a converter that accepts an integer value and returns its corresponding char.
    /// </summary>
    public class IntegerToCharConverter : IValueConverter
    {
        #region Public methods
        /// <summary>
        /// Converts an integer value to its corresponding char.
        /// </summary>
        /// <param name="value">The integer value</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used</param>
        /// <param name="culture">Not used.</param>
        /// <returns>The char that corresponds to <paramref name="value"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is long l)
            {
                return (char)l;
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