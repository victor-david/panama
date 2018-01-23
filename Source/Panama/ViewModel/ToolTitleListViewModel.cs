using Restless.App.Panama.Resources;
using Restless.Tools.Threading;
using Restless.Tools.Utility;
using System;
using System.ComponentModel;
using System.IO;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the orphan finder tool.
    /// </summary>
    public class ToolTitleListViewModel : DataGridViewModelBase
    {
        #region Private
        private string text;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the text of the title list document.
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                SetProperty(ref text, value);
            }
        }
        /// <summary>
        /// Gets the controller object for the title list operation.
        /// </summary>
        public ToolTitleListController Creator
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public ToolTitleListViewModel()
        {
            DisplayName = Strings.CommandToolTitleList;
            MaxCreatable = 1;
            Creator = new ToolTitleListController(this);
            Creator.Scanner.Completed += ScannerCompleted;
            Commands.Add("Begin", (o) => { Creator.Run(); });
            Commands.Add("OpenFile", (o) => { OpenHelper.OpenFile(Creator.TitleListFileName); });
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
            if (!e.Cancel)
            {
                Creator.Scanner.Completed -= ScannerCompleted;
            }
        }
        #endregion

        /************************************************************************/
        
        #region Private Methods
        private void ScannerCompleted(object sender, EventArgs e)
        {
            Text = File.ReadAllText(Creator.TitleListFileName);
        }
        #endregion
    }
}