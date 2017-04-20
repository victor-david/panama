using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Search;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for application settings.
    /// </summary>
    public class ConfigViewModel : DataGridViewModel<ConfigTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets a visibility value that determines if the boolean edit control is visible.
        /// </summary>
        public Visibility BooleanTypeVisibility
        {
            get { return GetVisibilityForTypes(ConfigTable.Defs.Types.Boolean); }
        }

        /// <summary>
        /// Gets a visibility value that determines if the string edit control is visible.
        /// </summary>
        public Visibility StringTypeVisibility
        {
            get { return GetVisibilityForTypes(ConfigTable.Defs.Types.String); }
        }

        /// <summary>
        /// Gets a visibility value that determines if the string edit control is visible.
        /// </summary>
        public Visibility MultiStringTypeVisibility
        {
            get { return GetVisibilityForTypes(ConfigTable.Defs.Types.MultiString); }
        }

        /// <summary>
        /// Gets a visibility value that determines if the color edit control is visible.
        /// </summary>
        public Visibility ColorTypeVisibility
        {
            get { return GetVisibilityForTypes(ConfigTable.Defs.Types.Color); }
        }

        /// <summary>
        /// Gets a visibility value that determines if the path edit control is visible.
        /// </summary>
        public Visibility PathTypeVisibility
        {
            get { return GetVisibilityForTypes(ConfigTable.Defs.Types.Path, ConfigTable.Defs.Types.Mapi); }
        }

        /// <summary>
        /// Gets a visibility value that determines if the object view control is visible.
        /// </summary>
        public Visibility ObjectTypeVisibility
        {
            get { return GetVisibilityForTypes(ConfigTable.Defs.Types.Object); }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public ConfigViewModel()
        {
            DisplayName = Strings.CommandConfig;
            MaxCreatable = 1;
            RawCommands.Add("Apply",RunApplyCommand, CanRunApplyOrRevertCommand);
            RawCommands.Add("Revert",RunRevertCommand, CanRunApplyOrRevertCommand);
            RawCommands.Add("Path", RunFolderSelectCommand);
            Columns.SetDefaultSort(Columns.Create("Id", ConfigTable.Defs.Columns.Id), ListSortDirection.Ascending);
            Columns.Create("Description", ConfigTable.Defs.Columns.Description).MakeFlexWidth(2);
            Columns.Create("Value", ConfigTable.Defs.Columns.Value).MakeSingleLine();
#if !DEBUG
            DataView.RowFilter = String.Format("{0}=1", ConfigTable.Defs.Columns.Edit);
#endif
            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandApplyConfig, Strings.CommandApplyConfigTooltip, RawCommands["Apply"], ResourceHelper.Get("ImageSave"), VisualCommandImageSize, VisualCommandFontSize));
            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandRevertConfig, Strings.CommandRevertConfigTooltip, RawCommands["Revert"], ResourceHelper.Get("ImageUndo"), VisualCommandImageSize, VisualCommandFontSize));
            DeleteCommand.Supported = CommandSupported.NoWithException;

        }
        #pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called when the selected item on the associated data grid has changed.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            OnPropertyChanged("BooleanTypeVisibility");
            OnPropertyChanged("StringTypeVisibility");
            OnPropertyChanged("MultiStringTypeVisibility");
            OnPropertyChanged("ColorTypeVisibility");
            OnPropertyChanged("PathTypeVisibility");
            OnPropertyChanged("ObjectTypeVisibility");
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private Visibility GetVisibilityForTypes(params string[] types)
        {
            if (SelectedRow != null)
            {
                foreach (string type in types)
                {
                    if (SelectedRow[ConfigTable.Defs.Columns.Type].ToString() == type)
                    {
                        return Visibility.Visible;
                    }
                }
            }
            return Visibility.Collapsed;
        }

        //private void RunPathPickerCommand(object o)
        //{
        //    using (var dialog = new CommonOpenFileDialog())
        //    {
        //        dialog.IsFolderPicker = true;

        //        string dir = SelectedRow[ConfigTable.Defs.Columns.Value].ToString();
        //        //dir = dir.Replace(@"\\", @"\");
        //        if (System.IO.Directory.Exists(dir))
        //        {
        //            dialog.InitialDirectory = dir;
        //        }
        //        CommonFileDialogResult result = dialog.ShowDialog();
        //        if (result == CommonFileDialogResult.Ok)
        //        {
        //            SelectedRow[ConfigTable.Defs.Columns.Value] = dialog.FileName;
        //            OnPropertyChanged("SelectedRow");
        //        }
        //    }
        //}

        private void RunFolderSelectCommand(object o)
        {
            if (SelectedRow != null)
            {
                switch (SelectedRow[ConfigTable.Defs.Columns.Type].ToString())
                {
                    case ConfigTable.Defs.Types.Path:
                        SelectFolder();
                        break;
                    case ConfigTable.Defs.Types.Mapi:
                        SelectMapiFolder();
                        break;
                }
            }
        }

        private void SelectFolder()
        {
            string dir = SelectedRow[ConfigTable.Defs.Columns.Value].ToString();
            using (var dialog = CommonDialogFactory.Create(dir, "Select a directory", true))
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    SelectedRow[ConfigTable.Defs.Columns.Value] = dialog.FileName;
                    OnPropertyChanged("SelectedRow");
                }
            }
        }

        private void SelectMapiFolder()
        {
            MessageSelectOptions ops = new MessageSelectOptions(MessageSelectMode.Folder, null);
            var w = WindowFactory.MessageSelect.Create(Strings.CaptionSelectMapiFolder, ops);
            w.ShowDialog();
            var vm = w.GetValue(WindowViewModel.ViewModelProperty) as MessageSelectWindowViewModel;

            if (vm != null && vm.SelectedItems != null)
            {
                var result = vm.SelectedItems[0] as WindowsSearchResult;
                if (result!= null)
                {
                    string url = result.Values[SysProps.System.ItemUrl].ToString();
                    // remove "mapi:" from string
                    SelectedRow[ConfigTable.Defs.Columns.Value] = url.Remove(0, 5);
                    OnPropertyChanged("SelectedRow");
                }
            }
        }

        private void RunApplyCommand(object o)
        {
            Table.Save();
        }

        private void RunRevertCommand(object o)
        {
            Table.RejectChanges();
            OnSelectedItemChanged();
        }

        private bool CanRunApplyOrRevertCommand(object o)
        {
            var t = Table.GetChanges(System.Data.DataRowState.Modified);
            return (t != null);
        }

        #endregion
    }
}