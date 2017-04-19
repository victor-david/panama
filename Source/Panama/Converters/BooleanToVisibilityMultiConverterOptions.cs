using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides option values that may be passed to <see cref="BooleanToVisibilityMultiConverter"/>
    /// that affect how the converter evaluates the passed boolean values.
    /// </summary>
    public enum BooleanToVisibilityMultiConverterOptions
    {
        /// <summary>
        /// If the first boolean value is true, or the second one is true, returns <see cref="Visibility.Collapsed"/>;
        /// otherwise, <see cref="Visibility.Visible"/>.
        /// </summary>
        OneTrueOrTwoTrue,

        /// <summary>
        /// If the first boolean value is true, and the second one is true, returns <see cref="Visibility.Collapsed"/>;
        /// otherwise, <see cref="Visibility.Visible"/>.
        /// </summary>
        OneTrueAndTwoTrue,

        /// <summary>
        /// If the first boolean value is false, or the second one is true, returns <see cref="Visibility.Collapsed"/>;
        /// otherwise, <see cref="Visibility.Visible"/>.
        /// </summary>
        OneFalseOrTwoTrue

    }
}
