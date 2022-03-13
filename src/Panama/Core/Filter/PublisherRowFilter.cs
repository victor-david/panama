using Restless.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.Panama.Core
{
    public class PublisherRowFilter : RowFilter
    {
        #region Private
        private ThreeWayState activeState;
        private ThreeWayState openState;
        private ThreeWayState inPeriodState;
        private ThreeWayState exclusiveState;
        private ThreeWayState followUpState;
        private ThreeWayState payingState;
        private ThreeWayState gonerState;
        #endregion

        #region Properties
        /// <inheritdoc/>
        public override bool IsAnyFilterActive => base.IsAnyFilterActive || IsAnyEvaluatorActive();

        /// <summary>
        /// Gets or sets the filter state for whether a publisher is active (non a goner)
        /// </summary>
        public ThreeWayState ActiveState
        {
            get => activeState;
            set
            {
                SetProperty(ref activeState, value);
                //SetFilterEvaluatorState(TitleRowFilterType.Ready, value);
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
                //SetFilterEvaluatorState(TitleRowFilterType.Ready, value);
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
                //SetFilterEvaluatorState(TitleRowFilterType.Ready, value);
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
                //SetFilterEvaluatorState(TitleRowFilterType.Ready, value);
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
                //SetFilterEvaluatorState(TitleRowFilterType.Ready, value);
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
                //SetFilterEvaluatorState(TitleRowFilterType.Ready, value);
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
                //SetFilterEvaluatorState(TitleRowFilterType.Ready, value);
                ApplyFilter();
            }
        }
        #endregion


        /************************************************************************/

        public PublisherRowFilter()
        {

        }


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

        public void SetToActive()
        {
            SetCustomPropertyState(() => ActiveState = ThreeWayState.On);
        }

        public void SetToOpenSubmission()
        {
            SetCustomPropertyState(() => OpenState = ThreeWayState.On);
        }

        public void SetToInPeriod()
        {
            SetCustomPropertyState(() => InPeriodState = ThreeWayState.On);
        }

        public void SetToExclusive()
        {
            SetCustomPropertyState(() => ExclusiveState = ThreeWayState.On);
        }

        public void SetToFollowup()
        {
            SetCustomPropertyState(() => FollowUpState = ThreeWayState.On);
        }

        public void SetToPaying()
        {
            SetCustomPropertyState(() => PayingState = ThreeWayState.On);
        }

        public void SetToGoner()
        {
            SetCustomPropertyState(() => GonerState = ThreeWayState.On);
        }

        public override bool OnDataRowFilter(DataRow item)
        {
            return true;
        }
        #endregion

        /************************************************************************/

        #region Private methods
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
            //if (filterEvaluators != null)
            //{
            //    foreach (KeyValuePair<TitleRowFilterType, TitleFilterEvaluator> pair in filterEvaluators)
            //    {
            //        if (filterEvaluators[pair.Key].IsActive)
            //        {
            //            return true;
            //        }
            //    }
            //}
            return true; // TODO - false
        }

        #endregion
    }
}
