using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Database.SQLite;
using Restless.Tools.Utility;
using System.Windows.Controls;
using Restless.Tools.OpenXml;
using System.ComponentModel;
using System.Windows;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that manages title versions.
    /// </summary>
    public class TitleVersionController : TitleController
    {
        #region Private
        private bool isOpenXml;
        private PropertiesAdapter properties;
        #endregion

        /************************************************************************/
        
        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates whether the latest version of the currently selected title is an Open XML document.
        /// </summary>
        public bool IsOpenXml
        {
            get { return isOpenXml; }
            private set
            {
                SetProperty(ref isOpenXml, value);
            }
        }

        /// <summary>
        /// Gets the package property object that contains the properties of the latest document version.
        /// </summary>
        public PropertiesAdapter Properties
        {
            get { return properties; }
            private set
            {
                SetProperty(ref properties, value);
            }
        }

        /// <summary>
        /// Gets the version name for this file with the title root portion
        /// </summary>
        public string VersionFileName
        {
            get
            {
                if (SelectedRow != null)
                {
                    return Paths.Title.WithRoot(SelectedRow[TitleVersionTable.Defs.Columns.FileName].ToString());
                }

                return null;
            }
        }

        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleVersionController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleVersionController(TitleViewModel owner)
            : base(owner)
        {
            AssignDataViewFrom(DatabaseController.Instance.GetTable<TitleVersionTable>());
            DataView.RowFilter = string.Format("{0}=-1", TitleVersionTable.Defs.Columns.TitleId);
            DataView.Sort = string.Format("{0}, {1} DESC", TitleVersionTable.Defs.Columns.TitleId, TitleVersionTable.Defs.Columns.Version);
            Columns.CreateImage<IntegerToImageConverter>("T", TitleVersionTable.Defs.Columns.DocType, "ImageFileType", 20.0);
            var col = Columns.Create("V", TitleVersionTable.Defs.Columns.Version).MakeCentered().MakeFixedWidth(FixedWidth.Standard);
            Columns.SetDefaultSort(col, ListSortDirection.Descending);
            Columns.Create("Updated", TitleVersionTable.Defs.Columns.Updated).MakeDate();
            Columns.Create("WC", TitleVersionTable.Defs.Columns.WordCount).MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Lang", TitleVersionTable.Defs.Columns.LangId).MakeFixedWidth(FixedWidth.ShortString);
            Columns.Create("File", TitleVersionTable.Defs.Columns.FileName);
            Columns.Create("Note", TitleVersionTable.Defs.Columns.Note);
            //Owner.RawCommands.Add("VersionAddByCopy", RunAddVersionByCopyCommand, (o) => { return SelectedRow != null; });
            Commands.Add("VersionAddByFile", RunAddVersionByFileCommand);
            Commands.Add("VersionReplace", RunReplaceVersionCommand, (o) => { return SelectedRow != null; });
            Commands.Add("VersionRemove", RunRemoveVersionCommand, (o) => { return SelectedRow != null; });
            Commands.Add("VersionMoveUp", RunMoveUpCommand, CanRunMoveUpCommand);
            Commands.Add("VersionMoveDown", RunMoveDownCommand, CanRunMoveDownCommand);
            Commands.Add("VersionSync", RunSyncCommand, (o) => { return SourceCount > 0 ; });
            Commands.Add("ContextMenuOpening", RunContextMenuOpeningCommand);
            Commands.Add("SaveProperty", RunSavePropertyCommand, CanRunSavePropertyCommand);

            foreach (DataRow row in DatabaseController.Instance.GetTable<LanguageTable>().Rows)
            {
                string langId = row[LanguageTable.Defs.Columns.Id].ToString();
                string langName = row[LanguageTable.Defs.Columns.Name].ToString();
                string commandId = string.Format("SetLang{0}", langId);
                Commands.Add(commandId, (o) => { SetLanguage(langId); }, CanRunCommandIfRowSelected);
                MenuItems.AddItem(string.Format("Set language to {0} ({1})", langName, langId), Commands[commandId], null, langId);
            }

            HeaderPreface = Strings.HeaderVersions;
        }
        #endregion

        /************************************************************************/

        #region Public methods

        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called when the selected item on the associated data grid has changed.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            PrepareForOpenXml();
            OnPropertyChanged(nameof(VersionFileName));
        }

        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            long titleId = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = string.Format("{0}={1}", TitleVersionTable.Defs.Columns.TitleId, titleId);
        }

        /// <summary>
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> to open the selected title version.
        /// </summary>
        /// <param name="item">The <see cref="DataRowView"/> object of the selected row.</param>
        protected override void RunOpenRowCommand(object item)
        {
            DataRowView view = item as DataRowView;
            if (view != null)
            {
                OpenFileRow(view.Row, TitleVersionTable.Defs.Columns.FileName, Config.Instance.FolderTitleRoot, (f) =>
                    {
                        Messages.ShowError(string.Format(Strings.FormatStringFileNotFound, f, "FolderTitleRoot"));
                    });
            }
        }
        #endregion
        
        /************************************************************************/

        #region Private methods
        private void RunAddVersionByFileCommand(object o)
        {

            if (Owner.SelectedRow != null)
            {
                long titleId = (long)Owner.SelectedRow[TitleTable.Defs.Columns.Id];
                using (var dialog = CommonDialogFactory.Create(Config.Instance.FolderTitleVersion, Strings.CaptionSelectTitleVersionAddByFile))
                {
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        DatabaseController.Instance.GetTable<TitleVersionTable>().AddVersion(titleId, Paths.Title.WithoutRoot(dialog.FileName));
                    }
                }
            }
        }

        private void RunRemoveVersionCommand(object o)
        {
            if (SelectedRow != null && Messages.ShowYesNo(Strings.ConfirmationRemoveTitleVersion))
            {
                long titleId = (long)SelectedRow[TitleVersionTable.Defs.Columns.TitleId];
                long version = (long)SelectedRow[TitleVersionTable.Defs.Columns.Version];
                DatabaseController.Instance.GetTable<TitleVersionTable>().RemoveVersion(titleId, version);
                DatabaseController.Instance.GetTable<TitleVersionTable>().Save();
            }
        }

        private void RunReplaceVersionCommand(object o)
        {
            if (Owner.SelectedRow != null && SelectedRow != null)
            {
                long titleId = (long)Owner.SelectedRow[TitleTable.Defs.Columns.Id];
                long version = (long)SelectedRow[TitleVersionTable.Defs.Columns.Version];
                using (var dialog = CommonDialogFactory.Create(Config.Instance.FolderTitleVersion, Strings.CaptionSelectTitleVersionReplaceByFile))
                {
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        DatabaseController.Instance.GetTable<TitleVersionTable>().ReplaceVersion(titleId, version, Paths.Title.WithoutRoot(dialog.FileName));
                        OnPropertyChanged(nameof(VersionFileName));
                    }
                }
            }
        }

        private void RunMoveUpCommand(object o)
        {
            /* move up means increasing the version number */
            long titleId = (long)SelectedRow[TitleVersionTable.Defs.Columns.TitleId];
            long version = (long)SelectedRow[TitleVersionTable.Defs.Columns.Version];
            DatabaseController.Instance.GetTable<TitleVersionTable>().ChangeVersionNumber(titleId, version, version + 1);

        }

        private void RunMoveDownCommand(object o)
        {
            /* move down means decreasing the version number */
            long titleId = (long)SelectedRow[TitleVersionTable.Defs.Columns.TitleId];
            long version = (long)SelectedRow[TitleVersionTable.Defs.Columns.Version];
            DatabaseController.Instance.GetTable<TitleVersionTable>().ChangeVersionNumber(titleId, version, version - 1);
        }

        private bool CanRunMoveUpCommand(object o)
        {
            return
                SelectedRow != null &&
                (long)SelectedRow[TitleVersionTable.Defs.Columns.Version] != SourceCount;
        }

        private bool CanRunMoveDownCommand(object o)
        {
            return
                SelectedRow != null &&
                (long)SelectedRow[TitleVersionTable.Defs.Columns.Version] != 1;
        }

        private void RunSyncCommand(object o)
        {
            if (Owner.SelectedPrimaryKey != null)
            {
                long titleId = (long)Owner.SelectedPrimaryKey;
                var window = WindowFactory.TitleVersionRename.Create(titleId);
                window.ShowDialog();
            }
        }
        
        private void RunContextMenuOpeningCommand(object args)
        {
            if (SelectedRow != null)
            {
                string langId = SelectedRow[TitleVersionTable.Defs.Columns.LangId].ToString();
                foreach (var item in MenuItems.OfType<MenuItem>())
                {
                    item.Icon = null;
                    if (langId.Equals(item.Tag))
                    {
                        item.Icon = ResourceHelper.Get("ImageDotGreenMenu");
                    }
                }
            }
        }

        private void SetLanguage(string langId)
        {
            if (SelectedRow != null)
            {
                SelectedRow[TitleVersionTable.Defs.Columns.LangId] = langId;
            }
        }

        private void RunSavePropertyCommand(object o)
        {
            if (Properties != null)
            {
                Execution.TryCatch(() =>
                {
                    Properties.Save();
                }, (ex) => { Owner.MainViewModel.CreateNotificationMessage(ex.Message); });
            }
        }

        private bool CanRunSavePropertyCommand(object o)
        {
            return IsOpenXml && Properties != null;
        }

        private void PrepareForOpenXml()
        {
            IsOpenXml = false;
            Properties = null;
            if (SelectedRow != null)
            {
                string fileName = Paths.Title.WithRoot(SelectedRow[TitleVersionTable.Defs.Columns.FileName].ToString());
                long docType = DatabaseController.Instance.GetTable<DocumentTypeTable>().GetDocTypeFromFileName(fileName);
                if (docType == DocumentTypeTable.Defs.Values.WordOpenXmlFileType)
                {
                    IsOpenXml = true;
                    Execution.TryCatch(() =>
                    {
                        Properties = OpenXmlDocument.Reader.GetProperties(fileName);
                    }, (ex) => { Owner.MainViewModel.CreateNotificationMessage(ex.Message); });
                }
            }
        }
        #endregion
    }
}
