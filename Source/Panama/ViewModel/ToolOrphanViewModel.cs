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
using System.Windows.Media.Imaging;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the orphan finder tool.
    /// </summary>
    public class ToolOrphanViewModel : DataGridPreviewViewModel
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the controller object for the orphan finder operation.
        /// </summary>
        public ToolOrphanFinderController Controller
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public ToolOrphanViewModel()
        {
            DisplayName = Strings.CommandToolOrphan;
            MaxCreatable = 1;
            Controller = new ToolOrphanFinderController(this);
            MainSource.Source = Controller.NotFound;

            Commands.Add("Begin", (o) => { Controller.Run(); });
            Columns.Create("Modified", "LastModified").MakeDate();
            Columns.SetDefaultSort(Columns.Create("File", "FileName"), ListSortDirection.Ascending);
            AddViewSourceSortDescriptions();
            // RawCommands["DeleteFile"] is created by ToolOrphanFinderController - it handles
            // file deletion and the removal of the corresponding item of its ObservableCollection
            MenuItems.AddItem("Delete this file", Commands["DeleteFile"], "ImageDeleteMenu");
            
        }
#pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called by the ancestor class when a preview of the selected item is needed.
        /// </summary>
        /// <param name="selectedItem">The currently selected grid item.</param>
        protected override void OnPreview(object selectedItem)
        {
            var item = selectedItem as FileScanDisplayObject;
            if (item != null)
            {
                string fileName = Paths.Title.WithRoot(item.FileName);
                PerformPreview(fileName);
            }
        }

        /// <summary>
        /// Gets the preview mode for the specified item.
        /// </summary>
        /// <param name="selectedItem">The selected grid item</param>
        /// <returns>The preview mode</returns>
        protected override PreviewMode GetPreviewMode(object selectedItem)
        {
            var item = selectedItem as FileScanDisplayObject;
            if (item != null)
            {
                return DocumentPreviewer.GetPreviewMode(item.FileName);
            }
            return PreviewMode.Unsupported;
        }

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
        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription("FileName", ListSortDirection.Ascending));
        }
        #endregion
    }
}