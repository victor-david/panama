using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;

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
                visible = value;
                OnPropertyChanged("Visible");
            }
        }

        //public int Count
        //{
        //    get { return  Source.Count; }
        //}
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
            DataView.RowFilter = String.Format("{0} IS NULL AND {1}=0", SubmissionBatchTable.Defs.Columns.Response, SubmissionBatchTable.Defs.Columns.Locked);
            DataView.Sort = String.Format("{0} DESC", SubmissionBatchTable.Defs.Columns.Submitted);

            Columns.Create("Id", SubmissionBatchTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Date", SubmissionBatchTable.Defs.Columns.Submitted).MakeDate();
            Columns.Create("Publisher", SubmissionBatchTable.Defs.Columns.Joined.Publisher);

            Owner.RawCommands.Add("AddTitleToSubmission", (o) => 
            {
                Visible = true;
            });

            Owner.RawCommands.Add("AddTitleToSubmissionConfirm", (o) =>
            {
                Int64 titleId = (Int64)Owner.SelectedPrimaryKey;
                Int64 batchId = (Int64)SelectedPrimaryKey;
                Int64 pubId = (Int64)SelectedRow[SubmissionBatchTable.Defs.Columns.PublisherId];
                if (ConfirmAddTitleToSubmission(titleId, pubId, batchId))
                {
                    DatabaseController.Instance.GetTable<SubmissionTable>().AddSubmission(batchId, titleId);
                }
            }, (o) => 
            { 
                return SelectedRow != null; 
            });

            Owner.RawCommands.Add("AddTitleToSubmissionCancel", (o) =>
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
        private bool ConfirmAddTitleToSubmission(Int64 titleId, Int64 publisherId, Int64 batchId)
        {
            int count = DatabaseController.Instance.GetTable<SubmissionBatchTable>().GetTitleToPublisherCount(titleId, publisherId, batchId);
            if (count > 0)
            {
                return Messages.ShowYesNo(Strings.ConfirmationTitleSubmittedPreviouslyToPublisher);
            }

            return true;
        }
        #endregion
    }
}
