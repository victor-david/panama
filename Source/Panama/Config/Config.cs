using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Filter;
using Restless.Tools.Utility;

namespace Restless.App.Panama.Configuration
{
    /// <summary>
    /// Provides configuration services for the application.
    /// </summary>
    public class Config
    {
        #region Private
        private static Config instance;
        private ConfigTable table;
        // private DataRowCacheDictionary rowCache;
        private Dictionary<string, DataRow> rowCache;
        #endregion

        /************************************************************************/
        
        #region Public fields
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
        /// Gets the color used to show a publisher in a submission period.
        /// </summary>
        public Color ColorPeriodPublisher
        {
            get { return GetColor("ColorPeriodPublisher"); }
        }

        /// <summary>
        /// Gets the published title color.
        /// </summary>
        public Color ColorPublishedTitle
        {
            get { return GetColor("ColorPublishedTitle"); }
        }
        
        /// <summary>
        /// Gets the submitted title color.
        /// </summary>
        public Color ColorSubmittedTitle
        {
            get { return GetColor("ColorSubmittedTitle"); }
        }

        /// <summary>
        /// Gets the folder for the export operation.
        /// </summary>
        public string FolderExport
        {
            get => GetItem(null);
        }

        /// <summary>
        /// Gets the folder for the MAPI operations.
        /// </summary>
        public string FolderMapi
        {
            get => GetItem(null);
        }

        /// <summary>
        /// Gets the folder for submission documents.
        /// </summary>
        public string FolderSubmissionDocument
        {
            get => GetItem(null);
        }

        /// <summary>
        /// Gets the folder for submission documents.
        /// </summary>
        public string FolderSubmissionMessageAttachment
        {
            get => GetItem(null);
        }

        /// <summary>
        /// Gets the default folder for selecting a title version document.
        /// </summary>
        public string FolderTitleVersion
        {
            get => GetItem(null);
        }

        /// <summary>
        /// Gets the folder title root.
        /// </summary>
        public string FolderTitleRoot
        {
            get => GetItem(null);
        }

        /// <summary>
        /// Gets or sets the grid splitter location for the table grid
        /// </summary>
        /// <remarks>
        /// This is a hidden internal value, used to remember the grid position.
        /// </remarks>
        public GridLength LeftColumnTable
        {
            get { return GetGridLength("LeftColumnTable"); }
            set { SetRowValue("LeftColumnTable", value.Value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the grid splitter location for the title grid
        /// </summary>
        /// <remarks>
        /// This is a hidden internal value, used to remember the grid position.
        /// </remarks>
        public GridLength LeftColumnTitle
        {
            get { return GetGridLength("LeftColumnTitle"); }
            set { SetRowValue("LeftColumnTitle", value.Value.ToString());}
        }

        /// <summary>
        /// Gets or sets the grid splitter location for the publisher grid
        /// </summary>
        /// <remarks>
        /// This is a hidden internal value, used to remember the grid position.
        /// </remarks>
        public GridLength LeftColumnPublisher
        {
            get { return GetGridLength("LeftColumnPublisher"); }
            set { SetRowValue("LeftColumnPublisher", value.Value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the grid splitter location for the submission grid
        /// </summary>
        /// <remarks>
        /// This is a hidden internal value, used to remember the grid position.
        /// </remarks>
        public GridLength LeftColumnSubmission
        {
            get { return GetGridLength("LeftColumnSubmission"); }
            set { SetRowValue("LeftColumnSubmission", value.Value.ToString()); }
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
                    Company = GetString("SubDocumentCompany"),
                    Text = GetString("SubDocumentText"),
                    Header = GetString("SubDocumentHeader"),
                    Footer = GetString("SubDocumentFooter"),
                    HeaderPageNumbers = GetBool("SubDocumentHeaderPages"),
                    FooterPageNumbers= GetBool("SubDocumentFooterPages")
                };
                return ops;
            }
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
            rowCache = new Dictionary<string, DataRow>();
            TitleFilter = GetRowValue("TitleFilter").Deserialize<TitleFilter>();
            PublisherFilter = GetRowValue("PublisherFilter").Deserialize<PublisherFilter>();
            //table.AddConfigValueIf("SyncDocumentInternalDates", "During a meta-data update operation, updates document internal dates (created, modified) to be consistent with the title written date and the file system modified date, respectively. This only works on Open Xml documents.", "bool", "0", true);
        }

        #endregion

        /************************************************************************/
        
        #region Public methods
        /// <summary>
        /// Saves the filter objects by serializing them into their rows.
        /// This method is called at shutdown.
        /// </summary>
        public void SaveFilterObjects()
        {
            SetRowValue("TitleFilter", TitleFilter.Serialize());
            SetRowValue("PublisherFilter", PublisherFilter.Serialize());
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







        #region Private methods

        private string GetString(string id)
        {
            return GetRowValue(id);
        }

        private bool GetBool(string id)
        {
            string value = GetRowValue(id);
            return (value == "1") ? true : false;
        }

        private Color GetColor(string id)
        {
            Color color = new Color();
            string value = GetRowValue(id);
            Execution.TryCatchSwallow(() => color = (Color)ColorConverter.ConvertFromString(value));
            return color;
        }

        private GridLength GetGridLength(string id)
        {
            int value = GetInteger(id);
            return new GridLength(value, GridUnitType.Pixel);
        }

        private int GetInteger(string id)
        {
            string value = GetRowValue(id);
            int val;
            int.TryParse(value, out val);
            return val;
        }

        private string GetRowValue(string id)
        {
            DataRow row = GetRowFromCache(id);
            return row[ConfigTable.Defs.Columns.Value].ToString();
        }

        /// <summary>
        /// Sets a value on the ConfigTable.Defs.Columns.Value column of a config row
        /// </summary>
        /// <param name="id">The row id</param>
        /// <param name="value">The value</param>
        private void SetRowValue(string id, string value)
        {
            //SetRowValue(id, ConfigTable.Defs.Columns.Value, value);
            DataRow row = GetRowFromCache(id);
            if (row[ConfigTable.Defs.Columns.Value].ToString() != value)
            {
                row[ConfigTable.Defs.Columns.Value] = value;
            }
        }

        /// <summary>
        /// Sets a value on the specified column of a config row.
        /// </summary>
        /// <param name="id">The row id</param>
        /// <param name="columnName">The column name</param>
        /// <param name="value">The value</param>
        private void SetRowValue(string id, string columnName, object value)
        {
            DataRow row = GetRowFromCache(id);
            if (row[columnName].ToString() != value.ToString())
            {
                row[columnName] = value;
            }
        }

        private DataRow GetRowFromCache(string id)
        {
            if (rowCache.ContainsKey(id))
            {
                return rowCache[id];
            }
            DataRow[] rows = table.Select(String.Format("{0}='{1}'", ConfigTable.Defs.Columns.Id, id));
            if (rows.Length == 1)
            {
                rowCache.Add(id, rows[0]);
                return rowCache[id];
            }

            throw new IndexOutOfRangeException();
        }
        #endregion
    }
}
