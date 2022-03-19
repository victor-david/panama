using Restless.Panama.Core;
using Restless.Panama.Resources;
using Restless.Panama.Tools;
using Restless.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace Restless.Panama.ViewModel
{
    public class ToolWindowViewModel : ApplicationViewModel
    {
        #region Private
        private bool isOperationInProgress;
        private NavigatorSection selectedSection;
        private readonly VersionUpdater versionUpdater;
        private readonly TitleExporter titleExporter;
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
                Config.SelectedToolSection = (int)selectedSection.Id;
            }
        }

        public bool IsOperationInProgress
        {
            get => isOperationInProgress;
            private set => SetProperty(ref isOperationInProgress, value);
        }

        public ObservableCollection<FileScanItem> Updated
        {
            get;
        }

        public ObservableCollection<FileScanItem> NotFound
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolWindowViewModel"/> class
        /// </summary>
        public ToolWindowViewModel()
        {
            Sections = new List<NavigatorSection>()
            {
                new NavigatorSection(Strings.HeaderToolMetadata, 1),
                new NavigatorSection(Strings.HeaderToolExport, 2),
                new NavigatorSection(Strings.HeaderToolTitleList, 3),
            };

            SetInitialSection();

            Updated = new ObservableCollection<FileScanItem>();
            NotFound = new ObservableCollection<FileScanItem>();

            Commands.Add("RunMetadata", RunMetadataCommand);
            Commands.Add("RunExport", RunExportCommand);
            Commands.Add("RunTitleList", RunTitleListCommand);

            versionUpdater = new VersionUpdater();

            titleExporter = new TitleExporter()
            {
                ExportDirectory = Config.FolderExport
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
                if (s.Id == Config.SelectedToolSection)
                {
                    selectedSection = s;
                }
            });
        }

        private async void RunMetadataCommand(object parm)
        {
            await RunTool(versionUpdater);
        }

        private async void RunExportCommand(object parm)
        {
            await RunTool(titleExporter);
        }

        private async void RunTitleListCommand(object parm)
        {
            IsOperationInProgress = true;
            await Task.Delay(1000);
            IsOperationInProgress = false;
        }

        private async Task RunTool(Scanner scanner)
        {
            try
            {
                IsOperationInProgress = true;
                Updated.Clear();
                NotFound.Clear();
                HandleResult(await scanner.ExecuteAsync());
            }
            catch (Exception ex)
            {
                MessageWindow.ShowError(ex.Message);
            }
            finally
            {
                IsOperationInProgress = false;
            }
        }

        private void HandleResult(FileScanResult result)
        {
            result.Updated.ForEach(item => Updated.Add(item));
            result.NotFound.ForEach(item => NotFound.Add(item));
        }
        #endregion
    }
}