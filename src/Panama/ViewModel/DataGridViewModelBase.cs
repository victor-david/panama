/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Toolkit.Controls;
using System.Windows.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Extends WorkspaceViewModel to provide base functionality for views that use DataGrid. This class must be interited.
    /// </summary>
    public abstract class DataGridViewModelBase: ApplicationViewModel
    {
        #region Private Vars
        private object selectedItem;
        #endregion

        /************************************************************************/

        #region Public fields / properties
        /// <summary>
        /// Gets the collection of columns. The view binds to this property so that columns can be manipulated from the VM
        /// </summary>
        public DataGridColumnCollection Columns
        {
            get;
        }

        /// <summary>
        /// Gets the collection of menu items. The view binds to this collection so that VMs can manipulate menu items programatically
        /// </summary>
        public MenuItemCollection MenuItems
        {
            get;
        }

        /// <summary>
        /// Gets the CollectionViewSource object. The view binds to this property
        /// </summary>
        public CollectionViewSource MainSource
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
        #endregion

        /************************************************************************/

        #region Protected properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridViewModelBase"/> class.
        /// </summary>
        protected DataGridViewModelBase()
        {
            Columns = new DataGridColumnCollection();
            MainSource = new CollectionViewSource();
            MenuItems = new MenuItemCollection();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Sets the selected item by starting a background thread and then dispatching
        /// from that thread to perform the set. Funky work around to get the row to
        /// highlight.
        /// </summary>
        /// <param name="item">The item</param>
        public void SetSelectedItem(object item)
        {
            // Kind of a kludgy work around.
            // If I set the SelectedItem directly, it does indeed
            // work (OnSelectedItemChanged() gets triggered, the
            // title edit appears, etc.) but the DataGridRow isn't
            // highlighted. (unless TitleView was already open)
            // If I start up a task and then dispatch the setting
            // of SelectedItem, the row does get highlighted.

            // this doesn't work
            //Application.Current.Dispatcher.BeginInvoke(new Action(() => SelectedItem = item));

            // TODO
            //TaskManager.Instance.ExecuteTask(AppTaskId.SelectedItemFunkyWorkaround, (token) =>
            //   {
            //       //System.Threading.Thread.Sleep(1);
            //       TaskManager.Instance.DispatchTask(() =>
            //           {
            //               SelectedItem = item;
            //           });
            //   }, null, null, false);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Override in a derived class to perform actions when the selected item changes.
        /// The base implementation does nothing.
        /// </summary>
        protected virtual void OnSelectedItemChanged()
        {
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        #endregion
    }
}