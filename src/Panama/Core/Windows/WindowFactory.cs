/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.View;
using Restless.App.Panama.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace Restless.App.Panama.Core
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
                var window = new MainWindow()
                {
                    Owner = null, // this is a top level window
                    MinWidth = Core.Config.Default.MainWindow.MinWidth,
                    MinHeight = Core.Config.Default.MainWindow.MinHeight,
                    DataContext = MainWindowViewModel.Instance,
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
                var window = new AboutWindow()
                {
                    Owner = Application.Current.MainWindow,
                    DataContext = new AboutWindowViewModel(),
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
            /// <param name="title">The text to use for the window title.</param>
            /// <returns>The window</returns>
            public static PublisherSelectWindow Create(string title)
            {
                var window = new PublisherSelectWindow
                {
                    Title = title,
                    Owner = Application.Current.MainWindow,
                    DataContext = new PublisherSelectWindowViewModel(),
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/
        
        #region MessageSelect
        /// <summary>
        /// Provides static methods for creating a message select window.
        /// </summary>
        public static class MessageSelect
        {
            /// <summary>
            /// Creates an instance of MessageSelectWindow and its corresponding view model.
            /// </summary>
            /// <param name="title">The title of the window.</param>
            /// <param name="options">Options that affect the use and layout of the window.</param>
            /// <returns>The window</returns>
            [Obsolete("Use MessageFileSelect instead")]
            public static MessageSelectWindow Create(string title, MessageSelectOptions options)
            {
                var window = new MessageSelectWindow
                {
                    Title = title,
                    Owner = Application.Current.MainWindow,
                    DataContext = new MessageSelectWindowViewModel(options)
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
                var window = new MessageFileSelectWindow
                {
                    Title = title,
                    Owner = Application.Current.MainWindow,
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
                var window = new SubmissionDocumentSelectWindow()
                {
                    Owner = Application.Current.MainWindow,
                    DataContext = new SubmissionDocumentSelectWindowViewModel()
                };
                SetWindowOwner(window);
                SetTextFormattingMode(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/

        #region CommandTools
        /// <summary>
        /// Provides static methods for creating a command tools window.
        /// </summary>
        public static class CommandTools
        {
            /// <summary>
            /// Creates an instance of CommandToolsWindow and its corresponding view model.
            /// </summary>
            /// <param name="ops">The startup options.</param>
            /// <returns>The window</returns>
            public static CommandToolsWindow Create(StartupOptions ops)
            {
                var window = new CommandToolsWindow()
                {
                    Owner = null, // this is a top level window
                    DataContext = new CommandToolsWindowViewModel(ops)
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
                var window = new TitleVersionRenameWindow
                {
                    Title = Resources.Strings.WindowTitleVersionRename,
                    Owner = Application.Current.MainWindow,
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
                var window = new AlertWindow()
                {
                    Owner = Application.Current.MainWindow,
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