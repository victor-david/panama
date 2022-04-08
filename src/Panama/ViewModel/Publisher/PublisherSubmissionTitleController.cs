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
using System.Data;
using System.Windows.Data;
using TableColumns = Restless.Panama.Database.Tables.SubmissionTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents the controller that handles the titles that have been submitted to a publisher.
    /// </summary>
    public class PublisherSubmissionTitleController : BaseController<PublisherViewModel, SubmissionTable>
    {
        #region Private
        private SubmissionRow selectedSubmission;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the selected submission
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
        /// Initializes a new instance of the <see cref="PublisherSubmissionTitleController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public PublisherSubmissionTitleController(PublisherViewModel owner) : base(owner)
        {
            Columns.Create("Id", TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Batch", TableColumns.BatchId)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W048);

            Columns.Create("Title", TableColumns.Joined.Title);

            Columns.Create("Written", TableColumns.Joined.Written).MakeDate();

            //ListView.GroupDescriptions.Add(new PropertyGroupDescription(TableColumns.Joined.Submitted));

            MenuItems.AddItem(Strings.MenuItemOpenTitleOrDoubleClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedSubmission = SubmissionRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.Joined.Submitted);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.Joined.PublisherId] == (Owner?.SelectedPublisher?.Id ?? 0);
        }

        /// <inheritdoc/>
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
    }
}