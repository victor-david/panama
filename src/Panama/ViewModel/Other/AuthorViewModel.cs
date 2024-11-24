using Restless.Panama.Core;
using Restless.Panama.Core.Converters;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System.Data;
using System.Globalization;
using TableColumns = Restless.Panama.Database.Tables.AuthorTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for authors management.
    /// </summary>
    public class AuthorViewModel : DataRowViewModel<AuthorTable>
    {
        #region Private
        private AuthorRow selectedAuthor;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => (SelectedAuthor?.Id ?? AuthorTable.Defs.Values.SystemAuthorId) != AuthorTable.Defs.Values.SystemAuthorId;

        /// <summary>
        /// Gets the currently selected author
        /// </summary>
        public AuthorRow SelectedAuthor
        {
            get => selectedAuthor;
            private set => SetProperty(ref selectedAuthor, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorViewModel"/> class.
        /// </summary>
        public AuthorViewModel()
        {
            Columns.Create(Header.Id, TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .MakeInitialSortAscending();

            Columns.CreateResource<BooleanToResourceConverter>(Header.DefaultShort, TableColumns.IsDefault, ResourceKeys.Icon.IconSquare)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(ToolTip.AuthorDefault);

            Columns.Create(Header.Name, TableColumns.Name);

            /* Context menu items */
            MenuItems.AddItem(Menu.AddAuthor, AddCommand).AddIconResource(ResourceKeys.Icon.IconAdd);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Menu.DeleteAuthor, DeleteCommand).AddIconResource(ResourceKeys.Icon.IconDelete);
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <inheritdoc/>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedAuthor = AuthorRow.Create(SelectedRow);
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            return DataRowCompareLong(item1, item2, TableColumns.Id);
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            if (MessageWindow.ShowContinueCancel(Confirm.AddAuthor))
            {
                Table.AddDefaultRow();
                Table.Save();
                // Filters.ClearAll();
                ForceListViewSort();
            }
        }

        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
        protected override void RunDeleteCommand()
        {
            if (DeleteCommandEnabled)
            {
                int childRowCount = SelectedRow.GetChildRows(AuthorTable.Defs.Relations.ToTitle).Length;
                if (childRowCount > 0)
                {
                    MessageWindow.ShowError(string.Format(CultureInfo.InvariantCulture, Error.CannotDeleteAuthor, childRowCount));
                    return;
                }

                if (MessageWindow.ShowYesNo(Confirm.DeleteAuthor))
                {
                    DeleteSelectedRow();
                }
            }
        }
        #endregion
    }
}