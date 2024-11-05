using Restless.Panama.Core;
using Restless.Panama.Resources;
using Restless.Panama.Tools;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    public class ToolWindowViewModel : WindowViewModel
    {
        #region Private
        private NavigatorSection selectedSection;
        private readonly VersionUpdater versionUpdater;
        private readonly SubmissionUpdater submissionUpdater;
        private readonly TitleExporter titleExporter;
        private readonly TitleLister titleLister;
        private readonly MessageSync messageSync;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the list of settings sections
        /// </summary>
        public List<NavigatorSection> Sections { get; }

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

        public ToolResultAdapter Adapter { get; }

        /// <summary>
        /// Gets the title list file name
        /// </summary>
        public string TitleListFileName
        {
            get;
        }

        public ICommand TitleMetaCommand { get; }
        public ICommand SubmissionMetaCommand { get; }
        public ICommand TitleExportCommand { get; }
        public ICommand TitleListCommand { get; }
        public ICommand MessageSyncCommand { get; }
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
                new NavigatorSection(Strings.HeaderToolMessage, 5),
            };

            SetInitialSection();

            Adapter = new ToolResultAdapter(6);

            TitleMetaCommand = RelayCommand.Create(p => RunTitleMetaCommand());
            SubmissionMetaCommand = RelayCommand.Create(p => RunSubmissionMetaCommand());
            TitleExportCommand = RelayCommand.Create(p => RunTitleExportCommand());
            TitleListCommand = RelayCommand.Create(p => RunTitleListCommand());
            MessageSyncCommand = RelayCommand.Create(p => RunMessageSyncCommand());

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

            messageSync = new MessageSync();

            TitleListFileName = Path.Combine(Config.FolderTitleRoot, TitleLister.ListFile);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override void RunResetWindowCommand()
        {
            WindowOwner.Width = Config.ToolWindow.DefaultWidth;
            WindowOwner.Height = Config.ToolWindow.DefaultHeight;
            WindowOwner.Top = (SystemParameters.WorkArea.Height / 2) - (WindowOwner.Height / 2);
            WindowOwner.Left = (SystemParameters.WorkArea.Width / 2) - (WindowOwner.Width / 2);
            WindowOwner.WindowState = WindowState.Normal;
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

        private async void RunTitleMetaCommand()
        {
            await RunTool(0, versionUpdater);
        }

        private async void RunSubmissionMetaCommand()
        {
            await RunTool(1, submissionUpdater);
        }

        private async void RunTitleExportCommand()
        {
            await RunTool(2, titleExporter);
        }

        private async void RunTitleListCommand()
        {
            await RunTool(3, titleLister);
        }

        private async void RunMessageSyncCommand()
        {
            await RunTool(4, messageSync);
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
            Adapter.SetOutputText(index, result.OutputText.ToString());
            Adapter.SetStatus(index, $"{result.ScanCount} items processed | {result.Updated.Count} updated | {result.NotFound.Count} not found");
        }
        #endregion
    }
}