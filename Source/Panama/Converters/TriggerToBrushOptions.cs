using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using Restless.App.Panama.Configuration;

namespace Restless.App.Panama.Converters
{
    /// <summary>
    /// Provides option values that may be passed to <see cref="TriggerToBrushConverter"/>
    /// that determine which user brush the converter returns.
    /// </summary>
    public enum TriggerToBrushOptions
    {
        /// <summary>
        /// Specifies that a default brush will be returned.
        /// </summary>
        None,

        /// <summary>
        /// Specifies that the returned brush is created from the background color of user option <see cref="Config.Colors.DataGridAlternation"/>.
        /// </summary>
        DataGridAlternation,

        /// <summary>
        /// Specifies that the returned brush is created from the foreground color of user option <see cref="Config.Colors.TitlePublished"/>.
        /// </summary>
        TitlePublishedFore,

        /// <summary>
        /// Specifies that the returned brush is created from the background color of user option <see cref="Config.Colors.TitlePublished"/>.
        /// </summary>
        TitlePublishedBack,

        /// <summary>
        /// Specifies that the returned brush is created from the foreground color user option <see cref="Config.Colors.TitleSubmitted"/>.
        /// </summary>
        TitleSubmittedFore,

        /// <summary>
        /// Specifies that the returned brush is created from the background color user option <see cref="Config.Colors.TitleSubmitted"/>.
        /// </summary>
        TitleSubmittedBack,


        /// <summary>
        /// Specifies that the returned brush is created from the foreground color user option <see cref="Config.Colors.PublisherPeriod"/>.
        /// </summary>
        PublisherPeriodFore,

        /// <summary>
        /// Specifies that the returned brush is created from the background color user option <see cref="Config.Colors.PublisherPeriod"/>.
        /// </summary>
        PublisherPeriodBack,

        /// <summary>
        /// Specifies that the returned brush is created from the foreground color of user option <see cref="Config.Colors.PublisherGoner"/>.
        /// </summary>
        PublisherGonerFore,

        /// <summary>
        /// Specifies that the returned brush is created from the background color of user option <see cref="Config.Colors.PublisherGoner"/>.
        /// </summary>
        PublisherGonerBack,

    }
}
