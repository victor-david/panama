/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Core;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Controls;
using Restless.Tools.Utility;
using System;
using System.Data;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that manages selection and updates of published titles.
    /// </summary>
    public class TitlePublishedController : TitleController
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the published date.
        /// </summary>
        public object PublishedDate
        {
            get
            {
                if (SelectedRow != null)
                {
                    if (SelectedRow[PublishedTable.Defs.Columns.Published] is DateTime dt)
                    {
                        return dt;
                    }
                }
                return null;
            }
            set
            {
                if (SelectedRow != null)
                {
                    if (value == null) value = DBNull.Value;
                    SelectedRow[PublishedTable.Defs.Columns.Published] = value;
                    OnPropertyChanged();
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitlePublishedController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitlePublishedController(TitleViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<PublishedTable>());
            DataView.RowFilter = string.Format("{0}=-1", PublishedTable.Defs.Columns.TitleId);
            DataView.Sort = string.Format("{0} DESC", PublishedTable.Defs.Columns.Added);
            Columns.Create("Added", PublishedTable.Defs.Columns.Added).MakeDate();
            Columns.Create("Published", PublishedTable.Defs.Columns.Published).MakeDate();
            Columns.Create("Publisher", PublishedTable.Defs.Columns.Joined.Publisher);
            Columns.Create("Url",  PublishedTable.Defs.Columns.Url);
            Commands.Add("PublishedAdd", RunAddPublishedCommand);
            Commands.Add("PublishedRemove", RunRemovePublishedCommand, (o) => SelectedRow != null);
            Commands.Add("ClearPublishedDate", (o) => PublishedDate = null);
            HeaderPreface = Strings.HeaderPublished;
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
            long titleId = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = $"{PublishedTable.Defs.Columns.TitleId}={titleId}";
        }

        /// <summary>
        /// Called when the selected item changes
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            OnPropertyChanged(nameof(PublishedDate));
        }

        /// <summary>
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> to open the url of the published title.
        /// </summary>
        /// <param name="item">The <see cref="DataRowView"/> object of the selected row.</param>
        protected override void RunOpenRowCommand(object item)
        {
            DataRowView view = item as DataRowView;
            if (view != null)
            {
                string url = view.Row[PublishedTable.Defs.Columns.Url].ToString();
                if (!string.IsNullOrEmpty(url))
                {
                    OpenHelper.OpenWebSite(null, url);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunAddPublishedCommand(object o)
        {
            var window = WindowFactory.PublisherSelect.Create(Strings.WindowTitleSelectPublisherForPublished);
            window.ShowDialog();
            var vm = window.DataContext as PublisherSelectWindowViewModel;
            if (vm != null)
            {
                Execution.TryCatch(() =>
                    {
                        long publisherId = vm.SelectedPublisherId;
                        if (publisherId > 0)
                        {
                            long titleId = (long)Owner.SelectedPrimaryKey;
                            DatabaseController.Instance.GetTable<PublishedTable>().Add(titleId, publisherId);
                        }
                    });
            }
        }

        private void RunRemovePublishedCommand(object o)
        {
            if (SelectedRow != null && Messages.ShowYesNo(Strings.ConfirmationRemoveTitlePublished))
            {
                SelectedRow.Delete();
                DatabaseController.Instance.GetTable<PublishedTable>().Save();
            }
        }
        #endregion
    }
}