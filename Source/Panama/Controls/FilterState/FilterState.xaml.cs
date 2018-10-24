using System.Windows;
using System.Windows.Controls;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Interaction logic for FilterState.xaml
    /// </summary>
    public partial class FilterState : UserControl
    {

        /// <summary>
        /// Gets or sets the control id. Each control on the same page nust have a unique id.
        /// </summary>
        public string Id
        {
            get => (string)GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        /// <summary>
        /// Dependency property for <see cref="Id"/>
        /// </summary>
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register
            (
                nameof(Id), typeof(string), typeof(FilterState), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the title for the control
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Dependency property for <see cref="Title"/>.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register
            (
                nameof(Title), typeof(string), typeof(FilterState), new PropertyMetadata(null)
            );

        /// <summary>
        /// Gets or sets the filter state
        /// </summary>
        public Filter.FilterState State
        {
            get => (Filter.FilterState)GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        /// <summary>
        /// Dependency property for <see cref="State"/>.
        /// </summary>
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register
            (
                nameof(State), typeof(Filter.FilterState), typeof(FilterState), 
                new FrameworkPropertyMetadata(Filter.FilterState.Either, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault)
            );

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterState"/> class.
        /// </summary>
        public FilterState()
        {
            InitializeComponent();
        }

        /************************************************************************/

        #region Private methods
        #endregion
    }
}
