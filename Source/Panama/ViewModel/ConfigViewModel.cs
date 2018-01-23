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
        private Int64 selectedSection;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the sections list.
        /// </summary>
        public GeneralOptionList Sections
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the selected section.
        /// </summary>
        public Int64 SelectedSection
        {
            get => selectedSection;
            private set => SetProperty(ref selectedSection, value);
        }

        /// <summary>
        /// Gets the list of available date formats
        /// </summary>
        public GeneralOptionList DateFormats
        {
            get;
            private set;
        }

        ///// <summary>
        ///// Gets a visibility value that determines if the boolean edit control is visible.
        ///// </summary>
        //public Visibility BooleanTypeVisibility
        //{
        //    get { return GetVisibilityForTypes(ConfigTable.Defs.Types.Boolean); }
        //}

        ///// <summary>
        ///// Gets a visibility value that determines if the string edit control is visible.
        ///// </summary>
        //public Visibility StringTypeVisibility
        //{
        //    get { return GetVisibilityForTypes(ConfigTable.Defs.Types.String); }
        //}

        ///// <summary>
        ///// Gets a visibility value that determines if the string edit control is visible.
        ///// </summary>
        //public Visibility MultiStringTypeVisibility
        //{
        //    get { return GetVisibilityForTypes(ConfigTable.Defs.Types.MultiString); }
        //}

        ///// <summary>
        ///// Gets a visibility value that determines if the color edit control is visible.
        ///// </summary>
        //public Visibility ColorTypeVisibility
        //{
        //    get { return GetVisibilityForTypes(ConfigTable.Defs.Types.Color); }
        //}

        ///// <summary>
        ///// Gets a visibility value that determines if the path edit control is visible.
        ///// </summary>
        //public Visibility PathTypeVisibility
        //{
        //    get { return GetVisibilityForTypes(ConfigTable.Defs.Types.Path, ConfigTable.Defs.Types.Mapi); }
        //}

        ///// <summary>
        ///// Gets a visibility value that determines if the object view control is visible.
        ///// </summary>
        //public Visibility ObjectTypeVisibility
        //{
        //    get { return GetVisibilityForTypes(ConfigTable.Defs.Types.Object); }
        //}
        #endregion

        /************************************************************************/

        #region Constructor
