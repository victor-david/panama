/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Windows.Data;
using System.Windows.Markup;

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides a converter that accepts a <see cref="DateTime"/> object and returns a formatted string.
    /// </summary>
    public class DateToFormattedDateConverter : MarkupExtension, IValueConverter
    {
        #region Public methods
        /// <summary>
        /// Converts a <see cref="DateTime"/> object to a formatted string.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> object.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>A date formatted string according to the application's <see cref="Core.Config.DateFormat"/> property.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is DateTime dt)
            {
                return dt.ToString(Core.Config.Instance.DateFormat);
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