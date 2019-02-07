/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Controls;
using Restless.Tools.Utility;
using System.Data;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that handles adding a title to an existing submission.
    /// </summary>
    /// <remarks>
    /// This controller displays submissions that are available for adding a title. 
    /// To be available, a submission must not yet have a response date and must be unlocked.
    /// </remarks>
    public class TitleAddToSubmissionController : TitleController
    {
        #region Private
        private bool visible = false;
        #endregion

        /************************************************************************/
        
        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if the portion of the UI bound to this property is visible.
        /// </summary>
        public bool Visible
        {
            get { return visible; }
            private set
            {
                SetProperty(ref visible, value);
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleAddToSubmissionController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleAddToSubmissionController(TitleViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<SubmissionBatchTable>());
            DataView.RowFilter = string.Format("{0} IS NULL AND {1}=0", SubmissionBatchTable.Defs.Columns.Response, SubmissionBatchTable.Defs.Columns.Locked);
            DataView.Sort = string.Format("{0} DESC", SubmissionBatchTable.Defs.Columns.Submitted);

            Columns.Create("Id", SubmissionBatchTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Date", SubmissionBatchTable.Defs.Columns.Submitted).MakeDate();

            Columns.CreateImage<BooleanToImageConverter>("E", SubmissionBatchTable.Defs.Columns.Joined.PublisherExclusive, "ImageExclamation")
                .AddToolTip(Strings.TooltipPublisherExclusive);

            Columns.Create("Publisher", SubmissionBatchTable.Defs.Columns.Joined.Publisher);

            Owner.Commands.Add("AddTitleToSubmission", (o) => 
            {
                Visible = true;
            });

            Owner.Commands.Add("AddTitleToSubmissionConfirm", (o) =>
            {
                long titleId = (long)Owner.SelectedPrimaryKey;
                long batchId = (long)SelectedPrimaryKey;
                long pubId = (long)SelectedRow[SubmissionBatchTable.Defs.Columns.PublisherId];
                if (ConfirmAddTitleToSubmission(titleId, pubId, batchId))
                {
                    DatabaseController.Instance.GetTable<SubmissionTable>().AddSubmission(batchId, titleId);
                }
            }, (o) => 
            { 
                return SelectedRow != null; 
            });

            Owner.Commands.Add("AddTitleToSubmissionCancel", (o) =>
            {
                Visible = false;
                SelectedItem = null;
            });
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
            Visible = false; 
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Confirms that the title should be added to the submission.
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="publisherId">The publisher id</param>
        /// <param name="batchId">The batch id</param>
        /// <returns>true if confirmed</returns>
        private bool ConfirmAddTitleToSubmission(long titleId, long publisherId, long batchId)
        {
            if (!CheckSimultaneousSubmission(titleId, publisherId))
            {
                return false;
            }

            int count = DatabaseController.Instance.GetTable<SubmissionBatchTable>().GetTitleToPublisherCount(titleId, publisherId, batchId);
            if (count > 0)
            {
                return Messages.ShowYesNo(Strings.ConfirmationTitleSubmittedPreviouslyToPublisher);
            }

            return true;
        }

        private bool CheckSimultaneousSubmission(long titleId, long publisherId)
        {
            int count = DatabaseController.Instance.GetTable<SubmissionBatchTable>().GetExclusiveCount(titleId, publisherId);
            if (count > 0)
            {
                return Messages.ShowYesNo(Strings.ConfirmationTitleSubmittedToExclusivePublisher);
            }
            return true;
        }
        #endregion
    }
}