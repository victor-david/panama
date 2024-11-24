using System.Collections.ObjectModel;
using System.Linq;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents a collection of <see cref="TitleVersionRenameItem"/> objects.
    /// </summary>
    public class TitleVersionRenameItemCollection : ObservableCollection<TitleVersionRenameItem>
    {
        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if any of the rename items is missing its original file.
        /// </summary>
        /// <returns></returns>
        public bool HaveAnyWithOriginalMissing()
        {
            return this.Any(item => !item.OriginalExists);
        }

        /// <summary>
        /// Gets a boolean value that indicates if all of the items are already renamed.
        /// </summary>
        public bool AreAllRenamed()
        {
            return this.Count(item => item.Same) == Count;
        }

        /// <summary>
        /// Gets a boolean value that indicates if any of the rename items have a new name that already exists.
        /// </summary>
        /// <returns></returns>
        public bool HaveAnyWithNewExists()
        {
            return this.Any(item => item.NewExists);
        }

        /// <summary>
        /// Gets a boolean values that indicates if any of the items can be renamed.
        /// </summary>
        /// <returns></returns>
        public bool HaveAnyCanRename()
        {
            return this.Any(item => item.CanRename);
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