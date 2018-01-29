using Restless.App.Panama.Controls;
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
using SystemColors = System.Windows.Media.Colors;

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
            /// Provides default property values for DataGrid
            /// </summary>
            public static class DataGrid
            {
                /// <summary>
                /// Gets the default value for data grid row height.
                /// </summary>
                public const int RowHeight = 24;

                /// <summary>
                /// Gets the default value for data grid alternation count;
                /// </summary>
                public const int AlternationCount = 2;

                /// <summary>
                /// Gets the minimum value for data grid row height.
                /// </summary>
                public const int MinRowHeight = 24;

                /// <summary>
                /// Gets the maximum value for data grid row height.
                /// </summary>
                public const int MaxRowHeight = 42;

                /// <summary>
                /// Gets the minimum value for data grid alternation count.
                /// </summary>
                public const int MinAlternationCount = 2;

                /// <summary>
                /// Gets the maximum value for data grid alternation count.
                /// </summary>
                public const int MaxAlternationCount = 5;

            }

            /// <summary>
            /// Gets default settings for the main window
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
                public const int MinWidth = 840;

                /// <summary>
                /// Gets the minimum height for the main window.
                /// </summary>
                public const int MinHeight = 760;
            }

            /// <summary>
            /// Provides default property values for miscellaneous properties.
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
            get => GetItem(Default.MainWindow.Width);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the height of the main window
        /// </summary>
        public int MainWindowHeight
        {
            get => GetItem(Default.MainWindow.Height);
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


        public int DataGridAlternationCount
        {
            get => GetItem(Default.DataGrid.AlternationCount);
            set
            {
                SetItem(value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the row height used in various data grids.
        /// </summary>
        public int DataGridRowHeight
        {
            get => GetItem(Default.DataGrid.RowHeight);
            set => SetItem(value);
        }


        public ConfigColors Colors
        {
            get;
            private set;
        }

        ///// <summary>
        ///// Gets the color object used to show a publisher marked as goner.
        ///// </summary>
        //public ConfigColor ColorGonerPublisher
        //{
        //    get;
        //    private set;
        //}

        ///// <summary>
        ///// Gets the color object used to show a publisher in a submission period.
        ///// </summary>
        //public ConfigColor ColorPeriodPublisher
        //{
        //    get;
        //    private set;
        //}

        ///// <summary>
        ///// Gets the color object used to show a published title.
        ///// </summary>
        //public ConfigColor ColorPublishedTitle
        //{
        //    get;
        //    private set;
        //}
        
        ///// <summary>
        ///// Gets the color object used to show a submitted title.
        ///// </summary>
        //public ConfigColor ColorSubmittedTitle
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// Gets or sets the mode used to sort the color pallete.
        /// </summary>
        public ColorSortingMode ColorSortingMode
        {
            get
            {
                Enum.TryParse(GetItem(ColorSortingMode.Alpha.ToString()), out ColorSortingMode mode);
                return mode;
            }
            set
            {
                SetItem(value.ToString());
            }
        }



        /// <summary>
        /// Gets or sets the folder for the export operation.
        /// </summary>
        public string FolderExport
        {
            get => GetItem(Default.Other.Folder);
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
            get => GetItem(Default.Other.Folder);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the folder for submission documents.
        /// </summary>
        
        public string FolderSubmissionMessageAttachment
        {
            get => GetItem(Default.Other.Folder);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the default folder for selecting a title version document.
        /// </summary>
        public string FolderTitleVersion
        {
            get => GetItem(Default.Other.Folder);
            set => SetItem(value);
        }

        /// <summary>
        /// Gets or sets the folder title root.
        /// </summary>
        public string FolderTitleRoot
        {
            get => GetItem(Default.Other.Folder);
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
            get => GetGridLength(Default.Other.SplitterWidth);
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
            get => GetGridLength(Default.Other.SplitterWidth);
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
            get => GetGridLength(Default.Other.SplitterWidth);
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
            get => GetGridLength(Default.Other.SplitterWidth);
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
            get => GetItem(Default.Other.DocumentFooter);
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
            //ColorGonerPublisher = new ConfigColor(this, nameof(ColorGonerPublisher), Default.Colors.GonerPublisher);
            //ColorPeriodPublisher = new ConfigColor(this, nameof(ColorPeriodPublisher), Default.Colors.PeriodPublisher);
            //ColorPublishedTitle = new ConfigColor(this, nameof(ColorPublishedTitle), Default.Colors.PublishedTitle);
            //ColorSubmittedTitle = new ConfigColor(this, nameof(ColorSubmittedTitle), Default.Colors.SubmittedTitle);
            Colors = new ConfigColors();

        }

        #endregion

        /************************************************************************/

        #region Public methods
        ///// <summary>
        ///// Resets the configuration colors.
        ///// </summary>
        //public void ResetColors()
        //{
        //    ColorGonerPublisher.ResetToDefault();
        //    ColorPeriodPublisher.ResetToDefault();
        //    ColorPublishedTitle.ResetToDefault();
        //    ColorSubmittedTitle.ResetToDefault();
        //}

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

        //private Color? GetColor(Color? defaultValue, [CallerMemberName] string id = null)
        //{
        //    string value = GetValueFromRow(id, defaultValue);
        //    if (String.IsNullOrEmpty(value))
        //    {
        //        return null;
        //    }
        //    try
        //    {
        //        return (Color)ColorConverter.ConvertFromString(value);
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}

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

        //private void SetColor(Color? value, [CallerMemberName] string id = null)
        //{
        //    if (value.HasValue)
        //    {
        //        SetItem(value.ToString(), id);
        //    }
        //    else
        //    {
        //        SetItem(null, id);
        //    }
        //    OnPropertyChanged(id);
        //}

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

        /************************************************************************/

        #region ConfigColor class
        ///// <summary>
        ///// Represents a single configuration color.
        ///// </summary>
        //public class ConfigColor
        //{
        //    private Config owner;
        //    private string id;
        //    private Color? defaultValue;

        //    /// <summary>
        //    /// Gets or sets the color.
        //    /// </summary>
        //    public Color? Color
        //    {
        //        get => owner.GetColor(defaultValue, id);
        //        set => owner.SetColor(value, id);
        //    }

        //    /// <summary>
        //    /// Gets a boolean value that indicates if <see cref="Color"/> contains a color value.
        //    /// </summary>
        //    public bool HasValue
        //    {
        //        get => Color != null && Color.HasValue;
        //    }

        //    /// <summary>
        //    /// 
        //    /// </summary>
        //    /// <param name="owner">The owner</param>
        //    /// <param name="id">The id associated with this color.</param>
        //    /// <param name="defaultValue">The default value associated with this color.</param>
        //    public ConfigColor(Config owner, string id, Color? defaultValue)
        //    {
        //        this.owner = owner ?? throw new ArgumentNullException();
        //        this.id = id ?? throw new ArgumentNullException();
        //        this.defaultValue = defaultValue;
        //    }

        //    /// <summary>
        //    /// Resets this configuration color to ts default value.
        //    /// </summary>
        //    public void ResetToDefault()
        //    {
        //        Color = defaultValue;
        //    }

        //    /// <summary>
        //    /// Get the brush according to the specified Color.
        //    /// </summary>
        //    /// <returns>The brush, or null if <see cref="HasValue"/> is false.</returns>
        //    public SolidColorBrush GetBrush()
        //    {
        //        if (HasValue)
        //        {
        //            return new SolidColorBrush(Color.Value);
        //        }
        //        return null;
        //    }
        //}

        #endregion
    }
}
