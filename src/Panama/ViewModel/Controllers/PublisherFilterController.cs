/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Core;
using Restless.App.Panama.Resources;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Core.Utility;
using System;
using System.Text;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides logic for contructing and applying filters on the publisher list.
    /// </summary>
    public class PublisherFilterController : PublisherController
    {
        #region Private
        private readonly StringBuilder filter;
        private readonly StringBuilder filterDesc;
        private bool applyFilterPaused;
        private bool trimNextFilterDesc;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a short description of the number of records in the data view
        /// </summary>
        public string RecordCountText => Format.Plural(Owner.DataView.Count, Strings.TextRecord, Strings.TextRecords);

        /// <summary>
        /// Gets the friendly description of the current filter
        /// </summary>
        public string Description
        {
            get
            {
                if (filterDesc.Length == 0)
                {
                    return "(none)";
                }
                return filterDesc.ToString();
            }
        }

        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherFilterController"/> class.
        /// </summary>
        /// <param name="owner">The owner of this controller.</param>
        public PublisherFilterController(PublisherViewModel owner)
            : base(owner)
        {
            filter = new StringBuilder(512);
            filterDesc = new StringBuilder(512);

            Config.Instance.PublisherFilter.Changed += (s, e) =>
            {
                Apply();
            };
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Applies the filter options
        /// </summary>
        public void Apply()
        {
            if (!applyFilterPaused)
            {
                ApplyPrivate();
            }
        }

        /// <summary>
        /// Sets the filter options to active (not a goner) only.
        /// </summary>
        public void SetToActive()
        {
            ClearAndApplyAction(() => Config.PublisherFilter.Goner = FilterState.No);
        }

        /// <summary>
        /// Sets the filter options to have submission only.
        /// </summary>
        public void SetToHaveSubmission()
        {
            ClearAndApplyAction(() => Config.PublisherFilter.HaveSubmission = FilterState.Yes);
        }

        /// <summary>
        /// Sets the filter options to in period only.
        /// </summary>
        public void SetToInPeriod()
        {
            ClearAndApplyAction(() =>
            {
                Config.PublisherFilter.InPeriod = FilterState.Yes;
                Config.PublisherFilter.Goner = FilterState.No;
            });
        }

        /// <summary>
        /// Sets the filter options to paying only.
        /// </summary>
        public void SetToPaying()
        {
            ClearAndApplyAction(() =>
            {
                Config.PublisherFilter.Paying = FilterState.Yes;
                Config.PublisherFilter.Goner = FilterState.No;
            });
        }

        /// <summary>
        /// Sets the filter options to followup only.
        /// </summary>
        public void SetToFollowup()
        {
            ClearAndApplyAction(() =>
            {
                Config.PublisherFilter.Followup = FilterState.Yes;
                Config.PublisherFilter.Goner = FilterState.No;
            });
        }

        /// <summary>
        /// Clears all filter options
        /// </summary>
        public void ClearAll()
        {
            ClearAndApplyAction(null);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Implemented but not used
        /// </summary>
        protected override void OnUpdate()
        {
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void ClearAndApplyAction(Action action)
        {
            applyFilterPaused = true;
            Config.Instance.PublisherFilter.Reset();
            action?.Invoke();
            applyFilterPaused = false;
            Apply();
        }

        private void ApplyPrivate()
        {
            filter.Clear();
            filterDesc.Clear();
            var f = Config.Instance.PublisherFilter;
            if (!string.IsNullOrEmpty(f.Text))
            {
                Append(
                    string.Format("{0} LIKE '%{1}%'", PublisherTable.Defs.Columns.Name, f.Text.Replace("'", "''")),
                    string.Format("Name contains {0}", f.Text), 2);
            }

            if (f.InPeriod != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(string.Format("{0}={1}", PublisherTable.Defs.Columns.Calculated.InSubmissionPeriod, (byte)f.InPeriod), FilterDescText(f.InPeriod, "in submission period"));
            }

            if (f.Exclusive != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(string.Format("{0}={1}", PublisherTable.Defs.Columns.Exclusive, (byte)f.Exclusive), FilterDescText(f.Exclusive, "exclusive (no simultaneous)", "simultaneous"));
            }

            if (f.Paying != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(string.Format("{0}={1}", PublisherTable.Defs.Columns.Paying, (byte)f.Paying), FilterDescText(f.Paying, "paying"));
            }

            if (f.Followup != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(string.Format("{0}={1}", PublisherTable.Defs.Columns.Followup, (byte)f.Followup), FilterDescText(f.Followup, "followup"));
            }

            if (f.Goner != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(string.Format("{0}={1}", PublisherTable.Defs.Columns.Goner, (byte)f.Goner), FilterDescText(f.Goner, "a goner"));
            }

            if (f.HaveSubmission != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(string.Format("{0}={1}", PublisherTable.Defs.Columns.Calculated.HaveActiveSubmission, (byte)f.HaveSubmission), FilterDescText(f.HaveSubmission, "have active submission"));
            }

            Owner.DataView.RowFilter = filter.ToString();
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(RecordCountText));
        }


        private string FilterDescText(FilterState state, string yesText, string noText = null)
        {
            if (string.IsNullOrEmpty(noText)) noText = $"not {yesText}";
            return state == FilterState.Yes ? yesText : noText;
        }

        private void Append(string filterExpression, string filterDescription, int linesToAppend = 0)
        {
            filter.Append(filterExpression);
            if (trimNextFilterDesc)
            {
                filterDescription = filterDescription.TrimStart(' ');
            }
            filterDesc.Append(filterDescription);
            for (int k = 0; k < linesToAppend; k++)
            {
                filterDesc.AppendLine();
            }
            trimNextFilterDesc = linesToAppend > 0;
        }
        #endregion
    }
}