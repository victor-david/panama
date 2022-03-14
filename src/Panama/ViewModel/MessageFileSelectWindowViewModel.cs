/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.View;
using Restless.Panama.Core;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="MessageFileSelectWindow"/>.
    /// </summary>
    public class MessageFileSelectWindowViewModel : WindowViewModel
    {
        #region Private
        private readonly string folder;
        private readonly ObservableCollection<MimeKitMessage> resultsView;
        private IList selectedDataGridItems;
        private Tuple<int,string> displayFilterSelection;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the list of items that were selected by the user, or null if nothing selected.
        /// </summary>
        public List<MimeKitMessage> SelectedItems
        {
            get;
            private set;
        }

        /// <summary>
        /// Sets the selected data grid items. This property is bound to the view.
        /// </summary>
        public IList SelectedDataGridItems
        {
            set => selectedDataGridItems = value;
        }

        /// <summary>
        /// Gets the range of values used to set the display filter.
        /// </summary>
        public ObservableCollection<Tuple<int, string>> DisplayFilter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the selected display filter.
        /// </summary>
        public Tuple<int, string> DisplayFilterSelection
        {
            get => displayFilterSelection;
            set
            {
                if (SetProperty(ref displayFilterSelection, value))
                {
                    Config.SubmissionMessageDisplay = value.Item1;
                    // TODO
                    // MainSource.View.Refresh();
                }
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageFileSelectWindowViewModel"/> class.
        /// </summary>
        /// <param name="folder">The folder where submission message file are stored.</param>
        public MessageFileSelectWindowViewModel(string folder)
        {
            if (string.IsNullOrEmpty(folder))
            {
                throw new ArgumentNullException(nameof(folder));
            }

            this.folder = folder;
            resultsView = new ObservableCollection<MimeKitMessage>();
            // TODO
            //MainSource.Source = resultsView;
            //MainSource.Filter += MainSourceFilter;

            //MainSource.SortDescriptions.Add(new SortDescription(nameof(MimeKitMessage.MessageDateUtc), ListSortDirection.Descending));

            Columns.CreateImage<BooleanToImageConverter>("E", nameof(MimeKitMessage.IsError), "ImageExclamation")
                .AddToolTip(Strings.TooltipMessageError);

            Columns.CreateImage<BooleanToImageConverter>("U", nameof(MimeKitMessage.InUse))
                .AddToolTip(Strings.TooltipMessageInUse);

            var dateCol = Columns.Create("Date", nameof(MimeKitMessage.MessageDateUtc)).MakeDate();
            Columns.Create("From", nameof(MimeKitMessage.FromName));
            Columns.Create("Subject", nameof(MimeKitMessage.Subject));
            Columns.SetDefaultSort(dateCol, ListSortDirection.Descending);
            Commands.Add("Select", RunSelectCommand, (o) => SelectedItem != null);
            InitDisplayFilter();
            GetResults();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion

        /************************************************************************/

        #region Private methods
        private void InitDisplayFilter()
        {
            // 0 = display all
            // 1 = display only unassigned
            // 7 = display last 7 days
            // 14 = display last 14 days
            // 21 = display last 21 days
            DisplayFilter = new ObservableCollection<Tuple<int, string>>
            {
                Tuple.Create(1, "Only unassigned"),
                Tuple.Create(7, "Last 7 days"),
                Tuple.Create(14, "Last 14 days"),
                Tuple.Create(21, "Last 21 days"),
                Tuple.Create(30, "Last 30 days"),
                Tuple.Create(0, "Display all")
            };

            foreach (var t in DisplayFilter)
            {
                if (t.Item1 == Config.SubmissionMessageDisplay)
                {
                    DisplayFilterSelection = t;
                }
            }
        }

        private void MainSourceFilter(object sender, FilterEventArgs e)
        {
            if (e.Item is MimeKitMessage m)
            {
                if (DisplayFilterSelection != null)
                {
                    e.Accepted = DisplayFilterSelection.Item1 switch
                    {
                        0 => true,
                        1 => !m.InUse,
                        _ => DateTime.Compare(DateTime.UtcNow, m.MessageDateUtc.AddDays(DisplayFilterSelection.Item1)) < 0,
                    };
                }
            }
        }

        private void GetResults()
        {
            resultsView.Clear();
            foreach (string file in Directory.EnumerateFiles(Config.FolderSubmissionMessage, "*.eml"))
            {
                resultsView.Add(new MimeKitMessage(file));
            }
        }

        private void RunSelectCommand(object o)
        {
            if (selectedDataGridItems != null && selectedDataGridItems.Count > 0)
            {
                SelectedItems = new List<MimeKitMessage>();
                foreach (var item in selectedDataGridItems.OfType<MimeKitMessage>())
                {
                    SelectedItems.Add(item);
                }
            }

            CloseWindowCommand.Execute(null);
        }
        #endregion
    }
}