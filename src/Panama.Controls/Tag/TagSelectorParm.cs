using System;

namespace Restless.Panama.Controls
{
    /// <summary>
    /// Represents the parameter that is sent on the <see cref="TagSelectorControl.TagItemClickedCommand"/>
    /// </summary>
    public class TagSelectorParm
    {
        #region Properties
        /// <summary>
        /// Gets the id of the <see cref="TagSelectorControl"/>
        /// </summary>
        public int SelectorControlId
        {
            get;
        }

        /// <summary>
        /// Gets the tag selector item
        /// </summary>
        public TagSelectorItem TagSelectorItem
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TagSelectorParm"/> class.
        /// </summary>
        /// <param name="selectorControlId">The selector control id</param>
        /// <param name="tagSelectorItem">The tag selector item</param>
        internal TagSelectorParm(int selectorControlId, TagSelectorItem tagSelectorItem)
        {
            TagSelectorItem = tagSelectorItem ?? throw new ArgumentNullException(nameof(tagSelectorItem));
            SelectorControlId = selectorControlId;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets a string representation of this object.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return $"Control Id: {SelectorControlId} Item: {TagSelectorItem}";
        }
        #endregion
    }
}
