/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Restless.Panama.ViewModel
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

        ///// <summary>
        ///// Gets the data grid columns.
        ///// </summary>
        //public DataGridColumnCollection Columns
        //{
        //    get;
        //}

        ///// <summary>
        ///// Gets the collection of menu items. The view binds to this collection so that VMs can manipulate menu items programatically
        ///// </summary>
        //public MenuItemCollection MenuItems
        //{
        //    get;
        //}
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AlertWindowViewModel"/> class.
        /// </summary>
        /// <param name="alerts">The alerts</param>
        public AlertWindowViewModel(ObservableCollection<AlertTable.RowObject> alerts)
        {
            Alerts = alerts ?? throw new ArgumentNullException(nameof(alerts));
            //Columns = new DataGridColumnCollection();
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

            //MenuItems = new MenuItemCollection();
            MenuItems.AddItem(Strings.CommandBrowseToUrlOrClick, Commands["OpenUrl"]).AddImageResource("ImageBrowseToUrlMenu");
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