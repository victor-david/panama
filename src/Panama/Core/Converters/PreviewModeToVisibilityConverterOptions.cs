using System.Windows;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides option values that may be passed to <see cref="PreviewModeToVisibilityConverter"/>
    /// that affect how the converter evaluates the passed PreviewMode value.
    /// </summary>
    public enum PreviewModeToVisibilityConverterOptions
    {
        /// <summary>
        /// If the value is PreviewMode.Text, return <see cref="Visibility.Visible"/>; otherwise, <see cref="Visibility.Collapsed"/>.
        /// </summary>
        TextToVisibility,

        /// <summary>
        /// If the value is PreviewMode.Image, return <see cref="Visibility.Visible"/>; otherwise, <see cref="Visibility.Collapsed"/>.
        /// </summary>
        ImageToVisibility,

        /// <summary>
        /// If the value is PreviewMode.Unsupported, return <see cref="Visibility.Visible"/>; otherwise, <see cref="Visibility.Collapsed"/>.
        /// </summary>
        UnsupportedToVisibility,
    }
}