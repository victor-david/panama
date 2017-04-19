using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Restless.Tools.Utility;
using Restless.App.Panama.Database.Tables;

namespace Restless.App.Panama
{
    /// <summary>
    /// Represents a dictionary of TagCache objects
    /// </summary>
    public class TagCacheDictionary
    {
        #region Private
        private Dictionary<Int64, TagCache> cache;
        private TagCache unknown;
        #endregion

        /************************************************************************/
        
        #region Public properties
        /// <summary>
        /// Gets a TagCache object by its tag id
        /// </summary>
        /// <param name="tagId">The tag id</param>
        /// <returns>The TagCache object, or the unknowm TagCache object if <paramref name="tagId"/> doesn't exist</returns>
        public TagCache this[Int64 tagId]
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
        public void Add(Int64 tagId, TagCache item)
        {
            Validations.ValidateNull(item, "Add.Item");
            cache.Add(tagId, item);
        }

        /// <summary>
        /// Adds an item to the dictionary
        /// </summary>
        /// <param name="tagRow">A DataRow from the tag table</param>
        public void Add(DataRow tagRow)
        {
            Validations.ValidateNull(tagRow, "Add.TagRow");
            Int64 tagId = (Int64)tagRow[TagTable.Defs.Columns.Id];
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
