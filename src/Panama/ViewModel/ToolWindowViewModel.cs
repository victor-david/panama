using Restless.Panama.Core;
using Restless.Panama.Resources;
using Restless.Panama.Tools;
using Restless.Toolkit.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace Restless.Panama.ViewModel
{
    public class ToolWindowViewModel : ApplicationViewModel
    {
        #region Private
        private bool isOperationInProgress;
        private NavigatorSection selectedSection;
        private readonly VersionUpdater versionUpdater;
        private readonly SubmissionUpdater submissionUpdater;
        private readonly TitleExporter titleExporter;
        private readonly TitleLister titleLister;
        private readonly OrphanFinder orphanFinder;
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

        public ToolResultAdapter Adapter
        {
            get;
        }

        /// <summary>
        /// Gets the title list file name
        /// </summary>
        public string TitleListFileName
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
                new NavigatorSection(Strings.HeaderToolTitleMetadata, 1),
                new NavigatorSection(Strings.HeaderToolSubmissionMetadata, 2),
                new NavigatorSection(Strings.HeaderToolExport, 3),
                new NavigatorSection(Strings.HeaderToolTitleList, 4),
                new NavigatorSection(Strings.HeaderToolOrphan, 5),
            };

            SetInitialSection();

            Adapter = new ToolResultAdapter(5);

            Commands.Add("RunTitleMetadata", RunTitleMetadataCommand);
            Commands.Add("RunSubmissionMetadata", RunSubmissionMetadataCommand);

            Commands.Add("RunExport", RunExportCommand);
            Commands.Add("RunTitleList", RunTitleListCommand);
            Commands.Add("RunOrphan", RunOrphanCommand);


            versionUpdater = new VersionUpdater();
            submissionUpdater = new SubmissionUpdater();

            titleExporter = new TitleExporter()
            {
                OutputDirectory = Config.FolderExport
            };

            titleLister = new TitleLister()
            {
                OutputDirectory = Config.FolderTitleRoot
            };

            orphanFinder = new OrphanFinder();

            TitleListFileName = Path.Combine(Config.FolderTitleRoot, TitleLister.ListFile);
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

        private async void RunTitleMetadataCommand(object parm)
        {
            await RunTool(0, versionUpdater);
        }

        private async void RunSubmissionMetadataCommand(object parm)
        {
            await RunTool(1, submissionUpdater);
        }

        private async void RunExportCommand(object parm)
        {
            await RunTool(2, titleExporter);
        }

        private async void RunTitleListCommand(object parm)
        {
            await RunTool(3, titleLister);
        }

        private async void RunOrphanCommand(object parm)
        {
            await RunTool(4, orphanFinder);
        }

        private async Task RunTool(int index, Scanner scanner)
        {
            try
            {
                IsOperationInProgress = true;
                Adapter.Clear(index);
                HandleResult(await scanner.ExecuteAsync(), index);
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

        private void HandleResult(FileScanResult result, int index)
        {
            Adapter.AddToUpdate(index, result.Updated);
            Adapter.AddToNotFound(index, result.NotFound);
            Adapter.SetStatus(index, $"{result.ScanCount} items processed | {result.Updated.Count} updated | {result.NotFound.Count} not found");
        }
        #endregion
    }
}