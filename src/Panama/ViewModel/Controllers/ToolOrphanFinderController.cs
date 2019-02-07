/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Resources;
using Restless.App.Panama.Tools;
using Restless.Tools.Utility;
using System.IO;

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
            get => AppTaskId.OrphanFind;
        }

        /// <summary>
        /// Gets the scanner object for this controller.
        /// </summary>
        public override FileScanBase Scanner
        {
            get => scanner;
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
                TaskManager.Instance.DispatchTask(() => AddToUpdated(e.Target));
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
            Owner.Commands.Add("DeleteFile", RunDeleteFileCommand, (o) => Owner.SelectedItem != null);
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
            if (string.IsNullOrEmpty(Config.Instance.FolderTitleRoot) || !Directory.Exists(Config.Instance.FolderTitleRoot))
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
            NotFoundHeader = string.Format(Strings.HeaderToolOperationOrphanNotFoundFormat, NotFound.Count);
        }

        private void RunOpenFileCommand(object o)
        {
            if (Owner.SelectedItem is FileScanDisplayObject row)
            {
                OpenHelper.OpenFile(Paths.Title.WithRoot(row.FileName));
            }
        }

        private void RunDeleteFileCommand(object o)
        {
            if (Owner.SelectedItem is FileScanDisplayObject row)
            {
                if (FileOperations.SendToRecycle(Paths.Title.WithRoot(row.FileName)))
                {
                    RemoveFromNotFound(row);
                }
            }
        }
        #endregion
    }
}