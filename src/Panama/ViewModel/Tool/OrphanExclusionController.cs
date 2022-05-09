using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System;
using System.Globalization;
using System.Windows.Data;
using TableColumns = Restless.Panama.Database.Tables.OrphanExclusionTable.Defs.Columns;
using TableValues = Restless.Panama.Database.Tables.OrphanExclusionTable.Defs.Values;

namespace Restless.Panama.ViewModel
{
    public class OrphanExclusionController : DataRowViewModel<OrphanExclusionTable>
    {
        private OrphanExclusionRow selectedOrphan;

        #region Properties
        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => !(SelectedOrphan?.IsSystem ?? true);

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

            Columns.Create<OrphanTypeConverter>("Type", TableColumns.Type)
                .MakeFixedWidth(FixedWidth.W076);

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

        #region Private helper class
        private class OrphanTypeConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value is long type
                    ? type switch
                    {
                        TableValues.FileType => "File",
                        TableValues.FileExtensionType => "Extension",
                        TableValues.DirectoryType => "Directory",
                        _ => "???"
                    }
                    : value;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        #endregion

    }
}