using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System.ComponentModel;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the title export tool.
    /// </summary>
    public class ToolExportViewModel : ApplicationViewModel
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
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolExportViewModel"/> class.
        /// </summary>
        /// <param name="owner">The VM that owns this view model.</param>
        public ToolExportViewModel(ApplicationViewModel owner) : base(owner)
        {
            DisplayName = Strings.CommandToolExport;
            MaxCreatable = 1;
            Commands.Add("Begin", (o) =>
            {
                Export.Run();
            });
            Export = new ToolExportTitleController(this);
        }
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