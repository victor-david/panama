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
        /// Gets the calculated button size
        /// </summary>
        public double ButtonSize
        {
            get { return ImageSize + 8.0; }
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
        /// Gets the icon object associated with the command.
        /// </summary>
        public object Icon
        {
            get;
            private set;
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
        public VisualCommandViewModel(string displayName, string toolTipText, ICommand command, object icon, double imageSize = 32.0, double fontSize = 11.5)
        {
            Validations.ValidateNull(command, "CommandViewModel.Command");
            DisplayName = displayName;
            TooltipText = toolTipText;
            Command = command;
            ImageSize = imageSize;
            FontSize = fontSize;
            Icon = icon; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualCommandViewModel"/> class.
        /// </summary>
        /// <param name="displayName">The display name associated with this command.</param>
        /// <param name="toolTipText">The tooltip text. Not all visuals may use this.</param>
        /// <param name="command">The command to excute.</param>
        public VisualCommandViewModel(string displayName, string toolTipText, ICommand command)
            : this(displayName, toolTipText, command, null)
        {
        }
        #endregion


    }
}