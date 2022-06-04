using Restless.Panama.Utility;
using Restless.Toolkit.Controls;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides top level exception handling
    /// </summary>
    public class TopLevelException
    {
        #region Private
        private static TopLevelException instance;
        #endregion

        /************************************************************************/

        #region Setup / teardown
        public static void Initialize()
        {
            if (instance == null)
            {
                instance = new TopLevelException();
            }
        }

        private TopLevelException()
        {
            Application.Current.Dispatcher.UnhandledExceptionFilter += DispatcherUnhandledExceptionFilter;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
            Application.Current.DispatcherUnhandledException += CurrentDispatcherUnhandledException;
            //TaskScheduler.UnobservedTaskException += TaskSchedulerUnobservedTaskException;
        }

        private void Shutdown()
        {
            Application.Current.Dispatcher.UnhandledExceptionFilter -= DispatcherUnhandledExceptionFilter;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomainUnhandledException;
            Application.Current.DispatcherUnhandledException -= CurrentDispatcherUnhandledException;
            // TaskScheduler.UnobservedTaskException -= TaskSchedulerUnobservedTaskException;
        }
        #endregion

        /************************************************************************/

        #region Handlers
        private void DispatcherUnhandledExceptionFilter(object sender, DispatcherUnhandledExceptionFilterEventArgs e)
        {
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(nameof(CurrentDomainUnhandledException), e.ExceptionObject as Exception);
        }

        private void CurrentDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            HandleException(nameof(CurrentDispatcherUnhandledException), e.Exception);
        }

        private void TaskSchedulerUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            HandleException(nameof(TaskSchedulerUnobservedTaskException), e.Exception);
        }

        private void HandleException(string source, Exception exception)
        {
            Logger.Instance.LogException(source, exception);
            WindowFactory.Terminate.Create(exception);
            Shutdown();
            Environment.Exit(0);
        }
        #endregion
    }
}
