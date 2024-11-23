using System.Windows;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Defines an interface that describes window ownership.
    /// </summary>
    public interface IWindowOwner
    {
        /// <summary>
        /// Gets or sets the window owner.
        /// </summary>
        Window WindowOwner { get; set; }

        /// <summary>
        /// Gets or sets a command to close the window.
        /// </summary>
        ICommand CloseWindowCommand { get; set; }
    }
}