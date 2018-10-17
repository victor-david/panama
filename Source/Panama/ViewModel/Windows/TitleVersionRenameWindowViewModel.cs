using Restless.App.Panama.Controls;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System;
using System.Data;
using System.Windows;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the display and selection logic for the <see cref="View.TitleVersionRenameWindow"/>.
    /// </summary>
    public class TitleVersionRenameWindowViewModel : WindowDataGridViewModel<DummyTable>
    {
        #region Private
        private TitleVersionRenameItemCollection renameView;
        private string operationMessage;
        private bool canRename;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets an operation message that can be bound to the UI. 
        /// Provides feedback such as "All files are already renamed", "Success. All files successfully renamed", etc.
        /// </summary>
        public string OperationMessage
        {
            get { return operationMessage; }
            private set
            {
                SetProperty(ref operationMessage, value);
            }

        }
        #endregion
        
        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleVersionRenameWindowViewModel"/> class.
        /// </summary>
        /// <param name="owner">The window that owns this view model.</param>
        /// <param name="titleId">The title id for the versions to rename.</param>
        public TitleVersionRenameWindowViewModel(Window owner, long titleId)
            :base(owner)
        {
            renameView = new TitleVersionRenameItemCollection();
            MainSource.Source = renameView;
            Columns.Create("Ver", TitleVersionRenameItem.Properties.Version).MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Old name", TitleVersionRenameItem.Properties.OriginalNameDisplay);
            Columns.Create("New name", TitleVersionRenameItem.Properties.NewNameDisplay);
            Columns.Create("Status", TitleVersionRenameItem.Properties.Status);
            Commands.Add("Rename", RunRenameCommand, CanRunRenameCommand);
            Commands.Add("Close", RunCloseCommand);
            PopulateRenameItems(titleId);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion

        /************************************************************************/
        
        #region Private methods
        private void PopulateRenameItems(long titleId)
        {
            DataRow[] titles = DatabaseController.Instance.GetTable<TitleTable>().Select(string.Format("{0}={1}", TitleTable.Defs.Columns.Id, titleId));
            if (titles.Length != 1)
            {
                throw new InvalidOperationException(Strings.InvalidOpTitleDoesNotExist);
            }

            string title = titles[0][TitleTable.Defs.Columns.Title].ToString();

            DataRow[] versions = DatabaseController.Instance.GetTable<TitleVersionTable>().GetAllVersions(titleId);
            foreach (DataRow row in versions)
            {
                renameView.Add(new TitleVersionRenameItem(row, title));
            }

            if (renameView.Count == 0)
            {
                OperationMessage = Strings.InvalidOpRenameCandidateListEmpty;
                return;
            }

            canRename = false;

            if (!renameView.AllOriginalExist)
            {
                OperationMessage = Strings.InvalidOpRenameFilesMissing;
            }
            else if (renameView.AllSame)
            {
                OperationMessage = Strings.InvalidOpRenameAllCandidatesAlreadyRenamed;
            }
            else
            {
                canRename = true;
            }
        }

        private void RunRenameCommand(object o)
        {
            Execution.TryCatch(() =>
                {
                    renameView.Rename();
                    DatabaseController.Instance.GetTable<TitleVersionTable>().Save();
                    OperationMessage = Strings.ConfirmationAllVersionFilesRenamed;
                    canRename = false;

                });
        }

        private bool CanRunRenameCommand(object o)
        {
            return canRename;
        }

        private void RunCloseCommand(object o)
        {
            Owner.Close();
        }
        #endregion
    }
}
