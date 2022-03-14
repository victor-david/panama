/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Resources;
using Restless.Panama.Database.Core;
using Restless.Toolkit.Core.Database.SQLite;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Mvvm;
using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using Restless.Panama.Core;
using Restless.Toolkit.Controls;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Extends DataGridViewModelBase to provide common functionality for views that use DataGrid to display table rows. This class must be interited.
    /// </summary>
    /// <typeparam name="T">The table type derived from <see cref="TableBase"/></typeparam>
    public abstract class DataGridViewModel<T> : ApplicationViewModel where T : TableBase
    {
        #region Private
        private object selectedItem;
        private bool isCustomFilterOpen;
        private string filterText;
        #endregion

        /************************************************************************/

        #region Public fields / properties
        /// <summary>
        /// Gets the table object associated with this instance
        /// </summary>
        public T Table => DatabaseController.Instance.GetTable<T>();

        /// <summary>
        /// Gets the main data view, the one associated with the <see cref="TableBase"/> type used to create this class.
        /// </summary>
        public DataView MainView
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list view
        /// </summary>
        public ListCollectionView ListView
        {
            get;
        }

        /// <summary>
        /// Gets or sets the selected item of the DataGrid.
        /// </summary>
        public object SelectedItem
        {
            get => selectedItem;
            set
            {
                SetProperty(ref selectedItem, value);
                OnSelectedItemChanged();
            }
        }

        /// <summary>
        /// Gets the collection of columns
        /// </summary>
        public DataGridColumnCollection Columns
        {
            get;
        }

        /// <summary>
        /// Gets the collection of menu items
        /// </summary>
        public MenuItemCollection MenuItems
        {
            get;
        }

        /// <summary>
        /// Gets a command to add a new record to the data table
        /// </summary>
        public RelayCommand AddCommand
        {
            get;
        }

        /// <summary>
        /// Gets a command to delete a record from the data table
        /// </summary>
        public RelayCommand DeleteCommand
        {
            get;
        }

        /// <summary>
        /// Gets a command to clear the filter.
        /// </summary>
        public ICommand ClearFilterCommand
        {
            get;
        }

        /// <summary>
        /// Gets a command to toggle the value of <see cref="IsCustomFilterOpen"/>.
        /// </summary>
        public ICommand ToggleCustomFilterCommand
        {
            get;
        }

        /// <summary>
        /// Gets a command to open the selected row.
        /// </summary>
        public ICommand OpenRowCommand
        {
            get;
        }

        /// <summary>
        /// Gets or sets a boolean value that determines if the custom filter popup is open
        /// </summary>
        public bool IsCustomFilterOpen
        {
            get => isCustomFilterOpen;
            set => SetProperty(ref isCustomFilterOpen, value);
        }

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

        /// <summary>
        /// Gets a visibility value that determines if the edit control section is visible.
        /// </summary>
        public Visibility EditVisibility => (SelectedItem != null) ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets the current count of rows in source
        /// </summary>
        public int SourceCount => (MainView != null) ? MainView.Count : 0;

        /// <summary>
        /// Gets or sets the text to use as a filter for the main data grid.
        /// Derived classes should not use this property to filter.
        /// Use the text passed with the OnFilterTextChanged method which has been sanitized.
        /// </summary>
        public string FilterText
        {
            get => filterText;
            set
            {
                filterText = value;
                if (!string.IsNullOrEmpty(filterText))
                {
                    OnFilterTextChanged(filterText.Replace("'","''"));
                }
                else
                {
                    MainView.RowFilter = null;
                    OnFilterTextCleared();
                }
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets (or sets from a derived class) the prompt text that is shown in the filter input box
        /// </summary>
        public string FilterPrompt
        {
            get;
            protected set;
        }
        #endregion

        /************************************************************************/

        #region Protected properties
        /// <summary>
        /// Gets the style resource that aligns a data cell to the right.
        /// </summary>
        protected Style NumericRightCell
        {
            get => (Style)LocalResources.Get("TextBlockRight");
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewModel{T}"/> class.
        /// </summary>
        protected DataGridViewModel()
        {
            MainView = new DataView(Table);
            MainView.ListChanged += DataViewListChanged;

            ListView = new ListCollectionView(MainView);
            using (ListView.DeferRefresh())
            {
                ListView.CustomSort = new GenericComparer<DataRowView>((x, y) => OnDataRowCompare(x.Row, y.Row));
                ListView.Filter = (item) => item is DataRowView view && OnDataRowFilter(view.Row);
            }

            Columns = new DataGridColumnCollection();
            MenuItems = new MenuItemCollection();

            AddCommand = RelayCommand.Create(RunAddCommand, CanRunAddCommand);
            DeleteCommand = RelayCommand.Create(RunDeleteCommand, CanRunDeleteCommand);
            ClearFilterCommand = RelayCommand.Create(RunClearFilterCommand, CanRunClearFilterCommand);
            OpenRowCommand = RelayCommand.Create(RunOpenRowCommand, CanRunOpenRowCommand);
            ToggleCustomFilterCommand = RelayCommand.Create(p => IsCustomFilterOpen = !IsCustomFilterOpen);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Assigns the <see cref="MainView"/> property so that it is associated with the specified table.
        /// </summary>
        /// <param name="table">The data table.</param>
        /// <remarks>
        /// <para>
        /// When a <see cref="DataGridViewModel{T}"/> object is created, the <see cref="MainView"/> property is created
        /// from the <see cref="TableBase"/> type declaration that was used to create the class.
        /// </para>
        /// <para>
        /// A derived class can reassign the <see cref="MainView"/> property so that is created from another table.
        /// This functionality is used by controllers derived from <see cref="ControllerBase{VM,T}"/> to display
        /// child rows that are related to the main table.
        /// </para>
        /// </remarks>
        protected void AssignDataViewFrom(DataTable table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            MainView.ListChanged -= DataViewListChanged;
            MainView = new DataView(table);
            MainView.ListChanged += DataViewListChanged;
        }

        /// <summary>
        /// Forces <see cref="ListView"/> to refresh its sorting
        /// </summary>
        protected void ForceListViewSort()
        {
            using (ListView.DeferRefresh())
            {
                ListView.CustomSort = new GenericComparer<DataRowView>((x, y) => OnDataRowCompare(x.Row, y.Row));
            }
        }

        /// <summary>
        /// Override in a derived class to perform actions when the selected item changes. 
        /// Always call the base implementation to perform standard operations.
        /// </summary>
        protected virtual void OnSelectedItemChanged()
        {
            OnPropertyChanged(nameof(SelectedRow));
            OnPropertyChanged(nameof(EditVisibility));
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
        /// Shortcut method. Compares two data rows in a stringwise fashion.
        /// </summary>
        /// <param name="item1">The first data row.</param>
        /// <param name="item2">The second data row.</param>
        /// <param name="columnName">The name of the column to compare</param>
        /// <returns>A string comparison result (zero, 1, or -1)</returns>
        protected int DataRowCompareString(DataRow item1, DataRow item2, string columnName)
        {
            return string.Compare(item1[columnName].ToString(), item2[columnName].ToString());
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
        /// Override in a derived class to establish the filter on the main data grid.
        /// This method is only called if the text has some characters. 
        /// </summary>
        /// <param name="text">The filter text as it should be applied to the row filter. This value has been cleaned by the FilterText setter</param>
        protected virtual void OnFilterTextChanged(string text)
        {
        }

        /// <summary>
        /// Override in a derived class to perform actions when the filter text is cleared.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void OnFilterTextCleared()
        {
        }

        /// <summary>
        /// When overriden in a derived class, runs the command to add a new record.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void RunAddCommand()
        {
        }

        /// <summary>
        /// When overriden in a derived class, runs the command predicate to check if the add command can execute.
        /// The base implementation returns <b>false</b>.
        /// </summary>
        /// <returns>The base implememtation always returns <b>false</b>.</returns>
        protected virtual bool CanRunAddCommand()
        {
            return false;
        }

        /// <summary>
        /// When overriden in a derived class, runs the command to delete a record.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void RunDeleteCommand()
        {
        }

        /// <summary>
        /// When overriden in a derived class, runs the command predicate to check if the delete command can execute.
        /// The base implementation returns <b>false</b>.
        /// </summary>
        /// <returns>The base implememtation always returns <b>false</b>.</returns>
        protected virtual bool CanRunDeleteCommand()
        {
            return false;
        }

        /// <summary>
        /// Override in a derived class to provide open row logic. The base implementation does nothing.
        /// </summary>
        /// <param name="item">The data item</param>
        protected virtual void RunOpenRowCommand(object item)
        {
        }

        /// <summary>
        /// Runs the command predicate to check if the open row command can execute.
        /// </summary>
        /// <param name="item">The object passed to the command method.</param>
        /// <returns>The base implememtation returns true if <see cref="IsSelectedRowAccessible"/> is true; otherwise, false.</returns>
        /// <remarks>
        /// The base implementation returns true if <see cref="IsSelectedRowAccessible"/> is true; otherwise, false. 
        /// If you need different logic, override this method.
        /// </remarks>
        protected virtual bool CanRunOpenRowCommand(object item)
        {
            return IsSelectedRowAccessible;
        }

        /// <summary>
        /// Called when the <see cref="MainView"/> changes. Override in a derived class to recieve notification.
        /// The base implementation does nothing.
        /// </summary>
        /// <param name="e">The arguments received from the event.</param>
        protected virtual void OnDataViewListChanged(ListChangedEventArgs e)
        {
        }

        /// <summary>
        /// Attempts to open the path contained in the specified data row.
        /// </summary>
        /// <param name="row">The data row.</param>
        /// <param name="fileColumnName">The column name that contains the path to open.</param>
        /// <param name="pathRoot">A path root that should be applied if needed.</param>
        /// <param name="notFound">The action to run if the file does not exists.</param>
        protected void OpenFileRow(DataRow row, string fileColumnName, string pathRoot, Action<string> notFound)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            if (string.IsNullOrEmpty(fileColumnName))
            {
                throw new ArgumentNullException(nameof(fileColumnName));
            }

            if (notFound == null)
            {
                throw new ArgumentNullException(nameof(notFound));
            }

            string file = row[fileColumnName].ToString();
            if (!Path.IsPathRooted(file) && !string.IsNullOrWhiteSpace(pathRoot))
            {
                file = Path.Combine(pathRoot, file);
            }
            if (!File.Exists(file))
            {
                notFound(file);
                return;
            }
            OpenHelper.OpenFile(file);
        }

        /// <summary>
        /// Called when this object is disposing to perform cleanup operations.
        /// </summary>
        //protected override void OnDispose()
        //{
        //    DataView.ListChanged -= DataViewListChanged;
        //    base.OnDispose();
        //}
        #endregion

        /************************************************************************/

        #region Private Methods

        private void RunAddCommand(object o)
        {
            RunAddCommand();
        }

        private bool CanRunAddCommand(object o)
        {
            return CanRunAddCommand();
        }


        private void RunDeleteCommand(object o)
        {
            RunDeleteCommand();
        }

        private bool CanRunDeleteCommand(object o)
        {
            return CanRunDeleteCommand();
        }

        private void RunClearFilterCommand(object o)
        {
            FilterText = null;
            OnPropertyChanged(nameof(FilterText));
        }

        private bool CanRunClearFilterCommand(object o)
        {
            return !string.IsNullOrEmpty(FilterText);
        }

        private void DataViewListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged(nameof(SourceCount));
            OnDataViewListChanged(e);
        }
        #endregion
    }
}