using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.App.Panama.Tools;
using Restless.Tools.Threading;
using Restless.Tools.Utility;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the controller that handles finding orphan files.
    /// </summary>
    public class ToolOrphanFinderController : ToolControllerBase<ToolOrphanViewModel>
    {
        #region Private
        private OrphanFinder scanner;
        #endregion
        
        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the task id for this controller.
        /// </summary>
        public override int TaskId
        {
            get { return AppTaskId.OrphanFind; }
        }

        /// <summary>
        /// Gets the scanner object for this controller.
        /// </summary>
        public override Tools.FileScanBase Scanner
        {
            get { return scanner; }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolOrphanFinderController"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="ToolOrphanViewModel"/> that owns this controller.</param>
        public ToolOrphanFinderController(ToolOrphanViewModel owner)
            :base(owner)
        {
            scanner = new OrphanFinder();
            scanner.Updated += (s, e) =>
            {
                TaskManager.Instance.DispatchTask(() => { AddToUpdated(e.Target); });
            };
            scanner.NotFound += (s, e) =>
            {
                TaskManager.Instance.DispatchTask(() => 
                { 
                    AddToNotFound(e.Target);
                    UpdateNotFoundHeader();
                });
            };

            Owner.Commands.Add("OpenFile", RunOpenFileCommand);
            Owner.Commands.Add("DeleteFile", RunDeleteFileCommand, (o) => { return Owner.SelectedItem != null; });
            UpdateNotFoundHeader();
        }
        #endregion


        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Runs the operation.
        /// </summary>
        public override void Run()
        {
            if (String.IsNullOrEmpty(Config.Instance.FolderTitleRoot) || !Directory.Exists(Config.Instance.FolderTitleRoot))
            {
                Messages.ShowError(Strings.InvalidOpTitleRootFolderNotSet);
                return;
            }
            ClearCollections();
            scanner.Execute(TaskId);
        }
        #endregion

        /************************************************************************/
        
        #region Private methods
        private void UpdateNotFoundHeader()
        {
            NotFoundHeader = String.Format(Strings.HeaderToolOperationOrphanNotFoundFormat, NotFound.Count);
        }

        private void RunOpenFileCommand(object o)
        {
            var row = Owner.SelectedItem as FileScanDisplayObject;
            if (row != null)
            {
                OpenHelper.OpenFile(Paths.Title.WithRoot(row.FileName));
            }
        }

        private void RunDeleteFileCommand(object o)
        {
            var row = Owner.SelectedItem as FileScanDisplayObject;
            if (row != null)
            {
                if (Restless.Tools.Utility.FileOperations.SendToRecycle(Paths.Title.WithRoot(row.FileName)))
                {
                    RemoveFromNotFound(row);
                }
            }
        }
        #endregion
    }
}
