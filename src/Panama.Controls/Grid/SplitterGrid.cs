using System;
using System.Windows;
using System.Windows.Controls;

namespace Restless.Panama.Controls
{
    public class SplitterGrid : Control
    {
        public static readonly ComponentResourceKey SplitterStyleKey = new ComponentResourceKey(typeof(SplitterGrid), nameof(SplitterStyleKey));
        public const double MinLeftMinWidth = 100;
        public const double MaxLeftMinWidth = 1000;
        public const double DefaultLeftMinWidth = 480;

        public const double MinRightMinWidth = 100;
        public const double MaxRightMinWidth = 480;
        public const double DefaultRightMinWidth = 280;

        public const double DefaultLeftWidth = 480;
        public const double DefaultLeftMaxWidth = 520;

        public SplitterGrid()
        {
        }

        static SplitterGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterGrid), new FrameworkPropertyMetadata(typeof(SplitterGrid)));
        }

        #region Dimensions
        /// <summary>
        /// Gets or sets the minimum width for the left column
        /// </summary>
        public double LeftMinWidth
        {
            get => (double)GetValue(LeftMinWidthProperty);
            set => SetValue(LeftMinWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LeftMinWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftMinWidthProperty = DependencyProperty.Register
            (
                nameof(LeftMinWidth), typeof(double), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultLeftMinWidth,
                    CoerceValueCallback = OnCoerceLeftMinWidth,
                }
            );

        private static object OnCoerceLeftMinWidth(DependencyObject d, object baseValue)
        {
            return Math.Clamp((double)baseValue, MinLeftMinWidth, MaxLeftMinWidth);
        }

        /// <summary>
        /// Gets or sets the minimum width for the right column
        /// </summary>
        public double RightMinWidth
        {
            get => (double)GetValue(RightMinWidthProperty);
            set => SetValue(RightMinWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RightMinWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RightMinWidthProperty = DependencyProperty.Register
            (
                nameof(RightMinWidth), typeof(double), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultRightMinWidth,
                    CoerceValueCallback = OnCoerceRightMinWidth
                }
            );

        private static object OnCoerceRightMinWidth(DependencyObject d, object baseValue)
        {
            return Math.Clamp((double)baseValue, MinRightMinWidth, MaxRightMinWidth);
        }

        /// <summary>
        /// Gets or sets the left grid width
        /// </summary>
        public GridLength LeftWidth
        {
            get => (GridLength)GetValue(LeftWidthProperty);
            set => SetValue(LeftWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="LeftWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftWidthProperty = DependencyProperty.Register
            (
                nameof(LeftWidth), typeof(GridLength), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = new GridLength(DefaultLeftWidth),
                    BindsTwoWayByDefault = true,
                    // CoerceValueCallback = OnCoerceLeftWidth,
                    PropertyChangedCallback = OnLeftWidthChanged
                }
            );

        //private static object OnCoerceLeftWidth(DependencyObject d, object baseValue)
        //{
        //    return (d as SplitterGrid)?.GetLeftWidth((GridLength)baseValue) ?? baseValue;
        //}

        private static void OnLeftWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SplitterGrid)?.SetLeftMaxWidth();
        }

        /// <summary>
        /// Gets the maxium width for the left column
        /// </summary>
        public double LeftMaxWidth
        {
            get => (double)GetValue(LeftMaxWidthProperty);
            private set => SetValue(LeftMaxWidthPropertyKey, value);
        }

        private static readonly DependencyPropertyKey LeftMaxWidthPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(LeftMaxWidth), typeof(double), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultLeftMaxWidth
                }
            );

        /// <summary>
        /// Identifies the <see cref="LeftMaxWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LeftMaxWidthProperty = LeftMaxWidthPropertyKey.DependencyProperty;

        #endregion

        /************************************************************************/

        #region Margins
        /// <summary>
        /// Gets or sets the margin for the left header
        /// </summary>
        public Thickness MarginHeaderLeft
        {
            get => (Thickness)GetValue(MarginHeaderLeftProperty);
            set => SetValue(MarginHeaderLeftProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MarginHeaderLeft"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarginHeaderLeftProperty = DependencyProperty.Register
            (
                nameof(MarginHeaderLeft), typeof(Thickness), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = new Thickness()
                }
            );

        /// <summary>
        /// Gets or sets the margin for the right header
        /// </summary>
        public Thickness MarginHeaderRight
        {
            get => (Thickness)GetValue(MarginHeaderRightProperty);
            set => SetValue(MarginHeaderRightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MarginHeaderRight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarginHeaderRightProperty = DependencyProperty.Register
            (
                nameof(MarginHeaderRight), typeof(Thickness), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = new Thickness()
                }
            );

        /// <summary>
        /// Gets or sets the margin for the left content
        /// </summary>
        public Thickness MarginContentLeft
        {
            get => (Thickness)GetValue(MarginContentLeftProperty);
            set => SetValue(MarginContentLeftProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MarginContentLeft"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarginContentLeftProperty = DependencyProperty.Register
            (
                nameof(MarginContentLeft), typeof(Thickness), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = new Thickness()
                }
            );

        /// <summary>
        /// Gets or sets the margin for the right content
        /// </summary>
        public Thickness MarginContentRight
        {
            get => (Thickness)GetValue(MarginContentRightProperty);
            set => SetValue(MarginContentRightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MarginContentRight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarginContentRightProperty = DependencyProperty.Register
            (
                nameof(MarginContentRight), typeof(Thickness), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = new Thickness()
                }
            );
        #endregion

        /************************************************************************/

        #region Header / content
        /// <summary>
        /// Gets or sets the left side header content
        /// </summary>
        public object HeaderLeft
        {
            get => GetValue(HeaderLeftProperty);
            set => SetValue(HeaderLeftProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="HeaderLeft"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderLeftProperty = DependencyProperty.Register
            (
                nameof(HeaderLeft), typeof(object), typeof(SplitterGrid), new FrameworkPropertyMetadata()
            );


        /// <summary>
        /// Gets or sets right side header content
        /// </summary>
        public object HeaderRight
        {
            get => GetValue(HeaderRightProperty);
            set => SetValue(HeaderRightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="HeaderRight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderRightProperty = DependencyProperty.Register
            (
                nameof(HeaderRight), typeof(object), typeof(SplitterGrid), new FrameworkPropertyMetadata()
            );


        /// <summary>
        /// Gets or sets the left side content
        /// </summary>
        public object ContentLeft
        {
            get => GetValue(ContentLeftProperty);
            set => SetValue(ContentLeftProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ContentLeft"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentLeftProperty = DependencyProperty.Register
            (
                nameof(ContentLeft), typeof(object), typeof(SplitterGrid), new FrameworkPropertyMetadata()
            );


        /// <summary>
        /// Gets or sets the right side content
        /// </summary>
        public object ContentRight
        {
            get => GetValue(ContentRightProperty);
            set => SetValue(ContentRightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ContentRight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentRightProperty = DependencyProperty.Register
            (
                nameof(ContentRight), typeof(object), typeof(SplitterGrid), new FrameworkPropertyMetadata()
            );
        #endregion

        /************************************************************************/

        #region Private methods
        private void SetLeftMaxWidth()
        {
            LeftMaxWidth = ActualWidth - RightMinWidth;
        }

        //private GridLength GetLeftWidth(GridLength current)
        //{
        //    return ActualWidth - current.Value < RightMinWidth ? new GridLength(ActualWidth - RightMinWidth) : current;
        //}
        #endregion

    }
}