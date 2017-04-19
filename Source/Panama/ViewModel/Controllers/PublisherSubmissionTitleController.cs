using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Controls;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represents the controller that handles the titles that have been submitted to a publisher.
    /// </summary>
    public class PublisherSubmissionTitleController : PublisherController
    {
        #region Private
        #endregion

        /************************************************************************/
        
        #region Public properties
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
            DataView.RowFilter = String.Format("{0}=-1", SubmissionTable.Defs.Columns.Joined.PublisherId);
            DataView.Sort = String.Format("{0} DESC", SubmissionTable.Defs.Columns.Joined.Submitted);
            //SortDirection = ListSortDirection.Ascending;
            Columns.Create("Id", SubmissionTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            var col = Columns.Create("Submitted", SubmissionTable.Defs.Columns.Joined.Submitted).MakeDate();
            Columns.SetDefaultSort(col, ListSortDirection.Descending);
            SortItemSource(col);
            Columns.Create("Title", SubmissionTable.Defs.Columns.Joined.Title);
            Columns.Create("Written", SubmissionTable.Defs.Columns.Joined.Written).MakeDate();
            RawCommands.Add("GridSorting", (o) => { SortItemSource(o as DataGridBoundColumn); });

            RawCommands.Add("GoToTitleRecord", RunGoToTitleRecordCommand);
            MenuItems.AddItem("Go to title record for this item", RawCommands["GoToTitleRecord"], "ImageBrowseToUrlMenu");

            //SortItemSource(SubmissionTable.Defs.Columns.Joined.Submitted);
            //MainSource.GroupDescriptions.Add(new PropertyGroupDescription(SubmissionTable.Defs.Columns.Joined.Submitted, new DateToFormattedDateConverter()));
            //MainSource.GroupDescriptions.Add(new PropertyGroupDescription(SubmissionTable.Defs.Columns.Joined.Publisher));

            //MainSource.SortDescriptions.Add(new SortDescription(SubmissionTable.Defs.Columns.Joined.Submitted, ListSortDirection.Descending));

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
            Int64 publisherId = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = String.Format("{0}={1}", SubmissionTable.Defs.Columns.Joined.PublisherId, publisherId);
        }
        #endregion
        
        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Groups and sorts the item source
        /// </summary>
        /// <param name="col">The column. This acts as a secondary sort.</param>
        private void SortItemSource(DataGridBoundColumn col)
        {
            //col.SortDirection = (col.SortDirection == ListSortDirection.Ascending) ? ListSortDirection.Descending : ListSortDirection.Ascending;
            if (col.SortDirection == null)
            {
                col.SortDirection = ListSortDirection.Ascending;
            }
            using (MainSource.DeferRefresh())
            {
                MainSource.GroupDescriptions.Clear();
                MainSource.SortDescriptions.Clear();
                MainSource.SortDescriptions.Add(new SortDescription(SubmissionTable.Defs.Columns.Joined.Submitted, ListSortDirection.Descending));
                MainSource.SortDescriptions.Add(new SortDescription(((System.Windows.Data.Binding)col.Binding).Path.Path, (ListSortDirection)col.SortDirection));
                MainSource.GroupDescriptions.Add(new PropertyGroupDescription(SubmissionTable.Defs.Columns.Joined.Submitted, new DateToFormattedDateConverter()));
            }
        }

        private void RunGoToTitleRecordCommand(object o)
        {
            if (SelectedRow != null)
            {
                Int64 titleId = (Int64)SelectedRow[SubmissionTable.Defs.Columns.TitleId];
                var ws = Owner.MainViewModel.SwitchToWorkspace<TitleViewModel>();
                if (ws != null)
                {
                    // in case the VM was already open with a filter applied.
                    ws.Filters.ClearAll();
                    // assigning the property applies the filter
                    ws.Config.TitleFilter.Id = titleId;
                    if (ws.DataView.Count == 1)
                    {
                        /* This method uses a funky work around */
                        ws.SetSelectedItem(ws.DataView[0]);
                        /* Can be assigned directly, but doesn't highlight the row */
                        //ws.SelectedItem = ws.DataView[0];
                    }
                }

            }
        }
        #endregion
    }
}
