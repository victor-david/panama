using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Restless.Tools.Utility;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the selection logic for <see cref="View.SubmissionDocumentSelectWindow"/>. 
    /// Used when the user wants to create a submission document.
    /// </summary>
    public class SubmissionDocumentSelectWindowViewModel : WindowViewModel
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the submission document creation type as selected by the user.
        /// </summary>
        public SubmissionDocumentCreateType CreateType
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
        /// <param name="owner">The window that owns this view model.</param>
        public SubmissionDocumentSelectWindowViewModel(Window owner)
            :base(owner)
        {
            CreateType = SubmissionDocumentCreateType.None;
            Commands.Add("CreateDocx", (o) =>
                {
                    CreateType = SubmissionDocumentCreateType.CreateDocX;
                    CloseCommand.Execute(null);
                });

            Commands.Add("CreatePlaceholder", (o) =>
            {
                CreateType = SubmissionDocumentCreateType.CreatePlaceholder;
                CloseCommand.Execute(null);
            });
        }
        #endregion

        /// <summary>
        /// Closes the owner window when the close command is executed
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                Owner.Close();
            }
            e.Cancel = true;
            base.OnClosing(e);
        }
    }
}
