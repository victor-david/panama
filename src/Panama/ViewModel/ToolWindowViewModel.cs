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

        public ToolResultData ToolResult
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
            };

            SetInitialSection();

            ToolResult = new ToolResultData(4);

            Commands.Add("RunTitleMetadata", RunTitleMetadataCommand);
            Commands.Add("RunSubmissionMetadata", RunSubmissionMetadataCommand);

            Commands.Add("RunExport", RunExportCommand);
            Commands.Add("RunTitleList", RunTitleListCommand);

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

        private async Task RunTool(int index, Scanner scanner)
        {
            try
            {
                IsOperationInProgress = true;
                ToolResult.Clear(index);
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
            ToolResult.AddToUpdate(index, result.Updated);
            ToolResult.AddToNotFound(index, result.NotFound);
            ToolResult.SetStatus(index, $"{result.ScanCount} items processed | {result.Updated.Count} updated | {result.NotFound.Count} not found");
        }
        #endregion
    }
}