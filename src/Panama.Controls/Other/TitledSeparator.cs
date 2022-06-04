using System.Windows;
using System.Windows.Controls;

namespace Restless.Panama.Controls
{
    public class TitledSeparator : Control
    {
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
                }
            );
        #endregion
    }
}