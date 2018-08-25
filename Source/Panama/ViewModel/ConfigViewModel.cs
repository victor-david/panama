using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
using ConfigDefault = Restless.App.Panama.Configuration.Config.Default;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for application settings.
    /// </summary>
    public class ConfigViewModel : WorkspaceViewModel
    {
        #region Private
        private Int64 selectedSection;
        private ObservableCollection<SampleTitle> sampleTitles;
        private ObservableCollection<SamplePublisher> samplePublishers;

        private ColorSortingMode colorSortingMode;
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

        /// <summary>
        /// Gets the list of sample titles used to preview color selections.
        /// </summary>
        public ObservableCollection<SamplePublisher> SamplePublishers
        {
            get => samplePublishers;
            private set => SetProperty(ref samplePublishers, value);
        }

        /// <summary>
        /// Gets a list of modes that may be used to sort the color palette.
        /// </summary>
        public GeneralOptionList ColorSortingModes
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the mode used to sort the color pallete.
        /// </summary>
        public ColorSortingMode ColorSortingMode
        {
            get => colorSortingMode;
            set
            {
                if (SetProperty(ref colorSortingMode, value))
                {
                    Config.ColorSortingMode = value;
                }
            }
        }

        /// <summary>
        /// Gets the range of values used to set the data grid view row height.
        /// </summary>
        public IEnumerable<int> DataGridRowHeight
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the range of values used to set the data grid view alternation count.
        /// </summary>
        public IEnumerable<int> DataGridAlternation
        {
            get;
            private set;
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
            Commands.Add("SwitchColorMode", RunSwitchColorMode);
            Commands.Add("ResetColors", RunResetColorSelections);
            Commands.Add("ColorChanged", RunSelectedColorChanged);
            InitializeSections();
            InitializeDateFormatOptions();
            InitializeSampleTitles();
            InitializeSamplePublications();
            InitializeColorSettings();
            InitializeRanges();
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        #endregion

        /************************************************************************/

        #region Private Methods

        private void InitializeSections()
        {
            Sections = new GeneralOptionList()
            {
                new GeneralOption() { IntValue = 1, Name = Strings.HeaderSettingsSectionGeneral, Command = Commands["SwitchSection"], CommandParm = 1 },
                new GeneralOption() { IntValue = 2, Name = Strings.HeaderSettingsSectionFolder, Command = Commands["SwitchSection"], CommandParm = 2 },
                new GeneralOption() { IntValue = 3, Name = Strings.HeaderSettingsSectionColor, Command = Commands["SwitchSection"], CommandParm = 3 },
                new GeneralOption() { IntValue = 4, Name = Strings.HeaderSettingsSectionSubmission, Command = Commands["SwitchSection"], CommandParm = 4 }
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

        private void InitializeColorSettings()
        {
            ColorSortingMode = Config.ColorSortingMode;
            ColorSortingModes = new GeneralOptionList()
            {
                new GeneralOption()
                {
                    Name = "Alpha", Value = Strings.TooltipColorAlpha,
                    Command = Commands["SwitchColorMode"], CommandParm = ColorSortingMode.Alpha
                },
                new GeneralOption()
                {
                    Name = "HSB", Value = Strings.TooltipColorHSB,
                    Command = Commands["SwitchColorMode"], CommandParm= ColorSortingMode.HSB
                }

            };
            RunSwitchColorMode(Config.ColorSortingMode);
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
            SamplePublishers = new ObservableCollection<SamplePublisher>()
            {
                new SamplePublisher(1, "Publisher #1", DateTime.Now.AddDays(-60), DateTime.Now.AddDays(-10), false, false),
                new SamplePublisher(2, "Publisher #2 (in period)", DateTime.Now.AddDays(-90), DateTime.Now.AddDays(-80), true, false),
                new SamplePublisher(3, "Publisher #3", DateTime.Now.AddDays(-110), DateTime.Now.AddDays(-90), false, false),
                new SamplePublisher(4, "Publisher #4 (goner)", DateTime.Now.AddDays(-150), DateTime.Now.AddDays(-148), false, true),
                new SamplePublisher(5, "Publisher #5", DateTime.Now.AddDays(-160), DateTime.Now.AddDays(-5), false, false),
            };
        }

        private void InitializeRanges()
        {
            DataGridRowHeight = Enumerable.Range(ConfigDefault.DataGrid.MinRowHeight, ConfigDefault.DataGrid.MaxRowHeight - ConfigDefault.DataGrid.MinRowHeight + 1);
            DataGridAlternation = Enumerable.Range(ConfigDefault.DataGrid.MinAlternationCount, ConfigDefault.DataGrid.MaxAlternationCount - ConfigDefault.DataGrid.MinAlternationCount + 1);
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

        private void RunSwitchColorMode(object parm)
        {
            ColorSortingMode selected = (ColorSortingMode)parm;
            foreach (var item in ColorSortingModes)
            {
                item.IsSelected = ((ColorSortingMode)item.CommandParm == selected);
            }
            ColorSortingMode = selected;
        }

        private void RunResetColorSelections(object parm)
        {
            Config.Colors.Reset();
        }

        private void RunSelectedColorChanged(object parm)
        {
            InitializeSampleTitles();
            InitializeSamplePublications();
        }
        #endregion

        /************************************************************************/

        #region Helper classes (used for color samples)
        /// <summary>
        /// Represents a sample title. Used to display configuration colors.
        /// </summary>
        public class SampleTitle
        {
            /// <summary>
            /// Gets the id of the sample title.
            /// </summary>
            public Int64 Id
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the title.
            /// </summary>
            public string Title
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the date written.
            /// </summary>
            public DateTime Written
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the date updated.
            /// </summary>
            public DateTime Updated
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a value that indicates if the title is published.
            /// </summary>
            public bool CalcIsPublished
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a value that indicates if the title is submitted.
            /// </summary>
            public bool CalcIsSubmitted
            {
                get;
                private set;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SampleTitle"/> class.
            /// </summary>
            /// <param name="id">The sample id.</param>
            /// <param name="title">The title.</param>
            /// <param name="written">The date the title was written.</param>
            /// <param name="updated">The date the title was updated.</param>
            /// <param name="isPublished">A boolean value that indicates if the title is published.</param>
            /// <param name="isSubmitted">A boolean value that indicates if the title is submitted.</param>
            public SampleTitle(Int64 id, string title, DateTime written, DateTime updated, bool isPublished, bool isSubmitted)
            {
                Id = id;
                Title = title;
                Written = written;
                Updated = updated;
                CalcIsPublished = isPublished;
                CalcIsSubmitted = isSubmitted;
            }
        }

        /// <summary>
        /// Represents a sample publisher. Used to display configuration colors.
        /// </summary>
        public class SamplePublisher
        {
            /// <summary>
            /// Gets the id of the sample publisher.
            /// </summary>
            public Int64 Id
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the name of the publisher.
            /// </summary>
            public string Name
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets the date this publisher was added.
            /// </summary>
            public DateTime Added
            {
                get;
                private set;
            }

            /// <summary>
            /// Get the last submission date to this publisher.
            /// </summary>
            public DateTime LastSub
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a value that indicates if the publisher is within their submission period.
            /// </summary>
            public bool CalcInPeriod
            {
                get;
                private set;
            }

            /// <summary>
            /// Gets a value that indicates if the publisher has been flagged as a goner.
            /// </summary>
            public bool goner
            {
                get;
                private set;
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="SamplePublisher"/> class.
            /// </summary>
            /// <param name="id">The sample id.</param>
            /// <param name="name">The name of the publisher.</param>
            /// <param name="added">The date added.</param>
            /// <param name="lastSub">The last submission date.</param>
            /// <param name="isInPeriod">A boolean value that indicates if the publisher is within their submission period.</param>
            /// <param name="isGoner">A boolean value that indicates if the publisher has been flagged as a goner.</param>
            public SamplePublisher(Int64 id, string name, DateTime added, DateTime lastSub, bool isInPeriod, bool isGoner)
            {
                Id = id;
                Name = name;
                Added = added;
                LastSub = lastSub;
                CalcInPeriod = isInPeriod;
                goner = isGoner;
            }
        }
        #endregion
    }
}