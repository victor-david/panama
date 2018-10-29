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
        /// Gets a <see cref="TitleVersionInfo"/> object that describes version information
        /// for the specified title.
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <returns>A <see cref="TitleVersionInfo"/> object that describes version information for <paramref name="titleId"/>.</returns>
        public TitleVersionInfo GetVersionInfo(long titleId)
        {
            return new TitleVersionInfo(this, titleId);
        }

        /// <summary>
        /// Provides an enumerable that gets all versions for the specified title
        /// in order of version ASC, revision ASC.
        /// </summary>
        /// <param name="titleId">The title id to get all versions for.</param>
        /// <returns>A <see cref="RowObject"/></returns>
        public IEnumerable<RowObject> GetAllVersions(long titleId)
        {
            DataRow[] rows = Select($"{Defs.Columns.TitleId}={titleId}", $"{Defs.Columns.Version} ASC, {Defs.Columns.Revision} ASC");
            foreach (DataRow row in rows)
            {
                yield return new RowObject(row);
            }
            yield break;
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
            DataRow[] rows = Select($"{Defs.Columns.FileName}='{fileName}'");
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
        public void AddVersion(long titleId, string filename)
        {
            Validations.ValidateNullEmpty(filename, "AddVersion.Filename");

            var verInfo = GetVersionInfo(titleId);
            long version = verInfo.VersionCount + 1;

            DataRow row = NewRow();
            row[Defs.Columns.TitleId] = titleId;
            // OnColumnChanged(e) method will update the associated fields: Updated, Size, and DocType
            row[Defs.Columns.FileName] = filename;
            row[Defs.Columns.Version] = version;
            row[Defs.Columns.Revision] = Defs.Values.RevisionA;
            row[Defs.Columns.LangId] = LanguageTable.Defs.Values.DefaultLanguageId;
            Rows.Add(row);
            Save();
        }

        /// <summary>
        /// Moves the version specified by <paramref name="current"/> up.
        /// </summary>
        /// <param name="current">The current version to move.</param>
        /// <remarks>
        /// See remarks for <see cref="MoveVersionDown(RowObject)"/> for more info.
        /// </remarks>
        public void MoveVersionUp(RowObject current)
        {
            Validations.ValidateNull(current, nameof(current));
            TitleVersionInfo verInfo = GetVersionInfo(current.TitleId);
            if (!verInfo.IsLatest(current))
            {
                var prev = verInfo.GetPrevious(current);
                if (current.Version == prev.Version)
                {
                    // same version number, swap current revison with prev revision.
                    long temp = current.Revision;
                    current.Revision = prev.Revision;
                    prev.Revision = temp;
                }
                else
                {
                    // Crossing a version boundary. 
                    //  1. Change revisions of CURRENT by decreasing all by 1 
                    //  2. Set version of CURRENT to the version of PREV
                    //  3. Set revision of CURRENT to the highest revision of PREV + 1.
                    //  4. if revision of CURRENT was A, then adjust all versions
                    //     because the version that CURRENT had no longer exists.
                    bool adjustVersions = current.Revision == Defs.Values.RevisionA;
                    verInfo.ChangeRevisions(current.Version, -1);
                    current.Version = prev.Version;
                    current.Revision = verInfo.GetLastRevision(prev.Version) + 1;
                    if (adjustVersions)
                    {
                        verInfo.RenumberAllVersions();
                    }
                }
            }
        }

        /// <summary>
        /// Moves the version specified by <paramref name="current"/> down.
        /// </summary>
        /// <param name="current">The current version to move.</param>
        /// <remarks>
        /// <para>"Down" is defined in relation to the standard order of versions and revisions. Example:</para>
        /// <list type="bullet">
        /// <item>2A</item>
        /// <item>2B</item>
        /// <item>2C</item>
        /// <item>1A</item>
        /// <item>1B</item>
        /// </list>
        /// <para>
        /// In the above example, to move item 2B (version 2, revision B) down means
        /// swapping its position with item 2C. Moving 2C down means moving that entry
        /// into version 1A, turning the current 1A into 1B and the current 1B into 1C.
        /// </para>
        /// </remarks>
        public void MoveVersionDown(RowObject current)
        {
            Validations.ValidateNull(current, nameof(current));
            TitleVersionInfo verInfo = GetVersionInfo(current.TitleId);
            if (!verInfo.IsEarliest(current))
            {
                var next = verInfo.GetNext(current);
                if (current.Version == next.Version)
                {
                    // same version number, swap current revison with next revision.
                    long temp = current.Revision;
                    current.Revision = next.Revision;
                    next.Revision = temp;
                }
                else
                {
                    // Crossing a version boundary. 
                    //  1. Change revisions of NEXT by increasing all by 1, leaving room for rev A.
                    //  2. Set version of CURRENT to the version of NEXT
                    //  3. Set revision of CURRENT to A.
                    //  4. if revision of CURRENT was A, then adjust all versions
                    //     because the version that CURRENT had no longer exists.
                    bool adjustVersions = current.Revision == Defs.Values.RevisionA;
                    verInfo.ChangeRevisions(next.Version, 1);
                    current.Version = next.Version;
                    current.Revision = Defs.Values.RevisionA;
                    if (adjustVersions)
                    {
                        verInfo.RenumberAllVersions();
                    }
                }
            }
        }

        /// <summary>
        /// Removes the specified version from the specified title and renumbers the remaining versions.
        /// </summary>
        /// <param name="current">The row object to remove</param>
        public void RemoveVersion(RowObject current)
        {
            Validations.ValidateNull(current, nameof(current));

            long titleId = current.TitleId;
            current.Row.Delete();
            var verInfo = GetVersionInfo(titleId);

            verInfo.RenumberAllVersions();
            verInfo.RenumberAllRevisions();
        }

        /// <summary>
        /// Converts the specified <see cref="RowObject"/> to its own version.
        /// </summary>
        /// <param name="current">The row object to convert to its own version</param>
        /// <remarks>
        /// This method creates a new version from <paramref name="current"/> that is
        /// one higher than the current number of versions associated with the title
        /// and changes the revision to <see cref="Defs.Values.RevisionA"/>. If the
        /// number of revisions associated with <paramref name="current"/> is not
        /// greater than 1 (i.e. the version has only a single revision), this
        /// method does nothing.
        /// </remarks>
        public void ConvertToVersion(RowObject current)
        {
            Validations.ValidateNull(current, nameof(current));
            var verInfo = GetVersionInfo(current.TitleId);
            if (verInfo.GetRevisionCount(current.Version) > 1)
            {
                long ver = current.Version;
                current.Version = verInfo.VersionCount + 1;
                current.Revision = Defs.Values.RevisionA;
                verInfo.RenumberAllRevisions(ver, true);
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

        ///// <summary>
        ///// Gets the first or last version for the specified title
        ///// </summary>
        ///// <param name="titleId">The title id</param>
        ///// <param name="getFirst">true to get the first version, false to get the last version</param>
        ///// <returns>A DataRow object with the specified first or last version; null if the title has no versions.</returns>
        //private DataRow GetFirstOrLastVersion(long titleId, bool getFirst)
        //{
        //    string order = (getFirst) ? "ASC" : "DESC";
        //    DataRow[] rows = Select($"{Defs.Columns.TitleId}={titleId}", $"{Defs.Columns.Version} {order}");
        //    if (rows.Length > 0)
        //    {
        //        return rows[0];
        //    }
        //    return null;
        //}


        /// <summary>
        /// Gets the specified version for the specified title, or null if it doesn't exist
        /// </summary>
        /// <param name="titleId">The title id</param>
        /// <param name="version">The version number to get</param>
        /// <returns>A DataRow object with the specified version for the title, or null if it doesn't exist.</returns>
        private DataRow GetVersion(long titleId, long version)
        {
            DataRow[] rows = Select($"{Defs.Columns.TitleId}={titleId} AND {Defs.Columns.Version}={version}");
            if (rows.Length == 1)
            {
                return rows[0];
            }
            return null;
        }
        #endregion

        /************************************************************************/

        #region TitleVersionInfo (nested class)
        /// <summary>
        /// Provides version information for a specified title
        /// </summary>
        public class TitleVersionInfo
        {
            #region Private
            /// <summary>
            /// Maps a dictionary of version numbers to a list of revision ids.
            /// </summary>
            private readonly Dictionary<long, List<long>> versionMap;
            private readonly TitleVersionTable owner;
            private readonly long titleId;
            #endregion

            /************************************************************************/

            #region Public properties
            /// <summary>
            /// Gets a list of <see cref="RowObject"/> for the title associated
            /// with this instance.
            /// </summary>
            public List<RowObject> Versions
            {
                get;
            }

            /// <summary>
            /// Gets the total number of versions. If no version carries multiple revisions,
            /// this value is the same as <see cref="Versions"/>.Count. Otherwise, it is less.
            /// </summary>
            public int VersionCount
            {
                get => versionMap.Count;
            }
            #endregion

            /************************************************************************/

            #region Constructor (internal)
            /// <summary>
            /// Initializes a new instance of the <see cref="TitleVersionInfo"/> class.
            /// </summary>
            /// <param name="owner">The title version table that owns this instance.</param>
            /// <param name="titleId">The title id to get the version information for.</param>
            internal TitleVersionInfo(TitleVersionTable owner, long titleId)
            {
                Versions = new List<RowObject>();
                versionMap = new Dictionary<long, List<long>>();
                this.owner = owner;
                this.titleId = titleId;
                BuildVersionsAndMap();
            }
            #endregion

            /************************************************************************/

            #region Public methods
            /// <summary>
            /// Gets a boolean value that indicates if the specified <see cref="RowObject"/>
            /// represents the latest version / revision, i.e. HighestVersion.RevA
            /// </summary>
            /// <param name="row">The row object</param>
            /// <returns>true if <paramref name="row"/> represents HighestVersion.RevA; otherwise, false</returns>
            public bool IsLatest(RowObject row)
            {
                Validations.ValidateNull(row, nameof(row));
                return row.Version == VersionCount && row.Revision == Defs.Values.RevisionA;
            }

            /// <summary>
            /// Gets a boolean value that indicates if the specified <see cref="RowObject"/>
            /// represents the earliest version / revision, i.e. Ver1.Rev[MaxRev]
            /// </summary>
            /// <param name="row">The row object</param>
            /// <returns>true if <paramref name="row"/> represents Ver1.Rev[MaxRev]; otherwise, false</returns>
            public bool IsEarliest(RowObject row)
            {
                Validations.ValidateNull(row, nameof(row));
                if (row.Version == 1)
                {
                    if (versionMap.ContainsKey(1))
                    {
                        int lastIdx = versionMap[1].Count - 1;
                        return row.Revision == versionMap[1][lastIdx];
                    }
                    throw new IndexOutOfRangeException($"IsEarliest-Invalid index 1", new Exception(ToString()));
                }
                return false;
            }

            /// <summary>
            /// Gets the number of revisions for the specified version.
            /// </summary>
            /// <param name="version">The version to check.</param>
            /// <returns>The number of revisions for <paramref name="version"/>.</returns>
            public int GetRevisionCount(long version)
            {
                if (versionMap.ContainsKey(version))
                {
                    return versionMap[version].Count;
                }
                throw new IndexOutOfRangeException($"GetRevisionCount-Invalid index {version}", new Exception(ToString()));
            }

            /// <summary>
            /// Returns a string representation of this object.
            /// </summary>
            /// <returns>A string representation of this object.</returns>
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Title: {titleId} Rows:{Versions.Count} Versions:{VersionCount}");

                foreach (var ver in versionMap)
                {
                    sb.Append($"{ver.Key} ");
                    foreach (long rev in ver.Value)
                    {
                        sb.Append((char)rev);
                    }
                    sb.AppendLine();
                }
                return sb.ToString();
            }
            #endregion

            /************************************************************************/

            #region Internal methods
            /// <summary>
            /// Get the <see cref="RowObject"/> that comes before the specified row object.
            /// </summary>
            /// <param name="rowObj">The row object.</param>
            /// <returns>The previous row object.</returns>
            /// <remarks>
            /// Previous is based upon the ordering of <see cref="Versions"/>. They are
            /// fetched from the data in descending order of version, ascending order of revision.
            /// 3A
            /// 3B - version 3 has 2 revisions.
            /// 2A
            /// 2B
            /// 2C - version 2 has 3 revisions.
            /// 1A - version 1 has 1 revision.
            /// </remarks>
            internal RowObject GetPrevious(RowObject rowObj)
            {
                return GetNeededRowObject(rowObj, -1);
            }

            /// <summary>
            /// Get the <see cref="RowObject"/> that comes after the specified row object.
            /// </summary>
            /// <param name="rowObj">The row object.</param>
            /// <returns>The next row object.</returns>
            /// <remarks>
            /// See remarks on <see cref="GetPrevious(RowObject)"/>.
            /// </remarks>
            internal RowObject GetNext(RowObject rowObj)
            {
                return GetNeededRowObject(rowObj, 1);
            }

            /// <summary>
            /// Changes all revisions for the specified version by the amount specified.
            /// </summary>
            /// <param name="version">The version</param>
            /// <param name="offset">The amount to change the revisions. 1 or -1</param>
            internal void ChangeRevisions(long version, int offset)
            {
                foreach (var row in Versions)
                {
                    if (row.Version == version)
                    {
                        row.Revision += offset;
                    }
                }
            }

            /// <summary>
            /// Gets the last (highest numbered) revision for the specified version.
            /// </summary>
            /// <param name="version">The version</param>
            /// <returns>The last revision number for <paramref name="version"/>.</returns>
            internal long GetLastRevision(long version)
            {
                if (versionMap.ContainsKey(version))
                {
                    if (versionMap[version].Count > 0)
                    {
                        int lastIdx = versionMap[version].Count - 1;
                        return versionMap[version][lastIdx];
                    }
                }
                throw new IndexOutOfRangeException($"GetLastRevision-Invalid index {version}", new Exception(ToString()));
            }

            /// <summary>
            /// Renumbers all revisions for all versions.
            /// </summary>
            internal void RenumberAllRevisions()
            {
                foreach (var kp in versionMap)
                {
                    RenumberAllRevisions(kp.Key, false);
                }
                BuildVersionsAndMap();
            }

            /// <summary>
            /// Renumbers all revisions of the specified version.
            /// </summary>
            /// <param name="version">The version to renumber the revisions for.</param>
            /// <param name="rebuildMap">true to rebuild the version map after renumbering; otherwise, false.</param>
            internal void RenumberAllRevisions(long version, bool rebuildMap)
            {
                long rev = Defs.Values.RevisionA;
                foreach (var row in Versions.Where((r) => r.Version == version))
                {
                    row.Revision = rev++;
                }
                if (rebuildMap)
                {
                    BuildVersionsAndMap();
                }
            }

            /// <summary>
            /// Renumbers all versions.
            /// </summary>
            internal void RenumberAllVersions()
            {
                long version = 0;
                DataRow[] rows = owner.Select($"{Defs.Columns.TitleId}={titleId}", $"{Defs.Columns.Version} ASC, {Defs.Columns.Revision} ASC");
                foreach (DataRow row in rows)
                {
                    RowObject rowObj = new RowObject(row);
                    if (rowObj.Revision == Defs.Values.RevisionA)
                    {
                        version++;
                    }
                    rowObj.Version = version;
                }
                BuildVersionsAndMap();
            }
            #endregion

            /************************************************************************/

            #region Private methods
            private void BuildVersionsAndMap()
            {
                Versions.Clear();
                versionMap.Clear();

                DataRow[] rows = owner.Select($"{Defs.Columns.TitleId}={titleId}", $"{Defs.Columns.Version} DESC, {Defs.Columns.Revision} ASC");

                long lastVer = -1;

                foreach (DataRow row in rows)
                {
                    RowObject rowObj = new RowObject(row);
                    Versions.Add(rowObj);

                    if (rowObj.Version != lastVer)
                    {
                        lastVer = rowObj.Version;
                        versionMap.Add(lastVer, new List<long>());
                        versionMap[lastVer].Add(rowObj.Revision);
                    }
                    else
                    {
                        versionMap[lastVer].Add(rowObj.Revision);
                    }
                }
            }

            private RowObject GetNeededRowObject(RowObject rowObj, int offset)
            {
                int idxNeeded = -1;
                foreach (RowObject row in Versions)
                {
                    if (row.Version == rowObj.Version && row.Revision == rowObj.Revision)
                    {
                        idxNeeded = Versions.IndexOf(row) + offset;
                        break;
                    }
                }
                if (idxNeeded < 0 || idxNeeded > Versions.Count - 1)
                {
                    throw new IndexOutOfRangeException($"GetNeededRowObject-Invalid index {idxNeeded}", new Exception(ToString()));
                }
                return Versions[idxNeeded];
            }
            #endregion
        }
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
