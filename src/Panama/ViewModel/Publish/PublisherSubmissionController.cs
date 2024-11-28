using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.SubmissionBatchTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents the controller that handles the submissions for a publisher.
    /// </summary>
    public class PublisherSubmissionController : BaseController<PublisherViewModel, SubmissionBatchTable>
    {
        #region Private
        private SubmissionBatchRow selectedBatch;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the currently selected submission batch row
        /// </summary>
        public SubmissionBatchRow SelectedBatch
        {
            get => selectedBatch;
            private set => SetProperty(ref selectedBatch, value);
        }

        /// <summary>
        /// Gets the submission title controller.
        /// </summary>
        public PublisherSubmissionTitleController Titles { get; }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherSubmissionController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public PublisherSubmissionController(PublisherViewModel owner): base(owner)
        {
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create(Header.Submitted, TableColumns.Submitted)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create(Header.Response, TableColumns.Response)
                .MakeDate();

            Columns.Create(Header.Type, TableColumns.Joined.ResponseTypeName)
                .MakeFixedWidth(FixedWidth.W112);

            Columns.Create(Header.Note, TableColumns.Notes).MakeSingleLine();

            Titles = new PublisherSubmissionTitleController(this);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            base.OnUpdate();
            Titles.Update();
        }

        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedBatch = SubmissionBatchRow.Create(SelectedRow);
            Titles.Update();
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareDateTime(item2, item1, TableColumns.Submitted);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.PublisherId] == (Owner?.SelectedPublisher?.Id ?? 0);
        }
        #endregion
    }
}