using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.View;
using Restless.App.Panama.ViewModel;

namespace Restless.App.Panama
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
                var window = new MainWindow();
                TextOptions.SetTextFormattingMode(window);
                var viewModel = new MainWindowViewModel(window);
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
                var window = new AboutWindow();
                TextOptions.SetTextFormattingMode(window);
                window.Owner = Application.Current.MainWindow;
                var viewModel = new AboutWindowViewModel(window);
                return window;
            }
        }
        #endregion

        /************************************************************************/
        
        #region PublisheSelect
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
                var window = new PublisherSelectWindow();
                window.Title = title;
                TextOptions.SetTextFormattingMode(window);
                window.Owner = Application.Current.MainWindow;
                var viewModel = new PublisherSelectWindowViewModel(window);
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
            public static MessageSelectWindow Create(string title, MessageSelectOptions options)
            {
                var window = new MessageSelectWindow();
                window.Title = title;
                TextOptions.SetTextFormattingMode(window);
                window.Owner = Application.Current.MainWindow;
                var viewModel = new MessageSelectWindowViewModel(window, options);
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
                var window = new SubmissionDocumentSelectWindow();
                TextOptions.SetTextFormattingMode(window);
                window.Owner = Application.Current.MainWindow;
                var viewModel = new SubmissionDocumentSelectWindowViewModel(window);
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
                var window = new CommandToolsWindow();
                TextOptions.SetTextFormattingMode(window);
                //window.Owner = Application.Current.MainWindow;
                var viewModel = new CommandToolsWindowViewModel(window, ops);
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
                    Title = Restless.App.Panama.Resources.Strings.WindowTitleVersionRename
                };
                TextOptions.SetTextFormattingMode(window);
                window.Owner = Application.Current.MainWindow;
                var viewModel = new TitleVersionRenameWindowViewModel(window, titleId);
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
                var window = new AlertWindow();
                TextOptions.SetTextFormattingMode(window);
                window.Owner = Application.Current.MainWindow;
                var viewModel = new AlertWindowViewModel(window, alerts);
                return window;
            }
        }
        #endregion
        /************************************************************************/

        #region Workspace
        /// <summary>
        /// Provides static methods for creating a workspace window.
        /// </summary>
        public static class Workspace
        {
            /// <summary>
            /// Creates an instance of WorkspaceWindow and its corresponding view model.
            /// </summary>
            /// <param name="title">The title of the window.</param>
            /// <param name="viewModel">The view model to associate with the window.</param>
            /// <returns>The window</returns>
            public static WorkspaceWindow Create(string title, WorkspaceViewModel viewModel)
            {
                var window = new WorkspaceWindow();
                window.Title = title;
                TextOptions.SetTextFormattingMode(window);
                window.Owner = Application.Current.MainWindow;
                //window.DataContext = viewModel;
                //window.DataContext = Application.Current.MainWindow.DataContext;
                window.Content = viewModel;
                return window;
            }
        }
        #endregion


        /************************************************************************/
        
        #region TextOptions (private)
        private static class TextOptions
        {
            public static void SetTextFormattingMode(DependencyObject element)
            {
                System.Windows.Media.TextOptions.SetTextFormattingMode(element, System.Windows.Media.TextFormattingMode.Display);
            }
        }
        #endregion
    }
}
