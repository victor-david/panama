/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Mvvm;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Linq;
using ResponseValues = Restless.Panama.Database.Tables.ResponseTable.Defs.Values;
using SubmissionValues = Restless.Panama.Database.Tables.SubmissionTable.Defs.Values;
using TableColumns = Restless.Panama.Database.Tables.SubmissionTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Submission title controller. Handles title related to a submission
    /// </summary>
    public class SubmissionTitleController : BaseController<SubmissionViewModel, SubmissionTable>
    {
        #region Private
        private SubmissionRow selectedSubmission;

        private static readonly Dictionary<long, string> PathMap = new()
        {
            { SubmissionValues.StatusWithdrawn, ResourceKeys.Icon.SquareSmallGrayIconKey },
            { SubmissionValues.StatusAccepted, ResourceKeys.Icon.SquareSmallGreenIconKey },
        };
        #endregion

        /************************************************************************/

        #region Public properties
        public override bool AddCommandEnabled => !(Owner.SelectedBatch?.IsLocked ?? true) && Owner.SelectedBatch.ResponseType == ResponseValues.NoResponse;

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

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionTitleController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public SubmissionTitleController(SubmissionViewModel owner) : base(owner)
        {
            Columns.Create("Id", TableColumns.Id)
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("O", TableColumns.Ordering)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip("Ordering");

            Columns.CreateResource<Int64ToPathConverter>("S", TableColumns.Status, PathMap)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(LocalResources.Get(ResourceKeys.ToolTip.SubmissionTitleStatusToolTip));

            Columns.Create("Title", TableColumns.Joined.Title);

            Columns.Create("Written", TableColumns.Joined.Written)
                .MakeDate();

            Commands.Add("TitleMoveUp", RunMoveUpCommand, CanRunMoveUpCommand);
            Commands.Add("TitleMoveDown", RunMoveDownCommand, CanRunMoveDownCommand);
            Commands.Add("CopyToClipboard", RunCopyToClipboardCommand);

            MenuItems.AddItem(Strings.MenuItemAddTitleToSubmission, AddCommand)
                .AddIconResource(ResourceKeys.Icon.PlusIconKey);

            MenuItems.AddSeparator();

            MenuItems.AddItem(
                Strings.MenuItemSetTitleStatusAccepted,
                RelayCommand.Create(RunSetTitleStatusCommand, CanRunSetTitleStatusCommand))
                .AddCommandParm(SubmissionValues.StatusAccepted)
                .AddIconResource(ResourceKeys.Icon.SquareSmallGreenIconKey);

            MenuItems.AddItem(
                Strings.MenuItemSetTitleStatusWithdrawn,
                RelayCommand.Create(RunSetTitleStatusCommand, CanRunSetTitleStatusCommand))
                .AddCommandParm(SubmissionValues.StatusWithdrawn)
                .AddIconResource(ResourceKeys.Icon.SquareSmallGrayIconKey);

            MenuItems.AddItem(
                Strings.MenuItemSetTitleStatusNone,
                RelayCommand.Create(RunSetTitleStatusCommand, CanRunSetTitleStatusCommand))
                .AddCommandParm(SubmissionValues.StatusNotSpecified);

            MenuItems.AddSeparator();

            MenuItems.AddItem(
                Strings.MenuItemRemoveTitleFromSubmission, 
                RelayCommand.Create(RunRemoveTitleFromSubmissionCommand, p => CanRunIfNotLocked()))
                .AddIconResource(ResourceKeys.Icon.XMediumIconKey);

            ListView.IsLiveSorting = true;
            ListView.LiveSortingProperties.Add(TableColumns.Ordering);
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
            return DataRowCompareLong(item1, item2, TableColumns.Ordering);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.BatchId] == (Owner?.SelectedBatch?.Id ?? 0);
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            ListView.Refresh();
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            if (WindowFactory.TitleSelect.Create().GetTitles() is List<TitleRow> titles && ConfirmTitleSubmission(titles))
            {
                titles.ForEach(title => Table.AddSubmission(Owner.SelectedBatch.Id, title.Id));
                ListView.Refresh();
            }
        }

        protected override void RunOpenRowCommand()
        {
            if (SelectedSubmission != null)
            {
                Database.Tables.TitleVersionController verController = TitleVersionTable.GetVersionController(SelectedSubmission.TitleId);
                if (verController.Versions.Count > 0)
                {
                    Open.TitleVersionFile(verController.Versions[0].FileName);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunMoveUpCommand(object parm)
        {
            if (CanRunMoveUpCommand(null))
            {
                Table.MoveSubmissionUp(SelectedSubmission);
            }
        }

        private bool CanRunMoveUpCommand(object parm)
        {
            return CanRunIfNotLocked() && SelectedSubmission.Ordering > 1;
        }

        private void RunMoveDownCommand(object parm)
        {
            if (CanRunMoveDownCommand(null))
            {
                Table.MoveSubmissionDown(SelectedSubmission);
            }
        }

        private bool CanRunMoveDownCommand(object parm)
        {
            return CanRunIfNotLocked() && SelectedSubmission.Ordering < Table.GetHighestOrdering(Owner.SelectedBatch.Id);
        }

        private bool CanRunIfNotLocked()
        {
            return
                IsSelectedRowAccessible &&
                SelectedSubmission != null &&
                Owner.IsSelectedRowAccessible &&
                !(Owner.SelectedBatch?.IsLocked ?? true);
        }

        private void RunSetTitleStatusCommand(object parm)
        {
            if (parm is long status)
            {
                SelectedSubmission.Status = status;
            }
        }

        private bool CanRunSetTitleStatusCommand(object parm)
        {
            return
                parm is long status && 
                SelectedSubmission != null &&
                (status != SubmissionValues.StatusAccepted || Owner.SelectedBatch?.ResponseType == ResponseValues.ResponseAccepted);
        }

        private void RunRemoveTitleFromSubmissionCommand(object parm)
        {
            if (MessageWindow.ShowYesNo(Strings.ConfirmationRemoveTitleFromSubmission))
            {
                DeleteSelectedRow();
            }
        }

        private void RunCopyToClipboardCommand(object parm)
        {
            Execution.TryCatch(() =>
            {
                StringBuilder builder = new();
                foreach (SubmissionRow row in Table.EnumerateAll(Owner.SelectedBatch.Id))
                {
                    builder.AppendLine(row.Title);
                }
                System.Windows.Clipboard.SetText(builder.ToString());
                MainWindowViewModel.Instance.CreateNotificationMessage(Strings.ConfirmationTitlesCopiedToClipboard);
            });
        }

        private bool ConfirmTitleSubmission(List<TitleRow> titles)
        {
            RemoveDuplicateTitles(titles);
            return titles.Count == 0 || WindowFactory.TitleConfirm.Create(Owner.SelectedBatch, titles).ConfirmTitles();
        }

        /* Removes titles that are already part of the current batch */
        private void RemoveDuplicateTitles(List<TitleRow> titles)
        {
            List<TitleRow> toRemove = new();

            titles.ForEach(title =>
            {
                if (Table.SubmissionExists(Owner.SelectedBatch.Id, title.Id))
                {
                    toRemove.Add(title);
                }
            });

            toRemove.ForEach(title =>
            {
                titles.Remove(title);
            });
        }
        #endregion
    }
}