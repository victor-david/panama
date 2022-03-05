/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.View;
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using System;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the display and selection logic for the <see cref="TitleVersionRenameWindow"/>.
    /// </summary>
    public class TitleVersionRenameWindowViewModel : WindowViewModel
    {
        #region Private
        private readonly TitleVersionRenameItemCollection renameView;
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
            get { return operationMessage; }
            private set
            {
                SetProperty(ref operationMessage, value);
            }

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
            renameView = new TitleVersionRenameItemCollection();
            MainSource.Source = renameView;
            Columns.Create("Ver", TitleVersionRenameItem.Properties.Version)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.Standard);

            Columns.Create<IntegerToCharConverter>("Rev", TitleVersionRenameItem.Properties.RevisionChar)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.Standard);

            Columns.Create("Old name", TitleVersionRenameItem.Properties.OriginalNameDisplay);
            Columns.Create("New name", TitleVersionRenameItem.Properties.NewNameDisplay);
            Columns.Create("Status", TitleVersionRenameItem.Properties.Status);
            Commands.Add("Rename", RunRenameCommand, CanRunRenameCommand);
            PopulateRenameItems(titleId);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion

        /************************************************************************/

        #region Private methods
        private void PopulateRenameItems(long titleId)
        {
            var title  = DatabaseController.Instance.GetTable<TitleTable>().GetSingleRecord(titleId);
            if (title == null)
            {
                throw new InvalidOperationException(Strings.InvalidOpTitleDoesNotExist);
            }

            foreach (var ver in DatabaseController.Instance.GetTable<TitleVersionTable>().EnumerateVersions(titleId))
            {
                renameView.Add(new TitleVersionRenameItem(ver, title.Title));
            }

            if (renameView.Count == 0)
            {
                OperationMessage = Strings.InvalidOpRenameCandidateListEmpty;
                return;
            }

            canRename = false;

            if (!renameView.AllOriginalExist)
            {
                OperationMessage = Strings.InvalidOpRenameFilesMissing;
            }
            else if (renameView.AllSame)
            {
                OperationMessage = Strings.InvalidOpRenameAllCandidatesAlreadyRenamed;
            }
            else
            {
                canRename = true;
            }
        }

        private void RunRenameCommand(object o)
        {
            Execution.TryCatch(() =>
            {
                renameView.Rename();
                DatabaseController.Instance.GetTable<TitleVersionTable>().Save();
                OperationMessage = Strings.ConfirmationAllVersionFilesRenamed;
                canRename = false;
            });
        }

        private bool CanRunRenameCommand(object o)
        {
            return canRename;
        }
        #endregion
    }
}