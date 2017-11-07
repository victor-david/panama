using System;
using System.ComponentModel;
using System.Diagnostics;
using Restless.Tools.Threading;
using System.Threading;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represents the base class for all view models in the application. 
    /// Provides support for property change notifications. This class must be inherited.
    /// </summary>
    public abstract class ViewModelBase : BindableBase, IDisposable
    {
        #region Private Vars
        //private string notificationMessage;
        private string tabDisplayName;
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        protected ViewModelBase()
        {
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the display name for this instance.
        /// This appears in the commands list and in the workspace tab (unless TabDisplayName is set).
        /// </summary>
        public virtual string DisplayName 
        { 
            get; 
            protected set; 
        }

        /// <summary>
        /// Gets the tab display name for this instance. If not set, gets DisplayName.
        /// </summary>
        public string TabDisplayName
        {
            get
            {
                return !String.IsNullOrEmpty(tabDisplayName) ? tabDisplayName : DisplayName;
            }
            protected set
            {
                tabDisplayName = value;
            }
        }
        #endregion

        /************************************************************************/

        #region IDisposable Members
        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        /// Runs when the object is disposing. A derived class can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", GetType().Name, DisplayName, GetHashCode());
            System.Diagnostics.Debug.WriteLine(msg);
        }
#endif
        #endregion

        /************************************************************************/

        #region Private Methods
        #endregion

    }
}