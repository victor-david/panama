using Restless.Panama.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Panama.Tools;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Restless.Panama.ViewModel
{
    public class ToolOrphanViewModel : DataViewModel<FileScanItem>
    {
        #region Private
        private PreviewMode previewMode;
        private string previewText;
        private ImageSource previewImageSource;
        private readonly ObservableCollection<FileScanItem> orphans;
        #endregion

        /************************************************************************/

        #region Properties
        public OrphanExclusionController Exclusions { get; }
        public FileScanItem SelectedOrphan => SelectedItem as FileScanItem;
        public ICommand StartScanCommand { get; }

        public PreviewMode PreviewMode
        {
            get => previewMode;
            private set => SetProperty(ref previewMode, value);
        }

        public string PreviewText
        {
            get => previewText;
            private set => SetProperty(ref previewText, value);
        }

        public ImageSource PreviewImageSource
        {
            get => previewImageSource;
            private set => SetProperty(ref previewImageSource, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolOrphanViewModel"/> class
        /// </summary>
        public ToolOrphanViewModel()
        {
            Exclusions = new OrphanExclusionController();

            Columns.Create(Header.File, nameof(FileScanItem.FullName)).MakeInitialSortAscending();

            MenuItems.AddItem(
                Strings.MenuItemExcludeOrphanFile,
                RelayCommand.Create(p => RunSetOrphanFileExclusion(), p => CanRunOrphanCommand()))
                .AddIconResource(ResourceKeys.Icon.IconFile);

            MenuItems.AddItem(
                Strings.MenuItemExcludeOrphanFileType,
                RelayCommand.Create(p => RunSetOrphanFileTypeExclusion(), p => CanRunOrphanCommand()))
                .AddIconResource(ResourceKeys.Icon.IconFileExtension);

            MenuItems.AddItem(
                Strings.MenuItemExcludeOrphanDirectory,
                RelayCommand.Create(p => RunSetOrphanDirectoryExclusion(), p => CanRunOrphanCommand()))
                .AddIconResource(ResourceKeys.Icon.IconFolder);

            MenuItems.AddSeparator();

            MenuItems.AddItem(
                Strings.MenuItemCreateTitleFromEntry,
                RelayCommand.Create(p => RunCreateTitleFromOrphan(), p => CanRunOrphanCommand()))
                .AddIconResource(ResourceKeys.Icon.IconAdd);

            StartScanCommand = RelayCommand.Create(p => RunScanCommand());

            orphans = new ObservableCollection<FileScanItem>();
            InitListView(orphans);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            OnPropertyChanged(nameof(SelectedOrphan));
            PrepareDocumentPreview();
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(FileScanItem item1, FileScanItem item2)
        {
            return string.Compare(item1.FullName, item2.FullName);
        }
        #endregion

        /************************************************************************/

        #region Private methods (scan)
        private async void RunScanCommand()
        {
            try
            {
                IsOperationInProgress = true;
                orphans.Clear();
                List<FileScanItem> results = await PerformOrphanScan();
                results.ForEach(orphans.Add);
            }
            finally
            {
                IsOperationInProgress = false;
            }
        }

        private async Task<List<FileScanItem>> PerformOrphanScan()
        {
            List<string> files = new();
            List<FileScanItem> result = new();
            await Task.Delay(10);

            foreach (string dir in Directory.EnumerateDirectories(Config.FolderTitleRoot, "*", SearchOption.AllDirectories))
            {
                if (!Exclusions.IsDirectoryExcluded(dir))
                {
                    files.AddRange(Directory.EnumerateFiles(dir, "*", SearchOption.TopDirectoryOnly));
                }
            }

            foreach (string file in files)
            {
                if (!Exclusions.IsFileExtensionExcluded(Path.GetExtension(file)) && !Exclusions.IsFileExcluded(Path.GetFileName(file)))
                {
                    if (!TitleVersionTable.VersionWithFileExists(Paths.Title.WithoutRoot(file)))
                    {
                        result.Add(FileScanItem.Create(file));
                    }
                }
            }
            return result;
        }
        #endregion

        /************************************************************************/

        #region Private methods (handlers)
        private void RunSetOrphanFileExclusion()
        {
            if (MessageWindow.ShowContinueCancel(GetOrphanDetailMessage(Strings.ConfirmationAddOrphanFileExclusion, SelectedOrphan.FileName)))
            {
                Exclusions.AddFileExclusion(SelectedOrphan);
            }
        }

        private void RunSetOrphanFileTypeExclusion()
        {
            if (MessageWindow.ShowContinueCancel(GetOrphanDetailMessage(Strings.ConfirmationAddOrphanFileTypeExclusion, SelectedOrphan.FileExtension)))
            {
                Exclusions.AddFileExtensionExclusion(SelectedOrphan);
            }
        }

        private void RunSetOrphanDirectoryExclusion()
        {
            if (MessageWindow.ShowContinueCancel(GetOrphanDetailMessage(Strings.ConfirmationAddOrphanDirectoryExclusion, SelectedOrphan.DirectoryName)))
            {
                Exclusions.AddDirectoryExclusion(SelectedOrphan);
            }
        }

        private void RunCreateTitleFromOrphan()
        {
            if (MessageWindow.ShowContinueCancel(GetOrphanDetailMessage(Strings.ConfirmationCreateTitleFromOrphan, SelectedOrphan.FullName)))
            {
                TitleRow row = new(TitleTable.AddDefaultRow())
                {
                    Title = $"{Strings.TextOrphan} {SelectedOrphan.FullName}",
                    Written = SelectedOrphan.LastWriteTimeUtc.ToUtcZero(),
                    Notes = $"{Strings.TextCreatedFromOrphan} {SelectedOrphan.FullName}, {SelectedOrphan.LastWriteTimeUtc}"
                };

                TitleVersionTable.GetVersionController(row.Id).Add(Paths.Title.WithoutRoot(SelectedOrphan.FullName));

                TitleVersionTable.Save();
                TitleTable.Save();
                orphans.Remove(SelectedOrphan);
                SelectedItem = null;
                MainWindowViewModel.Instance.NotifyUpdate<TitleViewModel>();
            }
        }

        private bool CanRunOrphanCommand()
        {
            return SelectedOrphan != null;
        }

        private string GetOrphanDetailMessage(string message, string detail)
        {
            return $"{message}{Environment.NewLine}{Environment.NewLine}{detail}";
        }
        #endregion

        /************************************************************************/

        #region Private methods (preview)
        private void PrepareDocumentPreview()
        {
            PreviewText = null;
            PreviewMode = PreviewMode.Unsupported;

            if (SelectedOrphan != null)
            {
                PreviewMode = DocumentPreviewer.GetPreviewMode(SelectedOrphan.FullName);
                switch (PreviewMode)
                {
                    case PreviewMode.Text:
                        PreviewText = DocumentPreviewer.GetText(SelectedOrphan.FullName);
                        break;
                    case PreviewMode.Image:
                        PreviewImageSource = DocumentPreviewer.GetImage(SelectedOrphan.FullName);
                        break;
                    case PreviewMode.None:
                    case PreviewMode.Unsupported:
                        break;
                }
            }
        }
        #endregion
    }
}