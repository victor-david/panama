using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.Panama.Controls;
using Restless.Panama.Core;
using Restless.Panama.Resources;
using Restless.Panama.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Restless.Panama.ViewModel
{
    public class SettingsWindowViewModel : ApplicationViewModel
    {
        #region Private
        private NavigatorSection selectedSection;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the list of settings sections
        /// </summary>
        public List<NavigatorSection> Sections
        {
            get;
        }

        /// <summary>
        /// Gets or sets the selection section
        /// </summary>
        public NavigatorSection SelectedSection
        {
            get => selectedSection;
            set
            {
                SetProperty(ref selectedSection, value);
                Config.SelectedConfigSection = (int)selectedSection.Id;
            }
        }

        /// <summary>
        /// Gets or sets the database location
        /// </summary>
        public string DatabaseLocation
        {
            get => RegistryManager.DatabaseDirectory;
            set => RegistryManager.SetDatabaseDirectory(value);
        }

        /// <summary>
        /// Gets the list of sample titles used to preview color selections.
        /// </summary>
        public ObservableCollection<SampleTitle> SampleTitles
        {
            get;
        }

        /// <summary>
        /// Gets the list of sample publishers used to preview color selections.
        /// </summary>
        public ObservableCollection<SamplePublisher> SamplePublishers
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindowViewModel"/> class
        /// </summary>
        public SettingsWindowViewModel()
        {
            Sections = new List<NavigatorSection>()
            {
                new NavigatorSection(Strings.HeaderSettingsDisplay, 1),
                new NavigatorSection(Strings.HeaderSettingsFolder, 2),
                new NavigatorSection(Strings.HeaderSettingsColor, 3),
                new NavigatorSection(Strings.HeaderSettingsSubmission, 4),
                new NavigatorSection(Strings.HeaderSettingsAdvanced, 5),
            };

            SetInitialSection();

            Commands.Add("SelectPath", RunSelectPathCommand);
            Commands.Add("ResetColors", p => Config.Colors.Reset());

            SampleTitles = new ObservableCollection<SampleTitle>()
            {
                new SampleTitle(1, "Title #1", -10, false, false, false),
                new SampleTitle(2, "Title #2 (published)", -20, true, false, false),
                new SampleTitle(3, "Title #3", -30, false, false, false),
                new SampleTitle(4, "Title #4 (submitted)", -40, false, false, true),
                new SampleTitle(5, "Title #5", -43, false, false, false),
                new SampleTitle(6, "Title #6 (self published)", -48, false, true, false),
            };

            SamplePublishers = new ObservableCollection<SamplePublisher>()
            {
                new SamplePublisher(1, "Publisher #1", -60, -10, false, false),
                new SamplePublisher(2, "Publisher #2 (in period)", -90, -80, true, false),
                new SamplePublisher(3, "Publisher #3", -110,  -90, false, false),
                new SamplePublisher(4, "Publisher #4 (goner)", -150, -148, false, true),
            };
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void SetInitialSection()
        {
            /* set the backing field to avoid changing config */
            selectedSection = Sections[0];
            Sections.ForEach(s =>
            {
                if (s.Id == Config.SelectedConfigSection)
                {
                    selectedSection = s;
                }
            });
        }

        private void RunSelectPathCommand(object parm)
        {
            if (parm is PathSelector selector)
            {
                SelectFileSystem(selector);
            }
        }

        private void SelectFileSystem(PathSelector selector)
        {
            string title = (selector.SelectorType == PathSelectorType.Folder) ? "Select a directory" : "Select a file";

            string initialDir = selector.Path;

            if (selector.SelectorType == PathSelectorType.File)
            {
                initialDir = System.IO.Path.GetDirectoryName(selector.Path);
            }

            using (CommonOpenFileDialog dialog = CommonDialogFactory.Create(initialDir, title, selector.SelectorType == PathSelectorType.Folder, selector.SelectorFileType))
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    selector.Path = dialog.FileName;
                }
            }
        }
        #endregion
    }
}