﻿using Restless.Tools.Utility;
using System;
using System.Linq;

namespace Restless.App.Panama.Filter
{
    /// <summary>
    /// Represents the various filtering options that can be applied to the title list.
    /// </summary>
    public sealed class TitleFilter : FilterBase
    {
        #region Private
        private FilterState everSubmitted;
        private FilterState published;
        private FilterState ready;
        private FilterState submitted;
        private TagFilterCombine tagCombine;
        private int wordCount;
        private string folder;
        #endregion

        /************************************************************************/
        
        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if any filter is active.
        /// </summary>
        public override bool IsAnyFilterActive
        {
            get
            {
                return
                    base.IsAnyFilterActive ||
                    EverSubmitted != FilterState.Either ||
                    Published != FilterState.Either ||
                    Ready != FilterState.Either ||
                    Submitted != FilterState.Either ||
                    !String.IsNullOrEmpty(Folder) ||
                    WordCount != 0 ||
                    Tags.Count > 0;
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates if the tag portion of the filter has any values.
        /// </summary>
        public bool IsTagFilterActive
        {
            get { return Tags.Count > 0; }
        }

        /// <summary>
        /// Gets or sets the filter's ever submitted option. 
        /// </summary>
        public FilterState EverSubmitted
        {
            get { return everSubmitted; }
            set 
            {
                if (SetProperty(ref everSubmitted, value))
                {
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter's published option. 
        /// </summary>
        public FilterState Published
        {
            get { return published; }
            set
            {
                if (SetProperty(ref published, value))
                {
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter's ready option. 
        /// </summary>
        public FilterState Ready
        {
            get { return ready; }
            set
            {
                if (SetProperty(ref ready, value))
                {
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter's submitted option. 
        /// </summary>
        public FilterState Submitted
        {
            get { return submitted; }
            set
            {
                if (SetProperty(ref submitted, value))
                {
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter's folder specification.
        /// This property enables filtering by the location of the latest version.
        /// </summary>
        public string Folder
        {
            get { return folder; }
            set
            {
                if (SetProperty(ref folder, value))
                {
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter's word count specification.
        /// This property enables filtering by the stored word count of the latest version.
        /// </summary>
        /// <remarks>
        /// This filter does not read the word count of titles in real time. If the text
        /// of titles has changed, you must run the meta data update tool that scans the
        /// file system documents to obtain their word count, and updates the associated
        /// version rows.
        /// </remarks>
        public int WordCount
        {
            get { return wordCount; }
            set
            {
                if (SetProperty(ref wordCount, value))
                {
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter's tag combine option. 
        /// </summary>
        public TagFilterCombine TagCombine
        {
            get { return tagCombine; }
            set
            {
                if (SetProperty(ref tagCombine, value))
                {
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets the filter's included tags options.
        /// </summary>
        public Integer64List Tags
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/
        
        #region Constructor
        #pragma warning disable 1591
        public TitleFilter()
        {
            Tags = new Integer64List();
            Reset();
        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Resets all filter properties to their default values;
        /// </summary>
        public override void Reset()
        {
            IsChangedEventSuspended = true;
            Tags.Clear();
            EverSubmitted = FilterState.Either;
            Published = FilterState.Either;
            Ready = FilterState.Either;
            Submitted = FilterState.Either;
            TagCombine = TagFilterCombine.And;
            Folder = null;
            WordCount = 0;
            base.Reset();
            IsChangedEventSuspended = false;
            OnChanged();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Raises the <see cref="FilterBase.Changed"/> event.
        /// </summary>
        protected override void OnChanged()
        {
            base.OnChanged();
            OnPropertyChanged(nameof(IsTagFilterActive));
        }
        #endregion
    }
}
