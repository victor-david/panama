using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System.Data;
using TableColumns = Restless.Panama.Database.Tables.SubmissionTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents the controller that handles the titles that have been submitted to a publisher.
    /// </summary>
    public class PublisherSubmissionTitleController : BaseController<PublisherSubmissionController, SubmissionTable>
    {
        #region Private
        private SubmissionRow selectedSubmission;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the selected submission
        /// </summary>
        public SubmissionRow SelectedSubmission
        {
            get => selectedSubmission;
            private set => SetProperty(ref selectedSubmission, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherSubmissionTitleController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public PublisherSubmissionTitleController(PublisherSubmissionController owner) : base(owner)
        {
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create(Header.Batch, TableColumns.BatchId)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W048);

            Columns.Create(Header.Title, TableColumns.Joined.Title).MakeInitialSortAscending();

            Columns.Create(Header.Written, TableColumns.Joined.Written).MakeDate();

            MenuItems.AddItem(Menu.OpenTitleOrDoubleClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.IconOpenWebSite);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedSubmission = SubmissionRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareString(item1, item2, TableColumns.Joined.Title);
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TableColumns.BatchId] == (Owner?.SelectedBatch?.Id ?? 0);
        }

        /// <inheritdoc/>
        protected override void RunOpenRowCommand()
        {
            if (SelectedSubmission != null)
            {
                Database.Tables.TitleVersionController verController = TitleVersionTable.GetVersionController(SelectedSubmission.TitleId);
                if (verController.Versions.Count > 0)
                {
                    Open.TitleVersionFile(verController.Versions[0].FileName);
                }
            }
        }
        #endregion
    }
}