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
        private NavigatorSection selectedSection;
        private LanguageItem selectedLanguage;
        private readonly StartupConfig startupConfig;
        //private string databaseLocation;
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

        public List<NavigatorHeader> NavigatorHeaders { get; }

        /// <summary>
        /// Gets or sets the database location
        /// </summary>
        public string DatabaseLocation
        {
            get => startupConfig.DatabaseLocation;
            set
            {
                startupConfig.DatabaseLocation = value;
                startupConfig.Save();
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates if a language switch is pending
        /// </summary>
        public bool IsLanguageChangePending => Config.LanguageId != LanguageManager.Instance.CurrentLanguageId;

        public LanguageItem SelectedLanguage
        {
            get => selectedLanguage;
            set
            {
                if (SetProperty(ref selectedLanguage, value) && selectedLanguage != null)
                {
                    Config.LanguageId = SelectedLanguage.Id;
                    OnPropertyChanged(nameof(IsLanguageChangePending));
                    /**
                     * The following can't be used unless live switching is implemented.
                     * Without live, setting the language causes view models that haven't
                     * yet been loaded to use the switched language, resulting in mixed.
                     */
                    //LanguageManager.Instance.SetLanguage(Config.LanguageId);
                    //MainWindowViewModel.Instance.SignalLanguageChange();
                    //SignalLanguageChange();
                }
            }
        }

        public LanguageItemCollection Languages => LanguageManager.Instance.Languages;

        public SettingsThemeController Themes { get; }
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
                new NavigatorSection(Settings.Display, 1),
                new NavigatorSection(Settings.Folder, 2),
                new NavigatorSection(Settings.Theme, 3),
                new NavigatorSection(Settings.Language, 4),
                new NavigatorSection(Settings.Color, 5),
                new NavigatorSection(Settings.Submission, 6),
                new NavigatorSection(Settings.Advanced, 7),
            };

            NavigatorHeaders = new List<NavigatorHeader>()
            {
                NavigatorHeader.None,
                NavigatorHeader.Simple,
                NavigatorHeader.Titled
            };

            // set the backing store to avoid triggering the setter
            selectedLanguage = LanguageManager.Instance.Languages.GetLanguageItem(Config.LanguageId);

            Themes = new SettingsThemeController();

            SetInitialSection();

            startupConfig = StartupConfig.GetStartupConfig();

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