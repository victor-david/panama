/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shapes;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides a converter that accepts a long integer value and returns a path resource
    /// </summary>
    public class Int64ToPathConverter : IValueConverter
    {
        #region Public methods
        /// <summary>
        /// Converts a long integer value to a path resource
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">A Dictionary<long, string> that maps values to resource ids</param>
        /// <param name="culture">Not used.</param>
        /// <returns>A path resource</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is long key && parameter is Dictionary<long, string> map && map.ContainsKey(key)
                ? LocalResources.Get<Path>(map[key])
                : null;
        }

        /// <summary>
        /// This method is not used. It throws a <see cref="NotImplementedException"/>
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}