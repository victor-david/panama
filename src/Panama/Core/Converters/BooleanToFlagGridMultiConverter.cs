/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Controls;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides a converter that returns a representation of a series of boolean flags
    /// </summary>
    public class BooleanToFlagGridMultiConverter : IMultiValueConverter
    {
        #region Public methods
        /// <summary>
        /// Converts boolean values into a <see cref="FlagGrid"/>
        /// </summary>
        /// <param name="values">The array of values.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Flag grid column collection</param>
        /// <param name="culture">Not used.</param>
        /// <returns>A <see cref="FlagGrid"/></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return
                FlagGrid.Create()
                .SetColumns(parameter as FlagGridColumnCollection)
                .CreateFlags(values);
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
        #endregion
    }
}