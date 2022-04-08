/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System;
using System.Collections;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides common functionality for views that use DataGrid to display data.
    /// This class must be interited.
    /// <typeparamref name="T">The data type</typeparamref>
    /// </summary>
    public abstract class DataViewModel<T> : ApplicationViewModel where T : class
    {
        #region Private
        private int selectedCount;
        private IList selectedItems;
        private object selectedItem;
        private bool isCustomFilterOpen;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the list view
        /// </summary>
        public ListCollectionView ListView
        {
            get;
            private set;
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
        /// Gets the number of items selected
        /// </summary>
        public int SelectedCount
        {
            get => selectedCount;
            private set => SetProperty(ref selectedCount, value);
        }

        /// <summary>
        /// Sets the IList of selected items
        /// </summary>
        public IList SelectedItems
        {
            get => selectedItems;
            set
            {
                SetProperty(ref selectedItems, value);
                SelectedCount = selectedItems?.Count ?? 0;
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
        /// Gets a boolean value that determines if <see cref="AddCommand"/> is enabled.
        /// </summary>
        public virtual bool AddCommandEnabled => false;

        /// <summary>
        /// Gets a boolean value that determines if <see cref="DeleteCommand"/> is enabled.
        /// </summary>
        public virtual bool DeleteCommandEnabled => false;

        /// <summary>
        /// Gets a boolean value that determines if <see cref="ClearFilterCommand"/> is enabled.
        /// </summary>
        public virtual bool ClearFilterCommandEnabled => false;

        /// <summary>
        /// Gets a boolean value that determines if <see cref="OpenRowCommand"/> is enabled.
        /// </summary>
        public virtual bool OpenRowCommandEnabled => false;

        /// <summary>
        /// Gets or sets a boolean value that determines if the custom filter popup is open
        /// </summary>
        public bool IsCustomFilterOpen
        {
            get => isCustomFilterOpen;
            set => SetProperty(ref isCustomFilterOpen, value);
        }

        /// <summary>
        /// Gets a visibility value that determines if the edit control section is visible.
        /// </summary>
        public Visibility EditVisibility => (SelectedItem != null) ? Visibility.Visible : Visibility.Collapsed;

        public bool HaveItems => (ListView?.Count ?? 0) > 0;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataViewModel{T}"/> class.
        /// </summary>
        protected DataViewModel()
        {
            Columns = new DataGridColumnCollection();
            MenuItems = new MenuItemCollection();

            AddCommand = RelayCommand.Create(p => RunAddCommand(), p => AddCommandEnabled);
            DeleteCommand = RelayCommand.Create(p => RunDeleteCommand(), p => DeleteCommandEnabled);
            ClearFilterCommand = RelayCommand.Create(p => RunClearFilterCommand(), p => ClearFilterCommandEnabled);
            OpenRowCommand = RelayCommand.Create(p => RunOpenRowCommand(), p => OpenRowCommandEnabled);
            ToggleCustomFilterCommand = RelayCommand.Create(p => IsCustomFilterOpen = !IsCustomFilterOpen);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Initializes <see cref="ListView"/> with the specified IList
        /// </summary>
        /// <param name="list">The IList</param>
        protected void InitListView(IList list)
        {
            ListView = new ListCollectionView(list ?? throw new ArgumentNullException(nameof(list)));
            using (ListView.DeferRefresh())
            {
                ListView.CustomSort = new GenericComparer<T>((x, y) => OnDataRowCompare(x, y));
                ListView.Filter = (item) => item is T data && OnDataRowFilter(data);
            }
        }

        /// <summary>
        /// Forces <see cref="ListView"/> to refresh its sorting
        /// </summary>
        protected void ForceListViewSort()
        {
            using (ListView.DeferRefresh())
            {
                ListView.CustomSort = new GenericComparer<T>((x, y) => OnDataRowCompare(x, y));
            }
        }

        /// <summary>
        /// Override in a derived class to process update. Always call the base method
        /// to refresh <see cref="ListView"/> and <see cref="HaveItems"/>.
        /// </summary>
        protected override void OnUpdate()
        {
            base.OnUpdate();
            ListView?.Refresh();
            OnPropertyChanged(nameof(HaveItems));
        }

        /// <summary>
        /// Override in a derived class to perform actions when the selected item changes. 
        /// Always call the base implementation to perform standard operations.
        /// </summary>
        protected virtual void OnSelectedItemChanged()
        {
            OnPropertyChanged(nameof(EditVisibility));
        }

        /// <summary>
        /// Override in a derived class to filter <see cref="ListView"/>. The base implementation returns true.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if <paramref name="item"/> is included; otherwise, false.</returns>
        protected virtual bool OnDataRowFilter(T item)
        {
            return true;
        }

        /// <summary>
        /// Override in a derived class to compares two data objects.
        /// The base method returns zero.
        /// </summary>
        /// <param name="item1">The first data object</param>
        /// <param name="item2">The second data object</param>
        /// <returns>An integer value 0, 1, or -1</returns>
        protected virtual int OnDataRowCompare(T item1, T item2)
        {
            return 0;
        }

        /// <summary>
        /// When overriden in a derived class, runs the command to clear filters.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void RunClearFilterCommand()
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
        /// When overriden in a derived class, runs the command to delete a record.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void RunDeleteCommand()
        {
        }

        /// <summary>
        /// Override in a derived class to provide open row logic. The base implementation does nothing.
        /// </summary>
        protected virtual void RunOpenRowCommand()
        {
        }
        #endregion
    }
}