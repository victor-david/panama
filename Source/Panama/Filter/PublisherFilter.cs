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
                    InPeriod != FilterState.Either ||
                    Exclusive != FilterState.Either ||
                    Paying != FilterState.Either ||
                    Followup != FilterState.Either ||
                    Goner != FilterState.Either;
            }
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
        #endregion

        /************************************************************************/
        
        #region Constructor
        #pragma warning disable 1591
        public PublisherFilter()
        {
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
            InPeriod = FilterState.Either;
            Exclusive = FilterState.Either;
            Paying = FilterState.Either;
            Followup = FilterState.Either;
            Goner = FilterState.Either;
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
