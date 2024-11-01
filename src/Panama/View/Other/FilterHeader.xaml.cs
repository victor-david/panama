using System.Windows;
using System.Windows.Controls;

namespace Restless.Panama.View
{
    /// <summary>
    /// Interaction logic for FilterHeader.xaml
    /// </summary>
    public partial class FilterHeader : UserControl
    {
        public FilterHeader()
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
                nameof(Title), typeof(string), typeof(FilterHeader), new FrameworkPropertyMetadata()
            );
    }
}