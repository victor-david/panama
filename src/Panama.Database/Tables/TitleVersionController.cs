/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Defs = Restless.Panama.Database.Tables.TitleVersionTable.Defs;

namespace Restless.Panama.Database.Tables
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
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets a reference to <see cref="TitleVersionTable"/>.
        /// </summary>
        public TitleVersionTable VersionTable => DatabaseController.Instance.GetTable<TitleVersionTable>();

        /// <summary>
        /// Gets the title id that is associated with this instance.
        /// </summary>
        public long TitleId
        {
            get;
        }

        /// <summary>
        /// Gets a list of <see cref="TitleVersionRow"/> for the title associated
        /// with this instance.
        /// </summary>
        public List<TitleVersionRow> Versions
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
        internal TitleVersionController(long titleId)
        {
            Versions = new List<TitleVersionRow>();
            versionMap = new Dictionary<long, List<long>>();
            TitleId = titleId;
            BuildVersionsAndMap();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Adds a new version using the specified file name.
        /// </summary>
        /// <param name="fileName">The name of the file for the new version.</param>
        public TitleVersionController Add(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException(nameof(fileName));
            }
            DataRow row = VersionTable.NewRow();
            row[Defs.Columns.TitleId] = TitleId;
            // OnColumnChanged(e) method will update the associated fields: Updated, Size, and DocType
            row[Defs.Columns.FileName] = fileName;
            row[Defs.Columns.Version] = VersionCount + 1;
            row[Defs.Columns.Revision] = Defs.Values.RevisionA;
            row[Defs.Columns.LangId] = LanguageTable.Defs.Values.DefaultLanguageId;
            VersionTable.Rows.Add(row);
            VersionTable.Save();
            BuildVersionsAndMap();
            return this;
        }

        /// <summary>
        /// Removes the specifed version.
        /// </summary>
        /// <param name="current">The version to remove.</param>
        public void Remove(TitleVersionRow current)
        {
            if (current == null)
            {
                throw new ArgumentNullException(nameof(current));
            }
            current.Row.Delete();
            RenumberAllVersions();
            RenumberAllRevisions();
        }

        /// <summary>
        /// Moves the version specified by <paramref name="current"/> up.
        /// </summary>
        /// <param name="current">The current version to move.</param>
        /// <remarks>
        /// See remarks for <see cref="MoveDown(TitleVersionRow)"/> for more info.
        /// </remarks>
        public void MoveUp(TitleVersionRow current)
        {
            if (current == null)
            {
                throw new ArgumentNullException(nameof(current));
            }

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
        public void MoveDown(TitleVersionRow current)
        {
            if (current == null)
            {
                throw new ArgumentNullException(nameof(current));
            }

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
        public void ConvertToVersion(TitleVersionRow current)
        {
            if (current == null)
            {
                throw new ArgumentNullException(nameof(current));
            }
            
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
        /// Gets a boolean value that indicates if the specified <see cref="TitleVersionRow"/>
        /// represents the latest version / revision, i.e. HighestVersion.RevA
        /// </summary>
        /// <param name="row">The row object</param>
        /// <returns>true if <paramref name="row"/> represents HighestVersion.RevA; otherwise, false</returns>
        public bool IsLatest(TitleVersionRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }
            return row.Version == VersionCount && row.Revision == Defs.Values.RevisionA;
        }

        /// <summary>
        /// Gets a boolean value that indicates if the specified <see cref="TitleVersionRow"/>
        /// represents the earliest version / revision, i.e. Ver1.Rev[MaxRev]
        /// </summary>
        /// <param name="row">The row object</param>
        /// <returns>true if <paramref name="row"/> represents Ver1.Rev[MaxRev]; otherwise, false</returns>
        public bool IsEarliest(TitleVersionRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

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
            sb.AppendLine($"Title: {TitleId} Rows:{Versions.Count} Versions:{VersionCount}");

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
        /// Get the <see cref="TitleVersionRow"/> that comes before the specified version row
        /// </summary>
        /// <param name="verRow">The version row.</param>
        /// <returns>The previous version row.</returns>
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
        private TitleVersionRow GetPrevious(TitleVersionRow verRow)
        {
            return GetVersionRowFromReference(verRow, -1);
        }

        /// <summary>
        /// Get the <see cref="TitleVersionRow"/> that comes after the specified version row
        /// </summary>
        /// <param name="versionRow">The row object.</param>
        /// <returns>The next row object.</returns>
        /// <remarks>
        /// See remarks on <see cref="GetPrevious(TitleVersionRow)"/>.
        /// </remarks>
        private TitleVersionRow GetNext(TitleVersionRow versionRow)
        {
            return GetVersionRowFromReference(versionRow, 1);
        }

        private TitleVersionRow GetVersionRowFromReference(TitleVersionRow referenceRow, int offset)
        {
            int idxNeeded = -1;
            foreach (TitleVersionRow row in Versions)
            {
                if (row.Version == referenceRow.Version && row.Revision == referenceRow.Revision)
                {
                    idxNeeded = Versions.IndexOf(row) + offset;
                    break;
                }
            }
            if (idxNeeded < 0 || idxNeeded > Versions.Count - 1)
            {
                throw new IndexOutOfRangeException($"{nameof(GetVersionRowFromReference)}-Invalid index {idxNeeded}", new Exception(ToString()));
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
            foreach (TitleVersionRow row in Versions)
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
            foreach (TitleVersionRow verRow in VersionTable.EnumerateVersions(TitleId, SortDirection.Ascending))
            {
                if (verRow.Revision == Defs.Values.RevisionA)
                {
                    version++;
                }
                verRow.Version = version;
            }
            BuildVersionsAndMap();
        }

        private void BuildVersionsAndMap()
        {
            Versions.Clear();
            versionMap.Clear();

            long lastVer = -1;

            foreach (TitleVersionRow verRow in VersionTable.EnumerateVersions(TitleId, SortDirection.Descending))
            {
                Versions.Add(verRow);
                if (verRow.Version != lastVer)
                {
                    lastVer = verRow.Version;
                    versionMap.Add(lastVer, new List<long>());
                    versionMap[lastVer].Add(verRow.Revision);
                }
                else
                {
                    versionMap[lastVer].Add(verRow.Revision);
                }
            }
        }
        #endregion
    }
}