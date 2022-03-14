using Restless.Toolkit.Controls;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides filtering capabilities for publisher rows
    /// </summary>
    public class PublisherRowFilter : RowFilter
    {
        #region Private
        private readonly Dictionary<PublisherRowFilterType, PublisherFilterEvaluator> filterEvaluators;
        private ThreeWayState activeState;
        private ThreeWayState openState;
        private ThreeWayState inPeriodState;
        private ThreeWayState exclusiveState;
        private ThreeWayState followUpState;
        private ThreeWayState payingState;
        private ThreeWayState gonerState;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool IsAnyFilterActive => base.IsAnyFilterActive || IsAnyEvaluatorActive();

        /// <summary>
        /// Gets or sets the filter state for whether a publisher is active (not a goner)
        /// </summary>
        public ThreeWayState ActiveState
        {
            get => activeState;
            set
            {
                SetProperty(ref activeState, value);
                SetFilterEvaluatorState(PublisherRowFilterType.Active, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a publisher has an open submission
        /// </summary>
        public ThreeWayState OpenState
        {
            get => openState;
            set
            {
                SetProperty(ref openState, value);
                SetFilterEvaluatorState(PublisherRowFilterType.OpenSubmission, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a publisher is currently within one of their submission periods
        /// </summary>
        public ThreeWayState InPeriodState
        {
            get => inPeriodState;
            set
            {
                SetProperty(ref inPeriodState, value);
                SetFilterEvaluatorState(PublisherRowFilterType.InPeriod, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a publisher is flagged as exclusive (no simultaneous submissions)
        /// </summary>
        public ThreeWayState ExclusiveState
        {
            get => exclusiveState;
            set
            {
                SetProperty(ref exclusiveState, value);
                SetFilterEvaluatorState(PublisherRowFilterType.Exclusive, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a publisher is flagged for follow up
        /// </summary>
        public ThreeWayState FollowUpState
        {
            get => followUpState;
            set
            {
                SetProperty(ref followUpState, value);
                SetFilterEvaluatorState(PublisherRowFilterType.FollowUp, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a publisher is flagged as a paying market
        /// </summary>
        public ThreeWayState PayingState
        {
            get => payingState;
            set
            {
                SetProperty(ref payingState, value);
                SetFilterEvaluatorState(PublisherRowFilterType.Paying, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a publisher is flagged as a goner
        /// </summary>
        public ThreeWayState GonerState
        {
            get => gonerState;
            set
            {
                SetProperty(ref gonerState, value);
                SetFilterEvaluatorState(PublisherRowFilterType.Goner, value);
                ApplyFilter();
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherRowFilter"/> class
        /// </summary>
        public PublisherRowFilter()
        {
            filterEvaluators = new Dictionary<PublisherRowFilterType, PublisherFilterEvaluator>()
            {
                { PublisherRowFilterType.Active, new PublisherFilterEvaluator(this, PublisherRowFilterType.Active) },
                { PublisherRowFilterType.OpenSubmission, new PublisherFilterEvaluator(this, PublisherRowFilterType.OpenSubmission) },
                { PublisherRowFilterType.InPeriod, new PublisherFilterEvaluator(this, PublisherRowFilterType.InPeriod) },
                { PublisherRowFilterType.Exclusive, new PublisherFilterEvaluator(this, PublisherRowFilterType.Exclusive) },
                { PublisherRowFilterType.FollowUp, new PublisherFilterEvaluator(this, PublisherRowFilterType.FollowUp) },
                { PublisherRowFilterType.Paying, new PublisherFilterEvaluator(this, PublisherRowFilterType.Paying) },
                { PublisherRowFilterType.Goner, new PublisherFilterEvaluator(this, PublisherRowFilterType.Goner) },
            };
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Clears all filters
        /// </summary>
        public override void ClearAll()
        {
            IncreaseSuspendLevel();
            base.ClearAll();
            ClearAllPropertyState();
            DecreaseSuspendLevel();
        }

        /// <summary>
        /// Sets <see cref="ActiveState"/> to on, clearing all other filters
        /// </summary>
        public void SetToActive()
        {
            SetCustomPropertyState(() => ActiveState = ThreeWayState.On);
        }

        /// <summary>
        /// Sets <see cref="OpenState"/> to on, clearing all other filters
        /// </summary>
        public void SetToOpenSubmission()
        {
            SetCustomPropertyState(() => OpenState = ThreeWayState.On);
        }

        /// <summary>
        /// Sets <see cref="InPeriodState"/> to on, clearing all other filters
        /// </summary>
        public void SetToInPeriod()
        {
            SetCustomPropertyState(() => InPeriodState = ThreeWayState.On);
        }

        /// <summary>
        /// Sets <see cref="ExclusiveState"/> to on, clearing all other filters
        /// </summary>
        public void SetToExclusive()
        {
            SetCustomPropertyState(() => ExclusiveState = ThreeWayState.On);
        }

        /// <summary>
        /// Sets <see cref="FollowUpState"/> to on, clearing all other filters
        /// </summary>
        public void SetToFollowup()
        {
            SetCustomPropertyState(() => FollowUpState = ThreeWayState.On);
        }

        /// <summary>
        /// Sets <see cref="PayingState"/> to on, clearing all other filters
        /// </summary>
        public void SetToPaying()
        {
            SetCustomPropertyState(() => PayingState = ThreeWayState.On);
        }

        /// <summary>
        /// Sets <see cref="GonerState"/> to on, clearing all other filters
        /// </summary>
        public void SetToGoner()
        {
            SetCustomPropertyState(() => GonerState = ThreeWayState.On);
        }

        /// <inheritdoc/>
        public override bool OnDataRowFilter(DataRow item)
        {
            return
                filterEvaluators[PublisherRowFilterType.Active].Evaluate(item) &&
                filterEvaluators[PublisherRowFilterType.OpenSubmission].Evaluate(item) &&
                filterEvaluators[PublisherRowFilterType.InPeriod].Evaluate(item) &&
                filterEvaluators[PublisherRowFilterType.Exclusive].Evaluate(item) &&
                filterEvaluators[PublisherRowFilterType.FollowUp].Evaluate(item) &&
                filterEvaluators[PublisherRowFilterType.Paying].Evaluate(item) &&
                filterEvaluators[PublisherRowFilterType.Goner].Evaluate(item);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void SetFilterEvaluatorState(PublisherRowFilterType key, ThreeWayState state)
        {
            if (filterEvaluators != null)
            {
                filterEvaluators[key].SetState(state);
            }
        }

        private void ClearAllPropertyState()
        {
            ActiveState = ThreeWayState.Neutral;
            OpenState = ThreeWayState.Neutral;
            InPeriodState = ThreeWayState.Neutral;
            ExclusiveState = ThreeWayState.Neutral;
            FollowUpState = ThreeWayState.Neutral;
            PayingState = ThreeWayState.Neutral;
            GonerState = ThreeWayState.Neutral;
        }

        private bool IsAnyEvaluatorActive()
        {
            if (filterEvaluators != null)
            {
                foreach (KeyValuePair<PublisherRowFilterType, PublisherFilterEvaluator> pair in filterEvaluators)
                {
                    if (filterEvaluators[pair.Key].IsActive)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion
    }
}