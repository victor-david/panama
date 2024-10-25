using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Restless.Panama.Controls
{
    public class UpdateToolControl : Control
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateToolControl"/> class
        /// </summary>
        public UpdateToolControl()
        {
        }

        static UpdateToolControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(UpdateToolControl), new FrameworkPropertyMetadata(typeof(UpdateToolControl)));
        }
        #endregion

        /************************************************************************/

        #region Mode
        /// <summary>
        /// Gets or sets the control mode
        /// </summary>
        public UpdateToolMode Mode
        {
            get => (UpdateToolMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Mode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register
            (
                nameof(Mode), typeof(UpdateToolMode), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
                {
                    DefaultValue = UpdateToolMode.Standard,
                    PropertyChangedCallback = OnModeChanged
                }
            );

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as UpdateToolControl)?.SetMode();
        }
        #endregion

        /************************************************************************/

        #region Text
        /// <summary>
        /// Gets or sets the header text
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
                nameof(Header), typeof(string), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
            );


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
                nameof(ExplanationText), typeof(string), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Gets or sets the text used to display the folder the operation is used upon
        /// </summary>
        public string FolderText
        {
            get => (string)GetValue(FolderTextProperty);
            set => SetValue(FolderTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="FolderText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty FolderTextProperty = DependencyProperty.Register
            (
                nameof(FolderText), typeof(string), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Gets or sets the output text
        /// </summary>
        public string OutputText
        {
            get => (string)GetValue(OutputTextProperty);
            set => SetValue(OutputTextProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="OutputText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OutputTextProperty = DependencyProperty.Register
            (
                nameof(OutputText), typeof(string), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
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
                nameof(StatusText), typeof(string), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
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
                nameof(UpdatedText), typeof(string), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
                {
                    DefaultValue = "Updated"
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
                nameof(NotFoundText), typeof(string), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
                {
                    DefaultValue = "Not Found"
                }
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
                nameof(UpdatedItemsSource), typeof(IEnumerable), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
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
                nameof(NotFoundItemsSource), typeof(IEnumerable), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
            );
        #endregion

        /************************************************************************/

        #region Button
        /// <summary>
        /// Gets or sets the content for the start button
        /// </summary>
        public object ButtonContent
        {
            get => GetValue(ButtonContentProperty);
            set => SetValue(ButtonContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the button style
        /// </summary>
        public Style ButtonStyle
        {
            get => (Style)GetValue(ButtonStyleProperty);
            set => SetValue(ButtonStyleProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register
            (
                nameof(ButtonStyle), typeof(Style), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Identifies the <see cref="ButtonContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.Register
            (
                nameof(ButtonContent), typeof(object), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
                {
                    DefaultValue = "Start"
                }
            );

        /// <summary>
        /// Gets or sets the button command
        /// </summary>
        public ICommand ButtonCommand
        {
            get => (ICommand)GetValue(ButtonCommandProperty);
            set => SetValue(ButtonCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ButtonCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonCommandProperty = DependencyProperty.Register
            (
                nameof(ButtonCommand), typeof(ICommand), typeof(UpdateToolControl), new FrameworkPropertyMetadata()
            );
        #endregion

        /************************************************************************/

        #region Private methods
        private void SetMode()
        {
        }
        #endregion
    }
}