using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Restless.App.Panama.ViewModel;

namespace Restless.App.Panama.Collections
{
    /// <summary>
    /// An observable collection of TagCommandViewModel objects
    /// </summary>
    public class TagCommandViewModelCollection : ObservableCollection<TagCommandViewModel>
    {

        /// <summary>
        /// Gets the TagCommandViewModel with the specified id
        /// </summary>
        /// <param name="tagId">The tag id</param>
        /// <returns>The TagCommandViewModel with the specified id, or null if not found.</returns>
        public TagCommandViewModel GetItem(long tagId)
        {
            foreach (TagCommandViewModel vm in this)
            {
                if (vm.TagId == tagId)
                {
                    return vm;
                }
            }
            return null;
        }
    }
}
