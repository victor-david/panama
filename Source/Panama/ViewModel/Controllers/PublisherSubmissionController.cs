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
        public PublisherSubmissionController(PublisherViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<SubmissionBatchTable>());
            DataView.RowFilter = string.Format("{0}=-1", SubmissionBatchTable.Defs.Columns.PublisherId);
            DataView.Sort = string.Format("{0} DESC", SubmissionBatchTable.Defs.Columns.Submitted);
            Columns.Create("Id", SubmissionBatchTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.SetDefaultSort(Columns.Create("Submitted", SubmissionBatchTable.Defs.Columns.Submitted).MakeDate(), ListSortDirection.Descending);
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
