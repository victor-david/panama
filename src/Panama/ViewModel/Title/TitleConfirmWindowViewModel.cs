/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Utility;
using Restless.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the display and selection logic for the <see cref="View.TitleConfirmWindow"/>.
    /// </summary>
    public class TitleConfirmWindowViewModel : DataViewModel<TitleSubmission>, IWindowOwner
    {
        #region Private
        private readonly SubmissionBatchRow submissionBatch;
        private readonly ObservableCollection<TitleSubmission> titles;
        private SubmissionBatchTable SubmissionBatchTable => DatabaseController.Instance.GetTable<SubmissionBatchTable>();
        #endregion

        /************************************************************************/

        #region IWindowOwner
        /// <summary>
        /// Gets or sets the window owner. Set in <see cref="WindowFactory"/>
        /// </summary>
        public Window WindowOwner { get; set; }

        /// <summary>
        /// Gets or sets a command to close the window. Set in <see cref="WindowFactory"/>
        /// </summary>
        public ICommand CloseWindowCommand { get; set; }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleConfirmWindowViewModel"/> class.
        /// </summary>
        public TitleConfirmWindowViewModel(SubmissionBatchRow submissionBatch, List<TitleRow> selectedTitles)
        {
            this.submissionBatch = submissionBatch ?? throw new ArgumentNullException(nameof(submissionBatch));

            Throw.IfNull(selectedTitles);

            Columns.Create("Id", nameof(TitleSubmission.Id))
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Title", nameof(TitleSubmission.Title));
            Columns.Create("Status", nameof(TitleSubmission.StatusString)).MakeFlexWidth(1.5);

            Commands.Add("Confirm", RunConfirmCommand);

            titles = new ObservableCollection<TitleSubmission>();
            selectedTitles.ForEach(t => titles.Add(new TitleSubmission(t, GetTitleSubmissionStatus(t))));
            InitListView(titles);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Gets the number of titles that do not have a status of <see cref="TitleSubmissionStatus.Okay"/>
        /// </summary>
        /// <returns>The number of titles that should be confirmed.</returns>
        public int GetConfirmationCount()
        {
            return titles.Count(title => title.Status != TitleSubmissionStatus.Okay);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override int OnDataRowCompare(TitleSubmission item1, TitleSubmission item2)
        {
            return string.Compare(item1.Title, item2.Title, StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunConfirmCommand(object parm)
        {
            WindowOwner.DialogResult = true;
            CloseWindowCommand.Execute(null);
        }

        private TitleSubmissionStatus GetTitleSubmissionStatus(TitleRow title)
        {
            return
                SubmissionBatchTable.GetExclusiveCount(title.Id, submissionBatch.PublisherId) > 0
                ? TitleSubmissionStatus.Exclusive
                : SubmissionBatchTable.GetTitleToPublisherCount(title.Id, submissionBatch.PublisherId, submissionBatch.Id) > 0
                ? TitleSubmissionStatus.SamePublisher
                : TitleSubmissionStatus.Okay;
        }
        #endregion
    }
}