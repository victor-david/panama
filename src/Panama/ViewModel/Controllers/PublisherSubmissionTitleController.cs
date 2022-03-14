/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Controls;
using System.ComponentModel;
using System.Data;
using System.Windows.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents the controller that handles the titles that have been submitted to a publisher.
    /// </summary>
    public class PublisherSubmissionTitleController : PublisherController
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
        /// Initializes a new instance of the <see cref="PublisherSubmissionTitleController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public PublisherSubmissionTitleController(PublisherViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<SubmissionTable>());
            MainView.RowFilter = string.Format("{0}=-1", SubmissionTable.Defs.Columns.Joined.PublisherId);
            MainView.Sort = string.Format("{0} DESC", SubmissionTable.Defs.Columns.Joined.Submitted);
            Columns.Create("Id", SubmissionTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.W042);
            //Columns.Create("Submitted", SubmissionTable.Defs.Columns.Joined.Submitted).MakeDate();
            Columns.Create("Title", SubmissionTable.Defs.Columns.Joined.Title);
            Columns.Create("Written", SubmissionTable.Defs.Columns.Joined.Written).MakeDate();
            //AddDataGridViewColumns();
            Commands.Add("GoToTitleRecord", RunGoToTitleRecordCommand);
            MenuItems.AddItem("Go to title record for this item", Commands["GoToTitleRecord"]).AddImageResource("ImageBrowseToUrlMenu");
            AddViewSourceSortDescriptions();
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
            MainView.RowFilter = string.Format("{0}={1}", SubmissionTable.Defs.Columns.Joined.PublisherId, publisherId);
            DataViewCount = MainView.Count;
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void AddViewSourceSortDescriptions()
        {
            // TODO
            //MainSource.SortDescriptions.Clear();
            //MainSource.GroupDescriptions.Clear();
            //// BUG: If grouped, and the first clicked parent has zero children, all the columns are scrunched together,
            //// and clicking on another parent (with children) does not change the columns.
            //// If not grouped, still scrunched if the first clicked parent has no children, but subsequent clicks
            //// on parents that do have children restore the columns.
            //// UPDATE 2018-08-25:
            ////   Workaround implemented. By binding the visibility of the data grid to the child count (0=hidden, otherwise visible)
            ////   the columns display as they should.
            //MainSource.GroupDescriptions.Add(new PropertyGroupDescription(SubmissionTable.Defs.Columns.Joined.Submitted, new DateToFormattedDateConverter()));
            //MainSource.SortDescriptions.Add(new SortDescription(SubmissionTable.Defs.Columns.Joined.Submitted, ListSortDirection.Descending));
            //MainSource.SortDescriptions.Add(new SortDescription(SubmissionTable.Defs.Columns.Joined.Title, ListSortDirection.Ascending));
        }

        private void RunGoToTitleRecordCommand(object o)
        {
            if (SelectedRow != null)
            {
                long titleId = (long)SelectedRow[SubmissionTable.Defs.Columns.TitleId];

                var ws = MainWindowViewModel.Instance.SwitchToWorkspace<TitleViewModel>();
                if (ws != null)
                {
                    ws.Config.TitleFilter.SetIdFilter(titleId);
                    if (ws.MainView.Count == 1)
                    {
                        /* This method uses a funky work around */
                        // TODO
                        //ws.SetSelectedItem(ws.MainView[0]);
                        /* Can be assigned directly, but doesn't highlight the row */
                        //ws.SelectedItem = ws.DataView[0];
                    }
                }

            }
        }
        #endregion
    }
}