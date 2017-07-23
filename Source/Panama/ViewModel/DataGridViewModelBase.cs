using System;
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
using System.Diagnostics;
using Restless.Tools.Threading;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Extends WorkspaceViewModel to provide base functionality for views that use DataGrid. This class must be interited.
    /// </summary>
    public abstract class DataGridViewModelBase: WorkspaceViewModel
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
            private set;
        }

        /// <summary>
        /// Gets the collection of menu items. The view binds to this collection so that VMs can manipulate menu items programatically
        /// </summary>
        public MenuItemCollection MenuItems
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the CollectionViewSource object. The view binds to this property
        /// </summary>
        public CollectionViewSource MainSource
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the selected item of the DataGrid.
        /// </summary>
        public object SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem = value;
                OnSelectedItemChanged();
                OnPropertyChanged("SelectedItem");
            }
        }
        #endregion

        /************************************************************************/

        #region Protected properties
        ///// <summary>
        ///// Provides the standard image size for a command
        ///// </summary>
        //protected const double VisualCommandImageSize = 20.0;
        ///// <summary>
        ///// Font size for a command
        ///// </summary>
        //protected const double VisualCommandFontSize = 10.5;
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        protected DataGridViewModelBase()
        {
            Columns = new DataGridColumnCollection();
            MainSource = new CollectionViewSource();
            MenuItems = new MenuItemCollection();
        }
        #pragma warning restore 1591
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
            TaskManager.Instance.ExecuteTask(AppTaskId.SelectedItemFunkyWorkaround, (token) =>
               {
                   //System.Threading.Thread.Sleep(1);
                   TaskManager.Instance.DispatchTask(() =>
                       {
                           SelectedItem = item;
                       });
               }, null, null, false);
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

        /// <summary>
        /// Returns true if a row is currently selected
        /// </summary>
        /// <param name="o">Not used, satisfies command interface</param>
        /// <returns>true if a row is currently selected; otherwise, false.</returns>
        protected bool CanRunCommandIfRowSelected(object o)
        {
            return (SelectedItem != null);
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        #endregion
    }
}