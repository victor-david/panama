using Restless.Panama.Database.Tables;
using System;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Core
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
        public TagCacheDictionary()
        {
            cache = new Dictionary<long, TagCache>();
            unknown = new TagCache(-1, "Unknown", "This is an unknown tag");
        }
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