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
            NavigatorItems.AddHeader(Header.Main);

            NavigatorItems.AddNavigator<TitleViewModel>(Menu.Titles, IconKind.SubtitlesOutline);
            NavigatorItems.AddNavigator<TitleQueueViewModel>(Menu.Queues, IconKind.TrayFull);
            NavigatorItems.AddNavigator<PublisherViewModel>(Menu.Publishers, IconKind.MessageCheckOutline);
            NavigatorItems.AddNavigator<SelfPublisherViewModel>(Menu.SelfPublishers, IconKind.MessageFlashOutline);
            NavigatorItems.AddNavigator<SubmissionViewModel>(Menu.Submissions, IconKind.MessageReplyTextOutline);

            NavigatorItems.AddHeader(Header.Settings);

            NavigatorItems.AddNavigator<AuthorViewModel>(Menu.Authors, IconKind.AccountOutline);
            NavigatorItems.AddNavigator<TagViewModel>(Menu.Tags, IconKind.TagOutline);

            NavigatorItems.AddHeader(Header.Other);

            NavigatorItems.AddNavigator<AlertViewModel>(Menu.Alerts, IconKind.TimerOutline);
            NavigatorItems.AddNavigator<UserNoteViewModel>(Menu.Notes, IconKind.NoteTextOutline);
            NavigatorItems.AddNavigator<LinkViewModel>(Menu.Links, IconKind.LinkVariant);
            NavigatorItems.AddNavigator<StatisticsViewModel>(Menu.Statistics, IconKind.Numeric);

            NavigatorItems.AddHeader(Header.Tools, ToolHeaderId);

            NavigatorItems.AddNavigator<ToolOrphanViewModel>(Menu.OrphanFinder, IconKind.ClipboardSearchOutline);
            NavigatorItems.AddNavigator<ToolSearchViewModel>(Menu.Search, IconKind.Magnify);
            NavigatorItems.AddNavigator<LinkVerifyViewModel>(Menu.LinkVerify, IconKind.LinkVariant);
            NavigatorItems.AddNavigator<TableViewModel>(Menu.Developer, IconKind.CodeBraces);

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
            NotificationMessage = "All data successfully saved to the database";
        }
        #endregion
    }
}