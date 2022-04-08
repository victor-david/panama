/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Toolkit.Controls;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to display information about the application tables.
    /// </summary>
    public class TableViewModel : DataViewModel<DataTable>
    {
        #region Private
        private readonly ObservableCollection<DataTable> tables;
        private DataTable selectedTable;
        private int selectedSection;
        #endregion

        /************************************************************************/

        #region Properties
        public int SelectedSection
        {
            get => selectedSection;
            set => SetProperty(ref selectedSection, value);
        }

        /// <summary>
        /// Gets the selected table
        /// </summary>
        public DataTable SelectedTable
        {
            get => selectedTable;
            private set => SetProperty(ref selectedTable, value);
        }

        /// <summary>
        /// Gets the column controller.
        /// </summary>
        public TableColumnController ColumnData
        {
            get;
        }

        /// <summary>
        /// Gets the controller for parent relations.
        /// </summary>
        public TableRelationController Parents
        {
            get;
        }

        /// <summary>
        /// Gets the controller for child relations.
        /// </summary>
        public TableRelationController Children
        {
            get;
        }

        /// <summary>
        /// Gets the unique constraints controller.
        /// </summary>
        public TableConstraintController<UniqueConstraint> Unique
        {
            get;
        }

        /// <summary>
        /// Gets the FK constraints controller.
        /// </summary>
        public TableConstraintController<ForeignKeyConstraint> Foreign
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TableViewModel"/> class.
        /// </summary>
        public TableViewModel()
        {
            Columns.Create("Namespace", nameof(DataTable.Namespace))
                .MakeFixedWidth(FixedWidth.W096);

            Columns.SetDefaultSort(Columns.Create("Name", nameof(DataTable.TableName)), ListSortDirection.Ascending);

            Columns.Create("Cols", $"{nameof(DataTable.Columns)}.{nameof(DataTable.Columns.Count)}")
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W052);

            Columns.Create("Rows", $"{nameof(DataTable.Rows)}.{nameof(DataTable.Rows.Count)}")
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W052);

            Columns.Create("PR", $"{nameof(DataTable.ParentRelations)}.{nameof(DataTable.ParentRelations.Count)}")
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W052);

            Columns.Create("CR", $"{nameof(DataTable.ChildRelations)}.{nameof(DataTable.ChildRelations.Count)}")
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W052);

            Columns.Create("C", $"{nameof(DataTable.Constraints)}.{nameof(DataTable.Constraints.Count)}")
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W052);

            ColumnData = new TableColumnController(this);
            Parents = new TableRelationController(this, TableRelationController.ControllerType.Parent);
            Children = new TableRelationController(this, TableRelationController.ControllerType.Child);
            Unique = new TableConstraintController<UniqueConstraint>(this);
            Foreign = new TableConstraintController<ForeignKeyConstraint>(this);

            tables = new ObservableCollection<DataTable>();

            foreach (DataTable table in DatabaseController.Instance.DataSet.Tables.OfType<DataTable>())
            {
                tables.Add(table);
            }

            InitListView(tables);
            SelectedSection = 1;
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        protected override void OnActivated()
        {
            base.OnActivated();
            SelectedItem = null;
        }

        /// <summary>
        /// Called when the selected item on the associated data grid has changed.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedTable = SelectedItem as DataTable;
            ColumnData.Update();
            Parents.Update(SelectedTable?.ParentRelations);
            Children.Update(SelectedTable?.ChildRelations);
            Unique.Update();
            Foreign.Update();
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataTable item1, DataTable item2)
        {
            return string.Compare(item1.TableName, item2.TableName, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}