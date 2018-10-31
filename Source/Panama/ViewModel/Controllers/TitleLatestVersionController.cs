using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides a intermediary class that implements the RunOpenRow command for the latest version of a title.
    /// </summary>
    /// <typeparam name="VM">A view model type derived from <see cref="DataGridViewModel{T}"/></typeparam>
    /// <typeparam name="T">A type derived from <see cref="TableBase"/></typeparam>
    public abstract class TitleLatestVersionController<VM, T> : ControllerBase<VM, T>
        where VM : DataGridViewModel<T>
        where T : TableBase
    {

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleLatestVersionController{VM,T}"/> class.
        /// </summary>
        /// <param name="owner">The owner of this controller</param>
        protected TitleLatestVersionController(VM owner) : base (owner)
        {
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> to open the latest version of a selected title.
        /// </summary>
        /// <param name="item">The <see cref="DataRowView"/> object of the selected row.</param>
        protected override void RunOpenRowCommand(object item)
        {
            if (item is DataRowView view)
            {
                long titleId = (long)view.Row[SubmissionTable.Defs.Columns.TitleId];
                var verController = DatabaseController.Instance.GetTable<TitleVersionTable>().GetVersionController(titleId);
                if (verController.Versions.Count > 0)
                {
                    OpenFileRow(verController.Versions[0].Row, TitleVersionTable.Defs.Columns.FileName, Config.Instance.FolderTitleRoot, (f) =>
                    {
                        Messages.ShowError(string.Format(Strings.FormatStringFileNotFound, f, "FolderTitleRoot"));
                    });
                }
            }
        }
        #endregion
    }
}
