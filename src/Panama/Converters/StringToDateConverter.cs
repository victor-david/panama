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

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides a converter that accepts a string value and returns a <see cref="DateTime"/> object.
    /// </summary>
    public class StringToDateConverter : IValueConverter
    {
        #region Public methods
        /// <summary>
        /// Attempts to convert a string value to a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="value">The string value</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>A <see cref="DateTime"/> object. If <paramref name="value"/> cannot be converted, returns <paramref name="value"/> unchanged.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try { return System.Convert.ToDateTime(value); }
            catch { return value; }
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