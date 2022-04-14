/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.ViewModel;
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Resources;
using Restless.Panama.Utility;
using System;
using System.Windows;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Controls;

namespace Restless.Panama
{
    /// <summary>
    /// Main application class. Provides starup and shutdown logic.
    /// </summary>
    public partial class App : Application
    {
        #region Protected methods
        /// <summary>
        /// Called when the application is starting.
        /// </summary>
        /// <param name="e">The starup event args.</param>
        /// <remarks>
        /// This method sets up the database and creates the main application window.
        /// </remarks>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            try
            {
                RunApplication(e);
            }

            catch (Exception ex)
            {
                MessageWindow.ShowError(ex.Message, null, false);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Called when the application is exiting to save any pending database updates.
        /// </summary>
        /// <param name="e">The exit event args</param>
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Config.Instance.SaveFilterObjects();
            DatabaseController.Instance.Shutdown(saveTables: true);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        /// <summary>
        /// Called from OnStartup(e) separately so we can catch an assembly missing.
        /// OnStartup() runs, then the runtime does a JIT for this method which needs other assemblies.
        /// If something is missing, the try/catch in OnStartup() handles it gracefully.
        /// </summary>
        /// <param name="e">The same parameter passed to OnStartup(e)</param>
        private void RunApplication(StartupEventArgs e)
        {
#if !DEBUG
            TopLevelException.Initialize();
#endif
            // Validations.ThrowIfNotWindows7();
            ShutdownMode = ShutdownMode.OnMainWindowClose;
            StartupOptions ops = new(e.Args);
            DatabaseController.Instance.Init(RegistryManager.DatabaseDirectory);

            if (ops.IsAnyOperationRequested)
            {
                Window tools = WindowFactory.StartupTool.Create();
                tools.Show();
                (tools.DataContext as StartupToolWindowViewModel)?.PerformOperations(ops);
            }
            else
            {
                // These are applied one time only. They don't change.
                DataGridColumnExtensions.CenterAlignedDataGridColumnHeaderStyleKey = ResourceKeys.Style.CenteredDataGridColumnHeaderStyle;
                DataGridColumnExtensions.CenterAlignedDataGridCellStyleKey = ResourceKeys.Style.CenteredDataGridCellStyle;
                WindowFactory.Main.Create().Show();
                DisplayAlertsIf();
            }
        }

        private void DisplayAlertsIf()
        {
            if (DatabaseController.Instance.GetTable<AlertTable>().HaveAlertsReady())
            {
                WindowFactory.Alert.Create().Show();
            }
        }
        #endregion
    }
}