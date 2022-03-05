/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System.Windows.Input;
using System.Windows.Media;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Extends CommandViewModel to provide a specialized view of actionable tags.
    /// </summary>
    /// <remarks>
    /// This class is used to provide actionable tag items and is used when selecting tags to be associated
    /// with a title, and for selecting tags to be used when filtering the title view by tag.
    /// </remarks>
    public class TagCommandViewModel : ApplicationViewModel
    {
        #region Private
        private SolidColorBrush foreground;
        #endregion

        /************************************************************************/

        #region Public Properties
        /// <summary>
        /// Gets the tag id associated with this command view.
        /// </summary>
        public long TagId
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tag name associated with this command view.
        /// </summary>
        public string TagName => DisplayName;

        /// <summary>
        /// Gets the tag description associated with this command view.
        /// </summary>
        public string TagDescription => "Need tool tip text";

        /// <summary>
        /// Gets or sets the foreground color for this command view
        /// </summary>
        public SolidColorBrush Foreground
        {
            get => foreground;
            set => SetProperty(ref foreground, value);
        }
        #endregion

        /************************************************************************/

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TagCommandViewModel"/> class.
        /// </summary>
        /// <param name="tagId">The tag id.</param>
        /// <param name="tagName">The name of the tag.</param>
        /// <param name="tagDescription">The description of the tag.</param>
        /// <param name="command">The command associated with the selection of this tag.</param>
        public TagCommandViewModel(long tagId, string tagName, string tagDescription, ICommand command) : base()
//            :base(tagName, tagDescription, command, DefaultMinWidth)
        {
            // TODO (constructor)
            TagId = tagId;
            ResetDefaultForeground();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Highlights the item by setting the Foreground property
        /// </summary>
        /// <param name="brush">The brush to use, or null to use the default brush</param>
        public void Highlight(System.Windows.Media.SolidColorBrush brush = null)
        {
            if (brush != null)
            {
                Foreground = brush;
            }
            else
            {
                Foreground = new System.Windows.Media.SolidColorBrush(Colors.Red);
            }
        }

        /// <summary>
        /// Resets the Foreground property to its defaul value (Colors.MidnightBlue)
        /// </summary>
        public void ResetDefaultForeground()
        {
            Foreground = new SolidColorBrush(Colors.MidnightBlue);
        }
        #endregion

    }
}