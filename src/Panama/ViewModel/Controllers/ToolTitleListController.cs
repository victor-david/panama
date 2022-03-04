/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Resources;
using Restless.Panama.Tools;
using Restless.Toolkit.Utility;
using System.IO;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the controller that handles creating a list of titles.
    /// </summary>
    public class ToolTitleListController : ToolControllerBase<ToolTitleListViewModel>
    {
        #region Private
        private readonly TitleLister scanner;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the task id for this controller.
        /// </summary>
        public override int TaskId
        {
            get { return AppTaskId.TitleList; }
        }

        /// <summary>
        /// Gets the scanner object for this controller.
        /// </summary>
        public override Tools.FileScanBase Scanner
        {
            get { return scanner; }
        }

        /// <summary>
        /// Gets the full path to the file that holds the list of titles.
        /// </summary>
        public string TitleListFileName
        {
            get { return scanner.TitleListFileName; }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolTitleListController"/> class.
        /// </summary>
        /// <param name="owner">The <see cref="ToolTitleListViewModel"/> that owns this controller.</param>
        public ToolTitleListController(ToolTitleListViewModel owner)
            :base(owner)
        {
            scanner = new TitleLister(Config.Instance.FolderTitleRoot);
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
        #endregion
    }
}