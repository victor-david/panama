using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Panama.Tools;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Restless.Panama.ViewModel
{
    public class ToolWindowViewModel : WindowViewModel
    {
        #region Private
        private bool isOperationInProgress;
        private NavigatorSection selectedSection;
        private readonly VersionUpdater versionUpdater;
        private readonly SubmissionUpdater submissionUpdater;
        private readonly TitleExporter titleExporter;
        private readonly TitleLister titleLister;
        private readonly MessageSync messageSync;
        private readonly OrphanFinder orphanFinder;
        private FileScanItem selectedOrphan;
        private PreviewMode orphanPreviewMode;
        private string orphanPreviewText;
        private ImageSource orphanImageSource;
        #endregion

        /************************************************************************/

        #region Properties
        private TitleTable TitleTable => DatabaseController.Instance.GetTable<TitleTable>();
        private TitleVersionTable TitleVersionTable => DatabaseController.Instance.GetTable<TitleVersionTable>();
        private OrphanExclusionTable OrphanExclusionTable => DatabaseController.Instance.GetTable<OrphanExclusionTable>();

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

        /// <summary>
        /// Gets the orphan context menu
        /// </summary>
        public ContextMenu OrphanContextMenu
        {
            get;
        }

        public FileScanItem SelectedOrphan
        {
            get => selectedOrphan;
            set
            {
                SetProperty(ref selectedOrphan, value);
                PreviewOrphan();
            }
        }

        public PreviewMode OrphanPreviewMode
        {
            get => orphanPreviewMode;
            private set => SetProperty(ref orphanPreviewMode, value);
        }

        public string OrphanPreviewText
        {
            get => orphanPreviewText;
            private set => SetProperty(ref orphanPreviewText, value);
        }

        public ImageSource OrphanImageSource
        {
            get => orphanImageSource;
            private set => SetProperty(ref orphanImageSource, value);
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
                new NavigatorSection(Strings.HeaderToolMessage, 5),
                new NavigatorSection(Strings.HeaderToolOrphan, 6),
            };

            SetInitialSection();

            Adapter = new ToolResultAdapter(6);

            Commands.Add("RunTitleMetadata", RunTitleMetadataCommand);
            Commands.Add("RunSubmissionMetadata", RunSubmissionMetadataCommand);

            Commands.Add("RunExport", RunExportCommand);
            Commands.Add("RunTitleList", RunTitleListCommand);
            Commands.Add("RunMessageSync", RunMessageSyncCommand);
            Commands.Add("RunOrphan", RunOrphanCommand);

            Commands.Add("ResetWindow", RunResetWindowCommand);

            OrphanContextMenu = new ContextMenu();

            OrphanContextMenu.Items.Add(CreateMenuItem(
                Strings.MenuItemExcludeOrphanFile,
                RelayCommand.Create(RunSetOrphanFileExclusion, CanRunOrphanCommand))
                .AddIconResource(ResourceKeys.Icon.SquareSmallRedIconKey));

            OrphanContextMenu.Items.Add(CreateMenuItem(
                Strings.MenuItemExcludeOrphanFileType,
                RelayCommand.Create(RunSetOrphanFileTypeExclusion, CanRunOrphanCommand))
                .AddIconResource(ResourceKeys.Icon.SquareSmallRedIconKey));

            OrphanContextMenu.Items.Add(CreateMenuItem(
                Strings.MenuItemExcludeOrphanDirectory,
                RelayCommand.Create(RunSetOrphanDirectoryExclusion, CanRunOrphanCommand))
                .AddIconResource(ResourceKeys.Icon.SquareSmallRedIconKey));

            OrphanContextMenu.Items.Add(new Separator());
            
            OrphanContextMenu.Items.Add(CreateMenuItem(
                Strings.MenuItemCreateTitleFromEntry,
                RelayCommand.Create(RunCreateTitleFromOrphan, CanRunOrphanCommand))
                .AddIconResource(ResourceKeys.Icon.PlusIconKey));

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

        private async void RunMessageSyncCommand(object parm)
        {
            await RunTool(4, messageSync);
        }

        private async void RunOrphanCommand(object parm)
        {
            await RunTool(5, orphanFinder);
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

        private void PreviewOrphan()
        {
            if (SelectedOrphan != null)
            {
                OrphanPreviewMode = DocumentPreviewer.GetPreviewMode(SelectedOrphan.FullName);
                switch (OrphanPreviewMode)
                {
                    case PreviewMode.Text:
                        OrphanPreviewText = DocumentPreviewer.GetText(SelectedOrphan.FullName);
                        break;
                    case PreviewMode.Image:
                        OrphanImageSource = DocumentPreviewer.GetImage(SelectedOrphan.FullName);
                        break;
                    case PreviewMode.None:
                    case PreviewMode.Unsupported:
                        break;
                }
            }
        }

        private void RunResetWindowCommand(object parm)
        {
            WindowOwner.Width = Config.ToolWindow.DefaultWidth;
            WindowOwner.Height = Config.ToolWindow.DefaultHeight;
            WindowOwner.Top = (SystemParameters.WorkArea.Height / 2) - (WindowOwner.Height / 2);
            WindowOwner.Left = (SystemParameters.WorkArea.Width / 2) - (WindowOwner.Width / 2);
            WindowOwner.WindowState = WindowState.Normal;
        }

        private void RunSetOrphanFileExclusion(object parm)
        {
            if (MessageWindow.ShowContinueCancel(GetOrphanDetailMessage(Strings.ConfirmationAddOrphanFileExclusion, SelectedOrphan.FileName)))
            {
                OrphanExclusionTable.AddFileExclusion(Paths.Title.WithoutRoot(SelectedOrphan.FullName));
            }
        }

        private void RunSetOrphanFileTypeExclusion(object parm)
        {
            if (MessageWindow.ShowContinueCancel(GetOrphanDetailMessage(Strings.ConfirmationAddOrphanFileTypeExclusion, SelectedOrphan.FileExtension)))
            {
                OrphanExclusionTable.AddFileExtensionExclusion(SelectedOrphan.FileExtension);
            }
        }

        private void RunSetOrphanDirectoryExclusion(object parm)
        {
            if (MessageWindow.ShowContinueCancel(GetOrphanDetailMessage(Strings.ConfirmationAddOrphanDirectoryExclusion, SelectedOrphan.DirectoryName)))
            {
                OrphanExclusionTable.AddDirectoryExclusion(Path.GetDirectoryName(Paths.Title.WithoutRoot(SelectedOrphan.FullName)));
            }
        }

        private void RunCreateTitleFromOrphan(object parm)
        {
            if (MessageWindow.ShowContinueCancel(GetOrphanDetailMessage(Strings.ConfirmationCreateTitleFromOrphan, SelectedOrphan.FullName)))
            {
                TitleRow row = new(TitleTable.AddDefaultRow())
                {
                    Title = $"{Strings.TextOrphan} {SelectedOrphan.FullName}",
                    Written = SelectedOrphan.LastWriteTimeUtc,
                    Notes = $"{Strings.TextCreatedFromOrphan} {SelectedOrphan.FullName}"
                };

                TitleVersionTable.GetVersionController(row.Id).Add(Paths.Title.WithoutRoot(SelectedOrphan.FullName));

                TitleVersionTable.Save();
                TitleTable.Save();
                Adapter.Updated[4].Remove(SelectedOrphan);
                SelectedOrphan = null;
                MainWindowViewModel.Instance.NotifyUpdate<TitleViewModel>();
            }
        }

        private bool CanRunOrphanCommand(object parm)
        {
            return SelectedOrphan != null;
        }

        private string GetOrphanDetailMessage(string message, string detail)
        {
            return $"{message}{Environment.NewLine}{Environment.NewLine}{detail}";
        }

        private MenuItem CreateMenuItem(string header, ICommand command)
        {
            MenuItem item = new()
            {
                Header = header,
                Command = command,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            return item;
        }
        #endregion
    }
}