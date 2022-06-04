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
    /// Provides a converter that accepts two <see cref="DateTime"/> objects and returns the number of days between them.
    /// </summary>
    public class DatesToDayDiffConverter : IMultiValueConverter
    {
        #region Public methods
        /// <summary>
        /// Receives two <see cref="DateTime"/> objects and returns the number of days between them.
        /// </summary>
        /// <param name="values">The array of <see cref="DateTime"/> objects.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>A string that contains the number of days between the two dates.</returns>
        /// <remarks>
        /// If there is not at least one element in <paramref name="values"/>, this method returns null.
        /// If there is only one <see cref="DateTime"/> object in <paramref name="values"/>, the difference is
        /// calculated between today and the supplied date.
        /// </remarks>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values.Length > 1)
            {
                DateTime date1 = DateTime.UtcNow;
                DateTime date2 = DateTime.UtcNow;
                if (values[0] is DateTime) date1 = (DateTime)values[0];
                if (values[1] is DateTime) date2 = (DateTime)values[1];
                return (date2 - date1).Days.ToString();
            }
            return null;
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