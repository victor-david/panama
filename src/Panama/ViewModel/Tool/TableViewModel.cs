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
using Restless.Toolkit.Mvvm;
using System.ComponentModel;
using System.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to display information about the application tables.
    /// </summary>
    public class TableViewModel : DataRowViewModel<TableTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the column controller.
        /// </summary>
        public TableColumnController ColumnData
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the controller for parent relations.
        /// </summary>
        public TableParentRelationController Parents
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the controller for child relations.
        /// </summary>
        public TableChildRelationController Children
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the unique constraints controller.
        /// </summary>
        public TableConstraintController<UniqueConstraint> Unique
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the FK constraints controller.
        /// </summary>
        public TableConstraintController<ForeignKeyConstraint> Foreign
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableViewModel"/> class.
        /// </summary>
        public TableViewModel()
        {
            DisplayName = Strings.CommandTable;
            Columns.SetDefaultSort(Columns.Create("Name", TableTable.Defs.Columns.Name), ListSortDirection.Ascending);
            Columns.Create("Cols", TableTable.Defs.Columns.ColumnCount).MakeFixedWidth(FixedWidth.W052);
            Columns.Create("Rows", TableTable.Defs.Columns.RowCount).MakeFixedWidth(FixedWidth.W052);
            Columns.Create("PR", TableTable.Defs.Columns.ParentRelationCount).MakeFixedWidth(FixedWidth.W052);
            Columns.Create("CR", TableTable.Defs.Columns.ChildRelationCount).MakeFixedWidth(FixedWidth.W052);
            Columns.Create("C", TableTable.Defs.Columns.ConstraintCount).MakeFixedWidth(FixedWidth.W052);
            AddViewSourceSortDescriptions();

            ColumnData = new TableColumnController(this);
            Parents = new TableParentRelationController(this);
            Children = new TableChildRelationController(this);
            Unique = new TableConstraintController<UniqueConstraint>(this);
            Foreign = new TableConstraintController<ForeignKeyConstraint>(this);
            Table.LoadTableData();
            DeleteCommand.Supported = CommandSupported.NoWithException;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called when the selected item on the associated data grid has changed.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            ColumnData.Update();
            Parents.Update();
            Children.Update();
            Unique.Update();
            Foreign.Update();
        }
        #endregion

        /************************************************************************/

        #region Private Methods

        private void AddViewSourceSortDescriptions()
        {
            // TODO
            //MainSource.SortDescriptions.Clear();
            //MainSource.SortDescriptions.Add(new SortDescription(TableTable.Defs.Columns.Name, ListSortDirection.Ascending));
        }
        #endregion
    }
}