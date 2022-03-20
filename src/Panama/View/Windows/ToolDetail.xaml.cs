using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Restless.Panama.View
{
    /// <summary>
    /// Interaction logic for ToolDetail.xaml
    /// </summary>
    public partial class ToolDetail : Grid
    {
        public const string DefaultUpdatedText = "Updated";
        public const string DefaultNotFoundText = "Not Found";

        public ToolDetail()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets...
        /// </summary>
        public string ExplanationText
        {
            get => (string)GetValue(ExplanationTextProperty);
            set => SetValue(ExplanationTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ExplanationText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExplanationTextProperty = DependencyProperty.Register
            (
                nameof(ExplanationText), typeof(string), typeof(ToolDetail), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Gets or sets the tool start command
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register
            (
                nameof(Command), typeof(ICommand), typeof(ToolDetail), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Gets or sets the items source for the updated list box
        /// </summary>
        public IEnumerable UpdatedItemsSource
        {
            get => (IEnumerable)GetValue(UpdatedItemsSourceProperty);
            set => SetValue(UpdatedItemsSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="UpdatedItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpdatedItemsSourceProperty = DependencyProperty.Register
            (
                nameof(UpdatedItemsSource), typeof(IEnumerable), typeof(ToolDetail), new FrameworkPropertyMetadata()
            );


        /// <summary>
        /// Gets or sets the items source for the not found list box
        /// </summary>
        public IEnumerable NotFoundItemsSource
        {
            get => (IEnumerable)GetValue(NotFoundItemsSourceProperty);
            set => SetValue(NotFoundItemsSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="NotFoundItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotFoundItemsSourceProperty = DependencyProperty.Register
            (
                nameof(NotFoundItemsSource), typeof(IEnumerable), typeof(ToolDetail), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Gets or sets the visibility of the updated list box and its associated text block
        /// </summary>
        public Visibility UpdatedVisibility
        {
            get => (Visibility)GetValue(UpdatedVisibilityProperty);
            set => SetValue(UpdatedVisibilityProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="UpdatedVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpdatedVisibilityProperty = DependencyProperty.Register
            (
                nameof(UpdatedVisibility), typeof(Visibility), typeof(ToolDetail), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Visibility.Visible
                }
            );

        /// <summary>
        /// Gets or sets the visibility of the not found list box and its associated text block
        /// </summary>
        public Visibility NotFoundVisibility
        {
            get => (Visibility)GetValue(NotFoundVisibilityProperty);
            set => SetValue(NotFoundVisibilityProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="NotFoundVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotFoundVisibilityProperty = DependencyProperty.Register
            (
                nameof(NotFoundVisibility), typeof(Visibility), typeof(ToolDetail), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Visibility.Visible
                }
            );

        /// <summary>
        /// Gets or sets the text for the updated list box
        /// </summary>
        public string UpdatedText
        {
            get => (string)GetValue(UpdatedTextProperty);
            set => SetValue(UpdatedTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="UpdatedText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpdatedTextProperty = DependencyProperty.Register
            (
                nameof(UpdatedText), typeof(string), typeof(ToolDetail), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultUpdatedText
                }
            );

        /// <summary>
        /// Gets or sets the text for the not found list box
        /// </summary>
        public string NotFoundText
        {
            get => (string)GetValue(NotFoundTextProperty);
            set => SetValue(NotFoundTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="NotFoundText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotFoundTextProperty = DependencyProperty.Register
            (
                nameof(NotFoundText), typeof(string), typeof(ToolDetail), new FrameworkPropertyMetadata()
                {
                    DefaultValue = DefaultNotFoundText
                }
            );

        /// <summary>
        /// Gets or sets a folder name to display
        /// </summary>
        public string FolderDisplay
        {
            get => (string)GetValue(FolderDisplayProperty);
            set => SetValue(FolderDisplayProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FolderDisplay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FolderDisplayProperty = DependencyProperty.Register
            (
                nameof(FolderDisplay), typeof(string), typeof(ToolDetail), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Gets or sets the status text
        /// </summary>
        public string StatusText
        {
            get => (string)GetValue(StatusTextProperty);
            set => SetValue(StatusTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="StatusText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StatusTextProperty = DependencyProperty.Register
            (
                nameof(StatusText), typeof(string), typeof(ToolDetail), new FrameworkPropertyMetadata()
            );

    }
}