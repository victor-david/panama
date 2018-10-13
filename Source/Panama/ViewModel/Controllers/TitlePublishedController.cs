using Restless.App.Panama.Controls;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Utility;
using System;
using System.Data;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that manages selection and updates of published titles.
    /// </summary>
    public class TitlePublishedController : TitleController
    {
        #region Private
        #endregion

        /************************************************************************/
        
        #region Public properties

        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitlePublishedController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitlePublishedController(TitleViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<PublishedTable>());
            DataView.RowFilter = String.Format("{0}=-1", PublishedTable.Defs.Columns.TitleId);
            DataView.Sort = String.Format("{0} DESC", PublishedTable.Defs.Columns.Added);
            Columns.Create("Added", PublishedTable.Defs.Columns.Added).MakeDate();
            Columns.Create("Published", PublishedTable.Defs.Columns.Published).MakeDate();
            Columns.Create("Publisher", PublishedTable.Defs.Columns.Joined.Publisher);
            Columns.Create("Url",  PublishedTable.Defs.Columns.Url);
            Owner.Commands.Add("PublishedAdd", RunAddPublishedCommand);
            Owner.Commands.Add("PublishedRemove", RunRemovePublishedCommand, (o) => SelectedRow != null);
            HeaderPreface = Strings.HeaderPublished;
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
            Int64 titleId = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = String.Format("{0}={1}", PublishedTable.Defs.Columns.TitleId, titleId);
        }

        /// <summary>
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> to open the url of the published title.
        /// </summary>
        /// <param name="item">The <see cref="DataRowView"/> object of the selected row.</param>
        protected override void RunOpenRowCommand(object item)
        {
            DataRowView view = item as DataRowView;
            if (view != null)
            {
                string url = view.Row[PublishedTable.Defs.Columns.Url].ToString();
                if (!String.IsNullOrEmpty(url))
                {
                    OpenHelper.OpenWebSite(null, url);
                }
            }
        }
        #endregion
        
        /************************************************************************/

        #region Private methods
        private void RunAddPublishedCommand(object o)
        {
            var select = WindowFactory.PublisherSelect.Create(Strings.WindowTitleSelectPublisherForPublished);
            select.ShowDialog();
            var vm = select.GetValue(WindowViewModel.ViewModelProperty) as PublisherSelectWindowViewModel;
            if (vm != null)
            {
                Execution.TryCatch(() =>
                    {
                        Int64 publisherId = vm.SelectedPublisherId;
                        if (publisherId > 0)
                        {
                            Int64 titleId = (Int64)Owner.SelectedPrimaryKey;
                            DatabaseController.Instance.GetTable<PublishedTable>().Add(titleId, publisherId);
                        }
                    });
            }
        }
      
        private void RunRemovePublishedCommand(object o)
        {
            if (SelectedRow != null && Messages.ShowYesNo(Strings.ConfirmationRemoveTitlePublished))
            {
                SelectedRow.Delete();
                DatabaseController.Instance.GetTable<PublishedTable>().Save();
            }
        }
        #endregion
    }
}
