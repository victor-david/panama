/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System.ComponentModel;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the display and selection logic for the <see cref="View.PublisherSelectWindow"/>.
    /// </summary>
    public class SelfPublisherSelectWindowViewModel : WindowViewModel<SelfPublisherTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the selected publisher id, or -1 if none selected.
        /// </summary>
        public long SelectedPublisherId
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfPublisherSelectWindowViewModel"/> class.
        /// </summary>
        public SelfPublisherSelectWindowViewModel()
        {
            Columns.Create("Id", SelfPublisherTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Name", SelfPublisherTable.Defs.Columns.Name);
            var col = Columns.Create("Added", SelfPublisherTable.Defs.Columns.Added).MakeDate();
            Columns.SetDefaultSort(col, ListSortDirection.Descending);
            Columns.Create("PC", SelfPublisherTable.Defs.Columns.Calculated.PubCount).MakeFixedWidth(FixedWidth.MediumNumeric)
                .AddSort(null, SelfPublisherTable.Defs.Columns.Name, DataGridColumnSortBehavior.AlwaysAscending);

            Commands.Add("Select", RunSelectCommand, (o) => IsSelectedRowAccessible);
            FilterPrompt = Strings.FilterPromptPublisher;
            SelectedPublisherId = -1;
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the filter text has changed to set the filter on the underlying data.
        /// </summary>
        /// <param name="text">The filter text.</param>
        protected override void OnFilterTextChanged(string text)
        {
            DataView.RowFilter = string.Format("{0} LIKE '%{1}%'", SelfPublisherTable.Defs.Columns.Name, text);
        }

        #endregion

        /************************************************************************/

        #region Private methods
        private void RunSelectCommand(object o)
        {
            if (SelectedPrimaryKey != null)
            {
                SelectedPublisherId = (long)SelectedPrimaryKey;
            }
            CloseWindowCommand.Execute(null);
        }
        #endregion
    }
}