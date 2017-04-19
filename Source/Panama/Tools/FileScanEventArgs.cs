using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Utility;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Represents the event arguments for file scan events.
    /// </summary>
    public class FileScanEventArgs : EventArgs
    {
        #region Public properties
        /// <summary>
        /// Gets the target associated with this event.
        /// </summary>
        public FileScanDisplayObject Target
        {
            get;
            private set;
        }
        #endregion
        
        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="FileScanEventArgs"/> class.
        /// </summary>
        /// <param name="target">The target</param>
        public FileScanEventArgs(FileScanDisplayObject target)
        {
            Validations.ValidateNull(target, "FileScanEventArgs.Target");
            Target = target;
        }
        #endregion
    }
}
