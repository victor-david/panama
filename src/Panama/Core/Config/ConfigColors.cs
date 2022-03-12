/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows.Media;
using SystemColors = System.Windows.Media.Colors;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides services for application color management.
    /// </summary>
    public class ConfigColors
    {
        #region Default values
        /// <summary>
        /// Provides static default values for properties.
        /// </summary>
        private static class Values
        {
            public static Color DataGridAlternationForeground = SystemColors.Transparent;
            public static Color DataGridAlternationBackground = (Color)ColorConverter.ConvertFromString("#FFCBE4EC");

            public static Color PublisherGonerForeground = SystemColors.OrangeRed;
            public static Color PublisherGonerBackground = SystemColors.Transparent;

            public static Color PublisherPeriodForeground = SystemColors.Blue;
            public static Color PublisherPeriodBackground = SystemColors.Transparent;

            public static Color TitlePublishedForeground = SystemColors.Transparent;
            public static Color TitlePublishedBackground = SystemColors.PaleGreen;

            public static Color TitleSelfPublishedForeground = SystemColors.Firebrick;
            public static Color TitleSelfPublishedBackground = SystemColors.Transparent;

            public static Color TitleSubmittedForeground = SystemColors.White;
            public static Color TitleSubmittedBackground = SystemColors.SeaGreen;
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
        }

        /// <summary>
        /// Get the color object used to display a publisher that has been flagged as a goner.
        /// </summary>
        public ConfigColor PublisherGoner
        {
            get;
        }

        /// <summary>
        /// Get the color object used to display a publisher that is within its submission period.
        /// </summary>
        public ConfigColor PublisherPeriod
        {
            get;
        }

        /// <summary>
        /// Get the color object used to display a title that has been published.
        /// </summary>
        public ConfigColor TitlePublished
        {
            get;
        }

        /// <summary>
        /// Get the color object used to display a title that has been self published.
        /// </summary>
        public ConfigColor TitleSelfPublished
        {
            get;
        }

        /// <summary>
        /// Get the color object used to display a title that is currently submitted.
        /// </summary>
        public ConfigColor TitleSubmitted
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigColors"/> class.
        /// </summary>
        internal ConfigColors()
        {
            DataGridAlternation = new ConfigColor(nameof(DataGridAlternation), Values.DataGridAlternationForeground, Values.DataGridAlternationBackground);
            PublisherGoner = new ConfigColor(nameof(PublisherGoner), Values.PublisherGonerForeground, Values.PublisherGonerBackground);
            PublisherPeriod = new ConfigColor(nameof(PublisherPeriod), Values.PublisherPeriodForeground, Values.PublisherPeriodBackground);
            TitlePublished = new ConfigColor(nameof(TitlePublished), Values.TitlePublishedForeground, Values.TitlePublishedBackground);
            TitleSelfPublished = new ConfigColor(nameof(TitleSelfPublished), Values.TitleSelfPublishedForeground, Values.TitleSelfPublishedBackground);
            TitleSubmitted = new ConfigColor(nameof(TitleSubmitted), Values.TitleSubmittedForeground, Values.TitleSubmittedBackground);
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
            TitleSelfPublished.ResetToDefault();
            TitleSubmitted.ResetToDefault();
        }
        #endregion
    }
}