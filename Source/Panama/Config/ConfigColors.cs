using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using SystemColors = System.Windows.Media.Colors;

namespace Restless.App.Panama.Configuration
{
    /// <summary>
    /// Provides services for application color management.
    /// </summary>
    public class ConfigColors
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public fields
        /// <summary>
        /// Provides static default values for properties.
        /// </summary>
        public static class Default
        {
            /// <summary>
            /// Gets the default foreground color for a publisher that is marked as a goner.
            /// </summary>
            public static Color PublisherGonerFore = SystemColors.Gray;

            /// <summary>
            /// Gets the default background color for a publisher that is within its submission period.
            /// </summary>
            public static Color PublisherPeriodBack = SystemColors.Beige;

            /// <summary>
            /// Gets the default background color for a title that is published.
            /// </summary>
            public static Color TitlePublishedBack = SystemColors.PaleGreen;

            /// <summary>
            /// Gets the default background color for a title that is currently submitted.
            /// </summary>
            public static Color TitleSubmittedBack = SystemColors.SeaGreen;
        }
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Get the color object used for foreground display of a publisher that has been flagged as a goner.
        /// </summary>
        public ConfigColor PublisherGonerFore
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the color object used for background display of a publisher that is within its submission period.
        /// </summary>
        public ConfigColor PublisherPeriodBack
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the color object used for background display of a title that has been published.
        /// </summary>
        public ConfigColor TitlePublishedBack
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the color object used for background display of a title that is currently submitted.
        /// </summary>
        public ConfigColor TitleSubmittedBack
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigColors"/> class.
        /// </summary>
        internal ConfigColors()
        {
            PublisherGonerFore = new ConfigColor(nameof(PublisherGonerFore), Default.PublisherGonerFore, true);
            PublisherPeriodBack = new ConfigColor(nameof(PublisherPeriodBack), Default.PublisherPeriodBack, true);
            TitlePublishedBack = new ConfigColor(nameof(TitlePublishedBack), Default.TitlePublishedBack, true);
            TitleSubmittedBack = new ConfigColor(nameof(TitleSubmittedBack), Default.TitleSubmittedBack, true);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Resets all colors to their default values
        /// </summary>
        public void Reset()
        {
            PublisherGonerFore.ResetToDefault();
            PublisherPeriodBack.ResetToDefault();
            TitlePublishedBack.ResetToDefault();
            TitleSubmittedBack.ResetToDefault();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion
    }
}
