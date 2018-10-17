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
    /// Represents the class that handles export for title versions.
    /// </summary>
    public class ToolExportTitleController : ToolControllerBase<ToolExportViewModel>
    {
        #region Private
        private TitleExporter scanner;
        #endregion
        
        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the task id for this controller.
        /// </summary>
        public override int TaskId
        {
            get { return AppTaskId.TitleExportScan; }
        }

        /// <summary>
        /// Gets the scanner object for this controller.
        /// </summary>
        public override Tools.FileScanBase Scanner
        {
            get { return scanner; }
        }

        /// <summary>
        /// Gets the header text for the UI. Instead of binding
        /// directly to Strings.XXX, we bind her so we can add "not set"
        /// if the export folder isn't set.
        /// </summary>
        public string HeaderText
        {
            get
            {
                string exportFolder = Config.Instance.FolderExport;
                if (string.IsNullOrEmpty(exportFolder))
                {
                    exportFolder = "(not set)";
                }
                return string.Format(Strings.HeaderToolOperationExport, exportFolder);
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolExportTitleController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public ToolExportTitleController(ToolExportViewModel owner)
            : base(owner)
        {
            scanner = new TitleExporter(Config.Instance.FolderExport);
            scanner.Updated += (s, e) =>
            {
                TaskManager.Instance.DispatchTask(() => { AddToUpdated(e.Target); });
            };
            scanner.NotFound += (s, e) =>
            {
                TaskManager.Instance.DispatchTask(() => { AddToNotFound(e.Target); });
            };
        }
        #endregion


        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Begins execution of the export task.
        /// </summary>
        public override void Run()
        {
            if (string.IsNullOrEmpty(Config.Instance.FolderExport) || !Directory.Exists(Config.Instance.FolderExport))
            {
                Messages.ShowError(Strings.InvalidOpExportFolderNotSet);
                return;
            }
            ClearCollections();
            scanner.Execute(TaskId);
        }
        #endregion
    }
}
