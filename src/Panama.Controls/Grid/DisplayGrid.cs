/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.Panama.Controls
{
    /// <summary>
    /// Represents a specialized grid control.
    /// </summary>
    /// <remarks>
    /// This class extends <see cref="Grid"/> for a special use case in which
    /// the control is created programaticly, not created via XAML.
    /// You can properties in a style, but the columns are created at run time.
    /// </remarks>
    public class DisplayGrid : Grid
    {
        #region Public fields
        public const double MinHeaderColumnWidth = 10;
        public const double MaxHeaderColumnWidth = 300;
        public const double DefaultHeaderColumnWidth = 120;

        public const double MinValueColumnWidth = 10;
        public const double MaxValueColumnWidth = 200;
        public const double DefaultValueColumnWidth = 48;

        public const double DefaultHeaderFontSize = 11;
        public const double DefaultValueFontSize = 11;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayGrid"/> class.
        /// </summary>
        public DisplayGrid()
        {
            CreateHeaderColumn();
        }
        #endregion

        /************************************************************************/

        #region Header
        /// <summary>
        /// Gets or sets the control header.
        /// </summary>
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that displays the control header.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register
            (
                nameof(Header), typeof(string), typeof(DisplayGrid), new FrameworkPropertyMetadata()
                {
                    PropertyChangedCallback = OnHeaderChanged
                }
            );

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DisplayGrid)?.SetHeaderValue();
        }

        /// <summary>
        /// Gets or sets the width of <see cref="Header"/>.
        /// </summary>
        public double HeaderColumnWidth
        {
            get => (double)GetValue(HeaderColumnWidthProperty);
            set => SetValue(HeaderColumnWidthProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that controls the width of <see cref="Header"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderColumnWidthProperty = DependencyProperty.Register
            (
                nameof(HeaderColumnWidth), typeof(double), typeof(DisplayGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultHeaderColumnWidth,
                    CoerceValueCallback = OnCoerceHeaderColumnWidth,
                    PropertyChangedCallback = OnHeaderColumnWidthChanged
                }
            );

        private static object OnCoerceHeaderColumnWidth(DependencyObject d, object baseValue)
        {
            return Math.Clamp((double)baseValue, MinHeaderColumnWidth, MaxHeaderColumnWidth);
        }

        private static void OnHeaderColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DisplayGrid)?.UpdateHeaderColumnWidth();
        }

        /// <summary>
        /// Gets or sets the foregound of <see cref="Header"/>.
        /// </summary>
        public Brush HeaderForeground
        {
            get => (Brush)GetValue(HeaderForegroundProperty);
            set => SetValue(HeaderForegroundProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that describes the foreground of <see cref="Header"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderForegroundProperty = DependencyProperty.Register
            (
                nameof(HeaderForeground), typeof(Brush), typeof(DisplayGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Brushes.Black
                }
            );

        /// <summary>
        /// Gets or sets the font size of <see cref="Header"/>.
        /// </summary>
        public double HeaderFontSize
        {
            get => (double)GetValue(HeaderFontSizeProperty);
            set => SetValue(HeaderFontSizeProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that describes the font size of <see cref="Header"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderFontSizeProperty = DependencyProperty.Register
            (
                nameof(HeaderFontSize), typeof(double), typeof(DisplayGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultHeaderFontSize
                }
            );
        #endregion

        /************************************************************************/

        #region Value
        /// <summary>
        /// Gets or sets the width of the controls's values.
        /// </summary>
        public double ValueColumnWidth
        {
            get => (double)GetValue(ValueColumnWidthProperty);
            set => SetValue(ValueColumnWidthProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that controls the width of the control's values.
        /// </summary>
        public static readonly DependencyProperty ValueColumnWidthProperty = DependencyProperty.Register
            (
                nameof(ValueColumnWidth), typeof(double), typeof(DisplayGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultValueColumnWidth,
                    CoerceValueCallback = OnCoerceValueColumnWidth,
                    PropertyChangedCallback = OnValueColumnWidthChanged
                }
            );

        private static object OnCoerceValueColumnWidth(DependencyObject d, object baseValue)
        {
            return Math.Clamp((double)baseValue, MinValueColumnWidth, MaxValueColumnWidth);
        }

        private static void OnValueColumnWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as DisplayGrid)?.UpdateValueColumnWidths();
        }

        /// <summary>
        /// Gets or sets the foregound of the controls' values.
        /// </summary>
        public Brush ValueForeground
        {
            get => (Brush)GetValue(ValueForegroundProperty);
            set => SetValue(ValueForegroundProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that describes the foreground of the control's values.
        /// </summary>
        public static readonly DependencyProperty ValueForegroundProperty = DependencyProperty.Register
            (
                nameof(ValueForeground), typeof(Brush), typeof(DisplayGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Brushes.Blue
                }
            );

        /// <summary>
        /// Gets or sets the font size of the control's values.
        /// </summary>
        public double ValueFontSize
        {
            get => (double)GetValue(ValueFontSizeProperty);
            set => SetValue(ValueFontSizeProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that describes the font size of <see cref="Header"/>.
        /// </summary>
        public static readonly DependencyProperty ValueFontSizeProperty = DependencyProperty.Register
            (
                nameof(ValueFontSize), typeof(double), typeof(DisplayGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultValueFontSize
                }
            );

        /// <summary>
        /// Gets or sets the horizonatl alignment for values
        /// </summary>
        public HorizontalAlignment ValueHorizontalAlignment
        {
            get => (HorizontalAlignment)GetValue(ValueHorizontalAlignmentProperty);
            set => SetValue(ValueHorizontalAlignmentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ValueHorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValueHorizontalAlignmentProperty = DependencyProperty.Register
            (
                nameof(ValueHorizontalAlignment), typeof(HorizontalAlignment), typeof(DisplayGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = HorizontalAlignment.Left
                }
            );
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Sets the values, creating the grid layout
        /// </summary>
        /// <param name="values">The values</param>
        public void SetValues(params object[] values)
        {
            CreateValuesLayout(values);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void CreateHeaderColumn()
        {
            if (ColumnDefinitions.Count == 0)
            {
                ColumnDefinitions.Add(new ColumnDefinition()
                {
                    Width = new GridLength(HeaderColumnWidth, GridUnitType.Pixel)
                });
            }
        }

        private void UpdateHeaderColumnWidth()
        {
            if (ColumnDefinitions.Count > 0)
            {
                ColumnDefinitions[0] = new ColumnDefinition()
                {
                    Width = new GridLength(HeaderColumnWidth, GridUnitType.Pixel)
                };
            }
        }

        private void SetHeaderValue()
        {
            TextBlock header = new TextBlock()
            {
                Text = Header,
                Foreground = HeaderForeground,
                FontSize = HeaderFontSize,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            SetColumn(header, 0);
            Children.Add(header);
        }

        private void UpdateValueColumnWidths()
        {
            for (int idx = 1; idx <= ColumnDefinitions.Count - 1; idx++)
            {
                ColumnDefinitions[idx] = new ColumnDefinition()
                {
                    Width = new GridLength(ValueColumnWidth, GridUnitType.Pixel)
                };
            }
        }

        private void CreateValuesLayout(params object[] values)
        {
            while (Children.Count > 1)
            {
                Children.RemoveAt(Children.Count - 1);
            }

            while (ColumnDefinitions.Count > 1)
            {
                ColumnDefinitions.RemoveAt(ColumnDefinitions.Count - 1);
            }

            for (int idx = 0; idx < values.Length; idx++)
            {
                ColumnDefinitions.Add(new ColumnDefinition() 
                {
                    Width = new GridLength(ValueColumnWidth, GridUnitType.Pixel)
                });

                TextBlock item = new TextBlock()
                {
                    Text = values[idx].ToString(),
                    Foreground = ValueForeground,
                    FontSize = ValueFontSize,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = ValueHorizontalAlignment
                };

                SetColumn(item, idx + 1);
                Children.Add(item);
            }
        }
        #endregion
    }
}