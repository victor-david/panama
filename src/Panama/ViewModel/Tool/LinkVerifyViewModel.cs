using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Controls;
using System.Data;
using System.Threading.Tasks;
using TableColumns = Restless.Panama.Database.Tables.LinkVerifyTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    public class LinkVerifyViewModel : DataRowViewModel<LinkVerifyTable>
    {
        #region Private
        private LinkVerifyRow selectedLink;
        private bool operationInProgress;
        private bool isCanceling;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the currently selected author
        /// </summary>
        public LinkVerifyRow SelectedLink
        {
            get => selectedLink;
            private set => SetProperty(ref selectedLink, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the verification operation is in progress
        /// </summary>
        public bool OperationInProgress
        {
            get => operationInProgress;
            private set => SetProperty(ref operationInProgress, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the verification operation is canceling
        /// </summary>
        public bool IsCanceling
        {
            get => isCanceling;
            private set => SetProperty(ref isCanceling, value);
        }
        #endregion

        /************************************************************************/

        #region Constuctor
        /// <summary>
        /// Initializes a new instance of the <see cref="LinkVerifyViewModel"/> class
        /// </summary>
        public LinkVerifyViewModel()
        {
            Columns.Create("Id", TableColumns.Id).MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Xid", TableColumns.Xid).MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Source", TableColumns.Source)
                .MakeFixedWidth(FixedWidth.W096)
                .MakeInitialSortAscending();

            Columns.Create("Url", TableColumns.Url);

            Columns.Create("Scanned", TableColumns.Scanned)
                .MakeDate();

            Columns.Create("Status", TableColumns.Status)
                .MakeFixedWidth(FixedWidth.W064)
                .MakeCentered();

            Commands.Add("Refresh", RunRefreshCommand);
            Commands.Add("Verify", RunVerifyCommand);
            Commands.Add("Cancel", p => IsCanceling = true);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void OnActivated()
        {
            base.OnActivated();
        }

        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedLink = LinkVerifyRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareString(item1, item2, TableColumns.Source);
        }
        #endregion

        private void RunRefreshCommand(object parm)
        {
            Table.Refresh();
            ListView.Refresh();
        }

        private async void RunVerifyCommand(object parm)
        {
            OperationInProgress = true;
            IsCanceling = false;

            foreach (LinkVerifyRow link in Table.EnumerateAll())
            {
                if (!IsCanceling)
                {
                    await Task.Delay(1000);
                    link.SetScanned();
                }
            }

            OperationInProgress = false;
            IsCanceling = false;
        }
    }
}