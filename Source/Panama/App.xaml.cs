using System;
using System.Windows;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Database;
using Restless.App.Panama.ViewModel;
using Restless.Tools.Utility;
using Restless.App.Panama.Resources;
using Restless.App.Panama.Database.Tables;

namespace Restless.App.Panama
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
                MessageBox.Show(string.Format(Strings.ApplicationFatalExceptionFormat, ex.Message), Strings.CaptionFatalError, MessageBoxButton.OK, MessageBoxImage.Error);
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
            TopLevelExceptionHandler.Initialize();
#endif
            Validations.ThrowIfNotWindows7();
            ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;
            StartupOptions ops = new StartupOptions(e.Args);
            DatabaseController.Instance.Init(ApplicationInfo.Instance.RootFolder, ops.DatabaseFileName);

            if (ops.IsAnyOperationRequested)
            {
                Window tools = WindowFactory.CommandTools.Create(ops);
                CommandToolsWindowViewModel vm = (CommandToolsWindowViewModel)tools.GetValue(WindowViewModel.ViewModelProperty);
                tools.Show();
                vm.PerformOperations();
            }
            else
            {
                Window main = WindowFactory.Main.Create();
                main.MinWidth = Config.Default.MainWindow.MinWidth;
                main.MinHeight = Config.Default.MainWindow.MinHeight;
                main.Width = Config.Instance.MainWindowWidth;
                main.Height = Config.Instance.MainWindowHeight;
                main.WindowState = Config.Instance.MainWindowState;
                main.Show();
                DisplayAlertsIf();
            }
        }

        private void DisplayAlertsIf()
        {
            var alerts = DatabaseController.Instance.GetTable<AlertTable>().GetReadyAlerts();
            if (alerts.Count > 0)
            {
                Window alert = WindowFactory.Alert.Create(alerts);
                alert.Show();
            }
        }
        #endregion
    }
}