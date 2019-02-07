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
    /// Provides a converter that that accepts two boolean values and returns one boolean value.
    /// </summary>
    public class BooleanToBooleanMultiConverter : IMultiValueConverter
    {
        #region Public methods
        /// <summary>
        /// Converts two boolean values into one.
        /// </summary>
        /// <param name="values">The array of values.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">Pass boolean true to reverse the evaluation of the parameters.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>true or false</returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IEnumerable<bool> bools = values.OfType<bool>();
            if (bools.Count<bool>() > 0)
            {
                bool reverse = false;
                if (parameter is bool) reverse = (bool)parameter;
                return Evaluate(bools, reverse);
            }
            return false;
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

        /************************************************************************/

        #region Private methods
        private bool Evaluate(IEnumerable<bool> bools, bool reverse)
        {
            bool result = reverse;

            foreach (bool item in bools)
            {
                if (!reverse)
                {
                    result = result && item;
                }
                else
                {
                    result = result && !item;
                }
            }
            return result;
        }
        #endregion
    } 
}