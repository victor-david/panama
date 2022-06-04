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
using Restless.Panama.View;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using System;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the display and selection logic for the <see cref="TitleVersionRenameWindow"/>.
    /// </summary>
    public class TitleVersionRenameWindowViewModel : DataViewModel<TitleVersionRenameItem>
    {
        #region Private
        private readonly TitleVersionRenameItemCollection renameItems;
        private string operationMessage;
        private bool canRename;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets an operation message that can be bound to the UI.
        /// Provides feedback such as "All files are already renamed", "Success. All files successfully renamed", etc.
        /// </summary>
        public string OperationMessage
        {
            get => operationMessage;
            private set => SetProperty(ref operationMessage, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleVersionRenameWindowViewModel"/> class.
        /// </summary>
        /// <param name="titleId">The title id for the versions to rename.</param>
        public TitleVersionRenameWindowViewModel(long titleId)
        {
            renameItems = new TitleVersionRenameItemCollection();

            Columns.Create("Ver", nameof(TitleVersionRenameItem.Version))
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create<IntegerToCharConverter>("Rev", nameof(TitleVersionRenameItem.RevisionChar))
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Old name", nameof(TitleVersionRenameItem.OriginalNameDisplay));
            Columns.Create("New name", nameof(TitleVersionRenameItem.NewNameDisplay));
            Columns.Create("Status", nameof(TitleVersionRenameItem.Status));
            Commands.Add("Rename", RunRenameCommand, p => canRename);

            PopulateRenameItems(titleId);
            InitListView(renameItems);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override int OnDataRowCompare(TitleVersionRenameItem item1, TitleVersionRenameItem item2)
        {
            int result = item1.Version.CompareTo(item2.Version);
            if (result == 0)
            {
                result = item1.RevisionChar.CompareTo(item2.RevisionChar);
            }
            return result;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void PopulateRenameItems(long titleId)
        {
            TitleRow title  = DatabaseController.Instance.GetTable<TitleTable>().GetSingleRecord(titleId);
            if (title == null)
            {
                throw new InvalidOperationException(Strings.InvalidOpTitleDoesNotExist);
            }

            foreach (TitleVersionRow ver in DatabaseController.Instance.GetTable<TitleVersionTable>().EnumerateVersions(titleId, SortDirection.Ascending))
            {
                renameItems.Add(new TitleVersionRenameItem(ver, title.Title));
            }

            if (renameItems.Count == 0)
            {
                OperationMessage = Strings.InvalidOpRenameCandidateListEmpty;
                return;
            }

            canRename = false;

            if (!renameItems.AllOriginalExist)
            {
                OperationMessage = Strings.InvalidOpRenameFilesMissing;
            }
            else if (renameItems.AllSame)
            {
                OperationMessage = Strings.InvalidOpRenameAllCandidatesAlreadyRenamed;
            }
            else
            {
                canRename = true;
            }
        }

        private void RunRenameCommand(object parm)
        {
            Execution.TryCatch(() =>
            {
                renameItems.Rename();
                DatabaseController.Instance.GetTable<TitleVersionTable>().Save();
                OperationMessage = Strings.ConfirmationAllVersionFilesRenamed;
                canRename = false;
            });
        }
        #endregion
    }
}