namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="View.AboutWindow"/>.
    /// </summary>
    public class AboutWindowViewModel : ApplicationViewModel
    {
        public string DatabaseLocation => Database.Core.DatabaseController.Instance.DatabaseRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutWindowViewModel"/> class.
        /// </summary>
        public AboutWindowViewModel()
        {
            DisplayName = $"About {AppInfo.Title}";
        }
    }
}