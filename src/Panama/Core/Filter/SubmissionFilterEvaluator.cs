using Restless.Toolkit.Controls;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.SubmissionBatchTable.Defs.Columns;
using Responses = Restless.Panama.Database.Tables.ResponseTable.Defs.Values;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents a submission filter evaluator. This class provides a series
    /// of predicate evaluator methods that check the incoming DataRow
    /// </summary>
    public class SubmissionFilterEvaluator : FilterEvaluator
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionFilterEvaluator"/> class
        /// </summary>
        /// <param name="filter">The filter that owns this evaluator</param>
        /// <param name="filterType">The filter type</param>
        public SubmissionFilterEvaluator(RowFilter filter, SubmissionRowFilterType filterType) : base(filter)
        {
            Evaluator = GetEvaluator(filterType);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private Predicate<DataRow> GetEvaluator(SubmissionRowFilterType filterType)
        {
            return filterType switch
            {
                SubmissionRowFilterType.Id => EvaluateId,
                SubmissionRowFilterType.Active => EvaluateActive,
                SubmissionRowFilterType.TryAgain => EvaluateTryAgain,
                SubmissionRowFilterType.Personal => EvaluatePersonal,
                SubmissionRowFilterType.Accepted => EvaluateAccepted,
                _ => EvaluateTrue,
            };
        }

        private bool EvaluateId(DataRow item)
        {
            long id = Filter.GetIdFilter();
            return id == -1 || id == (long)item[Columns.PublisherId];
        }

        private bool EvaluateActive(DataRow item)
        {
            return  State == ThreeWayState.Neutral || EvaluateLongColumn(item[Columns.ResponseType], Responses.NoResponse);
        }

        private bool EvaluateTryAgain(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateLongColumn(item[Columns.ResponseType], Responses.ResponseTryAgain);
        }

        private bool EvaluatePersonal(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateLongColumn(item[Columns.ResponseType], Responses.ResponsePersonal);
        }

        private bool EvaluateAccepted(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateLongColumn(item[Columns.ResponseType], Responses.ResponseAccepted);
        }
        #endregion
    }
}