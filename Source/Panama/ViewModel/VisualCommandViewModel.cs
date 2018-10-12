using System;
using System.Windows.Input;
using System.Windows.Controls;
using Restless.Tools.Utility;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represents an actionable item that has a visual component to be bound and displayed by a view.
    /// </summary>
    public class VisualCommandViewModel : ViewModelBase
    {
        private object icon;

        #region Public Fields
        /// <summary>
        /// The default image size. Represents both height and width.
        /// </summary>
        public const double DefaultImageSize = 32.0;
        /// <summary>
        /// The default font size for the text that accompanies the image.
        /// </summary>
        public const double DefaultFontSize = 11.5;
        /// <summary>
        /// The default minimum width.
        /// </summary>
        public const double DefaultMinWidth = 132.0;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets the image size associated with this instance.
        /// </summary>
        public double ImageSize
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the minimum width for the visual command
        /// </summary>
        public double MinWidth
        {
            get;
        }

        /// <summary>
        /// Gets the calculated button size
        /// </summary>
        public double ButtonSize
        {
            get => ImageSize + 8.0;
        }

        /// <summary>
        /// Gets the font size
        /// </summary>
        public double FontSize
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the icon object associated with the command.
        /// </summary>
        public object Icon
        {
            get => icon;
            set => SetProperty(ref icon, value);
        }

        /// <summary>
        /// Gets the tool tip text associated with the command.
        /// </summary>
        public string TooltipText
        {
            get;
            private set;
        }
        /// <summary>
        /// Gets the command associated with this instance.
        /// </summary>
        public ICommand Command 
        { 
            get; 
            private set; 
        }

        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualCommandViewModel"/> class.
        /// </summary>
        /// <param name="displayName">The display name associated with this command.</param>
        /// <param name="toolTipText">The tooltip text. Not all visuals may use this.</param>
        /// <param name="command">The command to excute.</param>
        /// <param name="icon">The icon associated with the visual command.</param>
        /// <param name="imageSize">The icon's image size. This is both height and width. The default is 32.0</param>
        /// <param name="fontSize">The font size for <paramref name="displayName"/>. The default is 11.5</param>
        /// <param name="minWidth">The button mimimum width. Default is <see cref="DefaultMinWidth"/></param>
        public VisualCommandViewModel
            (
                string displayName, string toolTipText, ICommand command, object icon,
                double imageSize = DefaultImageSize, double fontSize = DefaultFontSize, double minWidth = DefaultMinWidth
            )
        {
            Validations.ValidateNull(command, "CommandViewModel.Command");
            DisplayName = displayName;
            TooltipText = toolTipText;
            Command = command;
            ImageSize = imageSize;
            FontSize = fontSize;
            MinWidth = minWidth;
            Icon = icon; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualCommandViewModel"/> class.
        /// </summary>
        /// <param name="displayName">The display name associated with this command.</param>
        /// <param name="toolTipText">The tooltip text. Not all visuals may use this.</param>
        /// <param name="command">The command to excute.</param>
        /// <param name="minWidth">The button mimimum width. Default is <see cref="DefaultMinWidth"/></param>        /// 
        public VisualCommandViewModel(string displayName, string toolTipText, ICommand command, double minWidth)
            : this(displayName, toolTipText, command, null, DefaultImageSize, DefaultFontSize, minWidth)
        {
        }
        #endregion
    }
}