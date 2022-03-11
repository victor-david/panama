using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.Panama.Controls;
using Restless.Panama.Core;
using Restless.Panama.Resources;
using System.Collections.Generic;

namespace Restless.Panama.ViewModel
{
    public class SettingsWindowViewModel : ApplicationViewModel
    {
        #region Private
        private Section selectedSection;
        #endregion

        /************************************************************************/

        #region Helper class
        public class Section
        {
            public string Name { get; }
            public long Id { get; }
            public Section(string name, long id)
            {
                Name = name;
                Id = id;
            }
        }
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the list of settings sections
        /// </summary>
        public List<Section> Sections
        {
            get;
        }

        /// <summary>
        /// Gets or sets the selection section
        /// </summary>
        public Section SelectedSection
        {
            get => selectedSection;
            set
            {
                SetProperty(ref selectedSection, value);
                Config.SelectedConfigSection = (int)selectedSection.Id;
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsWindowViewModel"/> class
        /// </summary>
        public SettingsWindowViewModel()
        {
            Sections = new List<Section>()
            {
                new Section(Strings.HeaderSettingsDisplay, 1),
                new Section(Strings.HeaderSettingsFolder, 2),
                new Section(Strings.HeaderSettingsColor, 3),
                new Section(Strings.HeaderSettingsSubmission, 4),
                new Section(Strings.HeaderSettingsAdvanced, 5),
            };

            SetInitialSection();

            Commands.Add("SelectPath", RunSelectPathCommand);
            Commands.Add("ResetColors", p => Config.Colors.Reset());
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