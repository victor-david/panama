using System;
using System.ComponentModel;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents the event args for a search result even
    /// </summary>
    public class SearchResultEventArgs : CancelEventArgs
    {
        #region Public properties
        /// <summary>
        /// Gets the result associated with the event.
        /// </summary>
        public WindowsSearchResult Result
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor (internal)
        /// <summary>
        /// Creates a new instance of this class
        /// </summary>
        /// <param name="result">The search result</param>
        internal SearchResultEventArgs(WindowsSearchResult result)
        {
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }
        #endregion
    }
}
