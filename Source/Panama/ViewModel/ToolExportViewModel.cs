using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System.ComponentModel;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the title export tool.
    /// </summary>
    public class ToolExportViewModel : WorkspaceViewModel
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the controller object for the export operation
        /// </summary>
        public ToolExportTitleController Export
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public ToolExportViewModel()
        {
            DisplayName = Strings.CommandToolExport;
            MaxCreatable = 1;
            Commands.Add("Begin", (o) =>
            {
                Export.Run();
            });
            Export = new ToolExportTitleController(this);
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