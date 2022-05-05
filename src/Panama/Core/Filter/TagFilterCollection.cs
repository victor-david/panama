using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides a specialized list of tag ids used to filter
    /// titles by the tag(s) they contain
    /// </summary>
    public class TagFilterCollection : List<long>
    {
        #region Private
        private readonly TitleRowFilter owner;
        private readonly TitleTagTable titleTagTable;
        private readonly Dictionary<long, List<long>> tagTitleMap;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TagFilterCollection"/> class
        /// </summary>
        /// <param name="owner">The owner</param>
        public TagFilterCollection(TitleRowFilter owner)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            titleTagTable = DatabaseController.Instance.GetTable<TitleTagTable>();
            tagTitleMap = new Dictionary<long, List<long>>();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Adds a tag to the filter
        /// </summary>
        /// <param name="tagId">The tag id</param>
        public new void Add(long tagId)
        {
            if (!Contains(tagId))
            {
                base.Add(tagId);
                owner.ApplyFilter();
            }
        }

        /// <summary>
        /// Removes a tag from the filter
        /// </summary>
        /// <param name="tagId"></param>
        public new void Remove(long tagId)
        {
            if (Contains(tagId))
            {
                base.Remove(tagId);
                owner.ApplyFilter();
            }
        }

        /// <summary>
        /// Invalidates the tag/title map for the specified tag id
        /// </summary>
        /// <param name="tagId">The tag id</param>
        public void Invalidate(long tagId)
        {
            if (tagTitleMap.ContainsKey(tagId))
            {
                tagTitleMap.Remove(tagId);
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates whether
        /// the specified title id passes the tag filter
        /// </summary>
        /// <param name="titleId">The title id to check</param>
        /// <returns>
        /// true if <paramref name="titleId"/> is included in the filter; otherwise, false
        /// </returns>
        public bool IsTitleIdIncluded(long titleId)
        {
            if (Count > 0)
            {
                // the way this is right now multiple tags mean tag1 OR tag2, etc.
                foreach (long tagId in this)
                {
                    if (!tagTitleMap.ContainsKey(tagId))
                    {
                        tagTitleMap.Add(tagId, titleTagTable.EnumerateTitleIdsForTag(tagId).ToList());
                    }

                    if (tagTitleMap[tagId].Contains(titleId))
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }
        #endregion
    }
}