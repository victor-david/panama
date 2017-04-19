using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides options that may be used when cleaning a string with <see cref="StringToCleanStringConverter"/>.
    /// </summary>
    [Flags]
    public enum StringToCleanStringOptions
    {
        /// <summary>
        /// No options have been specified
        /// </summary>
        None = 0,
        /// <summary>
        /// Interior white space will compacted, two spaces become one, etc.
        /// </summary>
        TrimInterior = 1,
        /// <summary>
        /// Html will be removed.
        /// </summary>
        RemoveHtml = 2,
        /// <summary>
        /// All options.
        /// </summary>
        All = TrimInterior + RemoveHtml,
    }
}
