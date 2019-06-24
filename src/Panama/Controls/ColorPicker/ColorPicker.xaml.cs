/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        #region Public Properties
        /// <summary>
        /// Gets or sets the selected color used for this control.
        /// </summary>
        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        /// <summary>
        /// Dependency property definition for the <see cref="SelectedColor"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
            (
                nameof(SelectedColor), typeof(Color), typeof(ColorPicker),
                new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorPropertyChanged)
            );

        /// <summary>
        /// Gets or sets a command to run when <see cref="SelectedColor"/> changes.
        /// </summary>
        public ICommand SelectedColorChangedCommand
        {
            get => (ICommand)GetValue(SelectedColorChangedCommandProperty);
            set => SetValue(SelectedColorChangedCommandProperty, value);
        }

        /// <summary>
        /// Dependency property definition for the <see cref="SelectedColorChangedCommand"/> property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorChangedCommandProperty = DependencyProperty.Register
            (
                nameof(SelectedColorChangedCommand), typeof(ICommand), typeof(ColorPicker), new PropertyMetadata(null)
            );


        /// <summary>
        /// Gets or sets the collection of available colors.
        /// </summary>
        public ObservableCollection<ColorItem> AvailableColors
        {
            get => (ObservableCollection<ColorItem>)GetValue(AvailableColorsProperty);
            set => SetValue(AvailableColorsProperty, value);
        }

        /// <summary>
        /// Dependency property definition for the <see cref="AvailableColors"/> property.
        /// </summary>
        public static readonly DependencyProperty AvailableColorsProperty = DependencyProperty.Register
            (
                nameof(AvailableColors), typeof(ObservableCollection<ColorItem>), typeof(ColorPicker), new UIPropertyMetadata(CreateAvailableColors())
            );

        /// <summary>
        /// Gets or sets the mode to use when sorting the <see cref="AvailableColors"/> collection.
        /// </summary>
        public ColorSortingMode ColorSortingMode
        {
            get => (ColorSortingMode)GetValue(ColorSortingModeProperty);
            set => SetValue(ColorSortingModeProperty, value);
        }

        /// <summary>
        /// Dependency property definition for the <see cref="ColorSortingMode"/> property.
        /// </summary>
        public static readonly DependencyProperty ColorSortingModeProperty = DependencyProperty.Register
            (
                nameof(ColorSortingMode), typeof(ColorSortingMode), typeof(ColorPicker),
                new UIPropertyMetadata(ColorSortingMode.Alpha, OnColorSortingModeChanged)
            );

        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ColorPicker"/> class.
        /// </summary>
        public ColorPicker()
        {
            InitializeComponent();
            PART_AvailableColors.SelectionChanged += Color_SelectionChanged;
        }
        #endregion

        /************************************************************************/

        #region Private Methods

        private static void OnSelectedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker control && control.SelectedColorChangedCommand != null)
            {
                if (control.SelectedColorChangedCommand.CanExecute(e.NewValue))
                {
                    control.SelectedColorChangedCommand.Execute(e.NewValue);
                }
            }
        }

        private static ObservableCollection<ColorItem> CreateStandardColors()
        {
            ObservableCollection<ColorItem> collection = new ObservableCollection<ColorItem>
            {
                new ColorItem(Colors.Transparent, "Transparent"),
                new ColorItem(Colors.White, "White"),
                new ColorItem(Colors.Gray, "Gray"),
                new ColorItem(Colors.Black, "Black"),
                new ColorItem(Colors.Red, "Red"),
                new ColorItem(Colors.Green, "Green"),
                new ColorItem(Colors.Blue, "Blue"),
                new ColorItem(Colors.Yellow, "Yellow"),
                new ColorItem(Colors.Orange, "Orange"),
                new ColorItem(Colors.Purple, "Purple")
            };
            return collection;
        }

        private static ObservableCollection<ColorItem> CreateAvailableColors()
        {
            ObservableCollection<ColorItem> collection = new ObservableCollection<ColorItem>();

            foreach (var item in ColorUtilities.KnownColors)
            {
                //if (!String.Equals(item.Key, "Transparent"))
                {
                    var colorItem = new ColorItem(item.Value, item.Key);
                    if (!collection.Contains(colorItem))
                    {
                        collection.Add(colorItem);
                    }
                }
            }

            //foreach (ColorItem item in CreateStandardColors())
            //{
            //    if (!collection.Contains(item))
            //    collection.Add(item);
            //}

            return collection;
        }

        private static void OnColorSortingModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorPicker control)
            {
                control.OnColorSortingModeChanged();
            }
        }

        private void OnColorSortingModeChanged()
        {
            ListCollectionView lcv = (ListCollectionView)(CollectionViewSource.GetDefaultView(AvailableColors));
            if (lcv != null)
            {
                lcv.CustomSort = (ColorSortingMode == ColorSortingMode.HSB)
                                  ? new ColorSorter()
                                  : null;
            }
        }

        private void CloseColorPicker()
        {
            PART_ColorPickerToggleButton.IsChecked = false;
            ReleaseMouseCapture();
        }

        private void Color_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            if (e.AddedItems.Count > 0)
            {
                var colorItem = (ColorItem)e.AddedItems[0];
                SelectedColor = colorItem.Color;
                CloseColorPicker();
                lb.SelectedIndex = -1;
            }
        }
        #endregion
    }
}