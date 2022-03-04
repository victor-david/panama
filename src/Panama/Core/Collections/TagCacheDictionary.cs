/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Tables;
using System;
using System.Collections.Generic;
using System.Data;

namespace Restless.App.Panama.Core
{
    /// <summary>
    /// Represents a dictionary of TagCache objects
    /// </summary>
    public class TagCacheDictionary
    {
        #region Private
        private Dictionary<long, TagCache> cache;
        private TagCache unknown;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a TagCache object by its tag id
        /// </summary>
        /// <param name="tagId">The tag id</param>
        /// <returns>The TagCache object, or the unknowm TagCache object if <paramref name="tagId"/> doesn't exist</returns>
        public TagCache this[long tagId]
        {
            get
            {
                if (cache.ContainsKey(tagId))
                {
                    return cache[tagId];
                }
                return unknown;
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public TagCacheDictionary()
        {
            cache = new Dictionary<long, TagCache>();
            unknown = new TagCache(-1, "Unknown", "This is an unknown tag");
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        /// <summary>
        /// Adds an item to the dictionary
        /// </summary>
        /// <param name="tagId">The tag id</param>
        /// <param name="item">The item</param>
        public void Add(long tagId, TagCache item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }
            cache.Add(tagId, item);
        }

        /// <summary>
        /// Adds an item to the dictionary
        /// </summary>
        /// <param name="tagRow">A DataRow from the tag table</param>
        public void Add(DataRow tagRow)
        {
            if (tagRow == null)
            {
                throw new ArgumentNullException(nameof(tagRow));
            }
            long tagId = (long)tagRow[TagTable.Defs.Columns.Id];
            string tagName = tagRow[TagTable.Defs.Columns.Tag].ToString();
            string tagDesc = tagRow[TagTable.Defs.Columns.Description].ToString();
            Add(tagId, new TagCache(tagId, tagName, tagDesc));
        }

        /// <summary>
        /// Clears all the entries in the dictionary
        /// </summary>
        public void Clear()
        {
            cache.Clear();
        }
    }
}