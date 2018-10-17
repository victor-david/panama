﻿using System;
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
using Restless.App.Panama.Tools;
using Restless.Tools.Threading;
using Restless.App.Panama.Resources;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represents the class that handles updates for submission documents.
    /// </summary>
    public class ToolSubmissionDocumentController : ToolControllerBase<ToolMetaUpdateViewModel>
    {
        #region Private
        private SubmissionUpdater scanner;
        #endregion
        
        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the task id for this controller.
        /// </summary>
        public override int TaskId
        {
            get { return AppTaskId.SubmissionDocumentScan; }
        }

        /// <summary>
        /// Gets the scanner object for this controller.
        /// </summary>
        public override FileScanBase Scanner
        {
            get { return scanner; }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSubmissionDocumentController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public ToolSubmissionDocumentController(ToolMetaUpdateViewModel owner)
            : base(owner)
        {
            scanner = new SubmissionUpdater();

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
            UpdatedHeader = string.Format(Strings.HeaderToolOperationMetaSubmissionUpdatedFormat, Updated.Count);
        }

        private void UpdateNotFoundHeader()
        {
            NotFoundHeader = string.Format(Strings.HeaderToolOperationMetaSubmissionNotFoundFormat, NotFound.Count);
        }
        #endregion

    }
}
