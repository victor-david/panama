using Restless.Toolkit.Controls;
using System;
using System.Data;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents the base class for a filter action. 
    /// This class must be inherited.
    /// </summary>
    public abstract class FilterEvaluator
    {
        #region Properties
        /// <summary>
        /// Gets the current state of the filter action
        /// </summary>
        public ThreeWayState State
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the filter that owns this evaluator
        /// </summary>
        protected RowFilter Filter
        {
            get;
        }

        /// <summary>
        /// Gets or sets the filter evaluator
        /// </summary>
        protected Predicate<DataRow> Evaluator
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if any filter is active
        /// </summary>
        public bool IsActive => State != ThreeWayState.Neutral;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleFilterEvaluator"/> class
        /// </summary>
        /// <param name="filter">The filter that owns the evaluator</param>
        public FilterEvaluator(RowFilter filter)
        {
            Filter = filter ?? throw new ArgumentNullException(nameof(filter));
            State = ThreeWayState.Neutral;
            Evaluator = EvaluateTrue;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Sets the state of the filter action
        /// </summary>
        /// <param name="state"></param>
        public void SetState(ThreeWayState state)
        {
            State = state;
        }

        /// <summary>
        /// Performs the evaluation of the specified data row.
        /// </summary>
        /// <param name="item">The data row</param>
        /// <returns>true if the data row passes the filter; otherwise, false.</returns>
        public bool Evaluate(DataRow item)
        {
            return Evaluator(item);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        protected bool EvaluateTrue(DataRow item)
        {
            return true;
        }

        protected bool EvaluateBoolColumn(object column)
        {
            return State == ThreeWayState.On ? GetBoolColumnValue(column) : !GetBoolColumnValue(column);
        }

        protected bool EvaluateLongColumn(object column)
        {
            return State == ThreeWayState.On ? GetLongColumnValue(column) > 0 : GetLongColumnValue(column) == 0;
        }

        protected bool EvaluateLongColumn(object column, long value)
        {
            return State == ThreeWayState.On ? GetLongColumnValue(column) == value : GetLongColumnValue(column) != value;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private bool GetBoolColumnValue(object column)
        {
            return column is bool b && b;
        }

        private long GetLongColumnValue(object column)
        {
            return (column is long value) ? value : 0;
        }
        #endregion
    }
}