using System.Windows;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides option values that may be passed to <see cref="BooleanToVisibilityConverter"/>
    /// that affect how the converter evaluates the passed boolean value.
    /// </summary>
    public enum BooleanToVisibilityConverterOptions
    {
        /// <summary>
        /// If the boolean values is true, return <see cref="Visibility.Visible"/>; otherwise, <see cref="Visibility.Collapsed"/>.
        /// </summary>
        TrueToVisibility,

        /// <summary>
        /// If the boolean values is false, return <see cref="Visibility.Visible"/>; otherwise, <see cref="Visibility.Collapsed"/>.
        /// </summary>
        FalseToVisibility,
    }
}