using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.Tools.Controls;
using System.ComponentModel;
using System.Data;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides logic to display publishers that are related to a credential.
    /// </summary>
    public class CredentialPublisherController : ControllerBase<CredentialViewModel, CredentialTable>
    {
        #region Private
        #endregion

        /************************************************************************/
        
        #region Public properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialPublisherController"/> class.
        /// </summary>
        /// <param name="owner">The owner of this controller.</param>
        public CredentialPublisherController(CredentialViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<PublisherTable>());
            DataView.RowFilter = string.Format("{0}={1}", PublisherTable.Defs.Columns.CredentialId, -1);
            Columns.Create("Id", PublisherTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.SetDefaultSort(Columns.Create("Publisher", PublisherTable.Defs.Columns.Name), ListSortDirection.Ascending);
            Columns.Create("Added", PublisherTable.Defs.Columns.Added).MakeDate();
            Columns.Create("Last Sub", PublisherTable.Defs.Columns.Calculated.LastSub).MakeDate();
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
            long credentialId = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = string.Format("{0}={1}", PublisherTable.Defs.Columns.CredentialId, credentialId);
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(PublisherTable.Defs.Columns.Name, ListSortDirection.Ascending));
        }
        #endregion
    }
}
