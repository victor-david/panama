using Restless.Panama.Core;
using Restless.Panama.Core.Converters;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TableColumns = Restless.Panama.Database.Tables.TitleTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the display and selection logic for the <see cref="View.TitleSelectWindow"/>.
    /// </summary>
    public class TitleSelectWindowViewModel : WindowViewModel<TitleTable>
    {
        #region Private
        private string searchText;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets or sets the search text
        /// </summary>
        public string SearchText
        {
            get => searchText;
            set
            {
                SetProperty(ref searchText, value);
                ListView.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets a value that determines if the title list is filtered by the ready flag
        /// </summary>
        public bool IsReady
        {
            get => Config.SubmissionTitleReady;
            set
            {
                Config.SubmissionTitleReady = value;
                ListView.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets a value that determines if the title list is filtered by the quick flag
        /// </summary>
        public bool IsFlagged
        {
            get => Config.SubmissionTitleFlagged;
            set
            {
                Config.SubmissionTitleFlagged = value;
                ListView.Refresh();
            }
        }

        /// <summary>
        /// Gets the selected titles
        /// </summary>
        public List<TitleRow> SelectedTitles
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleSelectWindowViewModel"/> class.
        /// </summary>
        public TitleSelectWindowViewModel()
        {
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.CreateResource<BooleanToResourceConverter>(Header.ReadyShort, TableColumns.Ready, ResourceKeys.Icon.IconCheck)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(ToolTip.TitleFilterReady);

            Columns.CreateResource<BooleanToResourceConverter>(Header.QuickShort, TableColumns.QuickFlag, ResourceKeys.Icon.IconCheck)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(ToolTip.TitleFilterFlag);

            Columns.Create(Header.Title, TableColumns.Title);

            Columns.Create(Header.Written, TableColumns.Written)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create(Header.Updated, TableColumns.Calculated.LatestVersionDate).MakeDate();

            Commands.Add("Select", RunSelectCommand, p => IsSelectedRowAccessible);

            SelectedTitles = new List<TitleRow>();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.Written);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return TitleFilterIsReady(item) && TitleFilterIsFlagged(item) && TitleFilterHasText(item);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunSelectCommand(object parm)
        {
            PopulateSelectedTitles();
            if (SelectedTitles.Count > 0)
            {
                WindowOwner.DialogResult = true;
                CloseWindowCommand.Execute(null);
            }
        }

        private void PopulateSelectedTitles()
        {
            SelectedTitles.Clear();
            foreach (DataRowView dataRowView in SelectedItems.OfType<DataRowView>())
            {
                SelectedTitles.Add(new TitleRow(dataRowView.Row));
            }
        }

        private bool TitleFilterIsReady(DataRow item)
        {
            return !IsReady || (bool)item[TableColumns.Ready];
        }

        private bool TitleFilterIsFlagged(DataRow item)
        {
            return !IsFlagged || (bool)item[TableColumns.QuickFlag];
        }

        private bool TitleFilterHasText(DataRow item)
        {
            return
                string.IsNullOrWhiteSpace(SearchText) ||
                item[TableColumns.Title].ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }
        #endregion
    }
}