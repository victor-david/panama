/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Controls;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Core.Database.SQLite;
using Restless.Toolkit.Core.Utility;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides configuration services for the application.
    /// </summary>
    public sealed class Config : KeyValueTableBase, INotifyPropertyChanged
    {
        #region Static singleton access and constructor
        /// <summary>
        /// Gets the singleton instance of this class
        /// </summary>
        public static Config Instance { get; } = new Config();

        private Config() : base(DatabaseController.Instance.GetTable<ConfigTable>())
        {
            TitleFilter = GetItem(null, nameof(TitleFilter)).Deserialize<TitleFilter>();
            PublisherFilter = GetItem(null, nameof(PublisherFilter)).Deserialize<PublisherFilter>();
            Colors = new ConfigColors();
            // TODO
            // This is applied at when config is first created and when the DateFormat property is changed by the user in settings.
            //Restless.Tools.Controls.Default.Format.Date = DateFormat;
        }
        #endregion

        /************************************************************************/

        #region Public fields
        /// <summary>
        /// Provides static values for the main navigation pane.
        /// </summary>
        public static class MainNavigation
        {
            /// <summary>
            /// Gets the minimum width of the main navigation pane.
            /// </summary>
            public const double MinWidth = 146.0;

            /// <summary>
            /// Gets the maximum width of the main navigation pane.
            /// </summary>
            public const double MaxWidth = 292.0;

            /// <summary>
            /// Gets the default width of the main navigation pane.
            /// </summary>
            public const double DefaultWidth = 150.0;
        }

        /// <summary>
        /// Provides static values for the main window
        /// </summary>
        public static class MainWindow
        {
            /// <summary>
            /// Gets the default width for the main window.
            /// </summary>
            public const int Width = 1420;

            /// <summary>
            /// Gets the default height for the main window.
            /// </summary>
            public const int Height = 860;

            /// <summary>
            /// Gets the minimum width for the main window.
            /// </summary>
            public const int MinWidth = 960;

            /// <summary>
            /// Gets the minimum height for the main window.
            /// </summary>
            public const int MinHeight = 760;
        }

        /// <summary>
        /// Provides static values for DataGrid
        /// </summary>
        public static class DataGrid
        {            
            /// <summary>
            /// Gets the minimum value for data grid row height.
            /// </summary>
            public const double MinRowHeight = 24;

            /// <summary>
            /// Gets the maximum value for data grid row height.
            /// </summary>
            public const double MaxRowHeight = 42;

            /// <summary>
            /// Gets the default value for data grid row height.
            /// </summary>
            public const int DefaultRowHeight = 24;

            /// <summary>
            /// Gets the minimum value for data grid alternation count.
            /// </summary>
            public const double MinAlternationCount = 2;

            /// <summary>
            /// Gets the maximum value for data grid alternation count.
            /// </summary>
            public const double MaxAlternationCount = 5;

            /// <summary>
            /// Gets the default value for data grid alternation count;
            /// </summary>
            public const int DefaultAlternationCount = 2;
        }

        /// <summary>
        /// Provides static values for miscellaneous properties.
        /// </summary>
        public static class Other
        {
            /// <summary>
            /// Gets the default folder for title root, versions, etc.
            /// </summary>
            public const string Folder = @"C:\";

            /// <summary>
            /// Gets the default value for grid splitter.
            /// </summary>
            public const double SplitterWidth = 684;

            /// <summary>
            /// Gets the default value for a submission document footer.
            /// </summary>
            public const string DocumentFooter = "Submissions to [publisher] - [author] - [month], [year]";
        }
        #endregion

        /************************************************************************/

        #region Navigator
        /// <summary>
        /// Gets or sets the width of the main navigation panel.
        /// </summary>
        public int MainNavigationWidth
        {
            get => GetItem((int)MainNavigation.DefaultWidth);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets whether the titles navigator is expanded.
        /// </summary>
        public bool NavTitlesExpander
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets whether the tools navigator is expanded.
        /// </summary>
        public bool NavToolsExpander
        {
            get => GetItem(false);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets whether the other navigator is expanded.
        /// </summary>
        public bool NavOtherExpander
        {
            get => GetItem(false);
            set => SetItem(value);
        }
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the selected configuration section.
        /// </summary>
        public int SelectedConfigSection
        {
            get => GetItem(1);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the date format for the application.
        /// </summary>
        public string DateFormat
        {
            get => GetItem("MMM dd, yyyy");
            set
            {
                SetItem(value);
                // TODO
                //Restless.Tools.Controls.Default.Format.Date = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the main window
        /// </summary>
        public int MainWindowWidth
        {
            get => GetItem(MainWindow.Width);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the height of the main window
        /// </summary>
        public int MainWindowHeight
        {
            get => GetItem(MainWindow.Height);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the state of the main window
        /// </summary>
        public WindowState MainWindowState
        {
            get => (WindowState)GetItem((int)WindowState.Normal);
            set => SetItem((int)value);
        }

        /// <summary>
        /// Gets or sets the alternation count for data grids.
        /// </summary>
        public int DataGridAlternationCount
        {
            get => GetItem(DataGrid.DefaultAlternationCount);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the row height used in various data grids.
        /// </summary>
        public int DataGridRowHeight
        {
            get => GetItem(DataGrid.DefaultRowHeight);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets the set of configuration colors.
        /// </summary>
        public ConfigColors Colors
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the folder for the export operation.
        /// </summary>
        public string FolderExport
        {
            get => GetItem(Other.Folder);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the folder for the MAPI operations.
        /// </summary>
        public string FolderMapi
        {
            get => GetItem(null);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the folder for submission documents.
        /// </summary>
        public string FolderSubmissionDocument
        {
            get => GetItem(Other.Folder);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the folder for submission message files.
        /// </summary>
        public string FolderSubmissionMessage
        {
            get => GetItem(Other.Folder);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the folder for submission documents.
        /// </summary>
        public string FolderSubmissionMessageAttachment
        {
            get => GetItem(Other.Folder);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the default folder for selecting a title version document.
        /// </summary>
        public string FolderTitleVersion
        {
            get => GetItem(Other.Folder);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the folder title root.
        /// </summary>
        public string FolderTitleRoot
        {
            get => GetItem(Other.Folder);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the plain text viewer file.
        /// </summary>
        public string TextViewerFile
        {
            get => GetItem(null);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the template file that is used when creating a new submission document.
        /// Styles from this document are copied into the new document.
        /// </summary>
        public string TemplateFile
        {
            get => GetItem(null);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the grid splitter location for the table grid
        /// </summary>
        /// <remarks>
        /// This is a hidden internal value, used to remember the grid position.
        /// </remarks>
        public GridLength LeftColumnTable
        {
            get => GetGridLength(Other.SplitterWidth);
            set => SetGridLength(value);
        }

        /// <summary>
        /// Gets or sets the grid splitter location for the title grid
        /// </summary>
        /// <remarks>
        /// This is a hidden internal value, used to remember the grid position.
        /// </remarks>
        public GridLength LeftColumnTitle
        {
            get => GetGridLength(Other.SplitterWidth);
            set => SetGridLength(value);
        }

        /// <summary>
        /// Gets or sets the grid splitter location for the publisher grid
        /// </summary>
        /// <remarks>
        /// This is a hidden internal value, used to remember the grid position.
        /// </remarks>
        public GridLength LeftColumnPublisher
        {
            get => GetGridLength(Other.SplitterWidth);
            set => SetGridLength(value);
        }

        /// <summary>
        /// Gets or sets the grid splitter location for the self publisher grid
        /// </summary>
        /// <remarks>
        /// This is a hidden internal value, used to remember the grid position.
        /// </remarks>
        public GridLength LeftColumnSelfPublisher
        {
            get => GetGridLength(Other.SplitterWidth);
            set => SetGridLength(value);
        }

        /// <summary>
        /// Gets or sets the grid splitter location for the submission grid
        /// </summary>
        /// <remarks>
        /// This is a hidden internal value, used to remember the grid position.
        /// </remarks>
        public GridLength LeftColumnSubmission
        {
            get => GetGridLength(Other.SplitterWidth);
            set => SetGridLength(value);
        }

        /// <summary>
        /// Gets the options for creating a submission document.
        /// </summary>
        public SubmissionDocumentOptions SubmissionDocOptions
        {
            get
            {
                var ops = new SubmissionDocumentOptions()
                {
                    Company = SubmissionDocCompany,
                    Header = SubmissionDocHeader,
                    Footer = SubmissionDocFooter,
                    Text = SubmissionDocBody,
                    HeaderPageNumbers = SubmissionDocHeaderPages,
                    FooterPageNumbers = SubmissionDocFooterPages
                };
                return ops;
            }
        }

        /// <summary>
        /// Gets or sets the company name to place into a new submission document.
        /// </summary>
        public string SubmissionDocCompany
        {
            get => GetItem(null);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the header text to place into a new submission document.
        /// </summary>
        public string SubmissionDocHeader
        {
            get => GetItem(null);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if page numbers will be included in the header.
        /// </summary>
        public bool SubmissionDocHeaderPages
        {
            get => GetItem(false);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the footer text to place into a new submission document.
        /// </summary>
        public string SubmissionDocFooter
        {
            get => GetItem(Other.DocumentFooter);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if page numbers will be included in the footer.
        /// </summary>
        public bool SubmissionDocFooterPages
        {
            get => GetItem(false);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the body text to place into a new submission document.
        /// </summary>
        public string SubmissionDocBody
        {
            get => GetItem(null);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets a boolean value that indicates if document internal dates (created, modified)
        /// should be syncronized with title written date and file system modified date
        /// during the meta-data update operation. Only Open XML documents are affected.
        /// </summary>
        public bool SyncDocumentInternalDates
        {
            get => GetItem(true);
        }

        /// <summary>
        /// Gets the title filter object which describes how to filter title rows.
        /// </summary>
        public TitleFilter TitleFilter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the publisher filter object which describes how to filter publisher rows.
        /// </summary>
        public PublisherFilter PublisherFilter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the filter value used on the submission tab.
        /// This value is matched against either the name of the publisher
        /// or the name of the response type. When this value starts with "-",
        /// it filters for submissions that are active (no response date)
        /// </summary>
        public string SubmissionFilter
        {
            get => GetItem(null);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the display filter value for submission messages.
        /// </summary>
        /// <remarks>
        /// The values correspond to the following:
        /// 0 = display all
        /// 1 = display only unassigned
        /// 7 = display last 7 days
        /// 14 = display last 14 days
        /// 21 = display last 21 days
        /// 30 = display last 30 days
        /// </remarks>
        public int SubmissionMessageDisplay
        {
            get => GetItem(0);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets a value that specifies folder exclusions used during orphan detection.
        /// Values are separated by semi-colon and if found within a folder name indicate that
        /// the folder is not considered when looking for orphans.
        /// </summary>
        public string OrphanExclusions
        {
            get => GetItem(null);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if title versions are grouped
        /// </summary>
        public bool GroupTitleVersion
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets a value that indicates if title author is visible.
        /// </summary>
        /// <remarks>
        /// If only one author is used, this configuration item can be set to false
        /// since there's no need to see / edit the title author.
        /// </remarks>
        public bool IsTitleAuthorVisible
        {
            get => GetItem(true);
            set => SetItem(value);
        }
        #endregion

        /************************************************************************/

        #region INotifyPropertyChanged
        /// <summary>
        /// Occurs when a property changes
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Saves the filter objects by serializing them into their rows.
        /// This method is called at shutdown.
        /// </summary>
        public void SaveFilterObjects()
        {
            SetItem(TitleFilter.Serialize(), nameof(TitleFilter));
            SetItem(PublisherFilter.Serialize(), nameof(PublisherFilter));
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the base class has changed a value on the data row.
        /// </summary>
        /// <param name="propertyId"></param>
        protected override void OnRowValueChanged(string propertyId)
        {
            switch (propertyId)
            {
                case nameof(DataGridRowHeight):
                case nameof(DataGridAlternationCount):
                case nameof(IsTitleAuthorVisible):
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyId));
                    break;
                default:
                    break;
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private GridLength GetGridLength(double defaultValue, [CallerMemberName] string id = null)
        {
            double value = GetItem(defaultValue, id);
            return new GridLength(value, GridUnitType.Pixel);
        }

        private void SetGridLength(GridLength value, [CallerMemberName] string id = null)
        {
            SetItem(value.Value, id);
        }
        #endregion
    }
}