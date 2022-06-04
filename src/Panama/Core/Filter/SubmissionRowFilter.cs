using Restless.Toolkit.Controls;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides filtering capabilities for submissionr rows
    /// </summary>
    public class SubmissionRowFilter : RowFilter
    {
        #region Private
        private readonly Dictionary<SubmissionRowFilterType, SubmissionFilterEvaluator> filterEvaluators;
        private ThreeWayState activeState;
        private ThreeWayState tryAgainState;
        private ThreeWayState personalState;
        private ThreeWayState acceptedState;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        protected override bool IsIdFilterSupported => true;

        /// <inheritdoc/>
        public override bool IsAnyFilterActive => base.IsAnyFilterActive || IsAnyEvaluatorActive();

        /// <summary>
        /// Gets or sets the filter state for whether a submission is active (no response)
        /// </summary>
        public ThreeWayState ActiveState
        {
            get => activeState;
            set
            {
                SetProperty(ref activeState, value);
                SetFilterEvaluatorState(SubmissionRowFilterType.Active, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a submission has a try again response
        /// </summary>
        public ThreeWayState TryAgainState
        {
            get => tryAgainState;
            set
            {
                SetProperty(ref tryAgainState, value);
                SetFilterEvaluatorState(SubmissionRowFilterType.TryAgain, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a submission has a personal response
        /// </summary>
        public ThreeWayState PersonalState
        {
            get => personalState;
            set
            {
                SetProperty(ref personalState, value);
                SetFilterEvaluatorState(SubmissionRowFilterType.Personal, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a submission has an accepted response
        /// </summary>
        public ThreeWayState AcceptedState
        {
            get => acceptedState;
            set
            {
                SetProperty(ref acceptedState, value);
                SetFilterEvaluatorState(SubmissionRowFilterType.Accepted, value);
                ApplyFilter();
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionRowFilter"/> class
        /// </summary>
        public SubmissionRowFilter()
        {
            filterEvaluators = new Dictionary<SubmissionRowFilterType, SubmissionFilterEvaluator>()
            {
                { SubmissionRowFilterType.Id, new SubmissionFilterEvaluator(this, SubmissionRowFilterType.Id) },
                { SubmissionRowFilterType.Active, new SubmissionFilterEvaluator(this, SubmissionRowFilterType.Active) },
                { SubmissionRowFilterType.TryAgain, new SubmissionFilterEvaluator(this, SubmissionRowFilterType.TryAgain) },
                { SubmissionRowFilterType.Personal, new SubmissionFilterEvaluator(this, SubmissionRowFilterType.Personal) },
                { SubmissionRowFilterType.Accepted, new SubmissionFilterEvaluator(this, SubmissionRowFilterType.Accepted) }
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
        /// Sets <see cref="TryAgainState"/> to on, clearing all other filters
        /// </summary>
        public void SetToTryAgain()
        {
            SetCustomPropertyState(() => TryAgainState = ThreeWayState.On);
        }

        /// <summary>
        /// Sets <see cref="PersonalState"/> to on, clearing all other filters
        /// </summary>
        public void SetToPersonal()
        {
            SetCustomPropertyState(() => PersonalState = ThreeWayState.On);
        }

        /// <summary>
        /// Sets <see cref="AcceptedState"/> to on, clearing all other filters
        /// </summary>
        public void SetToAccepted()
        {
            SetCustomPropertyState(() => AcceptedState = ThreeWayState.On);
        }

        /// <inheritdoc/>
        public override bool OnDataRowFilter(DataRow item)
        {
            return
                filterEvaluators[SubmissionRowFilterType.Id].Evaluate(item) &&
                filterEvaluators[SubmissionRowFilterType.Active].Evaluate(item) &&
                filterEvaluators[SubmissionRowFilterType.TryAgain].Evaluate(item) &&
                filterEvaluators[SubmissionRowFilterType.Personal].Evaluate(item) &&
                filterEvaluators[SubmissionRowFilterType.Accepted].Evaluate(item);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void SetFilterEvaluatorState(SubmissionRowFilterType key, ThreeWayState state)
        {
            if (filterEvaluators != null)
            {
                filterEvaluators[key].SetState(state);
            }
        }

        private void ClearAllPropertyState()
        {
            ActiveState = ThreeWayState.Neutral;
            TryAgainState = ThreeWayState.Neutral;
            PersonalState = ThreeWayState.Neutral;
            AcceptedState = ThreeWayState.Neutral;
        }

        private bool IsAnyEvaluatorActive()
        {
            if (filterEvaluators != null)
            {
                foreach (KeyValuePair<SubmissionRowFilterType, SubmissionFilterEvaluator> pair in filterEvaluators)
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