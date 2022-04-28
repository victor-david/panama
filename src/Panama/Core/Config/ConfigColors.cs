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
            public static readonly Color DataGridAlternationForeground = SystemColors.Transparent;
            public static readonly Color DataGridAlternationBackground = (Color)ColorConverter.ConvertFromString("#FFCBE4EC");

            public static readonly Color TitleReadyDefault = SystemColors.Green;
            public static readonly Color TitleFlaggedDefault = SystemColors.Blue;
            public static readonly Color TitlePublishedDefault = SystemColors.Red;
            public static readonly Color TitleSelfPublishedDefault = SystemColors.Coral;
            public static readonly Color TitleSubmittedDefault = SystemColors.Black;

            public static readonly Color PublisherExclusiveDefault = SystemColors.Red;
            public static readonly Color PublisherPayingDefault = SystemColors.Green;
            public static readonly Color PublisherGonerDefault = SystemColors.Gray;
            public static readonly Color PublisherActiveSubmissionDefault = SystemColors.RoyalBlue;
            public static readonly Color PublisherPeriodDefault = SystemColors.Coral;

            public static readonly Color SubmissionOnlineDefault = SystemColors.Green;
            public static readonly Color SubmissionContestDefault = SystemColors.Gray;
            public static readonly Color SubmissionLockedDefault = SystemColors.MediumVioletRed;

            public static readonly Color PublisherGonerForeground = SystemColors.OrangeRed;
            public static readonly Color PublisherGonerBackground = SystemColors.Transparent;

            //public static Color PublisherPeriodForeground = SystemColors.Blue;
            //public static Color PublisherPeriodBackground = SystemColors.Transparent;

            //public static Color TitlePublishedForeground = SystemColors.Transparent;
            //public static Color TitlePublishedBackground = SystemColors.PaleGreen;

            //public static Color TitleSelfPublishedForeground = SystemColors.Firebrick;
            //public static Color TitleSelfPublishedBackground = SystemColors.Transparent;

            //public static Color TitleSubmittedForeground = SystemColors.White;
            //public static Color TitleSubmittedBackground = SystemColors.SeaGreen;
        }
        #endregion

        /************************************************************************/

        #region Public properties
        public ConfigColor DataGridAlternation { get; }
        public ConfigColor TitleReady { get; }
        public ConfigColor TitleFlagged { get; }
        public ConfigColor TitlePublished { get; }
        public ConfigColor TitleSelfPublished { get; }
        public ConfigColor TitleSubmitted { get; }

        public ConfigColor PublisherExclusive { get; }
        public ConfigColor PublisherPaying { get; }
        public ConfigColor PublisherGoner { get; }
        public ConfigColor PublisherActiveSubmission { get; }
        public ConfigColor PublisherPeriod { get; }

        public ConfigColor SubmissionOnline { get; }
        public ConfigColor SubmissionContest { get; }
        public ConfigColor SubmissionLocked { get; }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigColors"/> class.
        /// </summary>
        internal ConfigColors()
        {
            DataGridAlternation = new ConfigColor(nameof(DataGridAlternation), Values.DataGridAlternationForeground, Values.DataGridAlternationBackground);

            TitleReady = new ConfigColor(nameof(TitleReady), SystemColors.Transparent, Values.TitleReadyDefault);
            TitleFlagged = new ConfigColor(nameof(TitleFlagged), SystemColors.Transparent, Values.TitleFlaggedDefault);
            TitlePublished = new ConfigColor(nameof(TitlePublished), SystemColors.Transparent, Values.TitlePublishedDefault);
            TitleSelfPublished = new ConfigColor(nameof(TitleSelfPublished), SystemColors.Transparent, Values.TitleSelfPublishedDefault);
            TitleSubmitted = new ConfigColor(nameof(TitleSubmitted), SystemColors.Transparent, Values.TitleSubmittedDefault);

            PublisherExclusive = new ConfigColor(nameof(PublisherExclusive), SystemColors.Transparent, Values.PublisherExclusiveDefault);
            PublisherPaying = new ConfigColor(nameof(PublisherPaying), SystemColors.Transparent, Values.PublisherPayingDefault);
            PublisherGoner = new ConfigColor(nameof(PublisherGoner), SystemColors.Transparent, Values.PublisherGonerDefault);
            PublisherActiveSubmission = new ConfigColor(nameof(PublisherActiveSubmission), SystemColors.Transparent, Values.PublisherActiveSubmissionDefault);
            PublisherPeriod = new ConfigColor(nameof(PublisherPeriod), SystemColors.Transparent, Values.PublisherPeriodDefault);

            SubmissionOnline = new ConfigColor(nameof(SubmissionOnline), SystemColors.Transparent, Values.SubmissionOnlineDefault);
            SubmissionContest = new ConfigColor(nameof(SubmissionContest), SystemColors.Transparent, Values.SubmissionContestDefault);
            SubmissionLocked = new ConfigColor(nameof(SubmissionLocked), SystemColors.Transparent, Values.SubmissionLockedDefault);
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

            TitleReady.ResetToDefault();
            TitleFlagged.ResetToDefault();
            TitlePublished.ResetToDefault();
            TitleSelfPublished.ResetToDefault();
            TitleSubmitted.ResetToDefault();

            PublisherExclusive.ResetToDefault();
            PublisherPaying.ResetToDefault();
            PublisherGoner.ResetToDefault();
            PublisherActiveSubmission.ResetToDefault();
            PublisherPeriod.ResetToDefault();

            SubmissionOnline.ResetToDefault();
            SubmissionContest.ResetToDefault();
            SubmissionLocked.ResetToDefault();
        }
        #endregion
    }
}