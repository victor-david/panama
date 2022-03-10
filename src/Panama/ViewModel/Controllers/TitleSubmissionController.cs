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
using Restless.Toolkit.Controls;
using System.ComponentModel;
using System.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that displays the history of sibmissions for a title.
    /// </summary>
    public class TitleSubmissionController : TitleController
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties

        /// <summary>
        /// Gets the controller that displays available submissions.
        /// A submission is available if it does not yet have a response date and it is unlocked.
        /// </summary>
        public TitleAddToSubmissionController Available
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleSubmissionController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleSubmissionController(TitleViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<SubmissionTable>());
            MainView.RowFilter = string.Format("{0}=-1", SubmissionTable.Defs.Columns.TitleId);
            MainView.Sort = string.Format("{0} DESC", SubmissionTable.Defs.Columns.Joined.Submitted);
            Columns.Create("Id", SubmissionTable.Defs.Columns.BatchId)
                .MakeFixedWidth(FixedWidth.W042);
            Columns.CreateImage<IntegerToImageConverter>("S", SubmissionTable.Defs.Columns.Status, "ImageSubStatus")
                .AddToolTip("Submission status of this title");
            Columns.SetDefaultSort(Columns.Create("Submitted", SubmissionTable.Defs.Columns.Joined.Submitted)
                .MakeDate(), ListSortDirection.Descending);
            Columns.CreateImage<BooleanToImageConverter>("E", SubmissionTable.Defs.Columns.Joined.PublisherExclusive, "ImageExclamation")
                .AddToolTip(Strings.TooltipPublisherExclusive);
            Columns.Create("Publisher", SubmissionTable.Defs.Columns.Joined.Publisher);
            Columns.Create("Batch Response", SubmissionTable.Defs.Columns.Joined.ResponseTypeName);
            AddViewSourceSortDescriptions();

            Available = new TitleAddToSubmissionController(Owner);

            HeaderPreface = Strings.HeaderSubmissions;
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
            long titleId = GetOwnerSelectedPrimaryId();
            MainView.RowFilter = string.Format("{0}={1}", SubmissionTable.Defs.Columns.TitleId, titleId);
            Available.Update();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(SubmissionTable.Defs.Columns.Joined.SubmittedCalc, ListSortDirection.Descending));
            MainSource.SortDescriptions.Add(new SortDescription(SubmissionTable.Defs.Columns.Joined.Submitted, ListSortDirection.Descending));
        }
        #endregion
    }
}