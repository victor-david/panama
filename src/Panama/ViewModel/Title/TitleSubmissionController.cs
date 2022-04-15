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
using System.ComponentModel;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.SubmissionTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that displays the history of submissions for a title.
    /// </summary>
    public class TitleSubmissionController : BaseController<TitleViewModel, SubmissionTable>
    {
        #region Private
        private SubmissionRow selectedSubmission;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Get the selected submission
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
        /// Initializes a new instance of the <see cref="TitleSubmissionController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleSubmissionController(TitleViewModel owner) : base(owner)
        {
            Columns.Create("Id", TableColumns.TitleId)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.CreateResource<Int64ToPathConverter>("S", TableColumns.Status, ResourceKeys.Icon.TitleStatusIconMap)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(LocalResources.Get(ResourceKeys.ToolTip.SubmissionTitleStatusToolTip));

            Columns.SetDefaultSort(
                Columns.Create("Submitted", TableColumns.Joined.Submitted)
                .MakeDate(),
                ListSortDirection.Descending);

            Columns.CreateResource<BooleanToPathConverter>("E", TableColumns.Joined.PublisherExclusive, ResourceKeys.Icon.SquareSmallRedIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipPublisherExclusive);

            Columns.Create("Publisher", TableColumns.Joined.Publisher);
            Columns.Create("Response", TableColumns.Joined.ResponseTypeName);
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
            return (long)item[TableColumns.TitleId] == (Owner?.SelectedTitle?.Id ?? 0);
        }
        #endregion
    }
}