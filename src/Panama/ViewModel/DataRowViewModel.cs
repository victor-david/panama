/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Toolkit.Core.Database.SQLite;
using System;
using System.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides common functionality for views that use DataGrid to display table rows.
    /// This class must be interited.
    /// </summary>
    /// <typeparam name="T">The table type derived from <see cref="TableBase"/></typeparam>
    public abstract class DataRowViewModel<T> : DataViewModel<DataRowView> where T : TableBase
    {
        #region Properties
        /// <summary>
        /// Gets the table object associated with this instance
        /// </summary>
        public T Table => DatabaseController.Instance.GetTable<T>();

        /// <inheritdoc/>
        public override bool OpenRowCommandEnabled => IsSelectedRowAccessible;

        /// <summary>
        /// Gets the selected data row
        /// </summary>
        public DataRow SelectedRow => SelectedItem is DataRowView view ? view.Row : null;

        /// <summary>
        /// Gets a boolean value that indicates if <see cref="SelectedRow"/> is accessible.
        /// Returns true if the selected row is not null, is not detached, and is not deleted; otherwise, false.
        /// </summary>
        public bool IsSelectedRowAccessible
        {
            get => SelectedRow != null && SelectedRow.RowState != DataRowState.Deleted && SelectedRow.RowState != DataRowState.Detached;
        }

        /// <summary>
        /// Gets the primary key value of the selected row, or null if none 
        /// (no selected row or no primary key column on the table)
        /// </summary>
        public object SelectedPrimaryKey => IsSelectedRowAccessible && Table.PrimaryKeyName != null ? SelectedRow[Table.PrimaryKeyName] : null;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataRowViewModel{T}"/> class.
        /// </summary>
        protected DataRowViewModel()
        {
            InitListView(new DataView(Table));
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Override in a derived class to perform actions when the selected item changes. 
        /// Always call the base implementation to perform standard operations.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            OnPropertyChanged(nameof(SelectedRow));
        }

        protected sealed override int OnDataRowCompare(DataRowView item1, DataRowView item2)
        {
            return OnDataRowCompare(item1.Row, item2.Row);
        }

        protected sealed override bool OnDataRowFilter(DataRowView item)
        {
            return OnDataRowFilter(item.Row);
        }

        /// <summary>
        /// Override in a derived class to compares two specified <see cref="DataRow"/> objects.
        /// The base method returns zero.
        /// </summary>
        /// <param name="item1">The first data row</param>
        /// <param name="item2">The second data row</param>
        /// <returns>An integer value 0, 1, or -1</returns>
        protected virtual int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return 0;
        }

        /// <summary>
        /// Override in a derived class to filter <see cref="ListView"/>. The base implementation returns true.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if <paramref name="item"/> is included; otherwise, false.</returns>
        protected virtual bool OnDataRowFilter(DataRow item)
        {
            return true;
        }

        /// <summary>
        /// Shortcut method. Compares two data rows in a stringwise fashion.
        /// </summary>
        /// <param name="item1">The first data row.</param>
        /// <param name="item2">The second data row.</param>
        /// <param name="columnName">The name of the column to compare</param>
        /// <returns>A string comparison result (zero, 1, or -1)</returns>
        protected int DataRowCompareString(DataRow item1, DataRow item2, string columnName)
        {
            return string.Compare(item1[columnName].ToString(), item2[columnName].ToString(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Shortcut method. Compares two data rows in a boolean fashion.
        /// </summary>
        /// <param name="item1">The first data row.</param>
        /// <param name="item2">The second data row.</param>
        /// <param name="columnName">The name of the column to compare</param>
        /// <returns>A string comparison result (zero, 1, or -1)</returns>
        protected int DataRowCompareBoolean(DataRow item1, DataRow item2, string columnName)
        {
            return ((bool)item1[columnName]).CompareTo((bool)item2[columnName]);
        }

        /// <summary>
        /// Shortcut method. Compares two data rows in a long integer fashion.
        /// </summary>
        /// <param name="item1">The first data row.</param>
        /// <param name="item2">The second data row.</param>
        /// <param name="columnName">The name of the column to compare</param>
        /// <returns>A string comparison result (zero, 1, or -1)</returns>
        protected int DataRowCompareLong(DataRow item1, DataRow item2, string columnName)
        {
            return ((long)item1[columnName]).CompareTo((long)item2[columnName]);
        }

        /// <summary>
        /// Shortcut method. Compares two data rows in a date/time fashion.
        /// </summary>
        /// <param name="item1">The first data row.</param>
        /// <param name="item2">The second data row.</param>
        /// <param name="columnName">The name of the column to compare</param>
        /// <returns>A date/time comparison result (zero, 1, or -1)</returns>
        protected int DataRowCompareDateTime(DataRow item1, DataRow item2, string columnName)
        {
            return DateTime.Compare((DateTime)item1[columnName], (DateTime)item2[columnName]);
        }

        /// <summary>
        /// Deletes the selected row, saves the table, and refreshes <see cref="ListView"/>.
        /// </summary>
        protected void DeleteSelectedRow()
        {
            if (SelectedRow != null)
            {
                SelectedRow.Delete();
                Table.Save();
                ListView.Refresh();
            }
        }
        #endregion
    }
}