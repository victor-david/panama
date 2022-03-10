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
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Utility;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Restless.Panama.ViewModel
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
            MainView.RowFilter = string.Format("{0}=-1", SubmissionTable.Defs.Columns.BatchId);
            MainView.Sort = string.Format("{0},{1}", SubmissionTable.Defs.Columns.Ordering, SubmissionTable.Defs.Columns.Joined.Title);

            Columns.Create("Id", SubmissionTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.Create("O", SubmissionTable.Defs.Columns.Ordering).MakeCentered().MakeFixedWidth(FixedWidth.W042).AddToolTip("Ordering");

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
            MainView.RowFilter = string.Format("{0}={1}", SubmissionTable.Defs.Columns.BatchId, id);
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
            if (CanRunMoveUpCommand(null))
            {
                DatabaseController.Instance.GetTable<SubmissionTable>().MoveSubmissionUp(SelectedRow);
            }
        }

        private bool CanRunMoveUpCommand(object o)
        {
            return
                CanRunMoveCommandBase() &&
                (long)SelectedRow[SubmissionTable.Defs.Columns.Ordering] > 1;
        }

        private void RunMoveDownCommand(object o)
        {
            if (CanRunMoveDownCommand(null))
            {
                DatabaseController.Instance.GetTable<SubmissionTable>().MoveSubmissionDown(SelectedRow);
            }
        }

        private bool CanRunMoveDownCommand(object o)
        {
            return
                CanRunMoveCommandBase() &&
                (long)SelectedRow[SubmissionTable.Defs.Columns.Ordering] < DatabaseController.Instance.GetTable<SubmissionTable>().GetHighestOrdering((long)Owner.SelectedPrimaryKey);
        }

        private bool CanRunMoveCommandBase()
        {
            return
                Owner.IsSelectedRowAccessible &&
                !(bool)Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Locked] &&
                IsSelectedRowAccessible;
        }

        private void RunCopyToClipboardCommand(object o)
        {
            // Had a uncaught exception buried deep within the Clipboard.SetText() method.
            // May have been a one-off, but putting the following within a try/catch.
            Execution.TryCatch(() =>
            {
                StringBuilder builder = new();
                foreach (DataRowView rowv in MainView)
                {
                    builder.AppendLine(rowv.Row[SubmissionTable.Defs.Columns.Joined.Title].ToString());
                }
                System.Windows.Clipboard.SetText(builder.ToString());
                MainWindowViewModel.Instance.CreateNotificationMessage(Strings.ConfirmationTitlesCopiedToClipboard);
            });
        }
        #endregion

    }
}