using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Provides enumeration values that describe how a custom sort behaves.
    /// </summary>
    public enum DataGridColumnSortBehavior
    {
        /// <summary>
        /// The order of the custom column is the same as the primary column.
        /// </summary>
        FollowPrimary,
        /// <summary>
        /// Not used. Reserved for future expansion.
        /// </summary>
        FollowPreceeding,
        /// <summary>
        /// The order of the custom column is the reverse of the order of the primary column.
        /// </summary>
        ReverseFollowPrimary,
        /// <summary>
        /// Not used. Reserved for future expansion.
        /// </summary>
        ReverseFollowPreceeding,
        /// <summary>
        /// The order of the custom column is always ascending.
        /// </summary>
        AlwaysAscending,
        /// <summary>
        /// The order of the custom column is always descending.
        /// </summary>
        AlwaysDescending,
    }
}
