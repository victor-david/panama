using Restless.Toolkit.Controls;
using System.Collections.Generic;
using System.Data;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides filtering capabilities for title rows
    /// </summary>
    public class TitleRowFilter : RowFilter
    {
        #region Private
        private readonly Dictionary<TitleRowFilterType, TitleFilterEvaluator> filterEvaluators;
        private ThreeWayState readyState;
        private ThreeWayState flaggedState;
        private ThreeWayState currentlySubmittedState;
        private ThreeWayState everSubmittedState;
        private ThreeWayState publishedState;
        private ThreeWayState selfPublishedState;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        protected override bool IsIdFilterSupported => true;

        /// <inheritdoc/>
        protected override bool IsTextFilterSupported => false;

        /// <inheritdoc/>
        public override bool IsAnyFilterActive => base.IsAnyFilterActive || IsAnyEvaluatorActive();

        /// <summary>
        /// Gets or sets the filter state for whether a title is flagged as ready
        /// </summary>
        public ThreeWayState ReadyState
        {
            get => readyState;
            set
            {
                SetProperty(ref readyState, value);
                SetFilterEvaluatorState(TitleRowFilterType.Ready, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for wheter a title is flagged with the aux quick flag
        /// </summary>
        public ThreeWayState FlaggedState
        {
            get => flaggedState;
            set
            {
                SetProperty(ref flaggedState, value);
                SetFilterEvaluatorState(TitleRowFilterType.Flagged, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a title currently submitted
        /// </summary>
        public ThreeWayState CurrentlySubmittedState
        {
            get => currentlySubmittedState;
            set
            {
                SetProperty(ref currentlySubmittedState, value);
                SetFilterEvaluatorState(TitleRowFilterType.CurrentlySubmitted, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a title has ever been submitted
        /// </summary>
        public ThreeWayState EverSubmittedState
        {
            get => everSubmittedState;
            set
            {
                SetProperty(ref everSubmittedState, value);
                SetFilterEvaluatorState(TitleRowFilterType.EverSubmitted, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a title is published
        /// </summary>
        public ThreeWayState PublishedState
        {
            get => publishedState;
            set
            {
                SetProperty(ref publishedState, value);
                SetFilterEvaluatorState(TitleRowFilterType.Published, value);
                ApplyFilter();
            }
        }

        /// <summary>
        /// Gets or sets the filter state for whether a title is self published
        /// </summary>
        public ThreeWayState SelfPublishedState
        {
            get => selfPublishedState;
            set
            {
                SetProperty(ref selfPublishedState, value);
                SetFilterEvaluatorState(TitleRowFilterType.SelfPublished, value);
                ApplyFilter();
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleRowFilter"/> class.
        /// </summary>
        public TitleRowFilter()
        {
            filterEvaluators = new Dictionary<TitleRowFilterType, TitleFilterEvaluator>()
            {
                { TitleRowFilterType.Id, new TitleFilterEvaluator(this, TitleRowFilterType.Id) },
                { TitleRowFilterType.Ready, new TitleFilterEvaluator(this, TitleRowFilterType.Ready) },
                { TitleRowFilterType.Flagged, new TitleFilterEvaluator(this, TitleRowFilterType.Flagged) },
                { TitleRowFilterType.CurrentlySubmitted, new TitleFilterEvaluator(this, TitleRowFilterType.CurrentlySubmitted) },
                { TitleRowFilterType.EverSubmitted, new TitleFilterEvaluator(this, TitleRowFilterType.EverSubmitted) },
                { TitleRowFilterType.Published, new TitleFilterEvaluator(this, TitleRowFilterType.Published) },
                { TitleRowFilterType.SelfPublished, new TitleFilterEvaluator(this, TitleRowFilterType.SelfPublished) },
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
        /// Sets <see cref="ReadyState"/> to on, clearing all other filters
        /// </summary>
        public void SetToReady()
        {
            SetCustomPropertyState(() => ReadyState = ThreeWayState.On);
        }

        /// <summary>
        /// Sets <see cref="FlaggedState"/> to on, clearing all other filters
        /// </summary>
        public void SetToFlagged()
        {
            SetCustomPropertyState(() => FlaggedState = ThreeWayState.On);
        }

        /// <summary>
        /// Sets <see cref="CurrentlySubmittedState"/> to on, clearing all other filters
        /// </summary>
        public void SetToSubmitted()
        {
            SetCustomPropertyState(() => CurrentlySubmittedState = ThreeWayState.On);
        }

        /// <summary>
        /// Sets <see cref="PublishedState"/> to on, clearing all other filters
        /// </summary>
        public void SetToPublished()
        {
            SetCustomPropertyState(() => PublishedState = ThreeWayState.On);
        }

        /// <summary>
        /// Sets <see cref="SelfPublishedState"/> to on, clearing all other filters
        /// </summary>
        public void SetToSelfPublished()
        {
            SetCustomPropertyState(() => SelfPublishedState = ThreeWayState.On);
        }

        /// <inheritdoc/>
        public override bool OnDataRowFilter(DataRow item)
        {
            return
                filterEvaluators[TitleRowFilterType.Id].Evaluate(item) &&
                filterEvaluators[TitleRowFilterType.Ready].Evaluate(item) &&
                filterEvaluators[TitleRowFilterType.Flagged].Evaluate(item) &&
                filterEvaluators[TitleRowFilterType.CurrentlySubmitted].Evaluate(item) &&
                filterEvaluators[TitleRowFilterType.EverSubmitted].Evaluate(item) &&
                filterEvaluators[TitleRowFilterType.Published].Evaluate(item) &&
                filterEvaluators[TitleRowFilterType.SelfPublished].Evaluate(item);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void SetFilterEvaluatorState(TitleRowFilterType key, ThreeWayState state)
        {
            if (filterEvaluators != null)
            {
                filterEvaluators[key].SetState(state);
            }
        }

        private void ClearAllPropertyState()
        {
            ReadyState = ThreeWayState.Neutral;
            FlaggedState = ThreeWayState.Neutral;
            CurrentlySubmittedState = ThreeWayState.Neutral;
            EverSubmittedState = ThreeWayState.Neutral;
            PublishedState = ThreeWayState.Neutral;
            SelfPublishedState = ThreeWayState.Neutral;
        }

        private bool IsAnyEvaluatorActive()
        {
            if (filterEvaluators != null)
            {
                foreach (KeyValuePair<TitleRowFilterType, TitleFilterEvaluator> pair in filterEvaluators)
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