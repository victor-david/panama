using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;
using System.Windows;

namespace Restless.App.Panama.ViewModel
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
            get { return addControlVisibility; }
            private set
            {
                SetProperty(ref addControlVisibility, value);
            }
        }

        /// <summary>
        /// Gets a visibility value that determines if the notes edit control is visible.
        /// </summary>
        public Visibility NotesVisibility
        {
            get { return notesVisibility; }
            private set
            {
                SetProperty(ref notesVisibility, value);
            }
        }

        /// <summary>
        /// Gets or sets the start date when adding a submission period.
        /// </summary>
        public DateTime AddStart
        {
            get { return addStart; }
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
            get { return addEnd; }
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
        public PublisherPeriodController(PublisherViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<SubmissionPeriodTable>());
            DataView.RowFilter = String.Format("{0}=-1", SubmissionPeriodTable.Defs.Columns.PublisherId);
            DataView.Sort = String.Format("{0} ASC", SubmissionPeriodTable.Defs.Columns.Start);
            Columns.Create("Id", SubmissionPeriodTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Start", SubmissionPeriodTable.Defs.Columns.Start).MakeDate("MMMM dd");
            Columns.Create("End", SubmissionPeriodTable.Defs.Columns.End).MakeDate("MMMM dd");
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
            AddStart = new DateTime(DateTime.UtcNow.Year, 1, 1);
            AddEnd = new DateTime(DateTime.UtcNow.Year, 12, 31);

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
            Int64 publisherId = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = String.Format("{0}={1}", SubmissionPeriodTable.Defs.Columns.PublisherId, publisherId);
            SetAddControlVisibility(false);
            SetNotesVisibility(SelectedItem != null);
        }
        #endregion
        
        /************************************************************************/

        #region Private methods
        private void UpdateSelectedDisplay()
        {
            SelectedDisplay = String.Format("Period: {0} - {1}", AddStart.ToString("MMMM dd"), AddEnd.ToString("MMMM dd"));
            OnPropertyChanged(nameof(SelectedDisplay));
        }

        private void RunAddSubmissionPeriodCommand(object o)
        {
            AddSubmissionPeriod(AddStart, AddEnd);
        }

        private void RunAddSubmissionPeriodAllYearCommand(object o)
        {
            DateTime start = new DateTime(DateTime.UtcNow.Year, 1, 1);
            DateTime end = new DateTime(DateTime.UtcNow.Year, 12, 31);
            AddSubmissionPeriod(start, end);
        }

        private void AddSubmissionPeriod(DateTime start, DateTime end)
        {
            if (Owner.SelectedPrimaryKey != null)
            {
                Int64 publisherId = (Int64)Owner.SelectedPrimaryKey;
                DatabaseController.Instance.GetTable<SubmissionPeriodTable>().AddSubmissionPeriod(publisherId, start, end);
                SetAddControlVisibility(false);
                SetNotesVisibility(SelectedItem != null);
            }
        }

        private void RunRemoveSubmissionPeriodCommand(object o)
        {
            if (SelectedRow != null && Messages.ShowYesNo(Strings.ConfirmationRemoveSubmissionPeriod))
            {
                DatabaseController.Instance.GetTable<SubmissionPeriodTable>().RemoveSubmissionPeriod(SelectedRow);
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
