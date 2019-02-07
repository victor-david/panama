/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
namespace Restless.App.Panama.Filter
{
    /// <summary>
    /// Represents the various filtering options that can be applied to the publisher list.
    /// </summary>
    public sealed class PublisherFilter : FilterBase
    {
        #region Private
        private FilterState inPeriod;
        private FilterState exclusive;
        private FilterState paying;
        private FilterState followup;
        private FilterState goner;
        private FilterState haveSubmission;
        #endregion

        /************************************************************************/
        
        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if any filter is active.
        /// </summary>
        public override bool IsAnyFilterActive
        {
            get => base.IsAnyFilterActive || 
                   InPeriod != FilterState.Either ||
                   Exclusive != FilterState.Either ||
                   Paying != FilterState.Either ||
                   Followup != FilterState.Either ||
                   Goner != FilterState.Either ||
                   HaveSubmission != FilterState.Either;
        }

        /// <summary>
        /// Gets or sets the filter's in period option. 
        /// </summary>
        public FilterState InPeriod
        {
            get => inPeriod;
            set 
            {
                if (SetProperty(ref inPeriod, value))
                {
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter's simultaneous option. 
        /// </summary>
        public FilterState Exclusive
        {
            get => exclusive;
            set
            {
                if (SetProperty(ref exclusive, value))
                {
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter's paying option. 
        /// </summary>
        public FilterState Paying
        {
            get => paying;
            set
            {
                if (SetProperty(ref paying, value))
                {
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter's followup option. 
        /// </summary>
        public FilterState Followup
        {
            get => followup;
            set
            {
                if (SetProperty(ref followup, value))
                {
                    OnChanged();
                }
            }
        }


        /// <summary>
        /// Gets or sets the filter's goner option. 
        /// </summary>
        public FilterState Goner
        {
            get => goner;
            set
            {
                if (SetProperty(ref goner, value))
                {
                    OnChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the filter's have submission option. 
        /// </summary>
        public FilterState HaveSubmission
        {
            get => haveSubmission;
            set
            {
                if (SetProperty(ref haveSubmission, value))
                {
                    OnChanged();
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherFilter"/> class.
        /// </summary>
        public PublisherFilter()
        {
            Reset();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Resets all filter properties to their default values;
        /// </summary>
        public override void Reset()
        {
            IsChangedEventSuspended = true;
            InPeriod = FilterState.Either;
            Exclusive = FilterState.Either;
            Paying = FilterState.Either;
            Followup = FilterState.Either;
            Goner = FilterState.Either;
            HaveSubmission = FilterState.Either;
            base.Reset();
            IsChangedEventSuspended = false;
            OnChanged();
        }
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion


    }
}