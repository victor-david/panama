using Restless.Tools.Database.SQLite;
using Restless.Tools.OpenXml;
using Restless.Tools.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Linq;

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
                /// The name of the revision column
                /// </summary>
                public const string Revision = "revision";

                /// <summary>
                /// The name of the language id column
                /// </summary>
                public const string LangId = "langid";

                /// <summary>
                /// The name of the word count column
                /// </summary>
                public const string WordCount = "wordcount";
            }

            /// <summary>
            /// Provides static values associated with title versions.
            /// </summary>
            public static class Values
            {
                /// <summary>
                /// Represents the numerical value for revision A.
                /// </summary>
                public const long RevisionA = 65;
            }
        }

        /// <summary>
        /// Gets the column name of the primary key.
        /// </summary>
        public override string PrimaryKeyName
        {
            get => Defs.Columns.Id;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleVersionTable"/> class.
        /// </summary>
        public TitleVersionTable() : base(DatabaseController.Instance, Defs.TableName)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Loads the data from the database into the Data collection for this table.
        /// </summary>
        public override void Load()
        {
            Load(null, $"{Defs.Columns.TitleId},{Defs.Columns.Version}");
        }
        
        /// <summary>
        /// Gets a <see cref="TitleVersionController"/> object that describes version information
        /// and provides version management for the specified title.
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <returns>
        /// A <see cref="TitleVersionController"/> object that describes version information
        /// and provides version management for <paramref name="titleId"/>.
        /// </returns>
        public TitleVersionController GetVersionController(long titleId)
        {
            return new TitleVersionController(this, titleId);
        }

        /// <summary>
        /// Provides an enumerable that gets all versions for the specified title
        /// in order of version ASC, revision ASC.
        /// </summary>
        /// <param name="titleId">The title id to get all versions for.</param>
        /// <returns>A <see cref="RowObject"/></returns>
        public IEnumerable<RowObject> EnumerateVersions(long titleId)
        {
            DataRow[] rows = Select($"{Defs.Columns.TitleId}={titleId}", $"{Defs.Columns.Version} ASC, {Defs.Columns.Revision} ASC");
            foreach (DataRow row in rows)
            {
                yield return new RowObject(row);
            }
            yield break;
        }

        /// <summary>
        /// Gets a list of <see cref="RowObject"/> that contain the specified file name.
        /// </summary>
        /// <param name="fileName">The file name (should be non-rooted)</param>
        /// <returns>
        /// A list of <see cref="RowObject"/> that contain the file name.
        /// The list may be empty.
        /// </returns>
        public List<RowObject> GetVersionsWithFile(string fileName)
        {
            List<RowObject> result = new List<RowObject>();
            fileName = fileName.Replace("'", "''");
            DataRow[] rows = Select($"{Defs.Columns.FileName}='{fileName}'");
            foreach (DataRow row in rows)
            {
                result.Add(new RowObject(row));
            }
            return result;
        }

        /// <summary>
        /// Gets a boolean value that indicates if the specified file exists as a version record.
        /// </summary>
        /// <param name="fileName">The file name (should be non-rooted)</param>
        /// <returns>true if a record containing the title exists; otherwise, false.</returns>
        public bool VersionWithFileExists(string fileName)
        {
            return GetVersionsWithFile(fileName).Count > 0;
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
            public long Id
            {
                get => GetInt64(Defs.Columns.Id);
            }

            /// <summary>
            /// Gets the title id for this row object.
            /// </summary>
            public long TitleId
            {
                get => GetInt64(Defs.Columns.TitleId);
            }

            /// <summary>
            /// Gets or sets the document type for this row object.
            /// </summary>
            public long DocType
            {
                get => GetInt64(Defs.Columns.DocType);
                set => SetValue(Defs.Columns.DocType, value);
            }

            /// <summary>
            /// Gets or sets the file name for this row object.
            /// </summary>
            public string FileName
            {
                get => GetString(Defs.Columns.FileName);
                set => SetValue(Defs.Columns.FileName, value);
            }

            /// <summary>
            /// Gets or sets the note for this row object.
            /// </summary>
            public string Note
            {
                get => GetString(Defs.Columns.Note);
                set => SetValue(Defs.Columns.Note, value);
            }

            /// <summary>
            /// Gets or sets the updated value for this row object.
            /// </summary>
            public DateTime Updated
            {
                get => GetDateTime(Defs.Columns.Updated);
                set => SetValue(Defs.Columns.Updated, value);
            }

            /// <summary>
            /// Gets or sets the size for this row object.
            /// </summary>
            public long Size
            {
                get => GetInt64(Defs.Columns.Size);
                set => SetValue(Defs.Columns.Size, value);
            }

            /// <summary>
            /// Gets the version for this row object.
            /// </summary>
            public long Version
            {
                get => GetInt64(Defs.Columns.Version);
                internal set => SetValue(Defs.Columns.Version, value);
            }

            /// <summary>
            /// Gets the revision
            /// </summary>
            public long Revision
            {
                get => GetInt64(Defs.Columns.Revision);
                internal set => SetValue(Defs.Columns.Revision, value);
            }

            /// <summary>
            /// Gets or sets the language id.
            /// </summary>
            public string LanguageId
            {
                get => GetString(Defs.Columns.LangId);
                set => SetValue(Defs.Columns.LangId, value);
            }

            /// <summary>
            /// Gets or sets the word count for this row object.
            /// </summary>
            public long WordCount
            {
                get => GetInt64(Defs.Columns.WordCount);
                set => SetValue(Defs.Columns.WordCount, value);
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
