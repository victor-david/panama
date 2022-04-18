/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Controls;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.SubmissionBatchTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents the controller that handles the submissions for a publisher.
    /// </summary>
    public class PublisherSubmissionController : BaseController<PublisherViewModel, SubmissionBatchTable>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherSubmissionController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public PublisherSubmissionController(PublisherViewModel owner): base(owner)
        {
            Columns.Create("Id", TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Submitted", TableColumns.Submitted)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create("Response", TableColumns.Response)
                .MakeDate();

            Columns.Create("Type", TableColumns.Joined.ResponseTypeName)
                .MakeFixedWidth(FixedWidth.W096);

            Columns.Create("Note", TableColumns.Notes).MakeSingleLine();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.Submitted);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.PublisherId] == (Owner?.SelectedPublisher?.Id ?? 0);
        }
        #endregion
    }
}