#pragma warning disable 1591
        public ConfigViewModel()
        {
            DisplayName = Strings.CommandConfig;
            MaxCreatable = 1;
            Commands.Add("SwitchSection", RunSwitchSection);
            //Commands.Add("Apply",RunApplyCommand, CanRunApplyOrRevertCommand);
            //Commands.Add("Revert",RunRevertCommand, CanRunApplyOrRevertCommand);
            Commands.Add("Path", RunFolderSelectCommand);
            Columns.SetDefaultSort(Columns.Create("Id", ConfigTable.Defs.Columns.Id), ListSortDirection.Ascending);
            // Columns.Create("Description", ConfigTable.Defs.Columns.Description).MakeFlexWidth(2);
            Columns.Create("Value", ConfigTable.Defs.Columns.Value).MakeSingleLine();
#if !DEBUG
            // DataView.RowFilter = String.Format("{0}=1", ConfigTable.Defs.Columns.Edit);
#endif
            //VisualCommands.Add(new VisualCommandViewModel(Strings.CommandApplyConfig, Strings.CommandApplyConfigTooltip, Commands["Apply"], ResourceHelper.Get("ImageSave"), VisualCommandImageSize, VisualCommandFontSize));
            //VisualCommands.Add(new VisualCommandViewModel(Strings.CommandRevertConfig, Strings.CommandRevertConfigTooltip, Commands["Revert"], ResourceHelper.Get("ImageUndo"), VisualCommandImageSize, VisualCommandFontSize));
            DeleteCommand.Supported = CommandSupported.NoWithException;

            InitializeSections();
            InitializeDateFormatOptions();

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
            //OnPropertyChanged(nameof(BooleanTypeVisibility));
            //OnPropertyChanged(nameof(StringTypeVisibility));
            //OnPropertyChanged(nameof(MultiStringTypeVisibility));
            //OnPropertyChanged(nameof(ColorTypeVisibility));
            //OnPropertyChanged(nameof(PathTypeVisibility));
            //OnPropertyChanged(nameof(ObjectTypeVisibility));
        }
        #endregion

        /************************************************************************/

        #region Private Methods

        private void InitializeSections()
        {
            Sections = new GeneralOptionList
            {
                new GeneralOption() { IntValue = 1, Value = Strings.HeaderSettingsGeneral, Command = Commands["SwitchSection"], CommandParm = 1 },
                new GeneralOption() { IntValue = 2, Value = Strings.HeaderSettingsFolder, Command = Commands["SwitchSection"], CommandParm = 2 },
                new GeneralOption() { IntValue = 3, Value = Strings.HeaderSettingsColor, Command = Commands["SwitchSection"], CommandParm = 3 }
            };
            //Sections.Add(new GeneralOption() { IntValue = 5, Value = Strings.HeaderSettingsCurrency, Command = RawCommands["SwitchSection"], CommandParm = 5 });
            //Sections.Add(new GeneralOption() { IntValue = 6, Value = Strings.HeaderSettingsCategoryChart, Command = RawCommands["SwitchSection"], CommandParm = 6 });
            RunSwitchSection(Config.SelectedConfigSection);
            OnPropertyChanged(nameof(Sections));
        }



        private void InitializeDateFormatOptions()
        {
            DateFormats = new GeneralOptionList();
            DateFormats.AddObservable(new GeneralOption() { Value = "MM-dd-yy" });
            DateFormats[0].Name = DateTime.Now.ToString(DateFormats[0].Value);
            DateFormats[0].IsSelected = Config.DateFormat == DateFormats[0].Value;

            DateFormats.AddObservable(new GeneralOption() { Value = "MMM dd, yyyy" });
            DateFormats[1].Name = DateTime.Now.ToString(DateFormats[1].Value);
            DateFormats[1].IsSelected = Config.DateFormat == DateFormats[1].Value;

            DateFormats.AddObservable(new GeneralOption() { Value = "dd-MMM-yyyy" });
            DateFormats[2].Name = DateTime.Now.ToString(DateFormats[2].Value);
            DateFormats[2].IsSelected = Config.DateFormat == DateFormats[2].Value;

            DateFormats.AddObservable(new GeneralOption() { Value = Config.DateFormat });
            DateFormats[3].Name = "Other";
            DateFormats[3].IsSelected = !DateFormats[0].IsSelected && !DateFormats[1].IsSelected && !DateFormats[2].IsSelected;

            DateFormats.IsSelectedChanged += (s, e) =>
            {
                var op = (GeneralOption)s;
                if (op.IsSelected)
                {
                    // op 3 binds directly to Config.DateFormat
                    if (op.Index != 3)
                    {
                        Config.DateFormat = op.Value;
                    }
                }
            };
        }










        private void RunSwitchSection(object parm)
        {
            int selected = (int)parm;
            foreach (var section in Sections)
            {
                section.IsSelected = (section.IntValue == selected);
            }
            SelectedSection = selected;
            Config.SelectedConfigSection = selected;
        }


        //private Visibility GetVisibilityForTypes(params string[] types)
        //{
        //    if (SelectedRow != null)
        //    {
        //        foreach (string type in types)
        //        {
        //            if (SelectedRow[ConfigTable.Defs.Columns.Type].ToString() == type)
        //            {
        //                return Visibility.Visible;
        //            }
        //        }
        //    }
        //    return Visibility.Collapsed;
        //}

        private void RunFolderSelectCommand(object o)
        {
            if (SelectedRow != null)
            {
                //switch (SelectedRow[ConfigTable.Defs.Columns.Type].ToString())
                //{
                //    case ConfigTable.Defs.Types.Path:
                //        SelectFolder();
                //        break;
                //    case ConfigTable.Defs.Types.Mapi:
                //        SelectMapiFolder();
                //        break;
                //}
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
                    OnPropertyChanged(nameof(SelectedRow));
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
                    OnPropertyChanged(nameof(SelectedRow));
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