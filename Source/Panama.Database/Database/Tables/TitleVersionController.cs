using Restless.Tools.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Defs = Restless.App.Panama.Database.Tables.TitleVersionTable.Defs;
using RowObject = Restless.App.Panama.Database.Tables.TitleVersionTable.RowObject;

namespace Restless.App.Panama.Database.Tables
{
    /// <summary>
    /// Represents a controller used to manage and manipulate title versions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class is responsible for handling version changes for a title such as
    /// adding a version, removing a version, or changing a version / revision.
    /// To obtain an instance of this class, use <see cref="TitleVersionTable.GetVersionController(long)"/>.
    /// passing the title id.
    /// </para>
    /// <para>
    /// An instance of this class maintains a mapping of versions and revisions
    /// of the title to which it applies. As you add, remove or alter versions and revisions,
    /// the mapping is updated to reflect the changes.
    /// </para>
    /// </remarks>
    public class TitleVersionController
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
        /// Initializes a new instance of the <see cref="TitleVersionController"/> class.
        /// </summary>
        /// <param name="owner">The title version table that owns this instance.</param>
        /// <param name="titleId">The title id to get the version information for.</param>
        internal TitleVersionController(TitleVersionTable owner, long titleId)
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
        /// Adds a new version using the specified file name.
        /// </summary>
        /// <param name="fileName">The name of the file for the new version.</param>
        public void Add(string fileName)
        {
            Validations.ValidateNullEmpty(fileName, "AddVersion.Filename");
            DataRow row = owner.NewRow();
            row[Defs.Columns.TitleId] = titleId;
            // OnColumnChanged(e) method will update the associated fields: Updated, Size, and DocType
            row[Defs.Columns.FileName] = fileName;
            row[Defs.Columns.Version] = VersionCount + 1;
            row[Defs.Columns.Revision] = Defs.Values.RevisionA;
            row[Defs.Columns.LangId] = LanguageTable.Defs.Values.DefaultLanguageId;
            owner.Rows.Add(row);
            owner.Save();
            BuildVersionsAndMap();
        }

        /// <summary>
        /// Removes the specifed version.
        /// </summary>
        /// <param name="current">The version to remove.</param>
        public void Remove(RowObject current)
        {
            Validations.ValidateNull(current, nameof(current));

            long titleId = current.TitleId;
            current.Row.Delete();
            RenumberAllVersions();
            RenumberAllRevisions();
        }

        /// <summary>
        /// Moves the version specified by <paramref name="current"/> up.
        /// </summary>
        /// <param name="current">The current version to move.</param>
        /// <remarks>
        /// See remarks for <see cref="MoveDown(RowObject)"/> for more info.
        /// </remarks>
        public void MoveUp(RowObject current)
        {
            Validations.ValidateNull(current, nameof(current));

            if (!IsLatest(current))
            {
                var prev = GetPrevious(current);
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
                    ChangeRevisions(current.Version, -1);
                    current.Version = prev.Version;
                    current.Revision = GetLastRevision(prev.Version) + 1;
                    if (adjustVersions)
                    {
                        RenumberAllVersions();
                    }
                }
                BuildVersionsAndMap();
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
        public void MoveDown(RowObject current)
        {
            Validations.ValidateNull(current, nameof(current));

            if (!IsEarliest(current))
            {
                var next = GetNext(current);
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
                    ChangeRevisions(next.Version, 1);
                    current.Version = next.Version;
                    current.Revision = Defs.Values.RevisionA;
                    if (adjustVersions)
                    {
                        RenumberAllVersions();
                    }
                }
                BuildVersionsAndMap();
            }
        }

        /// <summary>
        /// Converts a revision to a separate version.
        /// </summary>
        /// <param name="current">The version /revision to convert.</param>
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
            
            if (GetRevisionCount(current.Version) > 1)
            {
                long ver = current.Version;
                current.Version = VersionCount + 1;
                current.Revision = Defs.Values.RevisionA;
                RenumberAllRevisions(ver, true);
                BuildVersionsAndMap();
            }
        }

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

        #region Private methods
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
        private RowObject GetPrevious(RowObject rowObj)
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
        private RowObject GetNext(RowObject rowObj)
        {
            return GetNeededRowObject(rowObj, 1);
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

        /// <summary>
        /// Changes all revisions for the specified version by the amount specified.
        /// </summary>
        /// <param name="version">The version</param>
        /// <param name="offset">The amount to change the revisions. 1 or -1</param>
        private void ChangeRevisions(long version, int offset)
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
        private long GetLastRevision(long version)
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
        private void RenumberAllRevisions()
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
        private void RenumberAllRevisions(long version, bool rebuildMap)
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
        private void RenumberAllVersions()
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
        #endregion
    }
}
