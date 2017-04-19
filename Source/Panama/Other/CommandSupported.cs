using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restless.App.Panama
{
    /// <summary>
    /// Provides enumeration values that describe whether a command is supported.
    /// </summary>
    public enum CommandSupported
    {
        /// <summary>
        /// Yes. The command is supported.
        /// </summary>
        Yes,
        /// <summary>
        /// No. The command is not supported.
        /// </summary>
        No,
        /// <summary>
        /// No. The command is not supported and should throw an exception if attempted.
        /// </summary>
        NoWithException
    }
}
