/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Resources;
using Restless.App.Panama.Tools;
using Restless.Tools.Utility;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represents the class that handles updates for title versions.
    /// </summary>
    public class ToolTitleVersionController : ToolControllerBase<ToolMetaUpdateViewModel>
    {
        #region Private
        private readonly VersionUpdater scanner;
        #endregion
        
        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the task id for this controller.
        /// </summary>
        public override int TaskId
        {
            get => AppTaskId.TitleVersionScan;
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
        /// Initializes a new instance of the <see cref="ToolTitleVersionController"/> class.
        /// </summary>
        /// <param name="owner">The owner of this controller.</param>
        public ToolTitleVersionController(ToolMetaUpdateViewModel owner)
            : base(owner)
        {
            scanner = new VersionUpdater();

            scanner.Updated += (s, e) =>
            {
                TaskManager.Instance.DispatchTask(() =>
                {
                    AddToUpdated(e.Target);
                    UpdateFoundHeader();
                });
            };

            scanner.NotFound += (s, e) =>
            {
                TaskManager.Instance.DispatchTask(() =>
                {
                    AddToNotFound(e.Target);
                    UpdateNotFoundHeader();
                });
            };

            UpdateFoundHeader();
            UpdateNotFoundHeader();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Begins execution of the operations for this controller
        /// </summary>
        public override void Run()
        {
            ClearCollections();
            scanner.Execute(TaskId);
        }
        #endregion

        /************************************************************************/
        
        #region Private methods
        private void UpdateFoundHeader()
        {
            UpdatedHeader = string.Format(Strings.HeaderToolOperationMetaTitleUpdatedFormat, Updated.Count);
        }

        private void UpdateNotFoundHeader()
        {
            NotFoundHeader = string.Format(Strings.HeaderToolOperationMetaTitleNotFoundFormat, NotFound.Count);
        }
        #endregion
    }
}