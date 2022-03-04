using Restless.Panama.Database.Core;
using Restless.Panama.Utility;
using System.Windows;

namespace Restless.Panama
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RegistryManager.Initialize();
            DatabaseController.Instance.Init(RegistryManager.DatabaseDirectory);
            MainWindow window = new();
            window.Show();

        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            DatabaseController.Instance.Shutdown(true);
        }
    }
}