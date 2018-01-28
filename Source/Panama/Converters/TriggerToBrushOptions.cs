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
        /// Specifies that the returned brush is created from user option <see cref="Config.ColorPublishedTitle"/>.
        /// </summary>
        Published,

        /// <summary>
        /// Specifies that the returned brush is created from user option <see cref="Config.ColorSubmittedTitle"/>.
        /// </summary>
        Submitted,

        /// <summary>
        /// Specifies that the returned brush is created from user option <see cref="Config.ColorPeriodPublisher"/>.
        /// </summary>
        Period,

        /// <summary>
        /// Specifies that the returned brush is created from user option <see cref="Config.ColorGonerPublisher"/>.
        /// </summary>
        Goner,

    }
}
