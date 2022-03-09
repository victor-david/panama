using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Restless.Panama.Controls
{
    public class TagSelectorControl : ItemsControl
    {
        #region Constructor
        public TagSelectorControl()
        {
            AddHandler(Button.ClickEvent, new RoutedEventHandler(ClickedEventHandler));
        }

        static TagSelectorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TagSelectorControl), new FrameworkPropertyMetadata(typeof(TagSelectorControl)));
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets the id of this control.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the orientation in which the tags are displayed
        /// </summary>
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register
            (
                nameof(Orientation), typeof(Orientation), typeof(TagSelectorControl), new FrameworkPropertyMetadata()
                {
                    DefaultValue = Orientation.Horizontal
                }
            );

        /// <summary>
        /// Gets or sets a command to exceute when a tag is clicked.
        /// A <see cref="TagSelectorParm"/> object is sent on the command
        /// </summary>
        public ICommand TagItemClickedCommand
        {
            get => (ICommand)GetValue(TagItemClickedCommandProperty);
            set => SetValue(TagItemClickedCommandProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="TagItemClickedCommand"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TagItemClickedCommandProperty = DependencyProperty.Register
            (
                nameof(TagItemClickedCommand), typeof(ICommand), typeof(TagSelectorControl), new FrameworkPropertyMetadata()
                {
                    DefaultValue = null
                }
            );
        #endregion

        /************************************************************************/

        #region Private methods
        private void ClickedEventHandler(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is TagSelectorItem item)
            {
                TagItemClickedCommand?.Execute(new TagSelectorParm(Id, item));
                e.Handled = true;
            }
        }
        #endregion
    }
}