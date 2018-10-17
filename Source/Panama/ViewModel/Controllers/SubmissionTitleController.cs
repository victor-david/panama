using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Configuration;
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
    /// Submission title controller. Handles title related to a submission
    /// </summary>
    public class SubmissionTitleController : SubmissionController
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        #endregion

        /************************************************************************/

        #region Protected properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionTitleController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public SubmissionTitleController(SubmissionViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<SubmissionTable>());
            DataView.RowFilter = string.Format("{0}=-1", SubmissionTable.Defs.Columns.BatchId);
            DataView.Sort = string.Format("{0},{1}", SubmissionTable.Defs.Columns.Ordering, SubmissionTable.Defs.Columns.Joined.Title);

            Columns.Create("Id", SubmissionTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.SetDefaultSort(Columns.Create("O", SubmissionTable.Defs.Columns.Ordering).MakeFixedWidth(FixedWidth.Standard).AddToolTip("Ordering"), ListSortDirection.Ascending);
            Columns.CreateImage<IntegerToImageConverter>("S", SubmissionTable.Defs.Columns.Status, "ImageSubStatus").AddToolTip("Submission status of this title");
            Columns.Create("Title", SubmissionTable.Defs.Columns.Joined.Title);
            Columns.Create("Written", SubmissionTable.Defs.Columns.Joined.Written).MakeDate();
            AddViewSourceSortDescriptions();

            Commands.Add("MoveUp", RunMoveUpCommand, CanRunMoveUpCommand);
            Commands.Add("MoveDown", RunMoveDownCommand, CanRunMoveDownCommand);

            Commands.Add("SetStatusAccepted", (o) =>
            {
                SelectedRow[SubmissionTable.Defs.Columns.Status] = SubmissionTable.Defs.Values.StatusAccepted;
            }, (o) =>
            {
                return 
                    SelectedRow != null &&  
                    Owner.SelectedRow != null &&
                    (long)Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.ResponseType] == ResponseTable.Defs.Values.ResponseAccepted;
            });

            Commands.Add("SetStatusWithdrawn", (o) =>
            {
                SelectedRow[SubmissionTable.Defs.Columns.Status] = SubmissionTable.Defs.Values.StatusWithdrawn;
            }, (o) =>
            {
                return SelectedRow != null;
            });

            Commands.Add("ResetStatus", (o) =>
            {
                SelectedRow[SubmissionTable.Defs.Columns.Status] = SubmissionTable.Defs.Values.StatusNotSpecified;
            }, (o) =>
            {
                return SelectedRow != null;
            });

            Commands.Add("RemoveFromSubmission", (o) =>
            {
                if (Messages.ShowYesNo(Strings.ConfirmationRemoveTitleFromSubmission))
                {
                    SelectedRow.Delete();
                    DatabaseController.Instance.GetTable<SubmissionTable>().Save();
                }
            }, (o) =>
            {
                return
                    SelectedRow != null &&
                    Owner.SelectedRow != null &&
                    !(bool)Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Locked];
            });

            Commands.Add("CopyToClipboard", RunCopyToClipboardCommand, (o) => { return SourceCount > 0; });

            HeaderPreface = Strings.HeaderTitles;
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
            long id = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = string.Format("{0}={1}", SubmissionTable.Defs.Columns.BatchId, id);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(SubmissionTable.Defs.Columns.Ordering, ListSortDirection.Ascending));
            MainSource.SortDescriptions.Add(new SortDescription(SubmissionTable.Defs.Columns.Joined.Title, ListSortDirection.Ascending));
        }


        private void RunMoveUpCommand(object o)
        {
            if (SelectedRow != null && Owner.SelectedRow != null)
            {
                long batchId = (long)Owner.SelectedPrimaryKey;
                long ordering = (long)SelectedRow[SubmissionTable.Defs.Columns.Ordering];
                DatabaseController.Instance.GetTable<SubmissionTable>().ChangeSubmissionOrdering(batchId, ordering, ordering - 1);
            }
        }
        
        private bool CanRunMoveUpCommand(object o)
        {
            return SelectedRow != null && (long)SelectedRow[SubmissionTable.Defs.Columns.Ordering] > 1;
        }

        private void RunMoveDownCommand(object o)
        {
            if (SelectedRow != null && Owner.SelectedRow != null)
            {
                long batchId = (long)Owner.SelectedPrimaryKey;
                long ordering = (long)SelectedRow[SubmissionTable.Defs.Columns.Ordering];
                DatabaseController.Instance.GetTable<SubmissionTable>().ChangeSubmissionOrdering(batchId, ordering, ordering + 1);
            }
        }

        private bool CanRunMoveDownCommand(object o)
        {
            return SelectedRow != null;
        }

        private void RunCopyToClipboardCommand(object o)
        {
            // Had a uncaught exception buried deep within the Clipboard.SetText() method.
            // May have been a one-off, but putting the following within a try/catch.
            Execution.TryCatch(() =>
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (DataRowView rowv in DataView)
                    {
                        builder.AppendLine(rowv.Row[SubmissionTable.Defs.Columns.Joined.Title].ToString());
                    }
                    System.Windows.Clipboard.SetText(builder.ToString());
                    Owner.MainViewModel.CreateNotificationMessage(Strings.ConfirmationTitlesCopiedToClipboard);
                });
        }
        #endregion

    }
}
