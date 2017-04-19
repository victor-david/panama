using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Restless.Tools.Utility;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="View.AboutWindow"/>.
    /// </summary>
    public class AboutWindowViewModel : WindowViewModel
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the application information object.
        /// </summary>
        public ApplicationInfo AppInfo
        {
            get;
            private set;
        }
        #endregion
        
        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutWindowViewModel"/> class.
        /// </summary>
        /// <param name="owner">The window that owns this view model.</param>
        public AboutWindowViewModel(Window owner)
            :base(owner)
        {
            AppInfo = ApplicationInfo.Instance;
        }
        #endregion
    }
}
