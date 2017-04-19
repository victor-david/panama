using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using Restless.App.Panama.Configuration;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used to display information about the application tables.
    /// </summary>
    public class TableViewModel : DataGridViewModel<TableTable>
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
        #pragma warning disable 1591
        public TableViewModel()
        {
            DisplayName = Strings.CommandTable;
            MaxCreatable = 1;
            Columns.SetDefaultSort(Columns.Create("Name", TableTable.Defs.Columns.Name), ListSortDirection.Ascending);
            Columns.Create("Cols", TableTable.Defs.Columns.ColumnCount).MakeFixedWidth(FixedWidth.MediumNumeric);
            Columns.Create("Rows", TableTable.Defs.Columns.RowCount).MakeFixedWidth(FixedWidth.MediumNumeric);
            Columns.Create("PR", TableTable.Defs.Columns.ParentRelationCount).MakeFixedWidth(FixedWidth.MediumNumeric);
            Columns.Create("CR", TableTable.Defs.Columns.ChildRelationCount).MakeFixedWidth(FixedWidth.MediumNumeric);
            Columns.Create("C", TableTable.Defs.Columns.ConstraintCount).MakeFixedWidth(FixedWidth.MediumNumeric);
            AddViewSourceSortDescriptions();
            FilterPrompt = Strings.FilterPromptTable;

            ColumnData = new TableColumnController(this);
            Parents = new TableParentRelationController(this);
            Children = new TableChildRelationController(this);
            Unique = new TableConstraintController<UniqueConstraint>(this);
            Foreign = new TableConstraintController<ForeignKeyConstraint>(this);
            Table.LoadTableData();
            DeleteCommand.Supported = CommandSupported.NoWithException;
        }
        #pragma warning restore 1591

        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called when the filter text has changed to set the filter on the underlying data.
        /// </summary>
        /// <param name="text">The filter text.</param>
        protected override void OnFilterTextChanged(string text)
        {
            DataView.RowFilter = String.Format("{0} LIKE '%{1}%'", TableTable.Defs.Columns.Name, text);
        }

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
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(TableTable.Defs.Columns.Name, ListSortDirection.Ascending));
        }
        #endregion
    }
}