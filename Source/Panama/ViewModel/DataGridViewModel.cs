﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Resources;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;
using System.IO;
using System.Diagnostics;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Extends DataGridViewModelBase to provide common functionality for views that use DataGrid to display table rows. This class must be interited.
    /// </summary>
    /// <typeparam name="T">The table type derived from <see cref="TableBase"/></typeparam>
    public abstract class DataGridViewModel<T> : DataGridViewModelBase where T: TableBase
    {
        #region Private Vars
        private string filterText;
        #endregion

        /************************************************************************/

        #region Public fields / properties
        /// <summary>
        /// Gets a command to add a new record to the data table
        /// </summary>
        public RelayCommand AddCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a command to delete a record from the data table
        /// </summary>
        public RelayCommand DeleteCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a command to clear the filter.
        /// </summary>
        public ICommand ClearFilterCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a command to open the selected row.
        /// </summary>
        public ICommand OpenRowCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the selected data row
        /// </summary>
        public DataRow SelectedRow
        {
            get
            {
                if (SelectedItem != null)
                {
                    return ((DataRowView)SelectedItem).Row;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the primary key value of the selected row, or null if none 
        /// (no selected row or no primary key column on the table)
        /// </summary>
        public object SelectedPrimaryKey
        {
            get
            {
                if (SelectedRow != null && Table.PrimaryKeyName != null)
                {
                    return SelectedRow[Table.PrimaryKeyName];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a visibility value that determines if the edit control section is visible.
        /// </summary>
        public Visibility EditVisibility
        {
            get
            {
                return (SelectedItem != null) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Gets the table object associated with this instance
        /// </summary>
        public T Table
        {
            get;
            private set;
        }


        /// <summary>
        /// Gets the current count of rows in source
        /// </summary>
        public int SourceCount
        {
            get
            {
                return (DataView != null) ? DataView.Count : 0;
            }
        }

        /// <summary>
        /// Gets the DataView created for this instance.
        /// </summary>
        public DataView DataView
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the text to use as a filter for the main data grid.
        /// Derived classes should not use this property to filter.
        /// Use the text passed with the OnFilterTextChanged method which has been sanitized.
        /// </summary>
        public string FilterText
        {
            get { return filterText; }
            set
            {
                filterText = value;
                if (!String.IsNullOrEmpty(filterText))
                {
                    OnFilterTextChanged(filterText.Replace("'","''"));
                }
                else
                {
                    DataView.RowFilter = null;
                }
                OnPropertyChanged("FilterText");
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
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        protected DataGridViewModel()
        {
            Table = DatabaseController.Instance.GetTable<T>();
            DataView = new System.Data.DataView(Table);
            DataView.ListChanged += new ListChangedEventHandler(DataViewListChanged);
            MainSource.Source = DataView;
            AddCommand = new RelayCommand(RunAddCommand, CanRunAddCommand);
            DeleteCommand = new RelayCommand(RunDeleteCommand, CanRunDeleteCommand);
            ClearFilterCommand = new RelayCommand(RunClearFilterCommand, CanRunClearFilterCommand);
            OpenRowCommand = new RelayCommand(RunOpenRowCommand, CanRunOpenRowCommand);
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Assigns the <see cref="DataView"/> property so that it is associated with the specified table.
        /// </summary>
        /// <param name="table">The data table.</param>
        /// <remarks>
        /// <para>
        /// When a <see cref="DataGridViewModel{T}"/> object is created, the <see cref="DataView"/> property is created
        /// from the <see cref="TableBase"/> type declaration that was used to create the class.
        /// </para>
        /// <para>
        /// A derived class can reassign the <see cref="DataView"/> property so that is created from another table.
        /// This functionality is used by controllers derived from <see cref="ControllerBase{VM,T}"/> to display
        /// child rows that are related to the main table.
        /// </para>
        /// </remarks>
        protected void AssignDataViewFrom(DataTable table)
        {
            Validations.ValidateNull(table, "AssignSourceFrom.Table");
            DataView.ListChanged -= DataViewListChanged;
            DataView = new DataView(table);
            DataView.ListChanged += DataViewListChanged;
            MainSource.Source = DataView;
        }

        /// <summary>
        /// Override in a derived class to perform actions when the selected item changes. 
        /// Always call the base implementation to perform standard operations.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            OnPropertyChanged("SelectedRow");
            OnPropertyChanged("EditVisibility");
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
        /// <returns>The base implememtation returns true if a row is selected; otherwise, false.</returns>
        /// <remarks>
        /// The base implementation returns true if a row is currently selected; otherwise, false. 
        /// If you need different logic, override this method.
        /// </remarks>
        protected virtual bool CanRunOpenRowCommand(object item)
        {
            return CanRunCommandIfRowSelected(item);
        }

        /// <summary>
        /// Called when the <see cref="DataView"/> changes. Override in a derived class to recieve notification.
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
            Validations.ValidateNull(row, "OpenFileRow.Row");
            Validations.ValidateNullEmpty(fileColumnName, "OpenFileRow.FileColumnName");
            Validations.ValidateNull(notFound, "OpenFileRow.NotFound");

            string file = row[fileColumnName].ToString();
            if (!Path.IsPathRooted(file) && !String.IsNullOrWhiteSpace(pathRoot))
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
        protected override void OnDispose()
        {
            DataView.ListChanged -= DataViewListChanged;
            base.OnDispose();
        }
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
            OnPropertyChanged("FilterText");
        }

        private bool CanRunClearFilterCommand(object o)
        {
            return !String.IsNullOrEmpty(FilterText);
        }

        private void DataViewListChanged(object sender, ListChangedEventArgs e)
        {
            OnPropertyChanged("SourceCount");
            OnDataViewListChanged(e);
        }
        #endregion
    }
}