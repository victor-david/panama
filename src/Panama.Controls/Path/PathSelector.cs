using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Restless.Panama.Controls
{
    public class PathSelector : Control
    {
        #region Constructors
        public PathSelector()
        {
            AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ClickedEventHandler));
        }

        static PathSelector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PathSelector), new FrameworkPropertyMetadata(typeof(PathSelector)));
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets the selector type
        /// </summary>
        public PathSelectorType SelectorType
        {
            get => (PathSelectorType)GetValue(SelectorTypeProperty);
            set => SetValue(SelectorTypeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectorType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorTypeProperty = DependencyProperty.Register
            (
                nameof(SelectorType), typeof(PathSelectorType), typeof(PathSelector), new FrameworkPropertyMetadata()
                {
                    DefaultValue = PathSelectorType.Folder
                }
            );

        /// <summary>
        /// Gets or sets the selector file type
        /// </summary>
        public long SelectorFileType
        {
            get => (long)GetValue(SelectorFileTypeProperty);
            set => SetValue(SelectorFileTypeProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectorFileType"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectorFileTypeProperty = DependencyProperty.Register
            (
                nameof(SelectorFileType), typeof(long), typeof(PathSelector), new FrameworkPropertyMetadata()
                {
                    DefaultValue = 0L
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
                nameof(Title), typeof(string), typeof(PathSelector), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Gets or sets the path
        /// </summary>
        public string Path
        {
            get => (string)GetValue(PathProperty);
            set => SetValue(PathProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Path"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register
            (
                nameof(Path), typeof(string), typeof(PathSelector), new FrameworkPropertyMetadata()
                {
                    DefaultValue = null,
                    BindsTwoWayByDefault = true
                }
            );

        /// <summary>
        /// Gets or sets the button content
        /// </summary>
        public object ButtonContent
        {
            get => GetValue(ButtonContentProperty);
            set => SetValue(ButtonContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ButtonContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.Register
            (
                nameof(ButtonContent), typeof(object), typeof(PathSelector), new FrameworkPropertyMetadata()
            );

        /// <summary>
        /// Gets or sets the command to execute when the select button is clicked
        /// </summary>
        public ICommand SelectCommand
        {
            get => (ICommand)GetValue(SelectCommandProperty);
            set => SetValue(SelectCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="SelectCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectCommandProperty = DependencyProperty.Register
            (
                nameof(SelectCommand), typeof(ICommand), typeof(PathSelector), new FrameworkPropertyMetadata()
            );
        #endregion

        /************************************************************************/

        #region ComponentResourceKey
        public static readonly ComponentResourceKey TextBlockStyleKey = new ComponentResourceKey(typeof(PathSelector), nameof(TextBlockStyleKey));
        public static readonly ComponentResourceKey TextBoxStyleKey = new ComponentResourceKey(typeof(PathSelector), nameof(TextBoxStyleKey));
        public static readonly ComponentResourceKey ButtonStyleKey = new ComponentResourceKey(typeof(PathSelector), nameof(ButtonStyleKey));
        #endregion

        /************************************************************************/

        #region Private methods
        private const string ButtonName = "D584BC74_5FC0_4705_9B0C_EF1D1C3E64DA";

        private void ClickedEventHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is Button item && item.Name == ButtonName)
            {
                SelectCommand?.Execute(this);
                e.Handled = true;
            }
        }
        #endregion
    }
}