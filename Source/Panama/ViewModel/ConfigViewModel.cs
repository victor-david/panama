using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
    public class ConfigViewModel : WorkspaceViewModel // DataGridViewModel<ConfigTable>
    {
        #region Private
        private Int64 selectedSection;
        //private Color colorPeriodPublisher;
        //private Color colorPublishedTitle;
        //private Color colorSubmittedTitle;
        private Color myColor;
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

        public Color MyColor
        {
            get => myColor;
            set
            {
                myColor = value;
                Debug.WriteLine(value.ToString());
            }
        }
        //public Color ColorPeriodPublisher
        //{
        //    get => colorPeriodPublisher;
        //    set
        //    {
        //        if (SetProperty(ref colorPeriodPublisher, value))
        //        {
        //            Config.ColorPeriodPublisher = value;
        //        }
        //    }
        //}

        //public Color ColorPublishedTitle
        //{
        //    get => colorPublishedTitle;
        //    set
        //    {
        //        if (SetProperty(ref colorPublishedTitle, value))
        //        {
        //            Config.ColorPublishedTitle = value;
        //        }
        //    }
        //}

        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigViewModel"/> class.
        /// </summary>
        public ConfigViewModel()
        {
            DisplayName = Strings.CommandConfig;
            MaxCreatable = 1;
            Commands.Add("SwitchSection", RunSwitchSection);
            Commands.Add("ResetColors", RunResetColorSelections);
            InitializeSections();
            InitializeDateFormatOptions();
            InitializeColors();
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        #endregion

        /************************************************************************/

        #region Private Methods

        private void InitializeSections()
        {
            Sections = new GeneralOptionList
            {
                new GeneralOption() { IntValue = 1, Value = Strings.HeaderSettingsSectionGeneral, Command = Commands["SwitchSection"], CommandParm = 1 },
                new GeneralOption() { IntValue = 2, Value = Strings.HeaderSettingsSectionFolder, Command = Commands["SwitchSection"], CommandParm = 2 },
                new GeneralOption() { IntValue = 3, Value = Strings.HeaderSettingsSectionColor, Command = Commands["SwitchSection"], CommandParm = 3 },
                new GeneralOption() { IntValue = 4, Value = Strings.HeaderSettingsSectionSubmission, Command = Commands["SwitchSection"], CommandParm = 4 }
            };
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

        private void InitializeColors()
        {
            //ColorPeriodPublisher = Config.ColorPeriodPublisher;
            //ColorPublishedTitle = Config.ColorPublishedTitle;
            MyColor = Colors.Red;

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


        private void RunResetColorSelections(object parm)
        {
            Config.ColorPeriodPublisher = (Color)ColorConverter.ConvertFromString("#74C1FFC1");
            Config.ColorPublishedTitle = (Color)ColorConverter.ConvertFromString("#FFD0FFC9");
            Config.ColorSubmittedTitle = (Color)ColorConverter.ConvertFromString("#FFFF0000");
        }
        #endregion
    }
}