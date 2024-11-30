using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the display and selection logic for the <see cref="View.TitleConfirmWindow"/>.
    /// </summary>
    public class TitleConfirmWindowViewModel : DataViewModel<TitleSubmission>, IWindow
    {
        #region Private
        private readonly SubmissionBatchRow submissionBatch;
        private readonly ObservableCollection<TitleSubmission> titles;
        #endregion

        /************************************************************************/

        #region IWindowOwner
        /// <summary>
        /// Gets or sets the window owner. Set in <see cref="WindowFactory"/>
        /// </summary>
        public Window Window { get; set; }

        /// <summary>
        /// Gets or sets a command to close the window. Set in <see cref="WindowFactory"/>
        /// </summary>
        public ICommand CloseWindowCommand { get; set; }

        /// <summary>
        /// Called when the window is closing. Established in <see cref="WindowFactory"/>
        /// </summary>
        /// <param name="e">The event args</param>
        public void OnWindowClosing(CancelEventArgs e)
        {
        }

        /// <summary>
        /// Called when the window has closed. Established in <see cref="WindowFactory"/>
        /// </summary>
        public void OnWindowClosed()
        {
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleConfirmWindowViewModel"/> class.
        /// </summary>
        public TitleConfirmWindowViewModel(SubmissionBatchRow submissionBatch, List<TitleRow> selectedTitles)
        {
            this.submissionBatch = submissionBatch ?? throw new ArgumentNullException(nameof(submissionBatch));

            ArgumentNullException.ThrowIfNull(selectedTitles);

            Columns.Create(Header.Id, nameof(TitleSubmission.Id))
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create(Header.Title, nameof(TitleSubmission.Title))
                .MakeFlexWidth(1.5)
                .MakeInitialSortAscending();

            Columns.Create(Header.Status, nameof(TitleSubmission.StatusString));

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
            Window.DialogResult = true;
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