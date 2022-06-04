using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Diagnostics;

namespace Restless.Panama.Controls
{
    /// <summary>
    /// Provides a simple color picker control
    /// </summary>
    public class SimpleColorPicker : Control
    {
        public const double MinSelectorSize = 16;
        public const double MaxSelectorSize = 64;
        public const double DefaultSelectorSize = 32;
        public const int ItemsPerRow = 10;
        //public const double DefaultPopupWidth = ColorItemControl.ItemTotalSize * ItemsPerRow;

        // public const int HowMany = (int)(DefaultPopupWidth / ColorItemControl.ItemTotalSize);

        public const double DefaultPanelWidth = ItemsPerRow * ColorItemControl.ItemTotalSize;

        public static readonly ComponentResourceKey TransparentBrushKey = new ComponentResourceKey(typeof(SimpleColorPicker), nameof(TransparentBrushKey));

        #region Constructors
        public SimpleColorPicker()
        {
            SelectedColorBrush = GetTransparentBrush();
            AvailableColors = new List<ColorItemControl>();
            InitializeAvailableColors();
            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ClickedEventHandler));
        }

        static SimpleColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SimpleColorPicker), new FrameworkPropertyMetadata(typeof(SimpleColorPicker)));
        }
        #endregion

        /************************************************************************/

        #region Color / Brushes
        /// <summary>
        /// Gets the list of available colors
        /// </summary>
        public List<ColorItemControl> AvailableColors
        {
            get => (List<ColorItemControl>)GetValue(AvailableColorsProperty);
            private set => SetValue(AvailableColorsPropertyKey, value);
        }

        private static readonly DependencyPropertyKey AvailableColorsPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(AvailableColors), typeof(List<ColorItemControl>), typeof(SimpleColorPicker), new FrameworkPropertyMetadata()
                {
                }
            );

        /// <summary>
        /// Identifies the <see cref="AvailableColors"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AvailableColorsProperty = AvailableColorsPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets the selected color used for this control.
        /// </summary>
        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectedColor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register
            (
                nameof(SelectedColor), typeof(Color), typeof(SimpleColorPicker), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Colors.Transparent,
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = OnSelectedColorPropertyChanged
                }
            );

        private static void OnSelectedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SimpleColorPicker control)
            {
                control.SelectedColorBrush = control.SelectedColor != Colors.Transparent ? new SolidColorBrush(control.SelectedColor) : GetTransparentBrush();
            }
        }

        /// <summary>
        /// Gets the brush that corresponds to the selected color
        /// </summary>
        public Brush SelectedColorBrush
        {
            get => (Brush)GetValue(SelectedColorBrushProperty);
            private set => SetValue(SelectedColorBrushPropertyKey, value);
        }

        private static readonly DependencyPropertyKey SelectedColorBrushPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(SelectedColorBrush), typeof(Brush), typeof(SimpleColorPicker), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Identifies the <see cref="SelectedColorBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedColorBrushProperty = SelectedColorBrushPropertyKey.DependencyProperty;


        /// <summary>
        /// Gets or sets the rollover border brush
        /// </summary>
        public Brush RolloverBorderBrush
        {
            get => (Brush)GetValue(RolloverBorderBrushProperty);
            set => SetValue(RolloverBorderBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RolloverBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RolloverBorderBrushProperty = DependencyProperty.Register
            (
                nameof(RolloverBorderBrush), typeof(Brush), typeof(SimpleColorPicker), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Brushes.DarkSlateGray
                }
            );

        /// <summary>
        /// Gets or sets the popup border brush
        /// </summary>
        public Brush PopupBorderBrush
        {
            get => (Brush)GetValue(PopupBorderBrushProperty);
            set => SetValue(PopupBorderBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PopupBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupBorderBrushProperty = DependencyProperty.Register
            (
                nameof(PopupBorderBrush), typeof(Brush), typeof(SimpleColorPicker), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Brushes.LightGray
                }
            );

        /// <summary>
        /// Gets or sets the popup background
        /// </summary>
        public Brush PopupBackground
        {
            get => (Brush)GetValue(PopupBackgroundProperty);
            set => SetValue(PopupBackgroundProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PopupBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupBackgroundProperty = DependencyProperty.Register
            (
                nameof(PopupBackground), typeof(Brush), typeof(SimpleColorPicker), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Brushes.White
                }
            );
        #endregion

        /************************************************************************/

        #region Dimensions
        /// <summary>
        /// Gets or sets the size of the selector
        /// </summary>
        public double SelectorSize
        {
            get => (double)GetValue(SelectorSizeProperty);
            set => SetValue(SelectorSizeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectorSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorSizeProperty = DependencyProperty.Register
            (
                nameof(SelectorSize), typeof(double), typeof(SimpleColorPicker), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultSelectorSize,
                    CoerceValueCallback = OnCoerceSelectorSize,
                    PropertyChangedCallback = OnSelectorSizeChanged
                }
            );

        private static object OnCoerceSelectorSize(DependencyObject d, object baseValue)
        {
            return baseValue is double value ? Math.Clamp(value, MinSelectorSize, MaxSelectorSize) : baseValue;
        }

        private static void OnSelectorSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SimpleColorPicker control)
            {
                control.PopupHorizontalOffset = control.SelectorSize / 2;
                control.PopupVerticalOffset = -(control.SelectorSize / 2);
            }
        }

        /// <summary>
        /// Gets the popup horizontal offset
        /// </summary>
        public double PopupHorizontalOffset
        {
            get => (double)GetValue(PopupHorizontalOffsetProperty);
            private set => SetValue(PopupHorizontalOffsetPropertyKey, value);
        }

        private static readonly DependencyPropertyKey PopupHorizontalOffsetPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(PopupHorizontalOffset), typeof(double), typeof(SimpleColorPicker), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultSelectorSize / 2
                }
            );

        /// <summary>
        /// Identifies the <see cref="PopupHorizontalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupHorizontalOffsetProperty = PopupHorizontalOffsetPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the popup vertical offset
        /// </summary>
        public double PopupVerticalOffset
        {
            get => (double)GetValue(PopupVerticalOffsetProperty);
            private set => SetValue(PopupVerticalOffsetPropertyKey, value);
        }

        private static readonly DependencyPropertyKey PopupVerticalOffsetPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(PopupVerticalOffset), typeof(double), typeof(SimpleColorPicker), new FrameworkPropertyMetadata()
                {
                    DefaultValue = -(DefaultSelectorSize / 2)
                }
            );

        /// <summary>
        /// Identifies the <see cref="PopupVerticalOffset"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PopupVerticalOffsetProperty = PopupVerticalOffsetPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Private methods
        private static Brush GetTransparentBrush()
        {
            return Application.Current.TryFindResource(TransparentBrushKey) as Brush;
        }

        private void InitializeAvailableColors()
        {
            PropertyInfo[] colorProps = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public);
            foreach (PropertyInfo info in colorProps)
            {
                AvailableColors.Add(new ColorItemControl((Color)info.GetValue(null), info.Name));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private const string ButtonName = "BAB3DCA3_DE3F_4C8B_927E_BE59F836C64A";

        private void ClickedEventHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button item && item.Name == ButtonName)
            {
                SelectedColor = (Color)item.Tag;
                e.Handled = true;
            }
        }
        #endregion
    }
}