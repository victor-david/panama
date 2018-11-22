using Restless.App.Panama.Collections;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Controls;
using Restless.Tools.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="View.AboutWindow"/>.
    /// </summary>
    public class AlertWindowViewModel : WindowViewModel
    {
        #region Private
        private AlertTable.RowObject selectedAlert;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a list of row objects that represents current alerts
        /// </summary>
        public ObservableCollection<AlertTable.RowObject> Alerts
        {
            get;
        }

        /// <summary>
        /// Gets or sets the selected alert object.
        /// </summary>
        public AlertTable.RowObject SelectedAlert
        {
            get => selectedAlert;
            set
            {
                if (SetProperty(ref selectedAlert, value))
                {
                }
            }
        }

        /// <summary>
        /// Gets the data grid columns.
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
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AlertWindowViewModel"/> class.
        /// </summary>
        /// <param name="owner">The window that owns this view model.</param>
        /// <param name="alerts">The alerts</param>
        public AlertWindowViewModel(Window owner, ObservableCollection<AlertTable.RowObject> alerts)
            :base(owner)
        {
            Alerts = alerts ?? throw new ArgumentNullException(nameof(alerts));
            Columns = new DataGridColumnCollection();
            var col = Columns.Create("Date", nameof(AlertTable.RowObject.Date)).MakeDate();
            Columns.SetDefaultSort(col, ListSortDirection.Ascending);
            Columns.Create("Title", nameof(AlertTable.RowObject.Title));
            Columns.Create("Url", nameof(AlertTable.RowObject.Url));

            Commands.Add("OpenUrl", OpenUrl, HasUrl);
            Commands.Add("Postpone1", Postpone1, IsAlertSelected);
            Commands.Add("Postpone3", Postpone3, IsAlertSelected);
            Commands.Add("Postpone5", Postpone5, IsAlertSelected);
            Commands.Add("Postpone7", Postpone7, IsAlertSelected);
            Commands.Add("Postpone10", Postpone10, IsAlertSelected);
            Commands.Add("Dismiss", Dismiss, IsAlertSelected);

            MenuItems = new MenuItemCollection();
            MenuItems.AddItem(Strings.CommandBrowseToUrlOrClick, Commands["OpenUrl"], "ImageBrowseToUrlMenu");
        }
        #endregion

        /************************************************************************/
        
        #region Private methods
        private bool IsAlertSelected(object parm)
        {
            return SelectedAlert != null;
        }

        private bool HasUrl(object parm)
        {
            return SelectedAlert != null && SelectedAlert.HasUrl;
        }

        private void OpenUrl(object parm)
        {
            if (SelectedAlert != null && !string.IsNullOrEmpty(SelectedAlert.Url))
            {
                OpenHelper.OpenWebSite(null, SelectedAlert.Url);
            }
        }

        private void Postpone1(object parm)
        {
            Postpone(1);
        }

        private void Postpone3(object parm)
        {
            Postpone(3);
        }

        private void Postpone5(object parm)
        {
            Postpone(5);
        }

        private void Postpone7(object parm)
        {
            Postpone(7);
        }

        private void Postpone10(object parm)
        {
            Postpone(10);
        }

        private void Postpone(int days)
        {
            if (SelectedAlert != null)
            {
                DateTime utc = DateTime.UtcNow.AddDays(days);
                SelectedAlert.Date = new DateTime(utc.Year, utc.Month, utc.Day);
                Alerts.Remove(SelectedAlert);
            }
        }

        private void Dismiss(object parm)
        {
            if (SelectedAlert != null)
            {
                SelectedAlert.Enabled = false;
                Alerts.Remove(SelectedAlert);
            }
        }
        #endregion
    }
}
