/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Toolkit.Controls;
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents a controller that displays the columns of a table.
    /// </summary>
    public class TableColumnController : TableBaseController<DataColumn>
    {
        #region Private
        private readonly ObservableCollection<DataColumn> dataColumns;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableColumnController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TableColumnController(TableViewModel owner) : base (owner)
        {
            Columns.Create("Name", nameof(DataColumn.ColumnName))
                .MakeFixedWidth(FixedWidth.W180);

            Columns.Create("Type", nameof(DataColumn.DataType));

            Columns.Create("Expression", nameof(DataColumn.Expression))
                .MakeFlexWidth(2.5);

            dataColumns = new ObservableCollection<DataColumn>();
            InitListView(dataColumns);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (Owner.SelectedTable != null)
            {
                dataColumns.Clear();
                foreach (DataColumn col in Owner.SelectedTable.Columns)
                {
                    dataColumns.Add(col);
                }
            }
        }

        protected override int OnDataRowCompare(DataColumn item1, DataColumn item2)
        {
            return string.Compare(item1.ColumnName, item2.ColumnName, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}