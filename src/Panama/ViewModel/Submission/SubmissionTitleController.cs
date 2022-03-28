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
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Submission title controller. Handles title related to a submission
    /// </summary>
    public class SubmissionTitleController : BaseController<SubmissionViewModel, SubmissionTable>
    {
        #region Private
        private SubmissionRow selectedSubmission;

        private static readonly Dictionary<long, string> PathMap = new Dictionary<long, string>
        {
            { SubmissionTable.Defs.Values.StatusWithdrawn, ResourceKeys.Icon.SquareSmallGrayIconKey },
            { SubmissionTable.Defs.Values.StatusAccepted, ResourceKeys.Icon.SquareSmallGreenIconKey },
        };
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the selected submission.
        /// </summary>
        public SubmissionRow SelectedSubmission
        {
            get => selectedSubmission;
            private set => SetProperty(ref selectedSubmission, value);
        }
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
        public SubmissionTitleController(SubmissionViewModel owner) : base(owner)
        {
            Columns.Create("Id", SubmissionTable.Defs.Columns.Id)
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("O", SubmissionTable.Defs.Columns.Ordering)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip("Ordering");

            Columns.CreateResource<Int64ToPathConverter>("S", SubmissionTable.Defs.Columns.Status, PathMap)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(LocalResources.Get(ResourceKeys.ToolTip.SubmissionTitleStatusToolTip));

            Columns.Create("Title", SubmissionTable.Defs.Columns.Joined.Title);

            Columns.Create("Written", SubmissionTable.Defs.Columns.Joined.Written)
                .MakeDate();

            Commands.Add("TitleMoveUp", RunMoveUpCommand, CanRunMoveUpCommand);
            Commands.Add("TitleMoveDown", RunMoveDownCommand, CanRunMoveDownCommand);

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
                if (MessageWindow.ShowYesNo(Strings.ConfirmationRemoveTitleFromSubmission))
                {
                    DeleteSelectedRow();
                }
            }, (o) =>
            {
                return
                    SelectedRow != null &&
                    Owner.SelectedRow != null &&
                    !(bool)Owner.SelectedRow[SubmissionBatchTable.Defs.Columns.Locked];
            });

            Commands.Add("CopyToClipboard", RunCopyToClipboardCommand, (o) => { return SourceCount > 0; });

            ListView.IsLiveSorting = true;
            ListView.LiveSortingProperties.Add(SubmissionTable.Defs.Columns.Ordering);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedSubmission = SubmissionRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareLong(item1, item2, SubmissionTable.Defs.Columns.Ordering);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[SubmissionTable.Defs.Columns.BatchId] == (Owner?.SelectedBatch?.Id ?? 0);
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            ListView.Refresh();
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void RunMoveUpCommand(object o)
        {
            if (CanRunMoveUpCommand(null))
            {
                Table.MoveSubmissionUp(SelectedSubmission);
            }
        }

        private bool CanRunMoveUpCommand(object o)
        {
            return CanRunMoveCommandBase() && SelectedSubmission.Ordering > 1;
        }

        private void RunMoveDownCommand(object o)
        {
            if (CanRunMoveDownCommand(null))
            {
                Table.MoveSubmissionDown(SelectedSubmission);
            }
        }

        private bool CanRunMoveDownCommand(object o)
        {
            return CanRunMoveCommandBase() && SelectedSubmission.Ordering < Table.GetHighestOrdering(Owner.SelectedBatch.Id);
        }

        private bool CanRunMoveCommandBase()
        {
            return
                IsSelectedRowAccessible &&
                SelectedSubmission != null &&
                Owner.IsSelectedRowAccessible &&
                !(Owner.SelectedBatch?.IsLocked ?? true);
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