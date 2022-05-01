using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.Panama.Controls
{
    /// <summary>
    /// Provides a simple color picker control
    /// </summary>
    public class SimpleColorPickerWrapper : Control
    {
        #region Constructors
        public SimpleColorPickerWrapper()
        {
        }

        static SimpleColorPickerWrapper()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SimpleColorPickerWrapper), new FrameworkPropertyMetadata(typeof(SimpleColorPickerWrapper)));
        }
        #endregion

        /************************************************************************/


        /// <summary>
        /// Gets or sets the header
        /// </summary>
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register
            (
                nameof(Header), typeof(string), typeof(SimpleColorPickerWrapper), new FrameworkPropertyMetadata()
            );


        /// <summary>
        /// Gets or sets...
        /// </summary>
        public Color Color
        {
            get => (Color)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Color"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register
            (
                nameof(Color), typeof(Color), typeof(SimpleColorPickerWrapper), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Colors.Transparent,
                    BindsTwoWayByDefault = true
                }
            );
    }
}