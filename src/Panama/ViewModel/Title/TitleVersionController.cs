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
using Restless.Toolkit.Mvvm;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Controls;

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
            Columns.Create("V", TitleVersionTable.Defs.Columns.Version)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create<IntegerToCharConverter>("Rev", TitleVersionTable.Defs.Columns.Revision)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Updated", TitleVersionTable.Defs.Columns.Updated)
                .MakeDate();

            Columns.Create("WC", TitleVersionTable.Defs.Columns.WordCount)
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Create("Lang", TitleVersionTable.Defs.Columns.LangId)
                .MakeFixedWidth(FixedWidth.W048);

            Columns.Create("File", TitleVersionTable.Defs.Columns.FileName);

            Commands.Add("VersionMoveUp", RunMoveUpCommand, CanRunMoveUpCommand);
            Commands.Add("VersionMoveDown", RunMoveDownCommand, CanRunMoveDownCommand);
            Commands.Add("VersionSync", RunSyncCommand);
            Commands.Add("ContextMenuOpening", RunContextMenuOpeningCommand);
            Commands.Add("SaveProperty", RunSavePropertyCommand, CanRunSavePropertyCommand);
            Commands.Add("SetLanguage", RunSetLanguageCommand, o => IsSelectedRowAccessible);

            MenuItems.AddItem(Strings.MenuItemAddTitleVersion, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddItem(Strings.MenuItemReplaceTitleVersion, RelayCommand.Create(RunReplaceVersionCommand, p => CanRunVersionCommand()));
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemMakeSeparateVersion, RelayCommand.Create(RunConvertToVersionCommand, CanRunConvertToVersionCommand));
            MenuItems.AddSeparator();

            foreach (DataRow row in DatabaseController.Instance.GetTable<LanguageTable>().Rows)
            {
                string langId = row[LanguageTable.Defs.Columns.Id].ToString();
                string langName = row[LanguageTable.Defs.Columns.Name].ToString();

                MenuItems.AddItem($"Set language to {langName} ({langId})", Commands["SetLanguage"]).AddCommandParm(langId).AddTag(langId);
            }

            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemRemoveTitleVersion, DeleteCommand).AddIconResource(ResourceKeys.Icon.XMediumIconKey);

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

        /// <inheritdoc/>
        protected override void RunDeleteCommand()
        {
            if (CanRunVersionCommand() && MessageWindow.ShowContinueCancel(Strings.ConfirmationRemoveTitleVersion))
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
                    if (item.Tag is string menuLangId)
                    {
                        item.Icon = null;
                        if (langId.Equals(menuLangId, StringComparison.Ordinal))
                        {
                            item.Icon = LocalResources.Get<System.Windows.Shapes.Path>(ResourceKeys.Icon.SquareSmallRedIconKey);
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