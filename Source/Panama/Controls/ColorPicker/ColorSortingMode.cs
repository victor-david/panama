using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Provides an enumeration of the modes in which the color pallete may be sorted.
    /// </summary>
    public enum ColorSortingMode
    {
        /// <summary>
        /// Alphabetical by color name.
        /// </summary>
        Alpha,
        /// <summary>
        /// Hue, then saturation, then brightness.
        /// </summary>
        HSB
    }
}
