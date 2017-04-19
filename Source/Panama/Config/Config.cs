using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
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
        /// Gets the default width for the main window, 1420
        /// </summary>
        public const int MainWindowDefaultWidth = 1420;

        /// <summary>
        /// Gets the default width for the height window, 840
        /// </summary>
        public const int MainWindowDefaultHeight = 840;

        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a value that indicates if we should automatically switch to the submssion tab when creating a new submssion
        /// </summary>
        public bool AutoSwitchToSubmission
        {
            get { return GetBool("AutoSwitchToSubmission"); }
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
        /// Gets the date format for the application.
        /// </summary>
        public string DateFormat
        {
            get { return GetString("DateFormat"); }
        }

        /// <summary>
        /// Gets the folder for the export operation.
        /// </summary>
        public string FolderExport
        {
            get { return GetString("FolderExport"); }
        }

        /// <summary>
        /// Gets the folder for the MAPI operations.
        /// </summary>
        public string FolderMapi
        {
            get { return GetString("FolderMapi"); }
        }

        /// <summary>
        /// Gets the folder for submission documents.
        /// </summary>
        public string FolderSubmissionDocument
        {
            get { return GetString(ConfigTable.Defs.FieldIds.FolderSubmissionDocument); }
        }

        /// <summary>
        /// Gets the folder for submission documents.
        /// </summary>
        public string FolderSubmissionMessageAttachment
        {
            get { return GetString(ConfigTable.Defs.FieldIds.FolderSubmissionMessageAttachment); }
        }

        /// <summary>
        /// Gets the default folder for selecting a title version document.
        /// </summary>
        public string FolderTitleVersion
        {
            get { return GetString("FolderTitle"); }
        }

        /// <summary>
        /// Gets the folder title root.
        /// </summary>
        public string FolderTitleRoot
        {
            get { return GetString(ConfigTable.Defs.FieldIds.FolderTitleRoot); }
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
            get { return GetBool("SyncDocumentInternalDates"); }
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
        /// Gets or sets the width of the main window
        /// </summary>
        public int MainWindowWidth
        {
            get { return GetInteger("MainWindowWidth"); }
            set { SetRowValue("MainWindowWidth", value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the height of the main window
        /// </summary>
        public int MainWindowHeight
        {
            get { return GetInteger("MainWindowHeight"); }
            set { SetRowValue("MainWindowHeight", value.ToString()); }
        }

        /// <summary>
        /// Gets or sets the state of the main window
        /// </summary>
        public WindowState MainWindowState
        {
            get { return (WindowState)GetInteger("MainWindowState"); }
            set 
            {
                int stateValue = (int)value;
                SetRowValue("MainWindowState", stateValue.ToString()); 
            }
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
            SetRowValue("TitleFilter", TitleFilter.Serialize<TitleFilter>());
            SetRowValue("PublisherFilter", PublisherFilter.Serialize<PublisherFilter>());
        }
        #endregion

        /************************************************************************/
        
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
