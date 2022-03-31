/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Resources;
using Restless.Panama.View;
using Restless.Toolkit.Controls;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="SubmissionMessageSelectWindow"/>.
    /// </summary>
    public class SubmissionMessageSelectWindowViewModel : WindowViewModel
    {
        #region Private
        private readonly ObservableCollection<MimeKitMessage> resultsView;
        private IList selectedDataGridItems;
        #endregion

        /************************************************************************/

        #region Public properties
        public int SelectedFilterValue
        {
            get => Config.SubmissionMessageDisplay;
            set
            {
                Config.SubmissionMessageDisplay = value;
            }
        }
        /// <summary>
        /// Gets the list of messages that were selected by the user
        /// </summary>
        public List<MimeKitMessage> SelectedMessages
        {
            get;
        }

        /// <summary>
        /// Sets the selected data grid items. This property is bound to the view.
        /// </summary>
        public IList SelectedDataGridItems
        {
            set => selectedDataGridItems = value;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionMessageSelectWindowViewModel"/> class.
        /// </summary>
        public SubmissionMessageSelectWindowViewModel()
        {
            resultsView = new ObservableCollection<MimeKitMessage>();

            SelectedMessages = new List<MimeKitMessage>();
            // TODO
            //MainView.
            //MainSource.Source = resultsView;
            //MainSource.Filter += MainSourceFilter;

            //MainSource.SortDescriptions.Add(new SortDescription(nameof(MimeKitMessage.MessageDateUtc), ListSortDirection.Descending));

            Columns.CreateResource<BooleanToPathConverter>("E", nameof(MimeKitMessage.IsError), ResourceKeys.Icon.SquareSmallRedIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.TooltipMessageError);

            Columns.CreateResource<BooleanToPathConverter>("U", nameof(MimeKitMessage.InUse), ResourceKeys.Icon.SquareSmallBlueIconKey)
                .AddToolTip(Strings.TooltipMessageInUse);

            Columns.SetDefaultSort(
                Columns.Create("Date", nameof(MimeKitMessage.MessageDateUtc)).MakeDate(),
                ListSortDirection.Descending);

            Columns.Create("From", nameof(MimeKitMessage.FromName));
            Columns.Create("Subject", nameof(MimeKitMessage.Subject));

            Commands.Add("Select", RunSelectCommand, p => SelectedItem != null);

            GetResults();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return base.OnDataRowCompare(item1, item2);
        }

        protected override bool OnDataRowFilter(DataRow item)
        {
            return base.OnDataRowFilter(item);
        }

        #endregion

        /************************************************************************/

        #region Private methods
        //private void MainSourceFilter(object sender, FilterEventArgs e)
        //{
        //    if (e.Item is MimeKitMessage m)
        //    {
        //        if (DisplayFilterSelection != null)
        //        {
        //            e.Accepted = DisplayFilterSelection.Item1 switch
        //            {
        //                0 => true,
        //                1 => !m.InUse,
        //                _ => DateTime.Compare(DateTime.UtcNow, m.MessageDateUtc.AddDays(DisplayFilterSelection.Item1)) < 0,
        //            };
        //        }
        //    }
        //}

        private void GetResults()
        {
            resultsView.Clear();
            foreach (string file in Directory.EnumerateFiles(Config.FolderSubmissionMessage, "*.eml"))
            {
                resultsView.Add(new MimeKitMessage(file));
            }
        }

        private void RunSelectCommand(object parm)
        {
            PopulateSelectedMessages();
            if (SelectedMessages.Count > 0)
            {
                WindowOwner.DialogResult = true;
                CloseWindowCommand.Execute(null);
            }
        }

        private void PopulateSelectedMessages()
        {
            SelectedMessages.Clear();
            //foreach (DataRowView dataRowView in SelectedDataGridItems.OfType<DataRowView>())
            //{
            //    SelectedTitles.Add(new TitleRow(dataRowView.Row));
            //}
        }
        #endregion
    }
}