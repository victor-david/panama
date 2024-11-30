using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using IconKind = MahApps.Metro.IconPacks.PackIconMaterialKind;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// The ViewModel for the application's main window.
    /// </summary>
    public class MainWindowViewModel : ApplicationViewModel, IWindow
    {
        #region Private
        private const int ToolHeaderId = 10;
        private readonly ViewModelCache viewModelCache;
        private NavigatorItem selectedNavigatorItem;
        private ApplicationViewModel selectedViewModel;
        private string notificationMessage;
        //private bool haveToolItems;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the navigator items
        /// </summary>
        public Core.NavigatorItemCollection NavigatorItems { get; }

        public NavigatorItem SelectedNavigatorItem
        {
            get => selectedNavigatorItem;
            set
            {
                SetProperty(ref selectedNavigatorItem, value);
                NavigateTo(selectedNavigatorItem);
            }
        }

        /// <summary>
        /// Gets or sets the selected view model.
        /// </summary>
        public ApplicationViewModel SelectedViewModel
        {
            get => selectedViewModel;
            set
            {
                var prevSelect = selectedViewModel;
                if (SetProperty(ref selectedViewModel, value))
                {
                    ChangeViewModelActivationState(prevSelect, selectedViewModel);
                }
            }
        }

        /// <summary>
        /// Gets a notification message
        /// </summary>
        public string NotificationMessage
        {
            get => notificationMessage;
            private set
            {
                SetProperty(ref notificationMessage, value);
                // Set backing store to null. Fixes a small problem where the same message
                // won't display twice in a row because the property hasn't changed.
                notificationMessage = null;
            }
        }
        #endregion

        /************************************************************************/

        #region Commands
        public ICommand OpenAboutCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenToolsCommand { get; }
        public ICommand SaveAllCommand { get; }
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Gets the singleton instance of this class.
        /// </summary>
        public static MainWindowViewModel Instance { get; } = new MainWindowViewModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class.
        /// </summary>
        private MainWindowViewModel()
        {
            DisplayName = $"{AppInfo.Title} {AppInfo.VersionMajor}";
#if DEBUG
            DisplayName += " (DEBUG)";
#endif
            OpenAboutCommand = RelayCommand.Create(p => WindowFactory.About.Create().ShowDialog());
            OpenSettingsCommand = RelayCommand.Create(p => WindowFactory.Settings.Create().ShowDialog());
            OpenToolsCommand = RelayCommand.Create(p => WindowFactory.Tool.Create().ShowDialog());
            SaveAllCommand = RelayCommand.Create(p => RunSaveCommand());

            NavigatorItems = new Core.NavigatorItemCollection(Config.NavigatorHeader, 10, 8);

            RegisterNavigatorItems();
            viewModelCache = new ViewModelCache();
        }
        #endregion

        /************************************************************************/

        #region IWindow
        /// <summary>
        /// Gets or sets the window owner. Set in <see cref="WindowFactory"/>
        /// </summary>
        public Window Window { get; set; }

        /// <summary>
        /// Gets or sets a command to close the window. Set in <see cref="WindowFactory"/>
        /// </summary>
        public ICommand CloseWindowCommand { get; set; }

        /// <summary>
        /// Called when the window is closing. Established in <see cref="WindowFactory"/>
        /// </summary>
        /// <param name="e">The event args</param>
        public void OnWindowClosing(CancelEventArgs e)
        {
            viewModelCache.SignalClosing();
            Config.SaveMainWindow(Window);
        }

        /// <summary>
        /// Called when the window has closed. Established in <see cref="WindowFactory"/>
        /// </summary>
        public void OnWindowClosed()
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Creates a notification message that displays on the main status bar
        /// </summary>
        /// <param name="message">The message</param>
        public void CreateNotificationMessage(string message)
        {
            NotificationMessage = message;
        }

        /// <summary>
        /// Notifies all active view models of the specified type to update
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        public void NotifyUpdate<T>() where T : ApplicationViewModel
        {
            foreach (T item in viewModelCache.OfType<T>().Where(p => p.IsActivated))
            {
                item.Update();
            }
        }

        public void SynchronizeNavigatorHeaders()
        {
            NavigatorItems.Header = Config.NavigatorHeader;
        }

        /// <summary>
        /// Synchonizes the state of configuration with corresponding navigator items
        /// </summary>
        public void SynchronizeNavigatorVisibility()
        {
            NavigatorItems.SetVisibility<TitleQueueViewModel>(Config.IsTitleQueueVisible);
            NavigatorItems.SetVisibility<LinkVerifyViewModel>(Config.IsVerifyLinkEnabled);
            NavigatorItems.SetVisibility<ToolSearchViewModel>(Config.IsSearchEnabled);
            NavigatorItems.SetVisibility<ToolOrphanViewModel>(Config.IsOrphanEnabled);
            NavigatorItems.SetVisibility<TableViewModel>(Config.IsDevToolEnabled);

            bool have = Config.IsVerifyLinkEnabled || Config.IsSearchEnabled || Config.IsOrphanEnabled || Config.IsDevToolEnabled;
            NavigatorItems.SetHeaderVisibility(ToolHeaderId, have);
        }

        /// <summary>
        /// Synchronizes the queue title menu items in <see cref="TitleViewModel"/>
        /// if it has been created.
        /// </summary>
        public void SynchronizeTitleQueue()
        {
            viewModelCache.Get<TitleViewModel>()?.SynchronizeQueueTitleMenuItems();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void RunResetWindowCommand()
        {
            Config.ResetMainWindow(Window);
        }
        #endregion

        /************************************************************************/

        #region Private methods (navigator)
        private void RegisterNavigatorItems()
        {
            NavigatorItems.AddHeader(Header.Main);

            NavigatorItems.AddNavigator<TitleViewModel>(Header.Titles, IconKind.SubtitlesOutline);
            NavigatorItems.AddNavigator<TitleQueueViewModel>(Header.Queues, IconKind.TrayFull);
            NavigatorItems.AddNavigator<PublisherViewModel>(Header.Publishers, IconKind.MessageCheckOutline);
            NavigatorItems.AddNavigator<SelfPublisherViewModel>(Header.SelfPublishers, IconKind.MessageFlashOutline);
            NavigatorItems.AddNavigator<SubmissionViewModel>(Header.Submissions, IconKind.MessageReplyTextOutline);
            NavigatorItems.AddNavigator<PublishedViewModel>(Header.Published, IconKind.NewspaperVariantOutline);

            NavigatorItems.AddHeader(Header.Settings);

            NavigatorItems.AddNavigator<AuthorViewModel>(Header.Authors, IconKind.AccountOutline);
            NavigatorItems.AddNavigator<TagViewModel>(Header.Tags, IconKind.TagOutline);

            NavigatorItems.AddHeader(Header.Other);

            NavigatorItems.AddNavigator<AlertViewModel>(Header.Alerts, IconKind.TimerOutline);
            NavigatorItems.AddNavigator<UserNoteViewModel>(Header.Notes, IconKind.NoteTextOutline);
            NavigatorItems.AddNavigator<LinkViewModel>(Header.Links, IconKind.LinkVariant);
            NavigatorItems.AddNavigator<StatisticsViewModel>(Header.Statistics, IconKind.Numeric);

            NavigatorItems.AddHeader(Header.Tools, ToolHeaderId);

            NavigatorItems.AddNavigator<ToolOrphanViewModel>(Header.Orphan, IconKind.ClipboardSearchOutline);
            NavigatorItems.AddNavigator<ToolSearchViewModel>(Header.Search, IconKind.Magnify);
            NavigatorItems.AddNavigator<LinkVerifyViewModel>(Header.LinkVerify, IconKind.LinkVariant);
            NavigatorItems.AddNavigator<TableViewModel>(Header.Developer, IconKind.CodeBraces);

            SynchronizeNavigatorVisibility();
        }

        private void NavigateTo(NavigatorItem navItem)
        {
            if (navItem != null && navItem.TargetType.IsAssignableTo(typeof(ApplicationViewModel)))
            {
                SelectedViewModel = viewModelCache.GetByNavigationItem(navItem);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods (VM management)
        /// <summary>
        /// Changes the activation state of the specified view models.
        /// </summary>
        /// <param name="previous">The previously selected view model. Gets deactived.</param>
        /// <param name="current">The currently selected view model. Gets activated.</param>
        private void ChangeViewModelActivationState(ApplicationViewModel previous, ApplicationViewModel current)
        {
            /* Deactivate the previously selected view model */
            previous?.Deactivate();
            current?.Activate();
        }
        #endregion

        /************************************************************************/

        #region Private methods (other)
        private void RunSaveCommand()
        {
            viewModelCache.SignalSave();
            Config.Instance.SaveFilterObjects();
            DatabaseController.Instance.Save();
            CreateNotificationMessage(Confirm.DataSavedToDatabase);
        }
        #endregion
    }
}