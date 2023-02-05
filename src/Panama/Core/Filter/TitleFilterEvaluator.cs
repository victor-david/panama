using Restless.Toolkit.Controls;
using System;
using System.Data;
using Columns = Restless.Panama.Database.Tables.TitleTable.Defs.Columns;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents a title filter evaluator. This class provides a series
    /// of predicate evaluator methods that check the incoming DataRow
    /// </summary>
    public class TitleFilterEvaluator : FilterEvaluator<TitleRowFilter>
    {
        private readonly TitleRowFilterType filterType;

        /// <inheritdoc/>
        public override bool IsActive => GetIsEvaluatorActive();

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleFilterEvaluator"/> class
        /// </summary>
        /// <param name="filter">The filter that owns this evaluator</param>
        /// <param name="filterType">The filter type</param>
        public TitleFilterEvaluator(TitleRowFilter filter, TitleRowFilterType filterType) : base(filter)
        {
            this.filterType = filterType;
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
                TitleRowFilterType.Directory => EvaluateDirectory,
                TitleRowFilterType.Ready => EvaluateReady,
                TitleRowFilterType.Flagged => EvaluateFlagged,
                TitleRowFilterType.CurrentlySubmitted => EvaluateCurrentlySubmitted,
                TitleRowFilterType.EverSubmitted => EvaluateEverSubmitted,
                TitleRowFilterType.Published => EvaluatePublished,
                TitleRowFilterType.SelfPublished => EvaluateSelfPublished,
                TitleRowFilterType.WordCount => EvaluateWordCount,
                TitleRowFilterType.Tag => EvaluateTags,
                _ => EvaluateTrue,
            };
        }

        private bool GetIsEvaluatorActive()
        {
            return filterType switch
            {
                TitleRowFilterType.Directory => !string.IsNullOrWhiteSpace(Filter.Directory),
                TitleRowFilterType.WordCount => Filter.WordCount != 0,
                TitleRowFilterType.Tag => Filter.Tags.Count > 0,
                _ => base.IsActive
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

        private bool EvaluateDirectory(DataRow item)
        {
            return
                string.IsNullOrWhiteSpace(Filter.Directory) ||
                item[Columns.Calculated.LastestVersionPath].ToString().Contains(Filter.Directory, StringComparison.InvariantCultureIgnoreCase);
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

        private bool EvaluateWordCount(DataRow item)
        {
            if (Filter.WordCount != 0)
            {
                if (item[Columns.Calculated.LastestVersionWordCount] is long wordCount)
                {
                    return (Filter.WordCount > 0) ? wordCount > Filter.WordCount : wordCount > 0 && wordCount < Math.Abs(Filter.WordCount);
                }
                return false;
            }
            return true;
        }

        private bool EvaluateTags(DataRow item)
        {
            return Filter.Tags.IsTitleIdIncluded((long)item[Columns.Id]);
        }
        #endregion
    }
}