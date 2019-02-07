/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
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
            /// Provides static values for default foreground colors.
            /// </summary>
            public static class Foreground
            {
                /// <summary>
                /// Gets the default foreground color for an alternating data grid row.
                /// </summary>
                public static Color DataGridAlternation = SystemColors.Transparent;

                /// <summary>
                /// Gets the default foreground color for a publisher that is marked as a goner.
                /// </summary>
                public static Color PublisherGoner = SystemColors.Gray;

                /// <summary>
                /// Gets the default foreground color for a publisher that is within its submission period.
                /// </summary>
                public static Color PublisherPeriod = SystemColors.Blue;

                /// <summary>
                /// Gets the default foreground color for a title that is published.
                /// </summary>
                public static Color TitlePublished = SystemColors.Transparent;

                /// <summary>
                /// Gets the default foreground color for a title that is currently submitted.
                /// </summary>
                public static Color TitleSubmitted = SystemColors.White;
            }

            /// <summary>
            /// Provides static values for default background colors.
            /// </summary>
            public static class Background
            {
                /// <summary>
                /// Gets the default background color for an alternating data grid row.
                /// </summary>
                public static Color DataGridAlternation = (Color)ColorConverter.ConvertFromString("#FFCBE4EC");

                // #FFCBE4EC
                // #FFE3EBEE

                /// <summary>
                /// Gets the default background color for a publisher that is marked as a goner.
                /// </summary>
                public static Color PublisherGoner = SystemColors.Transparent;

                /// <summary>
                /// Gets the default background color for a publisher that is within its submission period.
                /// </summary>
                public static Color PublisherPeriod = SystemColors.Transparent;

                /// <summary>
                /// Gets the default background color for a title that is published.
                /// </summary>
                public static Color TitlePublished = SystemColors.PaleGreen;

                /// <summary>
                /// Gets the default background color for a title that is currently submitted.
                /// </summary>
                public static Color TitleSubmitted = SystemColors.SeaGreen;

            }
        }
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Get the color object used to display alternation rows in data grids.
        /// </summary>
        public ConfigColor DataGridAlternation
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the color object used to display a publisher that has been flagged as a goner.
        /// </summary>
        public ConfigColor PublisherGoner
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the color object used to display a publisher that is within its submission period.
        /// </summary>
        public ConfigColor PublisherPeriod
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the color object used to display a title that has been published.
        /// </summary>
        public ConfigColor TitlePublished
        {
            get;
            private set;
        }

        /// <summary>
        /// Get the color object used to display a title that is currently submitted.
        /// </summary>
        public ConfigColor TitleSubmitted
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
            DataGridAlternation = new ConfigColor(nameof(DataGridAlternation), Default.Foreground.DataGridAlternation, Default.Background.DataGridAlternation);
            PublisherGoner = new ConfigColor(nameof(PublisherGoner), Default.Foreground.PublisherGoner, Default.Background.PublisherGoner);
            PublisherPeriod = new ConfigColor(nameof(PublisherPeriod), Default.Foreground.PublisherPeriod, Default.Background.PublisherPeriod);
            TitlePublished = new ConfigColor(nameof(TitlePublished), Default.Foreground.TitlePublished, Default.Background.TitlePublished);
            TitleSubmitted = new ConfigColor(nameof(TitleSubmitted), Default.Foreground.TitleSubmitted, Default.Background.TitleSubmitted);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Resets all colors to their default values
        /// </summary>
        public void Reset()
        {
            DataGridAlternation.ResetToDefault();
            PublisherGoner.ResetToDefault();
            PublisherPeriod.ResetToDefault();
            TitlePublished.ResetToDefault();
            TitleSubmitted.ResetToDefault();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion
    }
}