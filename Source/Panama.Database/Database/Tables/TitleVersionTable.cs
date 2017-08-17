using Restless.Tools.Database.SQLite;
using Restless.Tools.OpenXml;
using Restless.Tools.Utility;
using System;
using System.Data;
using System.IO;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents the table that contains information of versions of titles.
    /// </summary>
    [System.ComponentModel.DesignerCategory("foo")]
    public class TitleVersionTable : TableBase
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Provides static definitions for table properties such as column names and relation names.
        /// </summary>
        public static class Defs
        {
            /// <summary>
            /// Specifies the name of this table.
            /// </summary>
            public const string TableName = "titleversion";

            /// <summary>
            /// Provides static column names for this table.
            /// </summary>
            public static class Columns
            {
                /// <summary>
                /// The name of the id column. This is the table's primary key.
                /// </summary>
                public const string Id = "id";

                /// <summary>
                /// The name of the title id column.
                /// </summary>
                public const string TitleId = "titleid";

                /// <summary>
                /// The name of the document type column.
                /// </summary>
                public const string DocType = "doctype";

                /// <summary>
                /// The name of the file name column.
                /// </summary>
                public const string FileName = "filename";

                /// <summary>
                /// The name of the note column.
                /// </summary>
                public const string Note = "note";

                /// <summary>
                /// The name of the updated column.
                /// </summary>
                public const string Updated = "updated";

                /// <summary>
                /// The name of the size column
                /// </summary>
                public const string Size = "size";

                /// <summary>
                /// The name of the version column
                /// </summary>
                public const string Version = "version";

                /// <summary>
                /// The name of the language id column
                /// </summary>
                public const string LangId = "langid";

                /// <summary>
                /// The name of the word count column
                /// </summary>
                public const string WordCount = "wordcount";
            }
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get { return Defs.Columns.Id; }
        }

        ///// <summary>
        ///// Gets the statistics object. This return value is recreated
        ///// every time the property is accessed
        ///// </summary>
        //public TitleVersionTableStats Stats
        //{
        //    get { return new TitleVersionTableStats(this); }
        //}
        #endregion

        /************************************************************************/
        
        #region Constructor
        #pragma warning disable 1591
        public TitleVersionTable() : base(DatabaseController.Instance, Defs.TableName)
        {
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Data collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, Defs.Columns.TitleId);
        }
        
        /// <summary>
        /// Gets the first version of the specified title
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <returns>The data row that contains the first version, or null if titleid has no versions</returns>
        public DataRow GetFirstVersion(Int64 titleId)
        {
            return GetFirstOrLastVersion(titleId, true);
        }

        /// <summary>
        /// Gets the last version of the specified title
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <returns>The data row that contains the last version, or null if titleid has no versions</returns>
        public DataRow GetLastVersion(Int64 titleId)
        {
            return GetFirstOrLastVersion(titleId, false);
        }

        /// <summary>
        /// Gets all versions for the specified title in ascending order of version
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <returns>An array of DataRow objects</returns>
        public DataRow[] GetAllVersions(Int64 titleId)
        {
            return Select(String.Format("{0}={1}", Defs.Columns.TitleId, titleId), String.Format("{0} ASC", Defs.Columns.Version));
        }

        /// <summary>
        /// Gets the DataRow object that contains the specified file name, or null if none.
        /// </summary>
        /// <param name="fileName">The file name (should be non-rooted)</param>
        /// <returns>The first DataRow found, or null if none found.</returns>
        public DataRow GetVersionWithFile(string fileName)
        {
            // TODO: It's possible to have two titles that point to the same file for one of their versions.
            fileName = fileName.Replace("'", "''");
            DataRow[] rows = Select(String.Format("{0}='{1}'", Defs.Columns.FileName, fileName));
            if (rows.Length > 0)
            {
                return rows[0];
            }
            return null;
        }

        /// <summary>
        /// Gets a boolean value that indicates if the specified file exists as a version record.
        /// </summary>
        /// <param name="fileName">The file name (should be non-rooted)</param>
        /// <returns>true if a record containing the title exists; otherwise, false.</returns>
        public bool VersionWithFileExists(string fileName)
        {
            return (GetVersionWithFile(fileName) != null);
        }

        /// <summary>
        /// Adds a new version for the specified title
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="filename">The file name (should be stripped of title root)</param>
        public void AddVersion(Int64 titleId, string filename)
        {
            Validations.ValidateNullEmpty(filename, "AddVersion.Filename");
            Int64 version = 1;
            DataRow lastVersionRow = GetLastVersion(titleId);
            if (lastVersionRow != null)
            {
                version = (Int64)lastVersionRow[Defs.Columns.Version];
                version++;
            }

            DataRow row = NewRow();
            row[Defs.Columns.TitleId] = titleId;
            // OnColumnChanged(e) method will update the associated fields: Updated, Size, and DocType
            row[Defs.Columns.FileName] = filename;
            row[Defs.Columns.Version] = version;
            row[Defs.Columns.LangId] = LanguageTable.Defs.Values.DefaultLanguageId;
            Rows.Add(row);
            Save();
        }

        /// <summary>
        /// Replaces the file specification for the specified title and version
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="version">The version number that's receiving the replacement</param>
        /// <param name="filename">The file name (should be stripped of title root)</param>
        public void ReplaceVersion(Int64 titleId, Int64 version, string filename)
        {
            Validations.ValidateNullEmpty(filename, "ReplaceVersion.Filename");
            DataRow row = GetVersion(titleId, version);
            if (row != null)
            {
                // OnColumnChanged(e) method will update the associated fields: Updated, Size, and DocType
                row[Defs.Columns.FileName] = filename;
                Save();
            }
        }

        /// <summary>
        /// Changes the version for the specified title
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="version">The current version</param>
        /// <param name="newVersion">The new vesion (must be one less or one more than version)</param>
        /// <remarks>
        /// <paramref name="newVersion"/> must be one more or one less than <paramref name="version"/>. If not, this method does nothing.
        /// </remarks>
        public void ChangeVersionNumber(Int64 titleId, Int64 version, Int64 newVersion)
        {
            // Make sure we're only changing by 1.
            if (Math.Abs(newVersion - version) != 1)
            {
                return;
            }

            DataRow versionRow = null;
            DataRow newVersionRow = null;

            DataRow[] rows = GetAllVersions(titleId);

            foreach (DataRow row in rows)
            {
                Int64 rowVersion = (Int64)row[Defs.Columns.Version];
                if (rowVersion == version) versionRow = row;
                if (rowVersion == newVersion) newVersionRow = row;
            }

            if (versionRow != null && newVersionRow != null)
            {
                versionRow[Defs.Columns.Version] = newVersion;
                newVersionRow[Defs.Columns.Version] = version;
            }
        }

        /// <summary>
        /// Removes the specified version from the specified title and renumbers the remaining versions.
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="version">The version to remove</param>
        public void RemoveVersion(Int64 titleId, Int64 version)
        {
            DataRow[] rows = GetAllVersions(titleId);
            foreach (DataRow row in rows)
            {
                Int64 rowVersion = (Int64)row[Defs.Columns.Version];
                if (rowVersion == version) row.Delete();
            }

            rows = GetAllVersions(titleId);

            Int64 versionRenumber = 1;
            foreach (DataRow row in rows)
            {
                row[Defs.Columns.Version] = versionRenumber;
                versionRenumber++;
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Gets the DDL needed to create this table.
        /// </summary>
        /// <returns>A SQL string that describes how to create this table.</returns>
        protected override string GetDdl()
        {
            return Resources.Create.TitleVersion;
        }

        /// <summary>
        /// Sets extended properties on certain columns. See the base implemntation <see cref="TableBase.SetColumnProperties"/> for more information.
        /// </summary>
        protected override void SetColumnProperties()
        {
            SetColumnProperty(Columns[Defs.Columns.Id], DataColumnPropertyKey.ExcludeFromInsert, DataColumnPropertyKey.ExcludeFromUpdate, DataColumnPropertyKey.ReceiveInsertedId);
        }

        /// <summary>
        /// Called when a value of a column has changed, raises the <see cref="DataTable.ColumnChanged"/> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        /// <remarks>
        /// This method watches for changes on the <see cref="Defs.Columns.FileName"/> property and updates the 
        /// <see cref="Defs.Columns.Size"/>,
        /// <see cref="Defs.Columns.Updated"/> and
        /// <see cref="Defs.Columns.WordCount"/> columns accordingly.
        /// </remarks>
        protected override void OnColumnChanged(DataColumnChangeEventArgs e)
        {
            base.OnColumnChanged(e);
            if (e.Column.ColumnName == Defs.Columns.FileName)
            {
                string fullPath = Path.Combine(Controller.GetTable<ConfigTable>().GetRowValue(ConfigTable.Defs.FieldIds.FolderTitleRoot), e.ProposedValue.ToString());
                var info = new FileInfo(fullPath);
                if (info.Exists)
                {
                    e.Row[Defs.Columns.Size] = info.Length;
                    e.Row[Defs.Columns.Updated] = info.LastWriteTimeUtc;
                    e.Row[Defs.Columns.WordCount] = OpenXmlDocument.Reader.TryGetWordCount(fullPath);
                }
                else
                {
                    e.Row[Defs.Columns.Size] = 0;;
                    e.Row[Defs.Columns.Updated] = DateTime.UtcNow;
                    e.Row[Defs.Columns.WordCount] = 0;
                }
                e.Row[Defs.Columns.DocType] = Controller.GetTable<DocumentTypeTable>().GetDocTypeFromFileName(e.ProposedValue.ToString());
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods

        /// <summary>
        /// Gets the first or last version for the specified title
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="getFirst">true to get the first version, false to get the last version</param>
        /// <returns>A DataRow object with the specified first or last version; null if the title has no versions.</returns>
        private DataRow GetFirstOrLastVersion(Int64 titleId, bool getFirst)
        {
            string order = (getFirst) ? "ASC" : "DESC";
            DataRow[] rows = Select(String.Format("{0}={1}", Defs.Columns.TitleId, titleId), String.Format("{0} {1}", Defs.Columns.Version, order));
            if (rows.Length > 0)
            {
                return rows[0];
            }
            return null;
        }


        /// <summary>
        /// Gets the specified version for the specified title, or null if it doesn't exist
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="version">The version number to get</param>
        /// <returns>A DataRow object with the specified version for the title, or null if it doesn't exist.</returns>
        private DataRow GetVersion(Int64 titleId, Int64 version)
        {
            DataRow[] rows = Select(String.Format("{0}={1} AND {2}={3}", Defs.Columns.TitleId, titleId, Defs.Columns.Version, version));
            if (rows.Length == 1)
            {
                return rows[0];
            }
            return null;
        }



        #endregion

        /************************************************************************/

        #region ITableImport and IColumnRowImporter implementation (commented out)
        //public bool PerformImport()
        //{
        //    return DatabaseImporter.Instance.ImportTable(this, this, "title_version");
        //}

        //public string GetColumnName(string origColName)
        //{
        //    switch (origColName)
        //    {
        //        case "documentid": return Defs.Columns.Id;
        //        case "document_type": return Defs.Columns.DocType;
        //        case "vnote": return Defs.Columns.Note;
        //        case "lang_id": return Defs.Columns.LangId;
        //        default: return origColName;
        //    }
        //}

        //public bool GetRowConfirmation(System.Data.DataRow row)
        //{
        //    return true;
        //}

        //public bool IncludeColumn(string origColName)
        //{
        //    if (origColName == "ready" || origColName == "time_min" || origColName == "time_sec" || origColName == "date_timed")
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        #endregion
        
        /************************************************************************/
        
        #region Row Object
        /// <summary>
        /// Encapsulates a single row from the <see cref="TitleVersionTable"/>.
        /// </summary>
        public class RowObject : RowObjectBase<TitleVersionTable>
        {
            #region Public properties
            /// <summary>
            /// Gets the id for this row object.
            /// </summary>
            public Int64 Id
            {
                get { return GetInt64(Defs.Columns.Id); }
            }

            /// <summary>
            /// Gets the title id for this row object.
            /// </summary>
            public Int64 TitleId
            {
                get { return GetInt64(Defs.Columns.TitleId); }
            }

            /// <summary>
            /// Gets or sets the document type for this row object.
            /// </summary>
            public Int64 DocType
            {
                get { return GetInt64(Defs.Columns.DocType); }
                set { SetValue(Defs.Columns.DocType, value); }
            }

            /// <summary>
            /// Gets or sets the file name for this row object.
            /// </summary>
            public string FileName
            {
                get { return GetString(Defs.Columns.FileName); }
                set { SetValue(Defs.Columns.FileName, value); }
            }

            /// <summary>
            /// Gets or sets the note for this row object.
            /// </summary>
            public string Note
            {
                get { return GetString(Defs.Columns.Note); }
                set { SetValue(Defs.Columns.Note, value); }
            }

            /// <summary>
            /// Gets or sets the updated value for this row object.
            /// </summary>
            public DateTime Updated
            {
                get { return GetDateTime(Defs.Columns.Updated); }
                set { SetValue(Defs.Columns.Updated, value); }
            }

            /// <summary>
            /// Gets or sets the size for this row object.
            /// </summary>
            public Int64 Size
            {
                get { return GetInt64(Defs.Columns.Size); }
                set { SetValue(Defs.Columns.Size, value); }
            }

            /// <summary>
            /// Gets or sets the version for this row object.
            /// </summary>
            public Int64 Version
            {
                get { return GetInt64(Defs.Columns.Version); }
                set { SetValue(Defs.Columns.Version, value); }
            }

            /// <summary>
            /// Gets or sets the word count for this row object.
            /// </summary>
            public Int64 WordCount
            {
                get { return GetInt64(Defs.Columns.WordCount); }
                set { SetValue(Defs.Columns.WordCount, value); }
            }

            /// <summary>
            /// Gets the file information object for this row object.
            /// You must call the <see cref="SetFileInfo"/> method before accessing this property.
            /// </summary>
            public FileInfo Info
            {
                get;
                private set;
            }
            #endregion

            /************************************************************************/

            #region Constructor
            /// <summary>
            /// Initializes a new instance of the <see cref="RowObject"/> class.
            /// </summary>
            /// <param name="row">The data row</param>
            public RowObject(DataRow row)
                : base (row)
            {
            }
            #endregion

            /************************************************************************/

            #region Public methods
            /// <summary>
            /// Sets the <see cref="Info"/> property according to the specified full file name.
            /// </summary>
            /// <param name="fullName">The full path to the file.</param>
            public void SetFileInfo(string fullName)
            {
                Info = new FileInfo(fullName);
            }

            #endregion
        }
        #endregion


    }
}
