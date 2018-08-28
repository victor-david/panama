using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Resources;
using Restless.App.Panama.Filter;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides logic for contructing and applying filters on the publisher list.
    /// </summary>
    public class PublisherFilterController : PublisherController
    {
        #region Private
        private StringBuilder filter;
        private StringBuilder filterDesc;
        private bool applyFilterPaused;
        private bool trimNextFilterDesc;
        #endregion

        /************************************************************************/
        
        #region Public properties
        /// <summary>
        /// Gets a short description of the number of records in the data view
        /// </summary>
        public string RecordCountText
        {
            get { return Format.Plural(Owner.DataView.Count, Strings.TextRecord, Strings.TextRecords); }  
        }

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
        /// Sets the filter options to in period only.
        /// </summary>
        public void SetToInPeriod()
        {
            ClearAndApplyAction(() => Config.PublisherFilter.InPeriod = FilterState.Yes);
        }

        /// <summary>
        /// Sets the filter options to paying only.
        /// </summary>
        public void SetToPaying()
        {
            ClearAndApplyAction(() => Config.PublisherFilter.Paying = FilterState.Yes);
        }

        /// <summary>
        /// Sets the filter options to followup only.
        /// </summary>
        public void SetToFollowup()
        {
            ClearAndApplyAction(() => Config.PublisherFilter.Followup = FilterState.Yes);
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
            if (!String.IsNullOrEmpty(f.Text))
            {
                Append(
                    String.Format("{0} LIKE '%{1}%'", PublisherTable.Defs.Columns.Name, f.Text.Replace("'", "''")), 
                    String.Format("Name contains {0}", f.Text), 2);
            }
            
            if (f.InPeriod != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(String.Format("{0}={1}", PublisherTable.Defs.Columns.Calculated.InSubmissionPeriod, (byte)f.InPeriod), (f.InPeriod == FilterState.No ? "not in submission period" : "in submission period"));
            }

            if (f.Simultaneous != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(String.Format("{0}={1}", PublisherTable.Defs.Columns.Simultaneous, (byte)f.Simultaneous), (f.Simultaneous == FilterState.No ? "not simultaneous" : "simultaneous"));
            }

            if (f.Paying != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(String.Format("{0}={1}", PublisherTable.Defs.Columns.Paying, (byte)f.Paying), (f.Paying == FilterState.No ? "not paying" : "paying"));
            }

            if (f.Followup != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(String.Format("{0}={1}", PublisherTable.Defs.Columns.Followup, (byte)f.Followup), (f.Followup == FilterState.No ? "not followup" : "followup"));
            }

            if (f.Goner != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(String.Format("{0}={1}", PublisherTable.Defs.Columns.Goner, (byte)f.Goner), (f.Goner == FilterState.No ? "not a goner" : "a goner"));
            }

            Owner.DataView.RowFilter = filter.ToString();
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(RecordCountText));
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
