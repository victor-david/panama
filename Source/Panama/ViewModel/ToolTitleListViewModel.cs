using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.App.Panama.Controls;

using Restless.Tools.Utility;
using System.IO;
using Restless.Tools.Threading;
using System.Text;
using Restless.App.Panama.Tools;

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
                text = value;
                OnPropertyChanged("Text");
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
            RawCommands.Add("Begin", (o) => { Creator.Run(); });
            RawCommands.Add("OpenFile", (o) => { OpenHelper.OpenFile(Creator.TitleListFileName); });
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