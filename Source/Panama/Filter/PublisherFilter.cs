using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Utility;

namespace Restless.App.Panama.Filter
{
    /// <summary>
    /// Represents the various filtering options that can be applied to the publisher list.
    /// </summary>
    public sealed class PublisherFilter : FilterBase
    {
        #region Private
        private FilterState inPeriod;
        private FilterState simultaneous;
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
                    Simultaneous != FilterState.Either ||
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
            get { return inPeriod; }
            set 
            { 
                inPeriod = value;
                OnPropertyChanged("InPeriod");
                OnChanged();
            }
        }

        /// <summary>
        /// Gets or sets the filter's simultaneous option. 
        /// </summary>
        public FilterState Simultaneous
        {
            get { return simultaneous; }
            set
            {
                simultaneous = value;
                OnPropertyChanged("Simultaneous");
                OnChanged();
            }
        }

        /// <summary>
        /// Gets or sets the filter's paying option. 
        /// </summary>
        public FilterState Paying
        {
            get { return paying; }
            set
            {
                paying = value;
                OnPropertyChanged("Paying");
                OnChanged();
            }
        }

        /// <summary>
        /// Gets or sets the filter's followup option. 
        /// </summary>
        public FilterState Followup
        {
            get { return followup; }
            set
            {
                followup = value;
                OnPropertyChanged("Followup");
                OnChanged();
            }
        }


        /// <summary>
        /// Gets or sets the filter's goner option. 
        /// </summary>
        public FilterState Goner
        {
            get { return goner; }
            set
            {
                goner = value;
                OnPropertyChanged("Goner");
                OnChanged();
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
            Simultaneous = FilterState.Either;
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
