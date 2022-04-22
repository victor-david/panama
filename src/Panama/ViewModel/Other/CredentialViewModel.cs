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
using Restless.Toolkit.Mvvm;
using Restless.Toolkit.Utility;
using System.Data;
using System.Windows;
using TableColumns = Restless.Panama.Database.Tables.CredentialTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to view and manage credentials in the <see cref="CredentialTable"/>.
    /// </summary>
    public class CredentialViewModel : DataRowViewModel<CredentialTable>
    {
        #region Private
        private PublisherTable PublisherTable => DatabaseController.Instance.GetTable<PublisherTable>();
        private LinkTable LinkTable => DatabaseController.Instance.GetTable<LinkTable>();
        private CredentialRow selectedCredential;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

        /// <summary>
        /// Gets the selected credential
        /// </summary>
        public CredentialRow SelectedCredential
        {
            get => selectedCredential;
            private set => SetProperty(ref selectedCredential, value);
        }

        /// <summary>
        /// Gets the publisher controller.
        /// </summary>
        public CredentialPublisherController Publisher
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialViewModel"/> class.
        /// </summary>
        public CredentialViewModel()
        {
            Publisher = new CredentialPublisherController(this);

            Columns.Create("Id", TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Name", TableColumns.Name).MakeInitialSortAscending();
            Columns.Create("Login Id", TableColumns.LoginId);
            Columns.Create("Password", TableColumns.Password).MakeMasked();

            /* Context menu items */
            MenuItems.AddItem(Strings.MenuItemAddCredential, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(
                Strings.CommandCopyLoginId,
                RelayCommand.Create(p => CopyToClipboard(SelectedCredential?.LoginId), p => IsSelectedRowAccessible)
                );
            MenuItems.AddItem(
                Strings.CommandCopyPassword,
                RelayCommand.Create(p => CopyToClipboard(SelectedCredential?.Password), p => IsSelectedRowAccessible)
                );
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandDeleteCredential, DeleteCommand).AddIconResource(ResourceKeys.Icon.XMediumIconKey);

            ListView.IsLiveSorting = true;
            ListView.LiveSortingProperties.Add(TableColumns.Name);
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedCredential = CredentialRow.Create(SelectedRow);
            Publisher.Update();
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareString(item1, item2, TableColumns.Name);
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            Table.AddDefaultRow();
            Table.Save();
            ForceListViewSort();
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (SelectedCredential != null && Messages.ShowYesNo(Strings.ConfirmationDeleteCredential))
            {
                PublisherTable.ClearCredential(SelectedCredential.Id);
                LinkTable.ClearCredential(SelectedCredential.Id);
                DeleteSelectedRow();
            }
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void CopyToClipboard(string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                {
                    Clipboard.SetText(value);
                    MainWindowViewModel.Instance.CreateNotificationMessage("Copied to clipboard");
                }
            }
            catch
            {
            }
        }
        #endregion
    }
}