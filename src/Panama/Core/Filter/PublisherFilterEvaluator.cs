using Restless.Toolkit.Controls;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.PublisherTable.Defs.Columns;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents a publisher filter evaluator. This class provides a series
    /// of predicate evaluator methods that check the incoming DataRow
    /// </summary>
    public class PublisherFilterEvaluator : FilterEvaluator
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherFilterEvaluator"/> class
        /// </summary>
        /// <param name="filter">The filter that owns this evaluator</param>
        /// <param name="filterType">The filter type</param>
        public PublisherFilterEvaluator(RowFilter filter, PublisherRowFilterType filterType) : base(filter)
        {
            Evaluator = GetEvaluator(filterType);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private Predicate<DataRow> GetEvaluator(PublisherRowFilterType filterType)
        {
            return filterType switch
            {
                PublisherRowFilterType.Text => EvaluateText,
                PublisherRowFilterType.Active => EvaluateActive,
                PublisherRowFilterType.OpenSubmission => EvaluateOpenSubmission,
                PublisherRowFilterType.InPeriod => EvaluateInPeriod,
                PublisherRowFilterType.Exclusive => EvaluateExclusive,
                PublisherRowFilterType.FollowUp => EvaluateFollowUp,
                PublisherRowFilterType.Paying => EvaluatePaying,
                PublisherRowFilterType.Goner => EvaluateGoner,
                _ => EvaluateTrue,
            };
        }

        private bool EvaluateText(DataRow item)
        {
            return
                string.IsNullOrWhiteSpace(Filter.Text) ||
                item[Columns.Name].ToString().Contains(Filter.Text, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool EvaluateActive(DataRow item)
        {
            return  State == ThreeWayState.Neutral || !EvaluateBoolColumn(item[Columns.Goner]);
        }

        private bool EvaluateOpenSubmission(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateBoolColumn(item[Columns.Calculated.HaveActiveSubmission]);
        }

        private bool EvaluateInPeriod(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateBoolColumn(item[Columns.Calculated.InSubmissionPeriod]);
        }

        private bool EvaluateExclusive(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateBoolColumn(item[Columns.Exclusive]);
        }

        private bool EvaluateFollowUp(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateBoolColumn(item[Columns.Followup]);
        }

        private bool EvaluatePaying(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateBoolColumn(item[Columns.Paying]);
        }

        private bool EvaluateGoner(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateBoolColumn(item[Columns.Goner]);
        }
        #endregion
    }
}