using System;
using System.Collections.Generic;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents a list of <see cref="TitleExportCandidate"/> objects.
    /// </summary>
    public class TitleExportTitleList : List<TitleExportCandidate>
    {
        #region Public Methods
        /// <summary>
        /// Returns a boolean value that indicates whether the specified export path is represented in the list.
        /// </summary>
        /// <param name="exportPath">The path to check.</param>
        /// <returns>true if <paramref name="exportPath"/> is represented in the list; otherwise, false.</returns>
        public bool HasCandidateWithExportPath(string exportPath)
        {
            if (!string.IsNullOrWhiteSpace(exportPath))
            {
                foreach (TitleExportCandidate item in this)
                {
                    if (item.ExportFullName.Equals(exportPath, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}