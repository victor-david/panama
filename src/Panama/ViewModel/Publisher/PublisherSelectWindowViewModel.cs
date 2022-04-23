/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Controls;
using System;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.PublisherTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the display and selection logic for the <see cref="View.PublisherSelectWindow"/>.
    /// </summary>
    public class PublisherSelectWindowViewModel : WindowViewModel<PublisherTable>
    {
        #region Private
        private string searchText;
        private PublisherRow selectedPublisher;
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
        /// Gets the selected publisher
        /// </summary>
        public PublisherRow SelectedPublisher
        {
            get => selectedPublisher;
            private set => SetProperty(ref selectedPublisher, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherSelectWindowViewModel"/> class.
        /// </summary>
        public PublisherSelectWindowViewModel()
        {
            Columns.Create("Id", TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Name", TableColumns.Name);

            Columns.Create("Added", TableColumns.Added)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create("Last Sub", TableColumns.Calculated.LastSub)
                .MakeDate()
                .AddSort(null, TableColumns.Name, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("SC", TableColumns.Calculated.SubCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W052)
                .AddSort(null, TableColumns.Name, DataGridColumnSortBehavior.AlwaysAscending);

            Commands.Add("Select", RunSelectCommand, p => IsSelectedRowAccessible);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedPublisher = PublisherRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.Added);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return
                string.IsNullOrWhiteSpace(SearchText) ||
                item[TableColumns.Name].ToString().Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunSelectCommand(object parm)
        {
            if (SelectedPublisher != null)
            {
                WindowOwner.DialogResult = true;
                CloseWindowCommand.Execute(null);
            }
        }
        #endregion
    }
}