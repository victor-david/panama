/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.View;
using Restless.Panama.ViewModel;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides static methods for creating application windows.
    /// </summary>
    public static class WindowFactory
    {
        #region Main
        /// <summary>
        /// Provides static methods for creating the main application window.
        /// </summary>
        public static class Main
        {
            /// <summary>
            /// Creates an instance of MainWindow and its corresponding view model.
            /// </summary>
            /// <returns>The window</returns>
            public static MainWindow Create()
            {
                MainWindow window = new()
                {
                    Owner = null, // this is a top level window
                    MinWidth = Config.MainWindow.MinWidth,
                    MinHeight = Config.MainWindow.MinHeight,
                    Width = Config.Instance.MainWindowWidth,
                    Height = Config.Instance.MainWindowHeight,
                    WindowState = Config.Instance.MainWindowState,
                    DataContext = MainWindowViewModel.Instance,
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region Settings
        /// <summary>
        /// Provides static methods for creating the settings window
        /// </summary>
        public static class Settings
        {
            /// <summary>
            /// Creates an instance of SettingsWindow and its corresponding view model
            /// </summary>
            /// <returns>The window</returns>
            public static SettingsWindow Create()
            {
                SettingsWindow window = new()
                {
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    DataContext = new SettingsWindowViewModel()
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region Tool
        /// <summary>
        /// Provides static methods for creating the tool window
        /// </summary>
        public static class Tool
        {
            /// <summary>
            /// Creates an instance of ToolWindow and its corresponding view model
            /// </summary>
            /// <returns>The window</returns>
            public static ToolWindow Create()
            {
                ToolWindow window = new()
                {
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    MinHeight = Config.ToolWindow.MinHeight,
                    MinWidth = Config.ToolWindow.MinWidth,
                    Height = Config.Instance.ToolWindowHeight,
                    Width = Config.Instance.ToolWindowWidth,
                    DataContext = new ToolWindowViewModel()
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region About
        /// <summary>
        /// Provides static methods for creating the application's About window.
        /// </summary>
        public static class About
        {
            /// <summary>
            /// Creates an instance of AboutWindow and its corresponding view model.
            /// </summary>
            /// <returns>The window</returns>
            public static AboutWindow Create()
            {
                AboutWindow window = new()
                {
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    DataContext = new AboutWindowViewModel(),
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region TitleSelect
        /// <summary>
        /// Provides static methods for creating a title select window.
        /// </summary>
        public static class TitleSelect
        {
            /// <summary>
            /// Creates an instance of TitleSelectWindow and its corresponding view model.
            /// </summary>
            /// <returns>The window</returns>
            public static TitleSelectWindow Create()
            {
                TitleSelectWindow window = new()
                {
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    DataContext = new TitleSelectWindowViewModel(),
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region PublisherSelect
        /// <summary>
        /// Provides static methods for creating a publisher select window.
        /// </summary>
        public static class PublisherSelect
        {
            /// <summary>
            /// Creates an instance of PublisherSelectWindow and its corresponding view model.
            /// </summary>
            /// <returns>The window</returns>
            public static PublisherSelectWindow Create()
            {
                PublisherSelectWindow window = new()
                {
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    DataContext = new PublisherSelectWindowViewModel(),
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region SelfPublisherSelect
        /// <summary>
        /// Provides static methods for creating a self publisher select window.
        /// </summary>
        public static class SelfPublisherSelect
        {
            /// <summary>
            /// Creates an instance of PublisherSelectWindow and its corresponding view model.
            /// </summary>
            /// <param name="title">The text to use for the window title.</param>
            /// <returns>The window</returns>
            public static PublisherSelectWindow Create(string title)
            {
                PublisherSelectWindow window = new()
                {
                    Title = title,
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    DataContext = new SelfPublisherSelectWindowViewModel(),
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region MessageFileSelect
        /// <summary>
        /// Provides static methods for creating a message file select window.
        /// </summary>
        public static class MessageFileSelect
        {
            /// <summary>
            /// Creates an instance of MessageSelectWindow and its corresponding view model.
            /// </summary>
            /// <param name="title">The title of the window.</param>
            /// <param name="folder">The name of the folder in which to look for messages.</param>
            /// <returns>The window</returns>
            public static MessageFileSelectWindow Create(string title, string folder)
            {
                MessageFileSelectWindow window = new()
                {
                    Title = title,
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    DataContext = new MessageFileSelectWindowViewModel(folder),
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region SubmissionDocumentSelect
        /// <summary>
        /// Provides static methods for creating a submission document selection window.
        /// </summary>
        public static class SubmissionDocumentSelect
        {
            /// <summary>
            /// Creates an instance of SubmissionDocumentSelectWindow and its corresponding view model.
            /// </summary>
            /// <returns>The window</returns>
            public static SubmissionDocumentSelectWindow Create()
            {
                SubmissionDocumentSelectWindow window = new()
                {
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    DataContext = new SubmissionDocumentSelectWindowViewModel()
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region StartupTool
        /// <summary>
        /// Provides static methods for creating a startup tool window.
        /// </summary>
        public static class StartupTool
        {
            /// <summary>
            /// Creates an instance of StartupToolWindow and its corresponding view model.
            /// </summary>
            /// <returns>The window</returns>
            public static StartupToolWindow Create()
            {
                StartupToolWindow window = new()
                {
                    Owner = null, // this is a top level window
                    Width = Config.StartupToolWindow.DefaultWidth,
                    Height = Config.StartupToolWindow.DefaultHeight,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    DataContext = new StartupToolWindowViewModel()
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region TitleVersionRename
        /// <summary>
        /// Provides static methods for creating a title version rename window.
        /// </summary>
        public static class TitleVersionRename
        {
            /// <summary>
            /// Creates an instance of TitleVersionRenameWindow and its corresponding view model.
            /// </summary>
            /// <param name="titleId">The title id for the versions to rename.</param>
            /// <returns>The window</returns>
            public static TitleVersionRenameWindow Create(long titleId)
            {
                TitleVersionRenameWindow window = new()
                {
                    Title = Strings.WindowTitleVersionRename,
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    DataContext = new TitleVersionRenameWindowViewModel(titleId),

                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region Alert
        /// <summary>
        /// Provides static methods for creating the application's Alert window.
        /// </summary>
        public static class Alert
        {
            /// <summary>
            /// Creates an instance of AlertWindow and its corresponding view model.
            /// </summary>
            /// <param name="alerts">The list of alerts to display.</param>
            /// <returns>The window</returns>
            public static AlertWindow Create(ObservableCollection<AlertTable.RowObject> alerts)
            {
                AlertWindow window = new()
                {
                    Owner = Application.Current.MainWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    DataContext = new AlertWindowViewModel(alerts)
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private static void SetWindowOwner(Window window)
        {
            if (window.DataContext is IWindowOwner owner)
            {
                owner.WindowOwner = window;
            }
        }

        private static void SetTextFormattingMode(DependencyObject element)
        {
            TextOptions.SetTextFormattingMode(element, TextFormattingMode.Display);
        }
        #endregion
    }
}