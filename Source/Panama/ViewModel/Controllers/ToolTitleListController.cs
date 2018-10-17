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
    /// Provides the controller that handles creating a list of titles.
    /// </summary>
    public class ToolTitleListController : ToolControllerBase<ToolTitleListViewModel>
    {
        #region Private
        private TitleLister scanner;
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
