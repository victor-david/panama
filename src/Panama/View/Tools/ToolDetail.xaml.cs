using Restless.Panama.Core;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Restless.Panama.View
{
    /// <summary>
    /// Interaction logic for ToolDetail.xaml
    /// </summary>
    public partial class ToolDetail : Grid
    {
        #region Fields
        public const string DefaultUpdatedText = "Updated";
        public const string DefaultNotFoundText = "Not Found";
        #endregion

        /************************************************************************/

        #region Constructor
        public ToolDetail()
        {
            InitializeComponent();
        }
        #endregion

        /************************************************************************/

        #region Mode
        /// <summary>
        /// Gets or sets the control mode
        /// </summary>
        public ToolDetailMode Mode
        {
            get => (ToolDetailMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Mode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register
            (
                nameof(Mode), typeof(ToolDetailMode), typeof(ToolDetail), new FrameworkPropertyMetadata()
                {
                    DefaultValue = ToolDetailMode.Standard,
                    PropertyChangedCallback = OnModeChanged
                }
            );

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ToolDetail)?.SetMode();
        }
        #endregion

        /************************************************************************/

        #region Preview
        /// <summary>
        /// Gets or sets the preview mode
        /// </summary>
        public PreviewMode PreviewMode
        {
            get => (PreviewMode)GetValue(PreviewModeProperty);
            set => SetValue(PreviewModeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PreviewMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviewModeProperty = DependencyProperty.Register
            (
                nameof(PreviewMode), typeof(PreviewMode), typeof(ToolDetail), new FrameworkPropertyMetadata()
                {
                    DefaultValue = PreviewMode.None,
                }
            );

        /// <summary>
        /// Gets or sets the preview text
        /// </summary>
        public string PreviewText
        {
            get => (string)GetValue(PreviewTextProperty);
            set => SetValue(PreviewTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PreviewText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviewTextProperty = DependencyProperty.Register
            (
                nameof(PreviewText), typeof(string), typeof(ToolDetail), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Gets or sets the preview image source
        /// </summary>
        public ImageSource PreviewImageSource
        {
            get => (ImageSource)GetValue(PreviewImageSourceProperty);
            set => SetValue(PreviewImageSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="PreviewImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviewImageSourceProperty = DependencyProperty.Register
            (
                nameof(PreviewImageSource), typeof(ImageSource), typeof(ToolDetail), new FrameworkPropertyMetadata()
            );
        #endregion

        /************************************************************************/

        #region Other Text
        /// <summary>
        /// Gets or sets text that describes the tool
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
        #endregion

        /************************************************************************/

        #region ItemsSource
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
        /// Gets or sets the selected updated item
        /// </summary>
        public object SelectedUpdatedItem
        {
            get => GetValue(SelectedUpdatedItemProperty);
            set => SetValue(SelectedUpdatedItemProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectedUpdatedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedUpdatedItemProperty = DependencyProperty.Register
            (
                nameof(SelectedUpdatedItem), typeof(object), typeof(ToolDetail), new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                }
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
        /// Gets or sets the selected not found item
        /// </summary>
        public object SelectedNotFoundItem
        {
            get => GetValue(SelectedNotFoundItemProperty);
            set => SetValue(SelectedNotFoundItemProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectedNotFoundItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedNotFoundItemProperty = DependencyProperty.Register
            (
                nameof(SelectedNotFoundItem), typeof(object), typeof(ToolDetail), new FrameworkPropertyMetadata()
                {
                    BindsTwoWayByDefault = true
                }
            );
        #endregion

        /************************************************************************/

        #region Command
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
        #endregion

        /************************************************************************/

        #region Visibility (read only)
        /// <summary>
        /// Gets the visibility for the updated list box
        /// </summary>
        public Visibility UpdatedVisibility
        {
            get => (Visibility)GetValue(UpdatedVisibilityProperty);
            private set => SetValue(UpdatedVisibilityPropertyKey, value);
        }

        private static readonly DependencyPropertyKey UpdatedVisibilityPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(UpdatedVisibility), typeof(Visibility), typeof(ToolDetail), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Visibility.Visible
                }
            );

        /// <summary>
        /// Identifies the <see cref="UpdatedVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty UpdatedVisibilityProperty = UpdatedVisibilityPropertyKey.DependencyProperty;


        /// <summary>
        /// Gets the...
        /// </summary>
        public Visibility NotFoundVisibility
        {
            get => (Visibility)GetValue(NotFoundVisibilityProperty);
            private set => SetValue(NotFoundVisibilityPropertyKey, value);
        }

        private static readonly DependencyPropertyKey NotFoundVisibilityPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(NotFoundVisibility), typeof(Visibility), typeof(ToolDetail), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Visibility.Visible
                }
            );

        /// <summary>
        /// Identifies the <see cref="NotFoundVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NotFoundVisibilityProperty = NotFoundVisibilityPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the visibility of the previewer
        /// </summary>
        public Visibility PreviewVisibility
        {
            get => (Visibility)GetValue(PreviewVisibilityProperty);
            private set => SetValue(PreviewVisibilityPropertyKey, value);
        }

        private static readonly DependencyPropertyKey PreviewVisibilityPropertyKey = DependencyProperty.RegisterReadOnly
            (
                nameof(PreviewVisibility), typeof(Visibility), typeof(ToolDetail), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Visibility.Collapsed
                }
            );

        /// <summary>
        /// Identifies the <see cref="PreviewVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviewVisibilityProperty = PreviewVisibilityPropertyKey.DependencyProperty;
        #endregion

        /************************************************************************/

        #region Private methods
        private void SetMode()
        {
            UpdatedVisibility = (Mode is ToolDetailMode.Standard or ToolDetailMode.Preview) ? Visibility.Visible : Visibility.Collapsed;
            NotFoundVisibility = (Mode is ToolDetailMode.Standard) ? Visibility.Visible : Visibility.Collapsed;
            PreviewVisibility = (Mode is ToolDetailMode.Preview) ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion
    }
}