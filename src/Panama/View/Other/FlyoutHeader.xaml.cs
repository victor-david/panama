using System.Windows;
using System.Windows.Controls;

namespace Restless.Panama.View
{
    /// <summary>
    /// Interaction logic for FilterHeader.xaml
    /// </summary>
    public partial class FlyoutHeader : UserControl
    {
        public FlyoutHeader()
        {
            InitializeComponent();
        }

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
                nameof(Title), typeof(string), typeof(FlyoutHeader), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Gets or sets a value that determines of the clear filter button is displayed
        /// </summary>
        public bool ShowClearButton
        {
            get => (bool)GetValue(ShowClearButtonProperty);
            set => SetValue(ShowClearButtonProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ShowClearButton"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowClearButtonProperty = DependencyProperty.Register
            (
                nameof(ShowClearButton), typeof(bool), typeof(FlyoutHeader), new FrameworkPropertyMetadata()
                {
                    DefaultValue = true
                }
            );

    }
}