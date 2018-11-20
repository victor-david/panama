using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System.ComponentModel;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the meta-data update tool.
    /// </summary>
    public class ToolMetaUpdateViewModel : WorkspaceViewModel
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the controller object for title versions.
        /// </summary>
        public ToolTitleVersionController Versions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the controller object for submission documents.
        /// </summary>
        public ToolSubmissionDocumentController SubDocs
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public ToolMetaUpdateViewModel()
        {
            DisplayName = Strings.CommandToolMeta;
            MaxCreatable = 1;
            Commands.Add("Begin", (o) =>
                {
                    Versions.Run();
                    SubDocs.Run();
                });

            Versions = new ToolTitleVersionController(this);
            SubDocs = new ToolSubmissionDocumentController(this);
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Raises the Closing event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = TaskManager.Instance.WaitForAllRegisteredTasks(() =>
                {
                    MainViewModel.CreateNotificationMessage(Strings.NotificationCannotExitTasksAreRunning);
                    System.Media.SystemSounds.Beep.Play();

                }, null);
            base.OnClosing(e);
        }
        #endregion

        /************************************************************************/
        
        #region Private Methods
        #endregion
    }
}