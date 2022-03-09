using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Restless.Panama.Controls
{
    /// <summary>
    /// Extends button o provide a tag selector
    /// </summary>
    public class TagSelectorItem : Button
    {
        #region Constructor
        public TagSelectorItem()
        {
        }

        static TagSelectorItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TagSelectorItem), new FrameworkPropertyMetadata(typeof(TagSelectorItem)));
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets the tag id
        /// </summary>
        public long Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the corner radius
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="CornerRadius"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register
            (
                nameof(CornerRadius), typeof(CornerRadius), typeof(TagSelectorItem), new FrameworkPropertyMetadata()
                {
                    DefaultValue = new CornerRadius(2)
                }
            );

        /// <summary>
        /// Gets or sets the background rollover brush
        /// </summary>
        public Brush BackgroundRolloverBrush
        {
            get => (Brush)GetValue(BackgroundRolloverBrushProperty);
            set => SetValue(BackgroundRolloverBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="BackgroundRolloverBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundRolloverBrushProperty = DependencyProperty.Register
            (
                nameof(BackgroundRolloverBrush), typeof(Brush), typeof(TagSelectorItem), new FrameworkPropertyMetadata()
                {
                    DefaultValue = null
                }
            );

        /// <summary>
        /// Gets or sets the foreground rollover brush
        /// </summary>
        public Brush ForegroundRolloverBrush
        {
            get => (Brush)GetValue(ForegroundRolloverBrushProperty);
            set => SetValue(ForegroundRolloverBrushProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ForegroundRolloverBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ForegroundRolloverBrushProperty = DependencyProperty.Register
            (
                nameof(ForegroundRolloverBrush), typeof(Brush), typeof(TagSelectorItem), new FrameworkPropertyMetadata()
                {
                    DefaultValue = null
                }
            );
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Returns a new instance of <see cref="TagSelectorItem"/>
        /// with the same id, content, and tooltip
        /// </summary>
        /// <returns>A new instance</returns>
        public TagSelectorItem Clone()
        {
            return new TagSelectorItem()
            {
                Id = Id,
                Content = Content,
                ToolTip = ToolTip,
            };
        }

        /// <summary>
        /// Sets the IsEnabled property to true and returns this instance.
        /// </summary>
        /// <returns>This instance</returns>
        public TagSelectorItem Enable()
        {
            IsEnabled = true;
            return this;
        }

        /// <summary>
        /// Sets the IsEnabled property to false and returns this instance.
        /// </summary>
        /// <returns>This instance</returns>
        public TagSelectorItem Disable()
        {
            IsEnabled = false;
            return this;
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"{nameof(TagSelectorItem)} Id: {Id}";
        }
        #endregion
    }
}