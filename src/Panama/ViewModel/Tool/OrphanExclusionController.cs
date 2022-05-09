using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using TableColumns = Restless.Panama.Database.Tables.OrphanExclusionTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    public class OrphanExclusionController : DataRowViewModel<OrphanExclusionTable>
    {
        private OrphanExclusionRow selectedOrphan;

        #region Properties
        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => !SelectedOrphan?.IsSystem ?? true;

        /// <summary>
        /// Gets the selected orphan
        /// </summary>
        public OrphanExclusionRow SelectedOrphan
        {
            get => selectedOrphan;
            private set => SetProperty(ref selectedOrphan, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="OrphanExclusionController"/> class
        /// </summary>
        public OrphanExclusionController()
        {
            Columns.Create("Id", TableColumns.Id)
                .MakeFixedWidth(FixedWidth.W042)
                .MakeInitialSortAscending();

            Columns.Create("Type", TableColumns.Type);
            Columns.Create("Value", TableColumns.Value);
            Columns.Create("Created", TableColumns.Created)
                .MakeDate();

            MenuItems.AddItem(Strings.MenuItemRemoveExclusion, DeleteCommand)
                .AddIconResource(ResourceKeys.Icon.XMediumIconKey);
        }
        #endregion

        /************************************************************************/

        #region Proptected methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedOrphan = OrphanExclusionRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            DeleteSelectedRow();
        }
        #endregion
    }
}