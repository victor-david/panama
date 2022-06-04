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
using System;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.PublishedTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that manages selection and updates of published titles.
    /// </summary>
    public class TitlePublishedController : BaseController<TitleViewModel, PublishedTable>
    {
        #region Private
        private PublishedRow selectedPublished;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

        /// <inheritdoc/>
        public override bool OpenRowCommandEnabled => SelectedPublished?.HasUrl ?? false;

        /// <summary>
        /// Gets the selected published row
        /// </summary>
        public PublishedRow SelectedPublished
        {
            get => selectedPublished;
            private set => SetProperty(ref selectedPublished, value);
        }

        /// <summary>
        /// Gets or sets the published date.
        /// </summary>
        public DateTime? PublishedDate
        {
            get => SelectedPublished?.Published;
            set
            {
                SelectedPublished?.SetPublishedDate(value);
                OnPropertyChanged();
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitlePublishedController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitlePublishedController(TitleViewModel owner) : base(owner)
        {
            Columns.Create("Added", TableColumns.Added)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create("Published", TableColumns.Published)
                .MakeDate();

            Columns.Create("Publisher", TableColumns.Joined.Publisher);

            MenuItems.AddItem(Strings.MenuItemAddPublished, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddItem(Strings.MenuItemBrowseToUrlOrClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(
                Strings.MenuItemClearPublishedDate,
                RelayCommand.Create(RunClearPublishedDateCommand, p => SelectedPublished?.HasPublishedDate ?? false)
                );
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemRemovePublished, DeleteCommand).AddIconResource(ResourceKeys.Icon.XMediumIconKey);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedPublished = PublishedRow.Create(SelectedRow);
            OnPropertyChanged(nameof(PublishedDate));
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.Added);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.TitleId] == (Owner?.SelectedTitle?.Id ?? 0);
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            if (WindowFactory.PublisherSelect.Create().GetPublisher() is PublisherRow publisher)
            {
                Table.Add(Owner.SelectedTitle.Id, publisher.Id);
                ListView.Refresh();
            }
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (IsSelectedRowAccessible && MessageWindow.ShowContinueCancel(Strings.ConfirmationRemoveTitlePublished))
            {
                DeleteSelectedRow();
            }
        }

        /// <inheritdoc/>
        protected override void RunOpenRowCommand()
        {
            if (SelectedPublished?.HasUrl ?? false)
            {
                OpenHelper.OpenWebSite(null, SelectedPublished.Url);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunClearPublishedDateCommand(object parm)
        {
            if (MessageWindow.ShowContinueCancel(Strings.ConfirmationClearPublishedDate))
            {
                PublishedDate = null;
            }
        }
        #endregion
    }
}