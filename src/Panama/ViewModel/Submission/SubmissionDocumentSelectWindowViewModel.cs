using Restless.Panama.Core;
using Restless.Panama.View;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the selection logic for <see cref="SubmissionDocumentSelectWindow"/>.
    /// Used when the user wants to create a submission document.
    /// </summary>
    public class SubmissionDocumentSelectWindowViewModel : ApplicationViewModel, IWindow
    {
        #region Public properties
        /// <summary>
        /// Gets the submission document creation type as selected by the user.
        /// </summary>
        public SubmissionDocumentCreationType CreateType
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionDocumentSelectWindowViewModel"/> class.
        /// </summary>
        public SubmissionDocumentSelectWindowViewModel()
        {
            CreateType = SubmissionDocumentCreationType.None;
            Commands.Add("CreateDocX", p => CreateSelection(SubmissionDocumentCreationType.CreateDocX));
            Commands.Add("CreatePlaceholder", p => CreateSelection(SubmissionDocumentCreationType.CreatePlaceholder));
        }
        #endregion

        /************************************************************************/

        #region IWindow
        /// <summary>
        /// Gets or sets the window owner. Set in <see cref="WindowFactory"/>
        /// </summary>
        public Window Window { get; set; }

        /// <summary>
        /// Gets or sets a command to close the window. Set in <see cref="WindowFactory"/>
        /// </summary>
        public ICommand CloseWindowCommand { get; set; }

        /// <summary>
        /// Called when the window is closing. Established in <see cref="WindowFactory"/>
        /// </summary>
        /// <param name="e">The event args</param>
        public void OnWindowClosing(CancelEventArgs e)
        {
        }

        /// <summary>
        /// Called when the window has closed. Established in <see cref="WindowFactory"/>
        /// </summary>
        public void OnWindowClosed()
        {
        }
        #endregion
        private void CreateSelection(SubmissionDocumentCreationType creationType)
        {
            CreateType = creationType;
            Window.DialogResult = true;
            CloseWindowCommand.Execute(null);
        }
    }
}