/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents a single folder with associated file statistics.
    /// </summary>
    public class FolderStatisticItem
    {
        #region Public properties
        /// <summary>
        /// Gets the folder associated with the statistics.
        /// </summary>
        public string Folder
        {
            get;
        }

        /// <summary>
        /// Gets the display name for this item. Returns only the folder name.
        /// </summary>
        public string FolderDisplay => Path.GetFileName(Folder);

        /// <summary>
        /// Gets the depth of this folder statistic within the folder hierarchy.
        /// </summary>
        public int Depth
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the total number of files. Includes sub folders.
        /// </summary>
        public int Total
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the number of .docx files. Includes sub folders.
        /// </summary>
        public int Docx
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the number of .doc files. Includes sub folders.
        /// </summary>
        public int Doc
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the number of .pdf files. Includes sub folders.
        /// </summary>
        public int Pdf
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the number of .txt files. Includes sub folders.
        /// </summary>
        public int Txt
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the number of other files. Includes sub folders.
        /// </summary>
        public int Other
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the children of this item.
        /// </summary>
        public List<FolderStatisticItem> Children
        {
            get;
        }
        #endregion

        /************************************************************************/
        
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderStatisticItem"/> class.
        /// </summary>
        /// <param name="folder">The folder for which this set of statistics apply.</param>
        public FolderStatisticItem(string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                throw new ArgumentNullException(nameof(folder));
            }

            if (!Directory.Exists(folder))
            {
                throw new InvalidOperationException(Strings.InvalidOpDirectoryDoesNotExist);
            }
            Folder = folder;
            Children = new List<FolderStatisticItem>();
            Depth = 0;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Populates the folder statistics. This method recursively checks sub folders,
        /// creating items for the <see cref="Children"/> property.
        /// </summary>
        public async Task PopulateAsync()
        {
            await PopulateAsync(Folder, 0);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private async Task PopulateAsync(string folder, int depth)
        {
            Depth = depth;
            foreach (string file in Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly))
            {
                switch (Path.GetExtension(file).ToLower(CultureInfo.InvariantCulture))
                {
                    case ".docx":
                        Docx++;
                        break;

                    case ".doc":
                        Doc++;
                        break;

                    case ".pdf":
                        Pdf++;
                        break;

                    case ".txt":
                        Txt++;
                        break;

                    default:
                        Other++;
                        break;
                }

                Total++;
            }

            foreach (string dir in Directory.EnumerateDirectories(folder, "*", SearchOption.TopDirectoryOnly))
            {
                FolderStatisticItem child = new(dir);
                Children.Add(child);
                await child.PopulateAsync(dir, depth + 1);
            }
            SumChildren(this);
        }

        private void SumChildren(FolderStatisticItem item)
        {
            foreach (FolderStatisticItem child in item.Children)
            {
                SumChildren(child);
                Total += child.Total;
                Docx += child.Docx;
                Doc += child.Doc;
                Pdf += child.Pdf;
                Txt += child.Txt;
                Other += child.Other;
            }
        }
        #endregion
    }
}