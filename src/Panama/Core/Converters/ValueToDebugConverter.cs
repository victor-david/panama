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
using System.Globalization;
using System.Diagnostics;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides a debugging converter. When this converter is used, breaks into the debugger
    /// </summary>
    public class ValueToDebugConverter : IValueConverter
    {
        /// <summary>
        /// Breaks into the debugger.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns><paramref name="value"/> unchanged.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        /// <summary>
        /// Breaks into the debugger.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">Not used.</param>
        /// <returns><paramref name="value"/> unchanged.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }
}