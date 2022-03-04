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
using System.Windows;


namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage credentials in the <see cref="CredentialTable"/>.
    /// </summary>
    public class CredentialViewModel : DataGridViewModel<CredentialTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the publisher controller.
        /// </summary>
        public CredentialPublisherController Publisher
        {
            get;
            private set;
        }



        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialViewModel"/> class.
        /// </summary>
        /// <param name="owner">The VM that owns this view model.</param>
        public CredentialViewModel(ApplicationViewModel owner) : base(owner)
        {
            DisplayName = Strings.CommandCredential;
            MaxCreatable = 1;

            Publisher = new CredentialPublisherController(this);

            Columns.Create("Id", LinkTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.SetDefaultSort(Columns.Create("Name", CredentialTable.Defs.Columns.Name), ListSortDirection.Ascending);
            Columns.Create("Login Id", CredentialTable.Defs.Columns.LoginId);
            Columns.Create("Password", CredentialTable.Defs.Columns.Password).MakeMasked();
            AddViewSourceSortDescriptions();
            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandAddCredential, Strings.CommandAddCredentialTooltip, AddCommand, ResourceHelper.Get("ImageAdd"), VisualCommandImageSize, VisualCommandFontSize));

            Commands.Add("CopyLoginId", (o) =>
                {
                    CopyCredentialPart(CredentialTable.Defs.Columns.LoginId);
                },
                (o) => IsSelectedRowAccessible);

            Commands.Add("CopyPassword", (o) =>
            {
                CopyCredentialPart(CredentialTable.Defs.Columns.Password);
            },
            (o) => IsSelectedRowAccessible);


            /* Context menu items */
            MenuItems.AddItem(Strings.CommandCopyLoginId, Commands["CopyLoginId"]);
            MenuItems.AddItem(Strings.CommandCopyPassword, Commands["CopyPassword"]);

            MenuItems.AddItem(Strings.CommandDeleteCredential, DeleteCommand).AddImageResource("ImageDeleteMenu");
            FilterPrompt = Strings.FilterPromptCredential;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called when the selected item changes.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            Publisher.Update();
        }
        /// <summary>
        /// Called when the filter text has changed to set the filter on the underlying data.
        /// </summary>
        /// <param name="text">The filter text.</param>
        protected override void OnFilterTextChanged(string text)
        {
            DataView.RowFilter = string.Format("{0} LIKE '%{1}%'", CredentialTable.Defs.Columns.Name, text);
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            Table.AddDefaultRow();
            Table.Save();
            FilterText = null;
            AddViewSourceSortDescriptions();
            Columns.RestoreDefaultSort();
        }

        /// <summary>
        /// Called when the framework checks to see if Add command can execute
        /// </summary>
        /// <returns>This method always returns true.</returns>
        protected override bool CanRunAddCommand()
        {
            return true;
        }

        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
        protected override void RunDeleteCommand()
        {
            if (IsSelectedRowAccessible && Messages.ShowYesNo(Strings.ConfirmationDeleteCredential))
            {
                long credId = (long)SelectedPrimaryKey;
                DatabaseController.Instance.GetTable<PublisherTable>().ClearCredential(credId);
                DatabaseController.Instance.GetTable<LinkTable>().ClearCredential(credId);
                SelectedRow.Delete();
                Table.Save();
            }
        }

        /// <summary>
        /// Called when the framework checks to see if Delete command can execute
        /// </summary>
        /// <returns>true if a row is selected; otherwise, false.</returns>
        protected override bool CanRunDeleteCommand()
        {
            return IsSelectedRowAccessible;
        }
        #endregion

        /************************************************************************/

        #region Private Methods

        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(CredentialTable.Defs.Columns.Name, ListSortDirection.Ascending));
        }

        private void CopyCredentialPart(string columnName)
        {
            if (SelectedRow != null)
            {
                // Once in a while the Clipboard.SetText() method throws an exception.
                // It's rare, but catch it if it happens so it's not unhandled.
                Execution.TryCatch(() =>
                {
                    Clipboard.SetText(SelectedRow[columnName].ToString());
                    MainWindowViewModel.Instance.CreateNotificationMessage($"{columnName} copied to clipboard");
                });
            }
        }
        #endregion
    }
}