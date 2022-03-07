/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Utility;
using System;
using System.Data;
using System.Windows;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides logic for handling the submission periods that are associated with a publisher.
    /// </summary>
    public class PublisherPeriodController : PublisherController
    {
        #region Private
        private Visibility addControlVisibility;
        private Visibility notesVisibility;
        private DateTime addStart;
        private DateTime addEnd;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a visibility value that determines if the add submission period control is visible.
        /// </summary>
        public Visibility AddControlVisibility
        {
            get => addControlVisibility;
            private set => SetProperty(ref addControlVisibility, value);
        }

        /// <summary>
        /// Gets a visibility value that determines if the notes edit control is visible.
        /// </summary>
        public Visibility NotesVisibility
        {
            get => notesVisibility;
            private set => SetProperty(ref notesVisibility, value);
        }

        /// <summary>
        /// Gets or sets the start date when adding a submission period.
        /// </summary>
        public DateTime AddStart
        {
            get => addStart;
            set
            {
                if (SetProperty(ref addStart, value))
                {
                    UpdateSelectedDisplay();
                }
            }
        }

        /// <summary>
        /// Gets or sets the end date when adding a submission period.
        /// </summary>
        public DateTime AddEnd
        {
            get => addEnd;
            set
            {
                if (SetProperty(ref addEnd, value))
                {
                    UpdateSelectedDisplay();
                }
            }
        }

        /// <summary>
        /// Gets a friendly string that displays the date range in effect
        /// when adding a submission period.
        /// </summary>
        public string SelectedDisplay
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherPeriodController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public PublisherPeriodController(PublisherViewModel owner) : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<SubmissionPeriodTable>());
            DataView.RowFilter = string.Format("{0}=-1", SubmissionPeriodTable.Defs.Columns.PublisherId);
            DataView.Sort = string.Format("{0} ASC", SubmissionPeriodTable.Defs.Columns.Start);
            Columns.Create("Id", SubmissionPeriodTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.Create("Start", SubmissionPeriodTable.Defs.Columns.Start).MakeDate("MMMM dd", toLocal:false);
            Columns.Create("End", SubmissionPeriodTable.Defs.Columns.End).MakeDate("MMMM dd", toLocal:false);
            Columns.Create("Note", SubmissionPeriodTable.Defs.Columns.Notes);
            Owner.Commands.Add("PeriodAddShow", (o) =>
                {
                    SetAddControlVisibility(true);
                    SetNotesVisibility(false);
                },
                (o) => { return AddControlVisibility != Visibility.Visible; }
            );
            Owner.Commands.Add("PeriodRemove", RunRemoveSubmissionPeriodCommand, (o) =>
                { return SelectedRow != null && AddControlVisibility != Visibility.Visible; });
            Owner.Commands.Add("PeriodAddConfirm", RunAddSubmissionPeriodCommand);
            Owner.Commands.Add("PeriodAddConfirmAllYear", RunAddSubmissionPeriodAllYearCommand);

            Owner.Commands.Add("PeriodAddCancel", (o) =>
            {
                SetAddControlVisibility(false);
                SetNotesVisibility(SelectedItem != null);
            });
            AddControlVisibility = Visibility.Collapsed;
            NotesVisibility = Visibility.Collapsed;

            AddStart = new DateTime(DateTime.Now.Year, 1, 1);
            AddEnd = new DateTime(DateTime.Now.Year, 12, 31);
        }
        #endregion

        /************************************************************************/

        #region Public methods

        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the selected item on the associated data grid has changed.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SetAddControlVisibility(false);
            SetNotesVisibility(SelectedItem != null);
        }

        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            long publisherId = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = string.Format("{0}={1}", SubmissionPeriodTable.Defs.Columns.PublisherId, publisherId);
            SetAddControlVisibility(false);
            SetNotesVisibility(SelectedItem != null);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void UpdateSelectedDisplay()
        {
            SelectedDisplay = string.Format("Period: {0} - {1}", AddStart.ToString("MMMM dd"), AddEnd.ToString("MMMM dd"));
            OnPropertyChanged(nameof(SelectedDisplay));
        }

        private void RunAddSubmissionPeriodCommand(object o)
        {
            AddSubmissionPeriod(AddStart, AddEnd);
        }

        private void RunAddSubmissionPeriodAllYearCommand(object o)
        {
            DateTime start = new(DateTime.Now.Year, 1, 1);
            DateTime end = new(DateTime.Now.Year, 12, 31);
            AddSubmissionPeriod(start, end);
        }

        private void AddSubmissionPeriod(DateTime start, DateTime end)
        {
            if (Owner.SelectedPrimaryKey != null)
            {
                long publisherId = (long)Owner.SelectedPrimaryKey;
                DatabaseController.Instance.GetTable<SubmissionPeriodTable>().AddSubmissionPeriod(publisherId, start, end);
                SetAddControlVisibility(false);
                SetNotesVisibility(SelectedItem != null);
            }
        }

        private void RunRemoveSubmissionPeriodCommand(object o)
        {
            if (SelectedRow != null && Messages.ShowYesNo(Strings.ConfirmationRemoveSubmissionPeriod))
            {
                DatabaseController.Instance.GetTable<SubmissionPeriodTable>().DeleteSubmissionPeriod(SelectedRow);
            }
        }

        private void SetAddControlVisibility(bool visible)
        {
            AddControlVisibility = (visible) ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetNotesVisibility(bool visible)
        {
            NotesVisibility = (visible) ? Visibility.Visible : Visibility.Hidden;
        }
        #endregion
    }
}