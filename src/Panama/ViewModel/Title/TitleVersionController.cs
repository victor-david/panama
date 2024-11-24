using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.OpenXml;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Mvvm;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Menu = Restless.Panama.Resources.Menu;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that manages title versions.
    /// </summary>
    public class TitleVersionController : BaseController<TitleViewModel, TitleVersionTable>
    {
        #region Private
        private bool isOpenXml;
        private PropertiesAdapter properties;
        private Database.Tables.TitleVersionController verController;
        private TitleVersionRow selectedVersion;
        private const string InvalidVersionController = "Internal error. Invalid version or controller";
        #endregion

        /************************************************************************/

        #region Properties
        private DocumentTypeTable DocumentTypeTable => DatabaseController.Instance.GetTable<DocumentTypeTable>();
        private long CurrentTitleId => Owner?.SelectedTitle?.Id ?? 0;

        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => CanRunVersionCommand();

        /// <summary>
        /// Gets the currently selection version
        /// </summary>
        public TitleVersionRow SelectedVersion
        {
            get => selectedVersion;
            private set => SetProperty(ref selectedVersion, value);
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the latest version of the currently selected title is an Open XML document.
        /// </summary>
        public bool IsOpenXml
        {
            get => isOpenXml;
            private set => SetProperty(ref isOpenXml, value);
        }

        /// <summary>
        /// Gets the package property object that contains the properties of the latest document version.
        /// </summary>
        public PropertiesAdapter Properties
        {
            get => properties;
            private set => SetProperty(ref properties, value);
        }

        /// <summary>
        /// Gets the version name for this file with the title root portion
        /// </summary>
        public string VersionFileName => SelectedVersion != null ? Paths.Title.WithRoot(SelectedVersion.FileName) : null;

        /// <summary>
        /// Gets a boolean that indicates if there is at least one version
        /// </summary>
        public bool HaveVersion => ListView.Count > 0;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleVersionController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleVersionController(TitleViewModel owner) : base(owner)
        {
            Columns.Create(Header.VersionShort, TitleVersionTable.Defs.Columns.Version)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create<IntegerToCharConverter>(Header.RevisionShort, TitleVersionTable.Defs.Columns.Revision)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create(Header.Updated, TitleVersionTable.Defs.Columns.Updated)
                .MakeDate();

            Columns.Create(Header.WordCountShort, TitleVersionTable.Defs.Columns.WordCount)
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create(Header.LanguageShort, TitleVersionTable.Defs.Columns.LangId)
                .MakeFixedWidth(FixedWidth.W048);

            Columns.Create(Header.File, TitleVersionTable.Defs.Columns.FileName);

            Commands.Add("VersionMoveUp", p => RunMoveUpCommand(), p => CanRunMoveUpCommand());
            Commands.Add("VersionMoveDown", p => RunMoveDownCommand(), p => CanRunMoveDownCommand());
            Commands.Add("VersionSync", p => RunSyncCommand());
            Commands.Add("ContextMenuOpening", p => RunContextMenuOpeningCommand());
            Commands.Add("SaveProperty", p => RunSavePropertyCommand(), p => CanRunSavePropertyCommand());
            Commands.Add("SetLanguage", RunSetLanguageCommand, p => IsSelectedRowAccessible);

            MenuItems.AddItem(Menu.AddTitleVersion, AddCommand)
                .AddIconResource(ResourceKeys.Icon.IconAdd);

            MenuItems.AddItem(Menu.ReplaceTitleVersion,
                RelayCommand.Create(p => RunReplaceVersionCommand(), p => CanRunVersionCommand()))
                .AddIconResource(ResourceKeys.Icon.IconFileReplace);

            MenuItems.AddItem(Menu.CreateTitleVersionCopy,
                RelayCommand.Create(p => RunCreateTitleVersionCopyCommand(), p=> CanRunCreateTitleVersionCopyCommand()))
                .AddIconResource(ResourceKeys.Icon.IconCopy);

            MenuItems.AddSeparator();

            MenuItems.AddItem(Menu.MakeSeparateVersion, RelayCommand.Create(p => RunConvertToVersionCommand(), p => CanRunConvertToVersionCommand()));
            MenuItems.AddSeparator();

            foreach (DataRow row in DatabaseController.Instance.GetTable<LanguageTable>().Rows)
            {
                string langId = row[LanguageTable.Defs.Columns.Id].ToString();
                string langName = row[LanguageTable.Defs.Columns.Name].ToString();

                MenuItems.AddItem($"Set language to {langName} ({langId})", Commands["SetLanguage"]).AddCommandParm(langId).AddTag(langId);
            }

            MenuItems.AddSeparator();
            MenuItems.AddItem(Menu.RemoveTitleVersion, DeleteCommand).AddIconResource(ResourceKeys.Icon.IconDelete);

            ListView.IsLiveSorting = true;
            ListView.LiveSortingProperties.Add(TitleVersionTable.Defs.Columns.Version);
            ListView.LiveSortingProperties.Add(TitleVersionTable.Defs.Columns.Revision);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the selected item on the associated data grid has changed.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedVersion = IsSelectedRowAccessible ? new TitleVersionRow(SelectedRow) : null;
            PrepareForOpenXml();
            OnPropertyChanged(nameof(VersionFileName));
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return (long)item[TitleVersionTable.Defs.Columns.TitleId] == CurrentTitleId;
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            int value = DataRowCompareLong(item2, item1, TitleVersionTable.Defs.Columns.Version);
            if (value == 0)
            {
                value = DataRowCompareLong(item1, item2, TitleVersionTable.Defs.Columns.Revision);
            }
            return value;
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            SelectedVersion = null;
            verController = TitleVersionTable.GetVersionController(CurrentTitleId);
            ListView.Refresh();
            OnPropertyChanged(nameof(HaveVersion));
        }

        /// <inheritdoc/>
        protected override void RunAddCommand()
        {
            try
            {
                ValidateObject(verController, InvalidVersionController);

                using (CommonOpenFileDialog dialog = CommonDialogFactory.Create(Config.Instance.FolderTitleVersion, Header.SelectTitleVersionAddByFile))
                {
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        string fileName = Paths.Title.WithoutRoot(dialog.FileName);
                        // throws if file name already belongs to title
                        ValidateAddFileToTitle(fileName);

                        verController.Add(fileName);
                        OnUpdate();
                    }
                }

            }
            catch (Exception e)
            {
                MessageWindow.ShowError(e.Message);
            }
        }

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (CanRunVersionCommand() && MessageWindow.ShowContinueCancel(Confirm.RemoveTitleVersion))
            {
                verController.Remove(SelectedVersion);
                Table.Save();
                OnUpdate();
            }
        }

        /// <inheritdoc/>
        protected override void RunOpenRowCommand()
        {
            if (SelectedVersion != null)
            {
                Open.TitleVersionFile(SelectedVersion.FileName);
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunConvertToVersionCommand()
        {
            if (CanRunVersionCommand())
            {
                verController.ConvertToVersion(SelectedVersion);
            }
        }

        private bool CanRunConvertToVersionCommand()
        {
            return
                CanRunVersionCommand() &&
                verController.GetRevisionCount(SelectedVersion.Version) > 1;
        }

        private void RunReplaceVersionCommand()
        {
            try
            {
                ValidateObject(SelectedVersion, InvalidVersionController);

                using (CommonOpenFileDialog dialog = CommonDialogFactory.Create(Config.Instance.FolderTitleVersion, Header.SelectTitleVersionReplaceByFile))
                {
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        string fileName = Paths.Title.WithoutRoot(dialog.FileName);
                        // throws if file name already belongs to title
                        ValidateAddFileToTitle(fileName);

                        SelectedVersion.FileName = fileName;
                        OnPropertyChanged(nameof(VersionFileName));
                    }
                }
            }
            catch (Exception e)
            {
                MessageWindow.ShowError(e.Message);
            }
        }

        private void RunCreateTitleVersionCopyCommand()
        {
            try
            {
                ValidateObject(verController, InvalidVersionController);

                if (MessageWindow.ShowContinueCancel(Confirm.AddTitleVersionCopy))
                {
                    TitleVersionRow latest = verController.GetLatest();
                    string sourceFile = Paths.Title.WithRoot(latest.FileName);
                    string destFile = Path.Combine(Path.GetDirectoryName(sourceFile), Guid.NewGuid().ToString() + Path.GetExtension(sourceFile));
                    string versionFile = Paths.Title.WithoutRoot(destFile);

                    // throws if file name already belongs to title
                    // this should never throw because we created a unique file name
                    ValidateAddFileToTitle(versionFile);

                    // throws if the file doesn't exist or is in use
                    FileOperation.ValidateFile(sourceFile);

                    File.Copy(sourceFile, destFile);
                    TitleVersionRow newLatest = verController.Add(versionFile).GetLatest();
                    TitleVersionRenameItem rename = new(newLatest, Owner.SelectedTitle.Title);
                    rename.Rename();
                    OnUpdate();
                }
            }
            catch (Exception e)
            {
                MessageWindow.ShowError(e.Message);
            }
        }

        private bool CanRunCreateTitleVersionCopyCommand()
        {
            return ((verController?.VersionCount) ?? -1) > 0;
        }

        /// <summary>
        /// Validates that <paramref name="fileName"/> may be added to the title.
        /// </summary>
        /// <param name="fileName">The file name to check.</param>
        /// <exception cref="InvalidOperationException">The file name already belongs to the title</exception>
        private void ValidateAddFileToTitle(string fileName)
        {
            foreach (TitleVersionRow ver in Table.EnumerateVersions(fileName))
            {
                if (ver.TitleId == CurrentTitleId)
                {
                    StringBuilder sb = new();
                    sb.AppendLine(fileName);
                    sb.AppendLine();
                    sb.Append(Error.CannotAddVersionFile);
                    throw new InvalidOperationException(sb.ToString());
                }
            }
        }

        private static void ValidateObject(object obj, string message)
        {
            if (obj is null)
            {
                throw new InvalidOperationException(message);
            }
        }

        private void RunMoveUpCommand()
        {
            if (CanRunVersionCommand())
            {
                verController.MoveUp(SelectedVersion);
            }
        }

        private void RunMoveDownCommand()
        {
            if (CanRunVersionCommand())
            {
                verController.MoveDown(SelectedVersion);
            }
        }

        private bool CanRunMoveUpCommand()
        {
            return CanRunVersionCommand() && !verController.IsLatest(SelectedVersion);
        }

        private bool CanRunMoveDownCommand()
        {
            return CanRunVersionCommand() && !verController.IsEarliest(SelectedVersion);
        }

        /// <summary>
        /// Checks basic requirements for running a version command.
        /// </summary>
        private bool CanRunVersionCommand()
        {
            return (SelectedVersion?.TitleId ?? -2) == (verController?.TitleId ?? -1);
        }

        private void RunSyncCommand()
        {
            if (CurrentTitleId > 0)
            {
                WindowFactory.TitleVersionRename.Create(CurrentTitleId).ShowDialog();
            }
        }

        private void RunContextMenuOpeningCommand()
        {
            if (SelectedVersion != null)
            {
                string langId = SelectedVersion.LanguageId;
                foreach (MenuItem item in MenuItems.OfType<MenuItem>())
                {
                    if (item.Tag is string menuLangId)
                    {
                        item.Icon = null;
                        if (langId.Equals(menuLangId, StringComparison.Ordinal))
                        {
                            item.Icon = LocalResources.Get<System.Windows.Shapes.Path>(ResourceKeys.Icon.IconCheck);
                        }
                    }
                }
            }
        }

        private void RunSetLanguageCommand(object parm)
        {
            if (SelectedVersion != null && parm is string langId)
            {
                SelectedVersion.LanguageId = langId;
            }
        }

        private void RunSavePropertyCommand()
        {
            if (Properties != null)
            {
                Execution.TryCatch(() =>
                {
                    Properties.Save();
                }, (ex) => MainWindowViewModel.Instance.CreateNotificationMessage(ex.Message));
            }
        }

        private bool CanRunSavePropertyCommand()
        {
            return IsOpenXml && Properties != null;
        }

        private void PrepareForOpenXml()
        {
            IsOpenXml = false;
            Properties = null;
            if (SelectedVersion != null)
            {
                string fileName = Paths.Title.WithRoot(SelectedVersion.FileName);
                long docType = DocumentTypeTable.GetDocTypeFromFileName(fileName);
                if (docType == DocumentTypeTable.Defs.Values.WordOpenXmlFileType)
                {
                    IsOpenXml = true;
                    Execution.TryCatch(() =>
                    {
                        Properties = OpenXmlDocument.Reader.GetProperties(fileName);
                    }, (ex) => MainWindowViewModel.Instance.CreateNotificationMessage(ex.Message));
                }
            }
        }
        #endregion
    }
}