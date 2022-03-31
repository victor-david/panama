/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Core.Database.SQLite;
using Restless.Toolkit.Utility;
using System.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides a intermediary class that implements the RunOpenRow command for the latest version of a title.
    /// </summary>
    /// <typeparam name="VM">A view model type derived from <see cref="DataRowViewModel{T}"/></typeparam>
    /// <typeparam name="T">A type derived from <see cref="TableBase"/></typeparam>
    public abstract class TitleLatestVersionController<VM, T> : BaseController<VM, T>
        where VM : DataRowViewModel<T>
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
        /// Runs the <see cref="DataRowViewModel{T}.OpenRowCommand"/> to open the latest version of a selected title.
        /// </summary>
        protected override void RunOpenRowCommand()
        {
            // TODO
            //if (item is DataRowView view)
            //{
            //    long titleId = (long)view.Row[SubmissionTable.Defs.Columns.TitleId];
            //    var verController = DatabaseController.Instance.GetTable<TitleVersionTable>().GetVersionController(titleId);
            //    if (verController.Versions.Count > 0)
            //    {
            //        OpenFileRow(verController.Versions[0].Row, TitleVersionTable.Defs.Columns.FileName, Config.Instance.FolderTitleRoot, (f) =>
            //        {
            //            Messages.ShowError(string.Format(Strings.FormatStringFileNotFound, f, "FolderTitleRoot"));
            //        });
            //    }
            //}
        }
        #endregion
    }
}