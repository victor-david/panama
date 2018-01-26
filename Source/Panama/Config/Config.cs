using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Filter;
using Restless.Tools.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace Restless.App.Panama.Configuration
{
    /// <summary>
    /// Provides configuration services for the application.
    /// </summary>
    public sealed class Config : BindableBase
    {
        #region Private
        private static Config instance;
        private ConfigTable table;
        #endregion

        /************************************************************************/

        #region Public fields

        /// <summary>
        /// Provides static default values for properties
        /// </summary>
        public static class Default
        {
            /// <summary>
            /// Gets the default folder for title root, versions, etc.
            /// </summary>
            public const string Folder = @"C:\";

            /// <summary>
            /// Gets the default value for data grid row height.
            /// </summary>
            public const int DataGridRowHeight = 24;

            /// <summary>
            /// Gets the default value for grid splitter.
            /// </summary>
            public const double SplitterWidth = 684;

            /// <summary>
            /// Gets the default value for a submission document footer.
            /// </summary>
            public const string DocumentFooter = "Submissions to [publisher] - [author] - [month], [year]";

            /// <summary>
            /// Provides static default values for colors.
            /// </summary>
            public static class Color
            {
                /// <summary>
                /// Gets the default color for a publisher that is within its submission period.
                /// </summary>
                public const string PeriodPublisher = "#FFF5F5DC";

                /// <summary>
                /// Gets the default color for a title that is published.
                /// </summary>
                public const string PublishedTitle = "#FFD0FFC9";

                /// <summary>
                /// Gets the default color for a title that is currently submitted.
                /// </summary>
                public const string SubmittedTitle = "#FF2E8B57";
            }
        }

        /// <summary>
        /// Gets default settings for the main window
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
            public const int MinWidth = 840;

            /// <summary>
            /// Gets the minimum height for the main window.
            /// </summary>
            public const int MinHeight = 500;
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
            set => SetItem(value);
        }

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
            get => GetItem(WindowState.Normal);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the row height used in various data grids.
        /// </summary>
        public int DataGridRowHeight
        {
            get => GetItem(Default.DataGridRowHeight);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the color used to show a publisher in a submission period.
        /// </summary>
        public Color ColorPeriodPublisher
        {
            get => GetColor(Default.Color.PeriodPublisher);
            set => SetColor(value);
        }

        /// <summary>
        /// Gets or sets the published title color.
        /// </summary>
        public Color ColorPublishedTitle
        {
            get => GetColor(Default.Color.PublishedTitle);
            set => SetColor(value);
        }
        
        /// <summary>
        /// Gets the submitted title color.
        /// </summary>
        public Color ColorSubmittedTitle
        {
            get => GetColor(Default.Color.SubmittedTitle);
            set => SetColor(value);
        }

        /// <summary>
        /// Gets or sets the folder for the export operation.
        /// </summary>
        public string FolderExport
        {
            get => GetItem(Default.Folder);
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
            get => GetItem(Default.Folder);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the folder for submission documents.
        /// </summary>
        
        public string FolderSubmissionMessageAttachment
        {
            get => GetItem(Default.Folder);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the default folder for selecting a title version document.
        /// </summary>
        public string FolderTitleVersion
        {
            get => GetItem(Default.Folder);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the folder title root.
        /// </summary>
        public string FolderTitleRoot
        {
            get => GetItem(Default.Folder);
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
            get => GetGridLength(Default.SplitterWidth);
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
            get => GetGridLength(Default.SplitterWidth);
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
            get => GetGridLength(Default.SplitterWidth);
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
            get => GetGridLength(Default.SplitterWidth);
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
            get => GetItem(Default.DocumentFooter);
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
        #endregion

        /************************************************************************/

        #region Static singleton access and constructor
        /// <summary>
        /// Gets the singleton instance of this class
        /// </summary>
        public static Config Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Config();
                }
                return instance;
            }
        }

        private Config()
        {
            table = DatabaseController.Instance.GetTable<ConfigTable>();
            TitleFilter = GetValueFromRow(nameof(TitleFilter), null).Deserialize<TitleFilter>();
            PublisherFilter = GetValueFromRow(nameof(PublisherFilter), null).Deserialize<PublisherFilter>();
        }

        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Resets the configuration colors.
        /// </summary>
        public void ResetColors()
        {
            ColorPeriodPublisher = (Color)ColorConverter.ConvertFromString(Default.Color.PeriodPublisher);
            ColorPublishedTitle = (Color)ColorConverter.ConvertFromString(Default.Color.PublishedTitle);
            ColorSubmittedTitle = (Color)ColorConverter.ConvertFromString(Default.Color.SubmittedTitle);
        }

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

        #region Private methods

        private string GetItem(string defaultValue, [CallerMemberName] string id = null)
        {
            return GetValueFromRow(id, defaultValue);
        }

        private int GetItem(int defaultValue, [CallerMemberName] string id = null)
        {
            if (int.TryParse(GetValueFromRow(id, defaultValue), out int val))
            {
                return val;
            }
            return 0;
        }

        private Int64 GetItem(Int64 defaultValue, [CallerMemberName] string id = null)
        {
            if (Int64.TryParse(GetValueFromRow(id, defaultValue), out Int64 val))
            {
                return val;
            }
            return 0;
        }

        private double GetItem(double defaultValue, [CallerMemberName] string id = null)
        {
            if (double.TryParse(GetValueFromRow(id, defaultValue), out double val))
            {
                return val;
            }
            return 0;
        }

        private bool GetItem(bool defaultValue, [CallerMemberName] string id = null)
        {
            string val = GetValueFromRow(id, defaultValue);
            return (val.ToLower() == "true");
        }

        private WindowState GetItem(WindowState defaultValue, [CallerMemberName] string id = null)
        {
            int val = GetItem((int)defaultValue, id);
            return (WindowState)val;
        }

        private GridLength GetGridLength(double defaultValue, [CallerMemberName] string id = null)
        {
            double value = GetItem(defaultValue, id);
            return new GridLength(value, GridUnitType.Pixel);
        }

        private Color GetColor(string defaultValue, [CallerMemberName] string id = null)
        {
            Color color = new Color();
            string value = GetValueFromRow(id, defaultValue);
            Execution.TryCatchSwallow(() => color = (Color)ColorConverter.ConvertFromString(value));
            return color;
        }


        private void SetItem(string value, [CallerMemberName] string id = null)
        {
            SetRowValueIf(id, value);
        }

        private void SetItem(int value, [CallerMemberName] string id = null)
        {
            SetRowValueIf(id, value.ToString());
        }

        private void SetItem(Int64 value, [CallerMemberName] string id = null)
        {
            SetRowValueIf(id, value.ToString());
        }

        private void SetItem(bool value, [CallerMemberName] string id = null)
        {
            SetRowValueIf(id, value.ToString());
        }

        private void SetItem(WindowState value, [CallerMemberName] string id = null)
        {
            SetRowValueIf(id, ((int)value).ToString());
        }

        private void SetGridLength(GridLength value, [CallerMemberName] string id = null)
        {
            if (value != null)
            {
                SetRowValueIf(id, value.Value.ToString());
            }
        }

        private void SetColor(Color value, [CallerMemberName] string id = null)
        {
            SetItem(value.ToString(), id);
            OnPropertyChanged(id);
        }

        private string GetValueFromRow(string id, object defaultValue)
        {
            DataRow row = GetRow(id, defaultValue);
            return row[ConfigTable.Defs.Columns.Value].ToString();
        }

        private DataRow GetRow(string id, object defaultValue = null)
        {
            return table.GetConfigurationRow(id, defaultValue);
        }

        private void SetRowValueIf(string id, string value)
        {
            DataRow row = GetRow(id);
            string currentValue = row[ConfigTable.Defs.Columns.Value].ToString();
            if (currentValue != value)
            {
                row[ConfigTable.Defs.Columns.Value] = value;
            }
        }
        #endregion
    }
}
