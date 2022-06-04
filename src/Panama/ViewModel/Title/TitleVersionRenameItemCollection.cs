/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Collections.ObjectModel;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents a collection of <see cref="TitleVersionRenameItem"/> objects.
    /// </summary>
    public class TitleVersionRenameItemCollection : ObservableCollection<TitleVersionRenameItem>
    {
        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if all of the items in the collection
        /// represent a rename candidate for which the original file exists on disk.
        /// </summary>
        public bool AllOriginalExist
        {
            get
            {
                foreach (TitleVersionRenameItem item in this)
                {
                    if (!item.OriginalExists)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates if all of the items in the collection
        /// represent a rename candidate for which the original file and the proposed new file are the same,
        /// that is, already renamed.
        /// </summary>
        public bool AllSame
        {
            get
            {
                foreach (TitleVersionRenameItem item in this)
                {
                    if (!item.Same)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Performs the rename operation on all eligible items in the collection.
        /// </summary>
        public void Rename()
        {
            foreach (TitleVersionRenameItem item in this)
            {
                item.Rename();
            }
        }
        #endregion
    }
}