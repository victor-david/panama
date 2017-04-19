﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;


namespace Restless.App.Panama.ViewModel
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
        #pragma warning disable 1591
        public CredentialViewModel()
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

            RawCommands.Add("CopyLoginId", (o) =>
                {
                    CopyCredentialPart(CredentialTable.Defs.Columns.LoginId);
                },
                CanRunCommandIfRowSelected);

            RawCommands.Add("CopyPassword", (o) =>
            {
                CopyCredentialPart(CredentialTable.Defs.Columns.Password);
            },
            CanRunCommandIfRowSelected);


            /* Context menu items */
            MenuItems.AddItem(Strings.CommandCopyLoginId, RawCommands["CopyLoginId"]);
            MenuItems.AddItem(Strings.CommandCopyPassword, RawCommands["CopyPassword"]);

            MenuItems.AddItem(Strings.CommandDeleteCredential, DeleteCommand, "ImageDeleteMenu");
            FilterPrompt = Strings.FilterPromptCredential;
        }
        #pragma warning restore 1591
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
            DataView.RowFilter = String.Format("{0} LIKE '%{1}%'", CredentialTable.Defs.Columns.Name, text);
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
            if (Messages.ShowYesNo(Strings.ConfirmationDeleteCredential))
            {
                Int64 credId = (Int64)SelectedPrimaryKey;
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
            return CanRunCommandIfRowSelected(null);
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
                Clipboard.SetText(SelectedRow[columnName].ToString());
                MainViewModel.CreateNotificationMessage(String.Format("{0} copied to clipboard", columnName));
            }
        }
        #endregion
    }
}