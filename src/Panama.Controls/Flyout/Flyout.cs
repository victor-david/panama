using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Restless.Panama.Controls
{
    public class Flyout : ContentControl
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Flyout"/> class
        /// </summary>
        public Flyout()
        {
            MinWidth = 150;
            Width = 150;
        }

        static Flyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
            PropertyOverrides();
        }
        #endregion

        /************************************************************************/

        #region Header
        /// <summary>
        /// Gets or sets the header
        /// </summary>
        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register
            (
                nameof(Header), typeof(object), typeof(Flyout), new FrameworkPropertyMetadata()
            );
        #endregion

        /************************************************************************/

        #region Behavior
        /// <summary>
        /// Gets or sets whether the flyout is open
        /// </summary>
        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="IsOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register
            (
                nameof(IsOpen), typeof(bool), typeof(Flyout), new FrameworkPropertyMetadata()
                {
                    DefaultValue = false,
                    BindsTwoWayByDefault = true,
                    PropertyChangedCallback = OnIsOpenChanged
                }
            );

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Flyout)?.HandleIsOpenChanged();
        }

        /// <summary>
        /// Gets or sets the placement
        /// </summary>
        public FlyoutPlacement Placement
        {
            get => (FlyoutPlacement)GetValue(PlacementProperty);
            set => SetValue(PlacementProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Placement"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.Register
            (
                nameof(Placement), typeof(FlyoutPlacement), typeof(Flyout), new FrameworkPropertyMetadata()
                {
                    DefaultValue = FlyoutPlacement.Left,
                    PropertyChangedCallback = OnPositionChanged
                }
            );

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Flyout)?.HandlePlacementChanged();
        }
        #endregion

        /************************************************************************/

        #region
        /// <summary>
        /// Gets or sets the title
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register
            (
                nameof(Title), typeof(string), typeof(Flyout), new FrameworkPropertyMetadata()
                {
                    DefaultValue = null
                }
            );
        #endregion

        /************************************************************************/

        #region Internal
        /// <summary>
        /// Gets or sets the width (internal)
        /// </summary>
        internal double InternalWidth
        {
            get => (double)GetValue(InternalWidthProperty);
            set => SetValue(InternalWidthProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="InternalWidth"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty InternalWidthProperty = DependencyProperty.Register
            (
                nameof(InternalWidth), typeof(double), typeof(Flyout), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultInternalWidth
                }
            );

        /// <summary>
        /// Gets or sets the horizonal alignment (internal)
        /// </summary>
        internal HorizontalAlignment InternalHorizonalAlignment
        {
            get => (HorizontalAlignment)GetValue(InternalHorizonalAlignmentProperty);
            set => SetValue(InternalHorizonalAlignmentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="InternalHorizonalAlignment"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty InternalHorizonalAlignmentProperty = DependencyProperty.Register
            (
                nameof(InternalHorizonalAlignment), typeof(HorizontalAlignment), typeof(Flyout), new FrameworkPropertyMetadata()
                {
                    DefaultValue = HorizontalAlignment.Left
                }
            );
        #endregion

        /************************************************************************/
        #region Overrides
        private static readonly Thickness MarginZero = new(0);
        private const double MinFlyoutWidth = 150;
        private const double DefaultFlyoutWidth = 250;
        private const double DefaultInternalWidth = 0;

        private static void PropertyOverrides()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout))
            {
                DefaultValue = HorizontalAlignment.Left,
                CoerceValueCallback = (d, b) => (d as Flyout).GetHorizontalAlignment((HorizontalAlignment)b)
            });

            VerticalAlignmentProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout))
            {
                DefaultValue = VerticalAlignment.Stretch,
                CoerceValueCallback = (d, b) => (d as Flyout).GetVerticalAlignment((VerticalAlignment)b)
            });

            MinWidthProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout))
            {
                DefaultValue = 0d,
                CoerceValueCallback = (d, b) => 0d
            });

            WidthProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout))
            {
                DefaultValue = DefaultFlyoutWidth,
                CoerceValueCallback = (d, b) => Math.Max((double)b, MinFlyoutWidth)
            });

            HeightProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout))
            {
                DefaultValue = double.NaN,
                CoerceValueCallback = (d, b) => (d as Flyout).GetHeight((double)b)
            });

            MarginProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout))
            {
                DefaultValue = MarginZero,
                CoerceValueCallback = (d, b) => MarginZero
            });

        }
        #endregion

        /************************************************************************/

        #region Private methods
        private bool isPlacementChanging;

        private HorizontalAlignment GetHorizontalAlignment(HorizontalAlignment desiredValue)
        {
            return isPlacementChanging ? desiredValue : HorizontalAlignment;
        }

        private VerticalAlignment GetVerticalAlignment(VerticalAlignment desiredValue)
        {
            return isPlacementChanging ? desiredValue : VerticalAlignment;
        }

        private double GetHeight(double desiredValue)
        {
            return Placement == FlyoutPlacement.Left || Placement == FlyoutPlacement.Right ? double.NaN : desiredValue;
        }

        private void HandleIsOpenChanged()
        {
            if (IsOpen)
            {
                Animate(0, Width, 150);
            }
            else
            {
                Animate(Width, 0, 150);
            }
        }

        private void Animate(double from, double to, double milliSeconds)
        {
            DoubleAnimation da = new(from, to, TimeSpan.FromMilliseconds(milliSeconds));
            BeginAnimation(InternalWidthProperty, da);
        }

        private void HandlePlacementChanged()
        {
            isPlacementChanging = true;

            switch (Placement)
            {
                case FlyoutPlacement.Left:
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Stretch;
                    Height = double.NaN;
                    break;

                case FlyoutPlacement.TopLeft:
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Top;
                    break;

                case FlyoutPlacement.CenterLeft:
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Center;
                    break;

                case FlyoutPlacement.BottomLeft:
                    HorizontalAlignment = HorizontalAlignment.Left;
                    VerticalAlignment = VerticalAlignment.Bottom;
                    break;

                case FlyoutPlacement.Right:
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Stretch;
                    Height = double.NaN;
                    break;

                case FlyoutPlacement.TopRight:
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Top;
                    break;

                case FlyoutPlacement.CenterRight:
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Center;
                    break;

                case FlyoutPlacement.BottomRight:
                    HorizontalAlignment = HorizontalAlignment.Right;
                    VerticalAlignment = VerticalAlignment.Bottom;
                    break;
            }

            isPlacementChanging = false;
        }
        #endregion
    }
}