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
using System.Linq;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents a controller that displays tables contraints.
    /// </summary>
    /// <typeparam name="T">The type of contraint that this controller handles.</typeparam>
    public class TableConstraintController<T> : BaseController<TableViewModel, TableTable> where T : Constraint
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the collection of unique constraints for this controller
        /// </summary>
        public ObservableCollection<T> Constraints
        {
            get;
            private set;
        }

        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableConstraintController{T}"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TableConstraintController(TableViewModel owner)
            : base(owner)
        {
            Constraints = new ObservableCollection<T>();
            Columns.Create("Name", "ConstraintName").MakeFixedWidth(150);
            Columns.Create("Table", "Table.TableName");
            Columns.Create("Column", "Columns[0].ColumnName");
            if (typeof(T) == typeof(ForeignKeyConstraint))
            {
                Columns.Create("Delete Rule", "DeleteRule");
            }

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
            //string tableName = GetOwnerSelectedPrimaryIdString();
            //if (tableName != null)
            //{
            //    var table = DatabaseController.Instance.DataSet.Tables[tableName];
            //    Constraints.Clear();
            //    foreach (T c in table.Constraints.OfType<T>())
            //    {
            //        Constraints.Add(c);
            //    }
            //}
        }
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion
    }
}