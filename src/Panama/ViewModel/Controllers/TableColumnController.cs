/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Controls;
using System.Collections.ObjectModel;
using System.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents a controller that displays the columns of a table.
    /// </summary>
    public class TableColumnController : ControllerBase<TableViewModel, TableTable>
    {
        #region Private
        #endregion

        /************************************************************************/
        
        #region Public properties

        /// <summary>
        /// Gets the list of data columns
        /// </summary>
        public ObservableCollection<DataColumn> DataColumns
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableColumnController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TableColumnController(TableViewModel owner)
            : base(owner)
        {
            DataColumns = new ObservableCollection<DataColumn>();
            Columns.Create("Name", "ColumnName");
            Columns.Create("Type", "DataType");
            Columns.Create("Expression", "Expression").MakeFlexWidth(2.5);
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
            string tableName = GetOwnerSelectedPrimaryIdString();
            if (tableName != null)
            {
                DataTable table = DatabaseController.Instance.DataSet.Tables[tableName];
                DataColumns.Clear();
                foreach (DataColumn col in table.Columns)
                {
                    DataColumns.Add(col);
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion
    }
}