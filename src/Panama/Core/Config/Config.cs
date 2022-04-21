/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Core.Database.SQLite;
using Restless.Toolkit.Core.Utility;
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
            TitleFilter = GetItem(null, nameof(TitleFilter)).Deserialize<TitleRowFilter>();
            PublisherFilter = GetItem(null, nameof(PublisherFilter)).Deserialize<PublisherRowFilter>();
            SubmissionFilter = GetItem(null, nameof(SubmissionFilter)).Deserialize<SubmissionRowFilter>();
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
            public const int DefaultWidth = 1420;

            /// <summary>
            /// Gets the default height for the main window.
            /// </summary>
            public const int DefaultHeight = 860;

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
        /// Provides static values for the tools window
        /// </summary>
        public static class ToolWindow
        {
            /// <summary>
            /// Gets the default width for the tool window.
            /// </summary>
            public const int DefaultWidth = 856;

            /// <summary>
            /// Gets the default height for the tool window.
            /// </summary>
            public const int DefaultHeight = 526;

            /// <summary>
            /// Gets the minimum width for the tool window.
            /// </summary>
            public const int MinWidth = 560;

            /// <summary>
            /// Gets the minimum height for the tool window.
            /// </summary>
            public const int MinHeight = 392;
        }

        /// <summary>
        /// Provides static values for the startup tool window
        /// </summary>
        public static class StartupToolWindow
        {
            /// <summary>
            /// Gets the default width for the tool window.
            /// </summary>
            public const int DefaultWidth = 480;

            /// <summary>
            /// Gets the default height for the tool window.
            /// </summary>
            public const int DefaultHeight = 260;
        }

        /// <summary>
        /// Provides static values for grid detail
        /// </summary>
        public static class Grid
        {
            public const double MinAlertDetailWidth = 250;
            public const double MaxAlertDetailWidth = 460;
            public const double DefaultAlertDetailWidth = MinAlertDetailWidth;

            public const double MinAuthorDetailWidth = 250;
            public const double MaxAuthorDetailWidth = 460;
            public const double DefaultAuthorDetailWidth = MinAuthorDetailWidth;

            public const double MinCredentialDetailWidth = 302;
            public const double MaxCredentialDetailWidth = 582;
            public const double DefaultCredentialDetailWidth = MinCredentialDetailWidth;

            public const double MinLinkDetailWidth = 280;
            public const double MaxLinkDetailWidth = 480;
            public const double DefaultLinkDetailWidth = MinLinkDetailWidth;

            public const double MinNoteDetailWidth = 520;
            public const double MaxNoteDetailWidth = 720;
            public const double DefaultNoteDetailWidth = MinNoteDetailWidth;

            public const double MinPublisherDetailWidth = 390;
            public const double MaxPublisherDetailWidth = 520;
            public const double DefaultPublisherDetailWidth = MinPublisherDetailWidth;

            public const double MinSearchDetailWidth = 240;
            public const double MaxSearchDetailWidth = 520;
            public const double DefaultSearchDetailWidth = MinSearchDetailWidth;

            public const double MinSelfPublisherDetailWidth = 360;
            public const double MaxSelfPublisherDetailWidth = 520;
            public const double DefaultSelfPublisherDetailWidth = MinSelfPublisherDetailWidth;

            public const double MinSubmissionDetailWidth = 366;
            public const double MaxSubmissionDetailWidth = 620;
            public const double DefaultSubmissionDetailWidth = MinSubmissionDetailWidth;

            public const double MinTableDetailWidth = 302;
            public const double MaxTableDetailWidth = 716;
            public const double DefaultTableDetailWidth = MinTagDetailWidth;

            public const double MinTagDetailWidth = 302;
            public const double MaxTagDetailWidth = 582;
            public const double DefaultTagDetailWidth = MinTagDetailWidth;

            public const double MinTitleDetailWidth = 432;
            public const double MaxTitleDetailWidth = 560;
            public const double DefaultTitleDetailWidth = MinTitleDetailWidth;
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
        /// Gets or sets whether the settings navigator is expanded.
        /// </summary>
        public bool NavSettingsExpander
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

        #region Other expanders
        /// <summary>
        /// Gets or sets whether the statistic title expander is expanded.
        /// </summary>
        public bool IsStatisticTitleExpanded
        {
            get => GetItem(false);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets whether the statistic version expander is expanded.
        /// </summary>
        public bool IsStatisticVersionExpanded
        {
            get => GetItem(false);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets whether the statistic submission expander is expanded.
        /// </summary>
        public bool IsStatisticSubmissionExpanded
        {
            get => GetItem(false);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets whether the statistic publisher expander is expanded.
        /// </summary>
        public bool IsStatisticPublisherExpanded
        {
            get => GetItem(false);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets whether the statistic reply expander is expanded.
        /// </summary>
        public bool IsStatisticReplyExpanded
        {
            get => GetItem(false);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets whether the submission and response dates are expanded
        /// </summary>
        public bool IsSubmissionSubmittedDateExpanded
        {
            get => GetItem(false);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets whether the submission response date is expanded
        /// </summary>
        public bool IsSubmissionResponseDateExpanded
        {
            get => GetItem(false);
            set => SetItem(value);
        }
        #endregion

        /************************************************************************/

        #region Main Window
        /// <summary>
        /// Gets or sets the width of the main window
        /// </summary>
        public int MainWindowWidth
        {
            get => GetItem(MainWindow.DefaultWidth);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the height of the main window
        /// </summary>
        public int MainWindowHeight
        {
            get => GetItem(MainWindow.DefaultHeight);
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
        #endregion

        /************************************************************************/

        #region Tool Window
        /// <summary>
        /// Gets or sets the width of the tool window
        /// </summary>
        public int ToolWindowWidth
        {
            get => GetItem(ToolWindow.DefaultWidth);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the height of the tool window
        /// </summary>
        public int ToolWindowHeight
        {
            get => GetItem(ToolWindow.DefaultHeight);
            set => SetItem(value);
        }
        #endregion

        /************************************************************************/

        #region Data Grid
        /// <summary>
        /// Gets or sets the row height used in various data grids.
        /// </summary>
        public int DataGridRowHeight
        {
            get => GetItem(DataGrid.DefaultRowHeight);
            set => SetItem(value);
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
        /// Gets or sets the state of the title grid columns
        /// </summary>
        public string TitleGridColumnState
        {
            get => GetItem(null);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the state of the publisher grid columns
        /// </summary>
        public string PublisherGridColumnState
        {
            get => GetItem(null);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the state of the self publisher grid columns
        /// </summary>
        public string SelfPublisherGridColumnState
        {
            get => GetItem(null);
            set => SetItem(value);
        }
        #endregion

        /************************************************************************/

        #region Colors
        /// <summary>
        /// Gets the set of configuration colors.
        /// </summary>
        public ConfigColors Colors
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Folders
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
        #endregion

        /************************************************************************/

        #region Grid
        public bool AlertDetailExpanded
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        public double AlertDetailWidth
        {
            get => GetItem(Grid.DefaultAlertDetailWidth);
            set => SetItem(value);
        }

        public bool AuthorDetailExpanded
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        public double AuthorDetailWidth
        {
            get => GetItem(Grid.DefaultAuthorDetailWidth);
            set => SetItem(value);
        }

        public bool CredentialDetailExpanded
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        public double CredentialDetailWidth
        {
            get => GetItem(Grid.DefaultCredentialDetailWidth);
            set => SetItem(value);
        }

        public bool LinkDetailExpanded
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        public double LinkDetailWidth
        {
            get => GetItem(Grid.DefaultLinkDetailWidth);
            set => SetItem(value);
        }

        public bool NoteDetailExpanded
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        public double NoteDetailWidth
        {
            get => GetItem(Grid.DefaultNoteDetailWidth);
            set => SetItem(value);
        }

        public bool PublisherDetailExpanded
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        public double PublisherDetailWidth
        {
            get => GetItem(Grid.DefaultPublisherDetailWidth);
            set => SetItem(value);
        }

        public bool SearchDetailExpanded
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        public double SearchDetailWidth
        {
            get => GetItem(Grid.DefaultSearchDetailWidth);
            set => SetItem(value);
        }

        public bool SelfPublisherDetailExpanded
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        public double SelfPublisherDetailWidth
        {
            get => GetItem(Grid.DefaultSelfPublisherDetailWidth);
            set => SetItem(value);
        }

        public bool SubmissionDetailExpanded
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        public double SubmissionDetailWidth
        {
            get => GetItem(Grid.DefaultSubmissionDetailWidth);
            set => SetItem(value);
        }

        public bool TableDetailExpanded
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        public double TableDetailWidth
        {
            get => GetItem(Grid.DefaultTableDetailWidth);
            set => SetItem(value);
        }

        public bool TagDetailExpanded
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        public double TagDetailWidth
        {
            get => GetItem(Grid.DefaultTitleDetailWidth);
            set => SetItem(value);
        }

        public bool TitleDetailExpanded
        {
            get => GetItem(true);
            set => SetItem(value);
        }

        public double TitleDetailWidth
        {
            get => GetItem(Grid.DefaultTitleDetailWidth);
            set => SetItem(value);
        }
        #endregion

        /************************************************************************/

        #region Submission
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
        #endregion

        /************************************************************************/

        #region Filters
        /// <summary>
        /// Gets the title filter object which describes how to filter title rows.
        /// </summary>
        public TitleRowFilter TitleFilter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the publisher filter object which describes how to filter publisher rows.
        /// </summary>
        public PublisherRowFilter PublisherFilter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the submission filter object which describes how to filter submission rows.
        /// </summary>
        public SubmissionRowFilter SubmissionFilter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets a value that determines if the title list is filtered by the ready flag
        /// </summary>
        public bool SubmissionTitleReady
        {
            get => GetItem(false);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets a value that determines if the title list is filtered by the quick flag
        /// </summary>
        public bool SubmissionTitleFlagged
        {
            get => GetItem(false);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets a value that determines whether search
        /// results are filtered to those associated with a title version.
        /// </summary>
        public bool SearchVersionOnly
        {
            get => GetItem(false);
            set => SetItem(value);
        }
        #endregion

        /************************************************************************/

        #region Misc (selected config, text viewer, others)
        /// <summary>
        /// Gets or sets the selected configuration section.
        /// </summary>
        public int SelectedConfigSection
        {
            get => GetItem(1);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the selected tool section.
        /// </summary>
        public int SelectedToolSection
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
        /// Gets or sets the plain text viewer file.
        /// </summary>
        public string TextViewerFile
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
        /// the folder is ignored
        /// </summary>
        public string OrphanDirectoryExclusions
        {
            get => GetItem(null);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets a value that specifies file exclusions used during orphan detection.
        /// Values are separated by semi-colon and if found within a file name indicate that
        /// the file is ignored.
        /// </summary>
        public string OrphanFileExclusions
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
        /// </summary>
        public void SaveFilterObjects()
        {
            SetItem(TitleFilter.Serialize(), nameof(TitleFilter));
            SetItem(PublisherFilter.Serialize(), nameof(PublisherFilter));
            SetItem(SubmissionFilter.Serialize(), nameof(SubmissionFilter));
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