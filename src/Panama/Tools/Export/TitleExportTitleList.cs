/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents a list of <see cref="TitleExportCandidate"/> objects.
    /// </summary>
    public class TitleExportTitleList : List<TitleExportCandidate>
    {
        #region Private Vars
        #endregion

        /************************************************************************/

        #region Public Methods
        /// <summary>
        /// Returns a boolean value that indicates whether the specified export path is represented in the list.
        /// </summary>
        /// <param name="exportPath">The path to check.</param>
        /// <returns>true if <paramref name="exportPath"/> is represented in the list; otherwise, false.</returns>
        public bool HasCandidateWithExportPath(string exportPath)
        {
            if (string.IsNullOrWhiteSpace(exportPath)) return false;
            foreach (var item in this)
            {
                if (item.ExportPath.Equals(exportPath, StringComparison.InvariantCultureIgnoreCase)) return true;
            }
            return false;
        }
        #endregion
    }
}