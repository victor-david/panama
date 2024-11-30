using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System.Windows.Input;
using System.Windows.Markup;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Base class for application view models. This class must be interited.
    /// </summary>
    public abstract class ApplicationViewModel : ViewModelBase, INavigator
    {
        #region Private
        private bool isOperationInProgress;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the current Xml language. All windows bind (in style def) to this property
        /// </summary>
        public XmlLanguage CurrentXmlLanguage => LanguageManager.Instance.GetCurrentXmlLanguage();

        /// <summary>
        /// Gets the singletom instance of the application information object.
        /// </summary>
        public ApplicationInfo AppInfo => ApplicationInfo.Instance;

        /// <summary>
        /// Gets the singleton instance of the configuration object.
        /// </summary>
        public Config Config => Config.Instance;

        /// <summary>
        /// Gets or (from a derived class) sets a boolean value that indicates in an async operation is in progress
        /// </summary>
        public bool IsOperationInProgress
        {
            get => isOperationInProgress;
            protected set => SetProperty(ref isOperationInProgress, value);
        }
        #endregion

        #region Commands
        public ICommand ResetWindowCommand { get; }
        #endregion

        /************************************************************************/

        #region Tables
        /// <summary>
        /// Gets the link table
        /// </summary>
        protected LinkTable LinkTable => DatabaseController.Instance.GetTable<LinkTable>();

        /// <summary>
        /// Gets the orphan exclusion table
        /// </summary>
        protected OrphanExclusionTable OrphanExclusionTable => DatabaseController.Instance.GetTable<OrphanExclusionTable>();

        /// <summary>
        /// Gets the publisher table
        /// </summary>
        protected PublisherTable PublisherTable => DatabaseController.Instance.GetTable<PublisherTable>();

        /// <summary>
        /// Gets the response table
        /// </summary>
        protected ResponseTable ResponseTable => DatabaseController.Instance.GetTable<ResponseTable>();

        /// <summary>
        /// Gets the search table
        /// </summary>
        protected SearchTable SearchTable => DatabaseController.Instance.GetTable<SearchTable>();

        /// <summary>
        /// Gets the submission batch table
        /// </summary>
        protected SubmissionBatchTable SubmissionBatchTable => DatabaseController.Instance.GetTable<SubmissionBatchTable>();

        /// <summary>
        /// Gets the theme table
        /// </summary>
        protected ThemeTable ThemeTable => DatabaseController.Instance.GetTable<ThemeTable>();

        /// <summary>
        /// Gets the title table
        /// </summary>
        protected TitleTable TitleTable => DatabaseController.Instance.GetTable<TitleTable>();

        /// <summary>
        /// Gets the title version table
        /// </summary>
        protected TitleVersionTable TitleVersionTable => DatabaseController.Instance.GetTable<TitleVersionTable>();
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationViewModel"/> class.
        /// </summary>
        protected ApplicationViewModel()
        {
            ResetWindowCommand = RelayCommand.Create(p => RunResetWindowCommand());
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called in response to a signaled language change.
        /// When overriding, always call the base method to update <see cref="CurrentXmlLanguage"/>.
        /// </summary>
        protected override void OnLanguageChanged()
        {
            base.OnLanguageChanged();
            OnPropertyChanged(nameof(CurrentXmlLanguage));
        }

        /// <summary>
        /// Runs the reset window command. Override as needed, The base implementation does nothing.
        /// </summary>
        protected virtual void RunResetWindowCommand()
        {
        }
        #endregion
    }
}