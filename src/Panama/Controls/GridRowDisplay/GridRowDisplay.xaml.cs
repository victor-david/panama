/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Provides the interaction logic for a GridRowDisplay control.
    /// </summary>
    public partial class GridRowDisplay : UserControl
    {
        #region Private
        private static string ControlName = "PART_Control";
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the number of value columns this control has.
        /// </summary>
        public int Columns
        {
            get => (int)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        /// <summary>
        /// Defines a dependency property for the number of value columns.
        /// </summary>
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.Register
            (
                nameof(Columns), typeof(int), typeof(GridRowDisplay), new UIPropertyMetadata(0, ColumnChanged, CoerceColumnValue)
            );

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
                nameof(Header), typeof(string), typeof(GridRowDisplay), new UIPropertyMetadata(null)
            );
        
        /// <summary>
        /// Gets or sets the width of <see cref="Header"/>.
        /// </summary>
        public double HeaderWidth
        {
            get => (double)GetValue(HeaderWidthProperty);
            set => SetValue(HeaderWidthProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that controls the width of <see cref="Header"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderWidthProperty = DependencyProperty.Register
            (
                nameof(HeaderWidth), typeof(double), typeof(GridRowDisplay), new UIPropertyMetadata(120.0)
            );

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
                nameof(HeaderForeground), typeof(Brush), typeof(GridRowDisplay), new UIPropertyMetadata(new SolidColorBrush(Colors.Black))
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
                nameof(HeaderFontSize), typeof(double), typeof(GridRowDisplay), new UIPropertyMetadata(11.0)
            );

        /// <summary>
        /// Gets or sets the width of the controls's values.
        /// </summary>
        public double ValueWidth
        {
            get => (double)GetValue(ValueWidthProperty);
            set => SetValue(ValueWidthProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that controls the width of the control's values.
        /// </summary>
        public static readonly DependencyProperty ValueWidthProperty = DependencyProperty.Register
            (
                nameof(ValueWidth), typeof(double), typeof(GridRowDisplay), new UIPropertyMetadata(48.0, ValueWidthChanged)
            );

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
                nameof(ValueForeground), typeof(Brush), typeof(GridRowDisplay), new UIPropertyMetadata(new SolidColorBrush(Colors.Blue))
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
                nameof(ValueFontSize), typeof(double), typeof(GridRowDisplay), new UIPropertyMetadata(11.0)
            );

        /// <summary>
        /// Gets the values for the control.
        /// </summary>
        public ObservableCollection<object> Values
        {
            get => (ObservableCollection<object>)GetValue(ValuesProperty);
        }

        /// <summary>
        /// Registers the read-only dependency property <see cref="ValuesProperty"/>
        /// </summary>
        private static readonly DependencyPropertyKey ValuesKey = DependencyProperty.RegisterReadOnly
            (
                nameof(Values), typeof(ObservableCollection<object>), typeof(GridRowDisplay), new PropertyMetadata()
            );

        /// <summary>
        /// Defines a read-only dependency property that describes the <see cref="Values"/> collection.
        /// </summary>
        public static readonly DependencyProperty ValuesProperty = ValuesKey.DependencyProperty;

        /// <summary>
        /// Gets or sets the display value. This is mapped to the first item in <see cref="Values"/>
        /// </summary>
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that displays the control value.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
            (
                nameof(Value), typeof(object), typeof(GridRowDisplay), new UIPropertyMetadata(null, ValueChanged)
            );

        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="GridRowDisplay"/> class.
        /// </summary>
        public GridRowDisplay()
        {
            InitializeComponent();
            var collection = new ObservableCollection<object>();
            collection.CollectionChanged += new NotifyCollectionChangedEventHandler(ValuesChanged);
            SetValue(ValuesKey, collection);
            Columns = 1;
        }
        #endregion

        /************************************************************************/
        
        #region Public methods
        /// <summary>
        /// Sets the values of the <see cref="Values"/> property according to the specified parameters.
        /// </summary>
        /// <param name="values">The values to place into the <see cref="Values"/> collection.</param>
        public void SetValues(params object[] values)
        {
            for (int k = 0; k < values.Length; k++)
            {
                if (k < Values.Count)
                {
                    Values[k] = values[k];
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (static)

        private static object CoerceColumnValue(DependencyObject d, object baseValue)
        {
            int value = (int)baseValue;
            if (value < 1) value = 1;
            if (value > 10) value = 10;
            return value;
        }

        private static void ColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CreateLayout(d as GridRowDisplay);
        }

        private static void CreateLayout(GridRowDisplay g)
        {
            if (g == null) return;

            // remove all except the header colum definition.
            while (g.PART_Grid.ColumnDefinitions.Count > 1)
            {
                g.PART_Grid.ColumnDefinitions.RemoveAt(g.PART_Grid.ColumnDefinitions.Count - 1);
            }

            // add column definitions for each value column 
            for (int col = 1; col <= g.Columns; col++)
            {
                g.PART_Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(g.ValueWidth) });
            }

            // add value placeholders if needed.
            while (g.Values.Count < g.Columns)
            {
                g.Values.Add(null);
            }

            // remove value entries if needed
            while (g.Values.Count > g.Columns)
            {
                g.Values.RemoveAt(g.Values.Count - 1);
            }

            // remove all grid children except the header
            while (g.PART_Grid.Children.Count > 1)
            {
                g.PART_Grid.Children.RemoveAt(g.PART_Grid.Children.Count - 1);
            }
            // add children
            for (int col = 1; col <= g.Columns; col++)
            {
                TextBlock tb = new TextBlock
                {
                    VerticalAlignment = VerticalAlignment.Center
                };

                tb.SetValue(Grid.ColumnProperty, col);
                g.PART_Grid.Children.Add(tb);

                // Set the binding on the foregound and the font size.
                // The values themselves are handled by the event handler on Values.CollectionChanged.
                var bfore = new Binding(ValueForegroundProperty.Name)
                {
                    Mode = BindingMode.OneWay,
                    ElementName = ControlName
                };
                tb.SetBinding(TextBlock.ForegroundProperty, bfore);

                var bfont = new Binding(ValueFontSizeProperty.Name)
                {
                    Mode = BindingMode.OneWay,
                    ElementName = ControlName
                };
                tb.SetBinding(TextBlock.FontSizeProperty, bfont);
            }
        }

        private static void ValueWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GridRowDisplay g)) return;
            // adjust column definitions for each value column 
            for (int col = 1; col <= g.Columns; col++)
            {
                g.PART_Grid.ColumnDefinitions[col] = new ColumnDefinition() { Width = new GridLength(g.ValueWidth) };
            }
        }

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is GridRowDisplay g)) return;
            g.Values[0] = e.NewValue;
        }
        #endregion

        /************************************************************************/

        #region Private methods (instance)
        private void ValuesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                ((TextBlock)PART_Grid.Children[e.NewStartingIndex + 1]).Text = e.NewItems[0].ToString();
            }
        }
        #endregion
    }
}