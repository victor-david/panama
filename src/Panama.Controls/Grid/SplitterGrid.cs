using System;
using System.Windows;
using System.Windows.Controls;

namespace Restless.Panama.Controls
{
    /// <summary>
    /// Provides a specialized control that contains a grid
    /// that can be expanded / contracted to show or hide detail
    /// </summary>
    public class SplitterGrid : Control
    {
        #region Private
        private bool widthUpdateInProgress;
        #endregion

        /************************************************************************/

        #region Public fields
        public static readonly ComponentResourceKey SplitterStyleKey = new ComponentResourceKey(typeof(SplitterGrid), nameof(SplitterStyleKey));
        public static readonly ComponentResourceKey ToggleButtonStyleKey = new ComponentResourceKey(typeof(SplitterGrid), nameof(ToggleButtonStyleKey));

        public const double MinMinDetailWidth = 100;
        public const double DefaultMinDetailWidth = 220;
        public const double MinMaxDetailWidth = 120;
        public const double DefaultMaxDetailWidth = 600;
        public const double DefaultDetailWidth = 480;
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SplitterGrid"/> class
        /// </summary>
        public SplitterGrid()
        {
        }

        static SplitterGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitterGrid), new FrameworkPropertyMetadata(typeof(SplitterGrid)));
        }
        #endregion

        /************************************************************************/

        #region State
        /// <summary>
        /// Gets or sets a boolean value that determines if the detail panel may be collapsed
        /// </summary>
        public bool CanCollapseDetail
        {
            get => (bool)GetValue(CanCollapseDetailProperty);
            set => SetValue(CanCollapseDetailProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CanCollapseDetail"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CanCollapseDetailProperty = DependencyProperty.Register
            (
                nameof(CanCollapseDetail), typeof(bool), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = true,
                    PropertyChangedCallback = OnCanCollapseDetailChanged
                }
            );

        private static void OnCanCollapseDetailChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SplitterGrid control)
            {
                control.ExpanderVisibility = control.CanCollapseDetail ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that determines if the detail panel is expanded.
        /// </summary>
        public bool IsDetailExpanded
        {
            get => (bool)GetValue(IsDetailExpandedProperty);
            set => SetValue(IsDetailExpandedProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsDetailExpanded"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDetailExpandedProperty = DependencyProperty.Register
            (
                nameof(IsDetailExpanded), typeof(bool), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = true,
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = OnIsDetailExpandedChanged
                }
            );

        private static void OnIsDetailExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SplitterGrid)?.UpdateActualDetailWidth();
        }

        /// <summary>
        /// Gets the...
        /// </summary>
        public Visibility ExpanderVisibility
        {
            get => (Visibility)GetValue(ExpanderVisibilityProperty);
            private set => SetValue(ExpanderVisibilityPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ExpanderVisibilityPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ExpanderVisibility), typeof(Visibility), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Visibility.Visible
                }
            );

        /// <summary>
        /// Identifies the <see cref="ExpanderVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpanderVisibilityProperty = ExpanderVisibilityPropertyKey.DependencyProperty;



        /// <summary>
        /// Gets the...
        /// </summary>
        public Visibility SplitterVisibility
        {
            get => (Visibility)GetValue(SplitterVisibilityProperty);
            private set => SetValue(SplitterVisibilityPropertyKey, value);
        }

        private static readonly DependencyPropertyKey SplitterVisibilityPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(SplitterVisibility), typeof(Visibility), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Visibility.Visible
                }
            );

        /// <summary>
        /// Identifies the <see cref="SplitterVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SplitterVisibilityProperty = SplitterVisibilityPropertyKey.DependencyProperty;


        #endregion

        /************************************************************************/

        #region Dimensions
        /// <summary>
        /// Gets or sets the minimum detail width
        /// </summary>
        public double MinDetailWidth
        {
            get => (double)GetValue(MinDetailWidthProperty);
            set => SetValue(MinDetailWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MinDetailWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MinDetailWidthProperty = DependencyProperty.Register
            (
                nameof(MinDetailWidth), typeof(double), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultMinDetailWidth,
                    CoerceValueCallback = OnCoerceMinDetailWidth,
                    PropertyChangedCallback = OnMinDetailWidthChanged
                }
            );

        private static object OnCoerceMinDetailWidth(DependencyObject d, object baseValue)
        {
            return Math.Max((double)baseValue, MinMinDetailWidth);
        }

        private static void OnMinDetailWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SplitterGrid)?.HandleMinDetailWidthChanged();
        }

        /// <summary>
        /// Gets the actual min detail width
        /// </summary>
        public double ActualMinDetailWidth
        {
            get => (double)GetValue(ActualMinDetailWidthProperty);
            private set => SetValue(ActualMinDetailWidthPropertyKey, value);
        }

        private static readonly DependencyPropertyKey ActualMinDetailWidthPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(ActualMinDetailWidth), typeof(double), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultMinDetailWidth
                }
            );

        /// <summary>
        /// Identifies the <see cref="ActualMinDetailWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ActualMinDetailWidthProperty = ActualMinDetailWidthPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets the maximum detail width
        /// </summary>
        public double MaxDetailWidth
        {
            get => (double)GetValue(MaxDetailWidthProperty);
            set => SetValue(MaxDetailWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="MaxDetailWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxDetailWidthProperty = DependencyProperty.Register
            (
                nameof(MaxDetailWidth), typeof(double), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultMaxDetailWidth,
                    CoerceValueCallback = OnCoerceMaxDetailWidth
                }
            );

        private static object OnCoerceMaxDetailWidth(DependencyObject d, object baseValue)
        {
            return Math.Max((double)baseValue, MinMaxDetailWidth);
        }

        /// <summary>
        /// Gets or sets the detail width
        /// </summary>
        public double DetailWidth
        {
            get => (double)GetValue(DetailWidthProperty);
            set => SetValue(DetailWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DetailWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DetailWidthProperty = DependencyProperty.Register
            (
                nameof(DetailWidth), typeof(double), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultDetailWidth,
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = OnDetailWidthChanged
                }
            );

        private static void OnDetailWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SplitterGrid)?.HandleDetailWidthChanged();
        }

        /// <summary>
        /// Gets or sets the actual detail width
        /// </summary>
        internal GridLength ActualDetailWidth
        {
            get => (GridLength)GetValue(ActualDetailWidthProperty);
            set => SetValue(ActualDetailWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ActualDetailWidth"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty ActualDetailWidthProperty = DependencyProperty.Register
            (
                nameof(ActualDetailWidth), typeof(GridLength), typeof(SplitterGrid), new FrameworkPropertyMetadata()
                {
                    DefaultValue = new GridLength(DefaultDetailWidth),
                    PropertyChangedCallback = OnActualDetailWidthChanged
                }
            );

        private static void OnActualDetailWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as SplitterGrid)?.HandleActualDetailWidthChanged();
        }
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
        private void HandleMinDetailWidthChanged()
        {
        }

        private void HandleDetailWidthChanged()
        {
            if (!widthUpdateInProgress)
            {
                UpdateActualDetailWidth();
            }
        }

        private void HandleActualDetailWidthChanged()
        {
            if (IsDetailExpanded)
            {
                widthUpdateInProgress = true;
                DetailWidth = ActualDetailWidth.Value;
                widthUpdateInProgress = false;
            }
        }

        private void UpdateActualDetailWidth()
        {
            if (IsDetailExpanded)
            {
                ActualDetailWidth = new GridLength(DetailWidth);
                ActualMinDetailWidth = MinDetailWidth;
                SplitterVisibility = Visibility.Visible;
            }
            else
            {
                ActualMinDetailWidth = 0;
                ActualDetailWidth = new GridLength(0);
                SplitterVisibility = Visibility.Collapsed;
            }
        }
        #endregion
    }
}