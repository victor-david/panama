/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Core;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.Tools.Controls;
using System.ComponentModel;
using System.Data;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represents the controller that handles the submissions for a publisher.
    /// </summary>
    public class PublisherSubmissionController : PublisherController
    {
        #region Private
        private int dataViewCount;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the count of rows in the data view. The view binds to this property
        /// </summary>
        public int DataViewCount
        {
            get => dataViewCount;
            private set => SetProperty(ref dataViewCount, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherSubmissionController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public PublisherSubmissionController(PublisherViewModel owner): base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<SubmissionBatchTable>());
            DataView.RowFilter = string.Format("{0}=-1", SubmissionBatchTable.Defs.Columns.PublisherId);
            DataView.Sort = string.Format("{0} DESC", SubmissionBatchTable.Defs.Columns.Submitted);
            Columns.Create("Id", SubmissionBatchTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            var col = Columns.Create("Submitted", SubmissionBatchTable.Defs.Columns.Submitted).MakeDate();
            Columns.SetDefaultSort(col, ListSortDirection.Descending);
            Columns.Create("Response", SubmissionBatchTable.Defs.Columns.Response).MakeDate();
            Columns.Create("Type", SubmissionBatchTable.Defs.Columns.Joined.ResponseTypeName).MakeFixedWidth(FixedWidth.MediumString);
            Columns.Create("Note", SubmissionBatchTable.Defs.Columns.Notes).MakeSingleLine();
        }
        #endregion

        /************************************************************************/

        #region Public methods

        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            long publisherId = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = string.Format("{0}={1}", SubmissionBatchTable.Defs.Columns.PublisherId, publisherId);
            DataViewCount = DataView.Count;
        }
        #endregion

        /************************************************************************/

        #region Private methods

        #endregion
    }
}