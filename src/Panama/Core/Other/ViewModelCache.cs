using Restless.Panama.ViewModel;
using Restless.Toolkit.Controls;
using System;
using System.Collections.Generic;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents view models that have been cached for reuse.
    /// </summary>
    public class ViewModelCache : List<ApplicationViewModel>
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelCache"/> class.
        /// </summary>
        public ViewModelCache()
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets the <see cref="ApplicationViewModel"/> specified by <see cref="NavigatorItem"/>
        /// from cache. If it doesn't exists, creates it and caches it first.
        /// </summary>
        /// <param name="navItem">The navigation item.</param>
        /// <returns>An <see cref="ApplicationViewModel"/> object.</returns>
        public ApplicationViewModel GetByNavigationItem(NavigatorItem navItem)
        {
            if (navItem == null)
            {
                throw new ArgumentNullException(nameof(navItem));
            }

            return GetStandardItem(navItem);
        }

        /// <summary>
        /// Signals all view models in the collection to save state
        /// </summary>
        public void SignalSave()
        {
            foreach (ApplicationViewModel item in this)
            {
                item.SignalSave();
            }
        }

        /// <summary>
        /// Signals all view models in the collection that they are closing.
        /// </summary>
        public void SignalClosing()
        {
            foreach (ApplicationViewModel item in this)
            {
                item.SignalClosing();
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private ApplicationViewModel GetStandardItem(NavigatorItem navItem)
        {
            foreach (ApplicationViewModel item in this)
            {
                if (item.GetType() == navItem.TargetType)
                {
                    return item;
                }
            }

            ApplicationViewModel createdItem = Activator.CreateInstance(navItem.TargetType) as ApplicationViewModel;
            Add(createdItem);
            return createdItem;
        }
        #endregion
    }
}