using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restless.App.Panama.Filter
{
    /// <summary>
    /// Provides enumeration values that describe how multiple tag selections in a title filter are combined.
    /// </summary>
    public enum TagFilterCombine
    {
        /// <summary>
        /// Multiple tag selection are combined with OR.
        /// </summary>
        Or,
        /// <summary>
        /// Multiple tag selection are combined with AND.
        /// </summary>
        And,
        /// <summary>
        /// Multiple tag selection are combined with AND NOT.
        /// </summary>
        AndNot,

    }
}
