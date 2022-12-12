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
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

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
        private GridLength mainNavigationWidth;
        private string notificationMessage;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the navigator items
        /// </summary>
        public NavigatorItemCollection NavigatorItems
        {
            get;
        }

        /// <summary>
        /// Gets or sets the width of the main navigation pane.
        /// </summary>
        public GridLength MainNavigationWidth
        {
            get => mainNavigationWidth;
            set
            {
                if (SetProperty(ref mainNavigationWidth, value))
                {
                    Config.MainNavigationWidth = (int)value.Value;
                }
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
            Commands.Add("OpenAboutWindow", p => WindowFactory.About.Create().ShowDialog());
            Commands.Add("OpenSettingsWindow", p => WindowFactory.Settings.Create().ShowDialog());
            Commands.Add("OpenToolWindow", p => WindowFactory.Tool.Create().ShowDialog());

            Commands.Add("NavigateAuthor", p => NavigatorItems.Select<AuthorViewModel>());
            Commands.Add("NavigateCredential", p => NavigatorItems.Select<CredentialViewModel>());
            Commands.Add("NavigateLinkVerify", p => NavigatorItems.Select<LinkVerifyViewModel>());
            Commands.Add("NavigateSearch", p => NavigatorItems.Select<ToolSearchViewModel>());
            Commands.Add("NavigateTable", p => NavigatorItems.Select<TableViewModel>());
            Commands.Add("NavigateTag", p => NavigatorItems.Select<TagViewModel>());

            Commands.Add("Close", p => WindowOwner.Close());

            Commands.Add("ResetWindow", RunResetWindowCommand);
            Commands.Add("Save", RunSaveCommand);

            //Commands.Add("ToolConvert", p => NavigatorItems.Select<ToolConvertViewModel>(), CanRunToolConvertCommand);
            Commands.Add("ToolMessageSync", p => NavigatorItems.Select<ToolMessageSyncViewModel>());
            //Commands.Add("ToolScramble", p => NavigatorItems.Select<ToolScrambleViewModel>());

            MainNavigationWidth = new GridLength(Config.MainNavigationWidth, GridUnitType.Pixel);

            NavigatorItems = new NavigatorItemCollection(NavigationGroup.TotalNumberOfGroups);
            NavigatorItems.SelectedItemChanged += NavigatorItemsSelectedItemChanged;
            RegisterStandardNavigatorItems();
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
        #endregion

        /************************************************************************/

        #region Private methods (navigator)
        private void RegisterStandardNavigatorItems()
        {
            // NavigatorItems.Add<HomeViewModel>(NavigationGroup.Title, Strings.MenuItemSummary, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.ReportGeometryKey));
            NavigatorItems.Add<TitleViewModel>(NavigationGroup.Title, Strings.MenuItemTitles, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.TitleGeometryKey));
            if (Config.IsQueueFeatureEnabled)
            {
                NavigatorItems.Add<TitleQueueViewModel>(NavigationGroup.Title, Strings.MenuItemQueues, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.TitleQueueGeometryKey));
            }
            NavigatorItems.Add<PublisherViewModel>(NavigationGroup.Title, Strings.MenuItemPublishers, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.PublisherGeometryKey));
            NavigatorItems.Add<SelfPublisherViewModel>(NavigationGroup.Title, Strings.MenuItemSelfPublishers, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.PublisherGeometryKey));
            NavigatorItems.Add<SubmissionViewModel>(NavigationGroup.Title, Strings.MenuItemSubmissions, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.SubmissionGeometryKey));

            NavigatorItems.Add<AuthorViewModel>(NavigationGroup.Settings, Strings.MenuItemAuthors, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.UserGeometryKey));
            //NavigatorItems.Add<CredentialViewModel>(NavigationGroup.Settings, Strings.MenuItemCredentials, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.LockGeometryKey));
            NavigatorItems.Add<TagViewModel>(NavigationGroup.Settings, Strings.MenuItemTags, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.TagGeometryKey));

            NavigatorItems.Add<AlertViewModel>(NavigationGroup.Other, Strings.MenuItemAlerts, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.AlertGeometryKey));
            NavigatorItems.Add<UserNoteViewModel>(NavigationGroup.Other, Strings.MenuItemNotes, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.NoteGeometryKey));
            NavigatorItems.Add<LinkViewModel>(NavigationGroup.Other, Strings.MenuItemLinks, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.LinkGeometryKey));
            NavigatorItems.Add<StatisticsViewModel>(NavigationGroup.Other, Strings.MenuItemStatistics, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.StatisticGeometryKey));

            NavigatorItems.Add<TableViewModel>(NavigationGroup.OnlyMenu, Strings.MenuItemStatistics);
            NavigatorItems.Add<ToolSearchViewModel>(NavigationGroup.OnlyMenu, Strings.MenuItemSearch);
            NavigatorItems.Add<LinkVerifyViewModel>(NavigationGroup.OnlyMenu, Strings.MenuItemAddLink);
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
        private void RunSaveCommand(object parm)
        {
            viewModelCache.SignalSave();
            Config.Instance.SaveFilterObjects();
            DatabaseController.Instance.Save();
            NotificationMessage = "All data successfully saved to the database";
        }

        private void RunResetWindowCommand(object parm)
        {
            WindowOwner.Width = Config.MainWindow.DefaultWidth;
            WindowOwner.Height = Config.MainWindow.DefaultHeight;
            WindowOwner.Top = (SystemParameters.WorkArea.Height / 2) - (WindowOwner.Height / 2);
            WindowOwner.Left = (SystemParameters.WorkArea.Width / 2) - (WindowOwner.Width / 2);
            WindowOwner.WindowState = WindowState.Normal;
        }

        private bool CanRunToolConvertCommand(object o)
        {
#if DOCX
            return true;
#else
            return false;
#endif
        }
        #endregion
    }
}