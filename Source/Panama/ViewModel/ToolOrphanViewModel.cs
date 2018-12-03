using Restless.App.Panama.Configuration;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.App.Panama.Tools;
using Restless.Tools.Controls;
using Restless.Tools.Utility;
using System.ComponentModel;

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
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolOrphanViewModel"/> class.
        /// </summary>
        /// <param name="owner">The VM that owns this view model.</param>
        public ToolOrphanViewModel(ApplicationViewModel owner) : base(owner)
        {
            DisplayName = Strings.CommandToolOrphan;
            MaxCreatable = 1;
            Controller = new ToolOrphanFinderController(this);
            MainSource.Source = Controller.NotFound;

            Commands.Add("Begin", (o) => Controller.Run());
            Columns.Create("Modified", nameof(FileScanDisplayObject.LastModified)).MakeDate();
            Columns.Create("Size", nameof(FileScanDisplayObject.Size)).MakeNumeric(null, FixedWidth.LongerNumeric);
            Columns.SetDefaultSort(Columns.Create("File", nameof(FileScanDisplayObject.FileName)), ListSortDirection.Ascending);
            AddViewSourceSortDescriptions();
            Commands.Add("CreateTitle", RunCreateTitleCommand, (p)=> SelectedItem != null);
            MenuItems.AddItem("Create a title entry from this file", Commands["CreateTitle"]);
            MenuItems.AddSeparator();
            // Commands["DeleteFile"] is created by ToolOrphanFinderController - it handles
            // file deletion and the removal of the corresponding item of its ObservableCollection
            MenuItems.AddItem("Delete this file", Commands["DeleteFile"]).AddImageResource("ImageDeleteMenu");
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called by the ancestor class when a preview of the selected item is needed.
        /// </summary>
        /// <param name="selectedItem">The currently selected grid item.</param>
        protected override void OnPreview(object selectedItem)
        {
            if (selectedItem is FileScanDisplayObject item)
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
            if (selectedItem is FileScanDisplayObject item)
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

        private void RunCreateTitleCommand(object parm)
        {
            if (SelectedItem is FileScanDisplayObject file && Messages.ShowYesNo(Strings.ConfirmationCreateTitleFromOrphan))
            {
                var title = DatabaseController.Instance.GetTable<TitleTable>();
                var ver = DatabaseController.Instance.GetTable<TitleVersionTable>();
                var row = new TitleTable.RowObject(title.AddDefaultRow())
                {
                    Title = "Title created from orphaned file",
                    Written = file.LastModified,
                    Notes = $"This entry was created from orphaned file {file.FileName}"
                };

                // Get a version controller and add a version
                ver.GetVersionController(row.Id).Add(Paths.Title.WithoutRoot(file.FileName));

                ver.Save();
                title.Save();
                Controller.NotFound.Remove(file);
            }
        }
        #endregion
    }
}