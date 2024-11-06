using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using System.Data;
using System.Linq;
using TableColumns = Restless.Panama.Database.Tables.ThemeTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    public class SettingsThemeController : DataRowViewModel<ThemeTable>
    {
        #region Private
        private ThemeRow selectedTheme;
        #endregion

        /************************************************************************/

        #region Properties
        public ThemeRow SelectedTheme
        {
            get => selectedTheme;
            private set => SetProperty(ref selectedTheme, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsThemeController"/> class
        /// </summary>
        public SettingsThemeController()
        {
            Columns.CreateResource<BooleanToResourceConverter>("E", TableColumns.IsEnabled, ResourceKeys.Icon.IconCheck)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipThemeEnabled);

            Columns.Create("Base", TableColumns.ThemeBase);
            Columns.Create("Color", TableColumns.ThemeColor);

            SetInitialSelectedTheme();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedTheme = ThemeRow.Create(SelectedRow);
            SetTheme();
        }

        protected override bool OnDataRowFilter(DataRow item)
        {
            return base.OnDataRowFilter(item);
        }

        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            int value = DataRowCompareString(item2, item1, TableColumns.ThemeBase);
            if (value == 0)
            {
                value = DataRowCompareString(item1, item2, TableColumns.ThemeColor);
            }
            return value;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void SetTheme()
        {
            if (SelectedTheme != null)
            {
                ThemeManager.SetTheme(SelectedTheme.ThemeId);
                Config.ThemeId = SelectedTheme.ThemeId;
            }
        }

        private void SetInitialSelectedTheme()
        {
            DataRowView selected = null;
            foreach (DataRowView view in ListView.OfType<DataRowView>())
            {
                if (view.Row[TableColumns.ThemeId].Equals(Config.ThemeId))
                {
                    selected = view;
                    break;
                }
            }
            SelectedItem = selected;
        }
        #endregion
    }
}