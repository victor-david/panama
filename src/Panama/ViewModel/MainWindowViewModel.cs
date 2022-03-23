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
                System.Media.SystemSounds.Asterisk.Play();
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
            //Owner.Closing += new CancelEventHandler(MainWindowClosing);
            Commands.Add("OpenAboutWindow", p => WindowFactory.About.Create().ShowDialog());
            Commands.Add("OpenSettingsWindow", p => WindowFactory.Settings.Create().ShowDialog());
            Commands.Add("OpenToolWindow", p => WindowFactory.Tool.Create().ShowDialog());

            Commands.Add("NavigateAuthor", p => NavigatorItems.Select<AuthorViewModel>());
            Commands.Add("NavigateSearch", p => NavigatorItems.Select<ToolSearchViewModel>());
            Commands.Add("NavigateTag", p => NavigatorItems.Select<TagViewModel>());

            Commands.Add("Close", p => WindowOwner.Close());
            
            Commands.Add("Credential", p => NavigatorItems.Select<CredentialViewModel>());
            //Commands.Add("Alert", p => NavigatorItems.Select<AlertViewModel>());
            //Commands.Add("Link", p => NavigatorItems.Select<LinkViewModel>());
            //Commands.Add("Note", p => NavigatorItems.Select<UserNoteViewModel>());
            //Commands.Add("Publisher", p => NavigatorItems.Select<PublisherViewModel>());
            Commands.Add("ResetWindow", RunResetWindowCommand);
            //Commands.Add("Submission", p => NavigatorItems.Select<SubmissionViewModel>());
            Commands.Add("Save", Save);

            Commands.Add("Table", p => NavigatorItems.Select<TableViewModel>());

            //Commands.Add("ToolConvert", p => NavigatorItems.Select<ToolConvertViewModel>(), CanRunToolConvertCommand);
            Commands.Add("ToolMessageSync", p => NavigatorItems.Select<ToolMessageSyncViewModel>());
            //Commands.Add("ToolScramble", p => NavigatorItems.Select<ToolScrambleViewModel>());

            MainNavigationWidth = new GridLength(Config.MainNavigationWidth, GridUnitType.Pixel);

            NavigatorItems = new NavigatorItemCollection(NavigationGroup.TotalNumberOfGroups);
            NavigatorItems.SelectedItemChanged += NavigatorItemsSelectedItemChanged;
            RegisterStandardNavigatorItems();
            viewModelCache = new ViewModelCache();
#if DEBUG
            DisplayName = $"{AppInfo.Assembly.Title} {AppInfo.Assembly.VersionMajor} (DEBUG)";
#else
            DisplayName = $"{AppInfo.Assembly.Title} {AppInfo.Assembly.VersionMajor}";
#endif
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
        /// Notifies the workspace that a new record has been added that affects it
        /// </summary>
        /// <typeparam name="T">The type of workspace to notify</typeparam>
        public void NotifyWorkspaceOnRecordAdded<T>() where T : ApplicationViewModel
        {
            // TODO
            //var ws = GetFirstWorkspace<T>();
            //if (ws != null)
            //{
            //    ws.OnRecordAdded();
            //}
        }

        /// <summary>
        /// Creates if necessary and switches to the workspace specified by its type.
        /// </summary>
        /// <typeparam name="T">The type of workspace to switch to.</typeparam>
        /// <returns>The workspace</returns>
        public T SwitchToWorkspace<T>() where T : ApplicationViewModel
        {
            // TODO
            // The reason we don't use NavigatorItems.Select<T>() directly (which would work)
            // is to avoid the possibility that a VM allows more than one instance.
            // We want to go the first one. If there isn't a first one, we'll create.

            //var ws = GetFirstWorkspace<T>();
            //if (ws != null)
            //{
            //    SetActiveWorkspace(ws);
            //}
            //else
            //{
            //    Create<T>();
            //}

            //ICollectionView collectionView = CollectionViewSource.GetDefaultView(Workspaces);
            //if (collectionView != null)
            //{
            //    return collectionView.CurrentItem as T;
            //}
            return null;

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
            SetCancelIfTasksInProgress(e);

            if (!e.Cancel)
            {

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
            NavigatorItems.Add<TitleViewModel>(NavigationGroup.Title, Strings.MenuItemTitles, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.TitleGeometryKey));
            NavigatorItems.Add<PublisherViewModel>(NavigationGroup.Title, Strings.MenuItemPublishers, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.PublisherGeometryKey));
            NavigatorItems.Add<SelfPublisherViewModel>(NavigationGroup.Title, Strings.MenuItemSelfPublishers, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.PublisherGeometryKey));
            NavigatorItems.Add<SubmissionViewModel>(NavigationGroup.Title, Strings.MenuItemSubmissions, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.SubmissionGeometryKey));

            NavigatorItems.Add<AuthorViewModel>(NavigationGroup.Settings, Strings.MenuItemAuthors, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.UserGeometryKey));
            NavigatorItems.Add<TagViewModel>(NavigationGroup.Settings, Strings.MenuItemTags, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.TagGeometryKey));

            NavigatorItems.Add<AlertViewModel>(NavigationGroup.Other, Strings.MenuItemAlerts, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.AlertGeometryKey));
            NavigatorItems.Add<UserNoteViewModel>(NavigationGroup.Other, Strings.MenuItemNotes, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.NoteGeometryKey));
            NavigatorItems.Add<LinkViewModel>(NavigationGroup.Other, Strings.MenuItemLinks, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.LinkGeometryKey));
            NavigatorItems.Add<StatisticsViewModel>(NavigationGroup.Other, Strings.MenuItemStatistics, false, LocalResources.Get<Geometry>(ResourceKeys.Geometry.StatisticGeometryKey));

            NavigatorItems.Add<ToolSearchViewModel>(NavigationGroup.OnlyMenu, Strings.MenuItemStatistics); 
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
        private void Save(object o)
        {
            Config.Instance.SaveFilterObjects();
            DatabaseController.Instance.Save();
            NotificationMessage = "All data successfully saved to the database";
        }

        private void RunResetWindowCommand(object o)
        {
            WindowOwner.Width = Config.MainWindow.DefaultWidth;
            WindowOwner.Height = Config.MainWindow.DefaultHeight;
            WindowOwner.Top = (SystemParameters.WorkArea.Height / 2) - (WindowOwner.Height / 2);
            WindowOwner.Left = (SystemParameters.WorkArea.Width / 2) - (WindowOwner.Width / 2);
            WindowOwner.WindowState = WindowState.Normal;

            Config.ToolWindowWidth = Config.ToolWindow.DefaultWidth;
            Config.ToolWindowHeight = Config.ToolWindow.DefaultHeight;
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