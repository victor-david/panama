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
using System.Windows;
using System.Diagnostics;
using System.Windows.Markup;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides a converter that accepts a value type and returns a boolean. Used in radio button binding.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This converter can be used to convert a "0" / "1" string value (such as in the config table where all values a string) into boolean.
    /// </para>
    /// <code>
    ///   &lt;RadioButton GroupName="YesNo" Content="Yes" IsChecked="{Binding SelectedRow[strcol],Converter={cv:ValueToBooleanConverter},ConverterParameter=1}" /&gt;
    ///   &lt;RadioButton GroupName="YesNo" Content="No" IsChecked="{Binding SelectedRow[strcol],Converter={cv:ValueToBooleanConverter},ConverterParameter=0}" /&gt;
    /// </code>
    /// <para>
    /// By changing the ConverterParameter, you can bind a radio button to an underlying boolean value.
    /// </para>
    /// <code>
    ///   &lt;RadioButton GroupName="YesNo" Content="Yes" IsChecked="{Binding SelectedRow[boolcol],Converter={cv:StringToBooleanConverter},ConverterParameter={StaticResource True}}"/&gt;
    ///   &lt;RadioButton GroupName="YesNo" Content="No" IsChecked="{Binding SelectedRow[boolcol],Converter={cv:StringToBooleanConverter},ConverterParameter={StaticResource False}}"/&gt;
    /// </code>
    /// <para>
    /// Or to another underlying integer value.
    /// </para>
    /// <code>
    ///   &lt;RadioButton GroupName="Choices" Content="Choice1" IsChecked="{Binding SelectedRow[intcol],Converter={cv:ValueToBooleanConverter},ConverterParameter={StaticResource Integer1}}"/&gt;
    ///   &lt;RadioButton GroupName="Choices" Content="Choice2" IsChecked="{Binding SelectedRow[intcol],Converter={cv:ValueToBooleanConverter},ConverterParameter={StaticResource Integer2}}"/&gt;
    ///   &lt;RadioButton GroupName="Choices" Content="Choice3" IsChecked="{Binding SelectedRow[intcol],Converter={cv:ValueToBooleanConverter},ConverterParameter={StaticResource Integer3}}"/&gt;
    /// </code>
    /// </remarks>
    public class ValueToBooleanConverter : MarkupExtension, IValueConverter
    {
        #region Constructor
        #pragma warning disable 1591
        public ValueToBooleanConverter()
        {
            // prevents the designer that's referencing this converter directly from going stupid every time you type a character
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Converts a value type to a boolean value.
        /// </summary>
        /// <param name="value">The string or boolean value.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">The value to compare <paramref name="value"/> to.</param>
        /// <param name="culture">Not used.</param>
        /// <returns>true if <paramref name="value"/> equals <paramref name="parameter"/>; otherwise, false.</returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        /// <summary>
        /// Converts a boolean value to the specified <paramref name="parameter"/> value.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        /// <param name="targetType">Not used.</param>
        /// <param name="parameter">The value to return (if <paramref name="value"/> is true)</param>
        /// <param name="culture">Not used.</param>
        /// <returns><paramref name="parameter"/> if <paramref name="value"/> is true; otherwise, <see cref="Binding.DoNothing"/>.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
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