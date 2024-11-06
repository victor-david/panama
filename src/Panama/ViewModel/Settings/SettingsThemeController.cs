using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Mvvm;
using System.Data;
using System.Linq;
using System.Windows.Input;
using TableColumns = Restless.Panama.Database.Tables.ThemeTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    public class SettingsThemeController : DataRowViewModel<ThemeTable>
    {
        #region Properties
        public ICommand ResetThemeCommand { get; }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsThemeController"/> class
        /// </summary>
        public SettingsThemeController()
        {
            Columns.Create("Base", TableColumns.ThemeBase);
            Columns.Create("Color", TableColumns.ThemeColor);

            ResetThemeCommand = RelayCommand.Create(p => RunResetThemeCommand());

            SetSelectedTheme();
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SetTheme();
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
            if (SelectedRow != null)
            {
                string themeId = SelectedRow[TableColumns.ThemeId].ToString();
                ThemeManager.SetTheme(themeId);
                Config.ThemeId = themeId;
            }
        }

        private void SetSelectedTheme()
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

        private void RunResetThemeCommand()
        {
            Config.ThemeId = ThemeManager.DefaultTheme;
            ThemeManager.SetTheme(Config.ThemeId);
            SetSelectedTheme();
        }
        #endregion
    }
}