using System.Collections.ObjectModel;
using System.Linq;

namespace Restless.Panama.Controls
{
    /// <summary>
    /// Represents an observable collection of <see cref="TagSelectorItem"/>
    /// </summary>
    public class TagSelectorItemCollection : ObservableCollection<TagSelectorItem>
    {
        /// <summary>
        /// Gets the <see cref="TagSelectorItem"/> with the specified id.
        /// </summary>
        /// <param name="itemId">The item id</param>
        /// <returns>The first item with the specified id, or null</returns>
        public TagSelectorItem GetItem(long itemId)
        {
            return this.FirstOrDefault(item => item.Id == itemId);
        }

        /// <summary>
        /// Enables all items in the collection.
        /// </summary>
        public void EnableAll()
        {
            foreach (TagSelectorItem item in this)
            {
                item.Enable();
            }
        }
    }
}