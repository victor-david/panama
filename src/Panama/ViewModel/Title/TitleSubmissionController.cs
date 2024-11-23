using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Panama.View;
using Restless.Toolkit.Controls;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.SubmissionTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that displays the history of submissions for a title.
    /// </summary>
    public class TitleSubmissionController : BaseController<TitleViewModel, SubmissionTable>
    {
        #region Private
        private SubmissionBatchRow selectedBatch;
        private SubmissionRow selectedSubmission;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Get the selected submission
        /// </summary>
        public SubmissionRow SelectedSubmission
        {
            get => selectedSubmission;
            private set => SetProperty(ref selectedSubmission, value);
        }

        /// <summary>
        /// Gets the submission batch that corresponds to <see cref="SelectedSubmission"/>
        /// </summary>
        public SubmissionBatchRow SelectedBatch
        {
            get => selectedBatch;
            private set => SetProperty(ref selectedBatch, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleSubmissionController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleSubmissionController(TitleViewModel owner) : base(owner)
        {
            Columns.Create(Header.Id, TableColumns.TitleId)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.CreateResource<Int64ToResourceConverter>(Header.StatusShort, TableColumns.Status, ResourceKeys.Icon.GetTitleStatusIconMap())
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(SubmissionTitleStatusToolTip.Create(this));

            Columns.Create(Header.Submitted, TableColumns.Joined.Submitted)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.CreateResource<BooleanToResourceConverter>(Header.ExclusiveShort, TableColumns.Joined.PublisherExclusive, ResourceKeys.Icon.IconCheck)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(ToolTip.PublisherExclusive);

            Columns.Create(Header.Publisher, TableColumns.Joined.Publisher);
            Columns.Create(Header.Response, TableColumns.Joined.ResponseTypeName);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedSubmission = SubmissionRow.Create(SelectedRow);
            SelectedBatch = SubmissionBatchTable.GetSubmissionBatch(SelectedSubmission?.BatchId ?? 0);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.Joined.Submitted);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.TitleId] == (Owner?.SelectedTitle?.Id ?? 0);
        }
        #endregion
    }
}