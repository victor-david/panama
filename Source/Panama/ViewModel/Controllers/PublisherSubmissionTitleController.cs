using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.Tools.Controls;
using System.ComponentModel;
using System.Data;
using System.Windows.Data;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Represents the controller that handles the titles that have been submitted to a publisher.
    /// </summary>
    public class PublisherSubmissionTitleController : PublisherController
    {
        #region Private
        private int dataViewCount;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the count of rows in the data view. The view binds to this property
        /// </summary>
        public int DataViewCount
        {
            get => dataViewCount;
            private set => SetProperty(ref dataViewCount, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="PublisherSubmissionTitleController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public PublisherSubmissionTitleController(PublisherViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<SubmissionTable>());
            DataView.RowFilter = string.Format("{0}=-1", SubmissionTable.Defs.Columns.Joined.PublisherId);
            DataView.Sort = string.Format("{0} DESC", SubmissionTable.Defs.Columns.Joined.Submitted);
            Columns.Create("Id", SubmissionTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            //Columns.Create("Submitted", SubmissionTable.Defs.Columns.Joined.Submitted).MakeDate();
            Columns.Create("Title", SubmissionTable.Defs.Columns.Joined.Title);
            Columns.Create("Written", SubmissionTable.Defs.Columns.Joined.Written).MakeDate();
            //AddDataGridViewColumns();
            Commands.Add("GoToTitleRecord", RunGoToTitleRecordCommand);
            MenuItems.AddItem("Go to title record for this item", Commands["GoToTitleRecord"], "ImageBrowseToUrlMenu");
            AddViewSourceSortDescriptions();
        }
        #endregion

        /************************************************************************/

        #region Public methods

        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            long publisherId = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = string.Format("{0}={1}", SubmissionTable.Defs.Columns.Joined.PublisherId, publisherId);
            DataViewCount = DataView.Count;
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.GroupDescriptions.Clear();
            // BUG: If grouped, and the first clicked parent has zero children, all the columns are scrunched together,
            // and clicking on another parent (with children) does not change the columns.
            // If not grouped, still scrunched if the first clicked parent has no children, but subsequent clicks
            // on parents that do have children restore the columns.
            // UPDATE 2018-08-25:
            //   Workaround implemented. By binding the visibility of the data grid to the child count (0=hidden, otherwise visible)
            //   the columns display as they should.
            MainSource.GroupDescriptions.Add(new PropertyGroupDescription(SubmissionTable.Defs.Columns.Joined.Submitted, new DateToFormattedDateConverter()));
            MainSource.SortDescriptions.Add(new SortDescription(SubmissionTable.Defs.Columns.Joined.Submitted, ListSortDirection.Descending));
            MainSource.SortDescriptions.Add(new SortDescription(SubmissionTable.Defs.Columns.Joined.Title, ListSortDirection.Ascending));
        }

        private void RunGoToTitleRecordCommand(object o)
        {
            if (SelectedRow != null)
            {
                long titleId = (long)SelectedRow[SubmissionTable.Defs.Columns.TitleId];
                var ws = Owner.MainViewModel.SwitchToWorkspace<TitleViewModel>();
                if (ws != null)
                {
                    // in case the VM was already open with a filter applied.
                    ws.Filters.ClearAll();
                    // assigning the property applies the filter
                    ws.Config.TitleFilter.Id = titleId;
                    if (ws.DataView.Count == 1)
                    {
                        /* This method uses a funky work around */
                        ws.SetSelectedItem(ws.DataView[0]);
                        /* Can be assigned directly, but doesn't highlight the row */
                        //ws.SelectedItem = ws.DataView[0];
                    }
                }

            }
        }
        #endregion
    }
}
