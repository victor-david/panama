using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Provides the interaction logic for a KeyValueDisplay control.
    /// </summary>
    public partial class KeyValueDisplay : UserControl
    {
        #region Public properties
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
                nameof(Header), typeof(string), typeof(KeyValueDisplay), new UIPropertyMetadata(null)
            );


        /// <summary>
        /// Gets or sets the width of <see cref="Header"/>.
        /// </summary>
        public int HeaderWidth
        {
            get => (int)GetValue(HeaderWidthProperty);
            set => SetValue(HeaderWidthProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that controls the width of <see cref="Header"/>.
        /// </summary>
        public static readonly DependencyProperty HeaderWidthProperty = DependencyProperty.Register
            (
                nameof(HeaderWidth), typeof(int), typeof(KeyValueDisplay), new UIPropertyMetadata(120)
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
                nameof(HeaderForeground), typeof(Brush), typeof(KeyValueDisplay), new UIPropertyMetadata(new SolidColorBrush(Colors.Black))
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
                nameof(HeaderFontSize), typeof(double), typeof(KeyValueDisplay), new UIPropertyMetadata(11.0)
            );

        /// <summary>
        /// Gets or sets the vertical alignment for the header
        /// </summary>
        public VerticalAlignment HeaderVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(HeaderVerticalAlignmentProperty);
            set => SetValue(HeaderVerticalAlignmentProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that describes the vertical alignment of the header.
        /// </summary>
        public static readonly DependencyProperty HeaderVerticalAlignmentProperty = DependencyProperty.Register
            (
                nameof(HeaderVerticalAlignment), typeof(VerticalAlignment), typeof(KeyValueDisplay), new PropertyMetadata(VerticalAlignment.Center)
            );
        
        /// <summary>
        /// Gets or sets the display value.
        /// </summary>
        public string Value
        {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that displays the control value.
        /// </summary>
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
            (
                nameof(Value), typeof(string), typeof(KeyValueDisplay), new UIPropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the foregound of <see cref="Value"/>.
        /// </summary>
        public Brush ValueForeground
        {
            get => (Brush)GetValue(ValueForegroundProperty);
            set => SetValue(ValueForegroundProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that describes the foreground of <see cref="Value"/>.
        /// </summary>
        public static readonly DependencyProperty ValueForegroundProperty = DependencyProperty.Register
            (
                nameof(ValueForeground), typeof(Brush), typeof(KeyValueDisplay), new UIPropertyMetadata(new SolidColorBrush(Colors.Black))
            );

        /// <summary>
        /// Gets or sets the font size of <see cref="Value"/>.
        /// </summary>
        public double ValueFontSize
        {
            get => (double)GetValue(ValueFontSizeProperty);
            set => SetValue(ValueFontSizeProperty, value);
        }

        /// <summary>
        /// Defines a dependency property for the value font size.
        /// </summary>
        public static readonly DependencyProperty ValueFontSizeProperty = DependencyProperty.Register
            (
                nameof(ValueFontSize), typeof(double), typeof(KeyValueDisplay), new PropertyMetadata(11.0)
            );

        /// <summary>
        /// Gets or sets the vertical alignment for the value
        /// </summary>
        public VerticalAlignment ValueVerticalAlignment
        {
            get => (VerticalAlignment)GetValue(ValueVerticalAlignmentProperty);
            set => SetValue(ValueVerticalAlignmentProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that describes the vertical alignment of the value.
        /// </summary>
        public static readonly DependencyProperty ValueVerticalAlignmentProperty = DependencyProperty.Register
            (
                nameof(ValueVerticalAlignment), typeof(VerticalAlignment), typeof(KeyValueDisplay), new PropertyMetadata(VerticalAlignment.Center)
            );

        /// <summary>
        /// Gets or sets the display level of this control. This property affects
        /// the margin and header width to provide an indent
        /// </summary>
        public int DisplayLevel
        {
            get => (int)GetValue(DisplayLevelProperty);
            set => SetValue(DisplayLevelProperty, value);
        }

        /// <summary>
        /// Defines a dependency property that describes the display level.
        /// </summary>
        public static readonly DependencyProperty DisplayLevelProperty = DependencyProperty.Register
            (
                nameof(DisplayLevel), typeof(int), typeof(KeyValueDisplay), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.AffectsRender, DisplayLevelChanged)
            );
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueDisplay"/> class.
        /// </summary>
        public KeyValueDisplay()
        {
            InitializeComponent();
            VerticalContentAlignment = VerticalAlignment.Center;
        }
        #endregion

        /************************************************************************/


        #region Private methods

        private static void DisplayLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            int factor = 12;
            if (d is KeyValueDisplay obj)
            {
                int displayLevel = (int)e.NewValue;
                Thickness margin = new Thickness(obj.Margin.Left + (displayLevel * factor), obj.Margin.Top, obj.Margin.Right, obj.Margin.Bottom);
                obj.Margin = margin;
                if (obj.HeaderWidth >= (displayLevel * factor))
                {
                    obj.HeaderWidth -= displayLevel * factor;
                }
            }
        }
        #endregion
    }
}
