using System;
using System.Windows;
using System.Windows.Controls;

namespace Restless.Panama.Controls
{
    public class TitledSeparator : Separator
    {
        private const double MinSeparatorHeight = 1.0;
        private const double MaxSeparatorHeight = 5.0;
        public const double DefaultSeparatorHeight = 2.0;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitledSeparator"/> class
        /// </summary>
        public TitledSeparator()
        {
        }

        static TitledSeparator()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TitledSeparator), new FrameworkPropertyMetadata(typeof(TitledSeparator)));
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets an id value.
        /// </summary>
        public int Id { get;  set; }

        /// <summary>
        /// Gets or sets a status value.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if the title is displayed
        /// </summary>
        public bool DisplayTitle
        {
            get => (bool)GetValue(DisplayTitleProperty);
            set => SetValue(DisplayTitleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="DisplayTitle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayTitleProperty = DependencyProperty.Register
            (
                nameof(DisplayTitle), typeof(bool), typeof(TitledSeparator), new FrameworkPropertyMetadata()
                {
                    DefaultValue = true
                }
            );

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
                nameof(Title), typeof(string), typeof(TitledSeparator), new FrameworkPropertyMetadata()
                {
                    DefaultValue = null
                }
            );

        /// <summary>
        /// Gets or sets the separator height
        /// </summary>
        public double SeparatorHeight
        {
            get => (double)GetValue(SeparatorHeightProperty);
            set => SetValue(SeparatorHeightProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SeparatorHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeparatorHeightProperty = DependencyProperty.Register
            (
                nameof(SeparatorHeight), typeof(double), typeof(TitledSeparator), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultSeparatorHeight,
                    CoerceValueCallback = OnCoerceSeparatorHeight
                }
            );

        private static object OnCoerceSeparatorHeight(DependencyObject d, object baseValue)
        {
            return Math.Clamp((double)baseValue, MinSeparatorHeight, MaxSeparatorHeight);
        }
        #endregion
    }
}