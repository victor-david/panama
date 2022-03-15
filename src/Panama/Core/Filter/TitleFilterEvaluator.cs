﻿using Restless.Toolkit.Controls;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.TitleTable.Defs.Columns;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents a title filter evaluator. This class provides a series
    /// of predicate evaluator methods that check the incoming DataRow
    /// </summary>
    public class TitleFilterEvaluator : FilterEvaluator
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleFilterEvaluator"/> class
        /// </summary>
        /// <param name="filter">The filter that owns this evaluator</param>
        /// <param name="filterType">The filter type</param>
        public TitleFilterEvaluator(RowFilter filter, TitleRowFilterType filterType) : base(filter)
        {
            Evaluator = GetEvaluator(filterType);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private Predicate<DataRow> GetEvaluator(TitleRowFilterType filterType)
        {
            return filterType switch
            {
                TitleRowFilterType.Id => EvaluateId,
                TitleRowFilterType.Text => EvaluateText,
                TitleRowFilterType.Ready => EvaluateReady,
                TitleRowFilterType.Flagged => EvaluateFlagged,
                TitleRowFilterType.CurrentlySubmitted => EvaluateCurrentlySubmitted,
                TitleRowFilterType.EverSubmitted => EvaluateEverSubmitted,
                TitleRowFilterType.Published => EvaluatePublished,
                TitleRowFilterType.SelfPublished => EvaluateSelfPublished,
                _ => EvaluateTrue,
            };
        }

        private bool EvaluateId(DataRow item)
        {
            long id = Filter.GetIdFilter();
            return id == -1 || id == (long)item[Columns.Id];
        }

        private bool EvaluateText(DataRow item)
        {
            return
                string.IsNullOrWhiteSpace(Filter.Text) ||
                item[Columns.Title].ToString().Contains(Filter.Text, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool EvaluateReady(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateBoolColumn(item[Columns.Ready]);
        }

        private bool EvaluateFlagged(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateBoolColumn(item[Columns.QuickFlag]);
        }

        private bool EvaluateCurrentlySubmitted(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateLongColumn(item[Columns.Calculated.CurrentSubCount]);
        }

        private bool EvaluateEverSubmitted(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateLongColumn(item[Columns.Calculated.SubCount]);
        }

        private bool EvaluatePublished(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateBoolColumn(item[Columns.Calculated.IsPublished]);
        }

        private bool EvaluateSelfPublished(DataRow item)
        {
            return State == ThreeWayState.Neutral || EvaluateBoolColumn(item[Columns.Calculated.IsSelfPublished]);
        }
        #endregion
    }
}