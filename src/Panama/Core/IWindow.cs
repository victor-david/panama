using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Defines an interface that describes window ownership.
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// Gets or sets the associated window object.
        /// </summary>
        Window Window { get; set; }

        /// <summary>
        /// Gets or sets a command to close the window.
        /// </summary>
        ICommand CloseWindowCommand { get; set; }

        /// <summary>
        /// Called when the window is closing
        /// </summary>
        /// <param name="e">The event args</param>
        void OnWindowClosing(CancelEventArgs e);

        /// <summary>
        /// Called when the window has closed
        /// </summary>
        void OnWindowClosed();
    }
}