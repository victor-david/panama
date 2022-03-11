/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.Panama.Controls
{
    /// <summary>
    /// Represents a single color item.
    /// </summary>
    public class ColorItemControl : Control
    {
        public const int ItemMargin = 1;
        public const double ItemRawSize = 22;
        public const double ItemTotalSize = ItemRawSize + (ItemMargin * 2);
       

        #region Constructors
        internal ColorItemControl(Color color, string name)
        {
            Background = new SolidColorBrush(color);
            Color = color;
            DisplayName = name;
        }

        static ColorItemControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ColorItemControl), new FrameworkPropertyMetadata(typeof(ColorItemControl)));
        }
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the color associated with this control
        /// </summary>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            private set => SetValue(ColorPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ColorPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(Color), typeof(Color), typeof(ColorItemControl), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Colors.Transparent
                }
            );

        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty = ColorPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the display name for the color associated with this control
        /// </summary>
        public string DisplayName
        {
            get => (string)GetValue(DisplayNameProperty);
            private set => SetValue(DisplayNamePropertyKey, value);
        }

        private static readonly DependencyPropertyKey DisplayNamePropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(DisplayName), typeof(string), typeof(ColorItemControl), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Identifies the <see cref="DisplayName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayNameProperty = DisplayNamePropertyKey.DependencyProperty;
        #endregion
    }
}