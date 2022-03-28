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
using System;
using System.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that manages selection and updates of published titles.
    /// </summary>
    public class TitleSelfPublishedController : ControllerBase<TitleViewModel, TitleTable>
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
                    if (SelectedRow[SelfPublishedTable.Defs.Columns.Published] is DateTime dt)
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
                    SelectedRow[SelfPublishedTable.Defs.Columns.Published] = value;
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
        public TitleSelfPublishedController(TitleViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<SelfPublishedTable>());
            MainView.RowFilter = string.Format("{0}=-1", SelfPublishedTable.Defs.Columns.TitleId);
            MainView.Sort = string.Format("{0} DESC", SelfPublishedTable.Defs.Columns.Added);
            Columns.Create("Added", SelfPublishedTable.Defs.Columns.Added).MakeDate();
            Columns.Create("Published", SelfPublishedTable.Defs.Columns.Published).MakeDate();
            Columns.Create("Publisher", SelfPublishedTable.Defs.Columns.Joined.SelfPublisher);
            Columns.Create("Url",  SelfPublishedTable.Defs.Columns.Url);
            Commands.Add("PublishedAdd", RunAddPublishedCommand);
            Commands.Add("PublishedRemove", RunRemovePublishedCommand, (o) => SelectedRow != null);
            Commands.Add("ClearPublishedDate", (o) => PublishedDate = null);
            HeaderPreface = Strings.HeaderSelfPublished;
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
            MainView.RowFilter = $"{SelfPublishedTable.Defs.Columns.TitleId}={titleId}";
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
        protected override void RunOpenRowCommand()
        {
            // TODO
            //DataRowView view = item as DataRowView;
            //if (view != null)
            //{
            //    string url = view.Row[SelfPublishedTable.Defs.Columns.Url].ToString();
            //    if (!string.IsNullOrEmpty(url))
            //    {
            //        OpenHelper.OpenWebSite(null, url);
            //    }
            //}
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunAddPublishedCommand(object o)
        {
            var window = WindowFactory.SelfPublisherSelect.Create(Strings.WindowTitleSelectPublisherForPublished);
            window.ShowDialog();
            if (window.DataContext is SelfPublisherSelectWindowViewModel vm)
            {
                Execution.TryCatch(() =>
                    {
                        long publisherId = vm.SelectedPublisherId;
                        if (publisherId > 0)
                        {
                            long titleId = (long)Owner.SelectedPrimaryKey;
                            DatabaseController.Instance.GetTable<SelfPublishedTable>().Add(titleId, publisherId);
                        }
                    });
            }
        }

        private void RunRemovePublishedCommand(object o)
        {
            if (SelectedRow != null && Messages.ShowYesNo(Strings.ConfirmationRemoveTitlePublished))
            {
                SelectedRow.Delete();
                DatabaseController.Instance.GetTable<SelfPublishedTable>().Save();
            }
        }
        #endregion
    }
}