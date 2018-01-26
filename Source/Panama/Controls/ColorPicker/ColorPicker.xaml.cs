﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
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
        /// Gets or sets the selected color used for this control
        /// </summary>
        public Color SelectedColor
        {
            get { return (Color)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        /// <summary>
        /// Dependency property definition for the <see cref="SelectedColor"/> property
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
            (
                nameof(SelectedColor), typeof(Color), typeof(ColorPicker), 
                new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedColorPropertyChanged)
            );

        /// <summary>
        /// Gets or sets a command to run when <see cref="SelectedColor"/> changes.
        /// </summary>
        public ICommand SelectedColorChangedCommand
        {
            get { return (ICommand)GetValue(SelectedColorChangedCommandProperty); }
            set { SetValue(SelectedColorChangedCommandProperty, value); }
        }

        /// <summary>
        /// Dependency property definition for the <see cref="SelectedColorChangedCommand"/> property
        /// </summary>
        public static readonly DependencyProperty SelectedColorChangedCommandProperty = DependencyProperty.Register
            (
                nameof(SelectedColorChangedCommand), typeof(ICommand), typeof(ColorPicker), new PropertyMetadata(null)
            );
       

        public ObservableCollection<ColorItem> AvailableColors
        {
            get { return (ObservableCollection<ColorItem>)GetValue(AvailableColorsProperty); }
            set { SetValue(AvailableColorsProperty, value); }
        }

        public static readonly DependencyProperty AvailableColorsProperty = DependencyProperty.Register
            (
                nameof(AvailableColors), typeof(ObservableCollection<ColorItem>), typeof(ColorPicker), new UIPropertyMetadata(CreateAvailableColors())
            );

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register
            (
                nameof(IsOpen), typeof(bool), typeof(ColorPicker), new UIPropertyMetadata(false)
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
            ObservableCollection<ColorItem> standardColors = new ObservableCollection<ColorItem>
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
            return standardColors;
        }

        private static ObservableCollection<ColorItem> CreateAvailableColors()
        {
            ObservableCollection<ColorItem> standardColors = new ObservableCollection<ColorItem>();

            foreach (var item in ColorUtilities.KnownColors)
            {
                if (!String.Equals(item.Key, "Transparent"))
                {
                    var colorItem = new ColorItem(item.Value, item.Key);
                    if (!standardColors.Contains(colorItem))
                    {
                        standardColors.Add(colorItem);
                    }
                }
            }

            return standardColors;
        }

        private void CloseColorPicker()
        {
            IsOpen = false;
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
