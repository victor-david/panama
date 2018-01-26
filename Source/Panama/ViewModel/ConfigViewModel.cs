using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<SampleTitle> sampleTitles;
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

        /// <summary>
        /// Gets the list of sample titles used to preview color selections.
        /// </summary>
        public ObservableCollection<SampleTitle> SampleTitles
        {
            get => sampleTitles;
            private set => SetProperty(ref sampleTitles, value);
        }
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
            Commands.Add("ColorChanged", RunSelectedColorChanged);
            InitializeSections();
            InitializeDateFormatOptions();
            InitializeSampleTitles();
            InitializeSamplePublications();
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
            string[] formats = {"MM-dd-yy", "MMM dd, yyyy", "dd-MMM-yyyy", "dd-MM-yy", "dd-MM-yyyy" };
            DateFormats = new GeneralOptionList();
            foreach (string format in formats)
            {
                DateFormats.AddObservable(new GeneralOption()
                {
                    Value = format,
                    Name = DateTime.Now.ToString(format),
                    IsSelected = Config.DateFormat == format
                });
            }

            DateFormats.IsSelectedChanged += (s, e) =>
            {
                var op = (GeneralOption)s;
                if (op.IsSelected)
                {
                    Config.DateFormat = op.Value;
                }
                InitializeSampleTitles();
            };
        }

        private void InitializeSampleTitles()
        {
            SampleTitles = new ObservableCollection<SampleTitle>()
            {
                new SampleTitle(1, "Title #1", DateTime.Now.AddDays(-10), DateTime.Now.AddDays(-10), false, false),
                new SampleTitle(2, "Title #2 (published)", DateTime.Now.AddDays(-20), DateTime.Now.AddDays(-17),true, false),
                new SampleTitle(3, "Title #3", DateTime.Now.AddDays(-30), DateTime.Now.AddDays(-30), false, false),
                new SampleTitle(4, "Title #4 (submitted)", DateTime.Now.AddDays(-40), DateTime.Now.AddDays(-7), false, true),
                new SampleTitle(5, "Title #5", DateTime.Now.AddDays(-50), DateTime.Now.AddDays(-20), false, false),
            };
        }

        private void InitializeSamplePublications()
        {
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
            Config.ResetColors();
        }

        private void RunSelectedColorChanged(object parm)
        {
            InitializeSampleTitles();
        }


        #endregion

        /************************************************************************/

        #region Helper classes (used for color samples)
        public class SampleTitle : BindableBase
        {
            public Int64 Id
            {
                get;
                private set;
            }

            public string Title
            {
                get;
                private set;
            }

            public DateTime Written
            {
                get;
                private set;
            }

            public DateTime Updated
            {
                get;
                private set;
            }

            public bool IsPublished
            {
                get;
                private set;
            }

            public bool IsSubmitted
            {
                get;
                private set;
            }

            public SampleTitle(Int64 id, string title, DateTime written, DateTime updated, bool isPublished, bool isSubmitted)
            {
                Id = id;
                Title = title;
                Written = written;
                Updated = updated;
                IsPublished = isPublished;
                IsSubmitted = isSubmitted;
            }
        }
        #endregion
    }
}