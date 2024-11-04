/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System.Collections.Generic;
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
    public class MainWindowViewModel : WindowViewModel
    {
        #region Private
        private readonly ViewModelCache viewModelCache;
        private ApplicationViewModel selectedViewModel;
        private string notificationMessage;
        private bool haveToolItems;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the navigator items
        /// </summary>
        public NavigatorItemCollection NavigatorItems { get; }

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

        /// <summary>
        /// Gets the list of theme ids
        /// </summary>
        public static List<string> ThemeIds => ThemeManager.Themes;

        /// <summary>
        /// Gets a boolean value that indicates if there are any tools items visible
        /// </summary>
        public bool HaveToolItems
        {
            get => haveToolItems;
            set => SetProperty(ref haveToolItems, value);
        }
        #endregion

        /************************************************************************/

        #region Commands
        public ICommand NavigateLinkVerifyCommand { get; }
        public ICommand NavigateSearchCommand { get; }
        public ICommand OpenAboutCommand { get; }
        public ICommand OpenSettingsCommand { get; }
        public ICommand OpenToolsCommand { get; }
        public ICommand SwitchThemeCommand { get; }
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
            NavigateLinkVerifyCommand = RelayCommand.Create(p => NavigatorItems.Select<LinkVerifyViewModel>());
            NavigateSearchCommand = RelayCommand.Create(p => NavigatorItems.Select<ToolSearchViewModel>());
            OpenAboutCommand = RelayCommand.Create(p => WindowFactory.About.Create().ShowDialog());
            OpenSettingsCommand = RelayCommand.Create(p => WindowFactory.Settings.Create().ShowDialog());
            OpenToolsCommand = RelayCommand.Create(p => WindowFactory.Tool.Create().ShowDialog());
            SwitchThemeCommand = RelayCommand.Create(RunSwitchThemeCommand);
            SaveAllCommand = RelayCommand.Create(p => RunSaveCommand());

            NavigatorItems = new NavigatorItemCollection(NavigationGroup.TotalNumberOfGroups);
            NavigatorItems.SelectedItemChanged += NavigatorItemsSelectedItemChanged;

            RegisterNavigatorItems();
            viewModelCache = new ViewModelCache();
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
        /// Navigates to the specified view model
        /// </summary>
        /// <typeparam name="T">The type of view model</typeparam>
        public void NavigateTo<T>() where T : ApplicationViewModel
        {
            NavigatorItems.Select<T>();
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

        /// <summary>
        /// Synchonizes the state of configuration with corresponding navigator items
        /// </summary>
        public void SynchronizeNavigatorVisibility()
        {
            SetNavigatorItemVisibility<TitleQueueViewModel>(Config.IsTitleQueueVisible);
            SetNavigatorItemVisibility<LinkVerifyViewModel>(Config.IsVerifyLinkEnabled);
            SetNavigatorItemVisibility<ToolSearchViewModel>(Config.IsSearchEnabled);
            HaveToolItems = NavigatorItems.HaveVisibleItems(NavigationGroup.Tool);
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
        /// <summary>
        /// Called when the window is closing.
        /// </summary>
        /// <param name="e">Event args.</param>
        protected override void OnWindowClosing(CancelEventArgs e)
        {
            if (!e.Cancel)
            {
                viewModelCache.SignalClosing();
                Config.Instance.MainWindowWidth = (int)WindowOwner.Width;
                Config.Instance.MainWindowHeight = (int)WindowOwner.Height;
                if (WindowOwner.WindowState != WindowState.Minimized)
                {
                    Config.Instance.MainWindowState = WindowOwner.WindowState;
                }
            }
        }

        /// <inheritdoc/>
        protected override void RunResetWindowCommand()
        {
            WindowOwner.Width = Config.MainWindow.DefaultWidth;
            WindowOwner.Height = Config.MainWindow.DefaultHeight;
            WindowOwner.Top = (SystemParameters.WorkArea.Height / 2) - (WindowOwner.Height / 2);
            WindowOwner.Left = (SystemParameters.WorkArea.Width / 2) - (WindowOwner.Width / 2);
            WindowOwner.WindowState = WindowState.Normal;
        }
        #endregion

        /************************************************************************/

        #region Private methods (navigator)
        private void RegisterNavigatorItems()
        {
            // Group: title
            NavigatorItems.Add<TitleViewModel>(NavigationGroup.Title, Strings.MenuItemTitles, false, Icons.Get(IconKind.SubtitlesOutline));
            NavigatorItems.Add<TitleQueueViewModel>(NavigationGroup.Title, Strings.MenuItemQueues, false, Icons.Get(IconKind.TrayFull));
            NavigatorItems.Add<PublisherViewModel>(NavigationGroup.Title, Strings.MenuItemPublishers, false, Icons.Get(IconKind.MessageCheckOutline));
            NavigatorItems.Add<SelfPublisherViewModel>(NavigationGroup.Title, Strings.MenuItemSelfPublishers, false, Icons.Get(IconKind.MessageFlashOutline));
            NavigatorItems.Add<SubmissionViewModel>(NavigationGroup.Title, Strings.MenuItemSubmissions, false, Icons.Get(IconKind.MessageReplyTextOutline));

            // Group: Settings
            NavigatorItems.Add<AuthorViewModel>(NavigationGroup.Settings, Strings.MenuItemAuthors, false, Icons.Get(IconKind.Account));
            NavigatorItems.Add<TagViewModel>(NavigationGroup.Settings, Strings.MenuItemTags, false, Icons.Get(IconKind.TagOutline));

            // Group: Other
            NavigatorItems.Add<AlertViewModel>(NavigationGroup.Other, Strings.MenuItemAlerts, false, Icons.Get(IconKind.TimerOutline));
            NavigatorItems.Add<UserNoteViewModel>(NavigationGroup.Other, Strings.MenuItemNotes, false, Icons.Get(IconKind.NoteTextOutline));
            NavigatorItems.Add<LinkViewModel>(NavigationGroup.Other, Strings.MenuItemLinks, false, Icons.Get(IconKind.LinkVariant));
            NavigatorItems.Add<StatisticsViewModel>(NavigationGroup.Other, Strings.MenuItemStatistics, false, Icons.Get(IconKind.Numeric));

            // Group: Tool
            NavigatorItems.Add<ToolSearchViewModel>(NavigationGroup.Tool, Strings.MenuItemSearch, false, Icons.Get(IconKind.Magnify));
            NavigatorItems.Add<LinkVerifyViewModel>(NavigationGroup.Tool, Strings.MenuItemLinkVerify, false, Icons.Get(IconKind.LinkVariant));

            //NavigatorItems.Add<TableViewModel>(NavigationGroup.OnlyMenu, Strings.MenuItemStatistics);

            SynchronizeNavigatorVisibility();
        }

        private void SetNavigatorItemVisibility<T>(bool isItemVisible) where T : ApplicationViewModel
        {
            if (NavigatorItems.TryGet<T>() is NavigatorItem item)
            {
                item.IsItemVisible = isItemVisible;
            }
        }

        private void NavigatorItemsSelectedItemChanged(object sender, NavigatorItem navItem)
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

        private void RunSwitchThemeCommand(object parm)
        {
            if (parm is string themeId)
            {
                ThemeManager.SetTheme(themeId);
                Config.ThemeId = themeId;
            }
        }

        private void RunSaveCommand()
        {
            viewModelCache.SignalSave();
            Config.Instance.SaveFilterObjects();
            DatabaseController.Instance.Save();
            NotificationMessage = "All data successfully saved to the database";
        }
        #endregion
    }
}