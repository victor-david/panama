/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.OpenXml;
using Restless.Toolkit.Core.Utility;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that manages title versions.
    /// </summary>
    public class TitleVersionController : DataGridViewModelController<TitleViewModel, TitleVersionTable>
    {
        #region Private
        private bool isOpenXml;
        private PropertiesAdapter properties;
        private Database.Tables.TitleVersionController verController;
        private TitleVersionRow selectedVersion;
        #endregion

        /************************************************************************/

        #region Properties
        private DocumentTypeTable DocumentTypeTable => DatabaseController.Instance.GetTable<DocumentTypeTable>();
        private long CurrentTitleId => Owner?.SelectedTitle?.Id ?? 0;

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
            //Columns.CreateImage<IntegerToImageConverter>("T", TitleVersionTable.Defs.Columns.DocType, "ImageFileType", 20.0);
            Columns.Create("V", TitleVersionTable.Defs.Columns.Version)
            .MakeCentered()
            .MakeFixedWidth(FixedWidth.W042);

            Columns.Create<IntegerToCharConverter>("Rev", TitleVersionTable.Defs.Columns.Revision)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);
            Columns.Create("Updated", TitleVersionTable.Defs.Columns.Updated).MakeDate();
            Columns.Create("WC", TitleVersionTable.Defs.Columns.WordCount).MakeFixedWidth(FixedWidth.W042);
            Columns.Create("Lang", TitleVersionTable.Defs.Columns.LangId).MakeFixedWidth(FixedWidth.W048);
            Columns.Create("File", TitleVersionTable.Defs.Columns.FileName);

            Commands.Add("ConvertToVersion", RunConvertToVersionCommand, CanRunConvertToVersionCommand);
            Commands.Add("VersionAddByFile", RunAddVersionByFileCommand);
            Commands.Add("VersionReplace", RunReplaceVersionCommand, o => IsSelectedRowAccessible);
            Commands.Add("VersionRemove", RunRemoveVersionCommand, o => IsSelectedRowAccessible);
            Commands.Add("VersionMoveUp", RunMoveUpCommand, CanRunMoveUpCommand);
            Commands.Add("VersionMoveDown", RunMoveDownCommand, CanRunMoveDownCommand);
            Commands.Add("VersionSync", RunSyncCommand, o => SourceCount > 0 );
            Commands.Add("ContextMenuOpening", RunContextMenuOpeningCommand);
            Commands.Add("SaveProperty", RunSavePropertyCommand, CanRunSavePropertyCommand);
            Commands.Add("SetLanguage", RunSetLanguageCommand, o => IsSelectedRowAccessible);

            //MenuItems.AddItem("Make this a revision of the version above", Commands["ConvertToRevision"]);
            MenuItems.AddItem("Make this a separate version", Commands["ConvertToVersion"]);
            MenuItems.AddSeparator();

            foreach (DataRow row in DatabaseController.Instance.GetTable<LanguageTable>().Rows)
            {
                string langId = row[LanguageTable.Defs.Columns.Id].ToString();
                string langName = row[LanguageTable.Defs.Columns.Name].ToString();

                MenuItems.AddItem($"Set language to {langName} ({langId})", Commands["SetLanguage"]).AddCommandParm(langId).AddTag(langId);
            }

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

        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            SelectedVersion = null;
            verController = TitleVersionTable.GetVersionController(CurrentTitleId);
            ListView.Refresh();
            OnPropertyChanged(nameof(HaveVersion));
        }

        /// <summary>
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> to open the selected title version.
        /// </summary>
        protected override void RunOpenRowCommand()
        {
            if (SelectedVersion != null)
            {
                OpenFileRow(SelectedVersion.Row, TitleVersionTable.Defs.Columns.FileName, Config.Instance.FolderTitleRoot, (f) =>
                    {
                        MessageWindow.ShowError(string.Format(CultureInfo.InvariantCulture, Strings.FormatStringFileNotFound, f, nameof(Config.FolderTitleRoot)));
                    });
            }
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void RunConvertToVersionCommand(object parm)
        {
            if (CanRunVersionCommand())
            {
                verController.ConvertToVersion(SelectedVersion);
            }
        }

        private bool CanRunConvertToVersionCommand(object parm)
        {
            return
                CanRunVersionCommand() &&
                verController.GetRevisionCount(SelectedVersion.Version) > 1;
        }

        private void RunAddVersionByFileCommand(object parm)
        {
            if (verController != null)
            {
                using (CommonOpenFileDialog dialog = CommonDialogFactory.Create(Config.Instance.FolderTitleVersion, Strings.CaptionSelectTitleVersionAddByFile))
                {
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        string fileName = Paths.Title.WithoutRoot(dialog.FileName);
                        if (CanAddFileToTitle(fileName))
                        {
                            verController.Add(fileName);
                            OnUpdate();
                        }
                    }
                }
            }
        }

        private void RunRemoveVersionCommand(object parm)
        {
            if (CanRunVersionCommand() && MessageWindow.ShowYesNo(Strings.ConfirmationRemoveTitleVersion))
            {
                verController.Remove(SelectedVersion);
                Table.Save();
                OnUpdate();
            }
        }

        private void RunReplaceVersionCommand(object parm)
        {
            if (SelectedVersion != null)
            {
                using (CommonOpenFileDialog dialog = CommonDialogFactory.Create(Config.Instance.FolderTitleVersion, Strings.CaptionSelectTitleVersionReplaceByFile))
                {
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        string fileName = Paths.Title.WithoutRoot(dialog.FileName);
                        if (CanAddFileToTitle(fileName))
                        {
                            SelectedVersion.FileName = fileName;
                            OnPropertyChanged(nameof(VersionFileName));
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Checks to see if <paramref name="fileName"/> already belongs to the title.
        /// If so, displays a message and returns false.
        /// </summary>
        /// <param name="fileName">The file name to check.</param>
        /// <returns>true if <paramref name="fileName"/> does not already belong to </returns>
        private bool CanAddFileToTitle(string fileName)
        {
            foreach (TitleVersionRow ver in Table.EnumerateVersions(fileName))
            {
                if (ver.TitleId == CurrentTitleId)
                {
                    StringBuilder sb = new();
                    sb.AppendLine(fileName);
                    sb.AppendLine();
                    sb.Append(Strings.InvalidOpCannotAddVersionFile);
                    MessageWindow.ShowError(sb.ToString());
                    return false;
                }
            }
            return true;
        }

        private void RunMoveUpCommand(object parm)
        {
            if (CanRunVersionCommand())
            {
                verController.MoveUp(SelectedVersion);
            }
        }

        private void RunMoveDownCommand(object parm)
        {
            if (CanRunVersionCommand())
            {
                verController.MoveDown(SelectedVersion);
            }
        }

        private bool CanRunMoveUpCommand(object parm)
        {
            return CanRunVersionCommand() && !verController.IsLatest(SelectedVersion);
        }

        private bool CanRunMoveDownCommand(object parm)
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

        private void RunSyncCommand(object parm)
        {
            if (CurrentTitleId > 0)
            {
                WindowFactory.TitleVersionRename.Create(CurrentTitleId).ShowDialog();
            }
        }

        private void RunContextMenuOpeningCommand(object args)
        {
            if (SelectedVersion != null)
            {
                string langId = SelectedVersion.LanguageId;
                foreach (MenuItem item in MenuItems.OfType<MenuItem>())
                {
                    item.Icon = null;
                    if (langId.Equals(item.Tag))
                    {
                        item.Icon = LocalResources.Get<System.Windows.Shapes.Path>(ResourceKeys.Icon.SquareSmallRedIconKey);
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

        private void RunSavePropertyCommand(object parm)
        {
            if (Properties != null)
            {
                Execution.TryCatch(() =>
                {
                    Properties.Save();
                }, (ex) => MainWindowViewModel.Instance.CreateNotificationMessage(ex.Message));
            }
        }

        private bool CanRunSavePropertyCommand(object parm)
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