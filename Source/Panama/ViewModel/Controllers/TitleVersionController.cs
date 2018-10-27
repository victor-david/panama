using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.OpenXml;
using Restless.Tools.Utility;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

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
        private int dataViewCount;
        private TitleVersionTable versionTable;
        private TitleVersionTable.TitleVersionInfo verInfo;
        private TitleVersionTable.RowObject selectedRowObj;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the header text
        /// </summary>
        public override string Header
        {
            get
            {
                if (verInfo == null) return HeaderPreface;
                if (verInfo.VersionCount == verInfo.Versions.Count)
                {
                    return $"{HeaderPreface} ({verInfo.VersionCount})";
                }
                return $"{HeaderPreface} ({verInfo.VersionCount}/{verInfo.Versions.Count})";
            }
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
        public string VersionFileName
        {
            get
            {
                if (selectedRowObj != null)
                {
                    return Paths.Title.WithRoot(selectedRowObj.FileName);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the count of rows in the data view. The view binds to this property
        /// </summary>
        public int DataViewCount
        {
            get => dataViewCount;
            private set => SetProperty(ref dataViewCount, value);
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
            versionTable = DatabaseController.Instance.GetTable<TitleVersionTable>();
            AssignDataViewFrom(versionTable);
            DataView.RowFilter = $"{TitleVersionTable.Defs.Columns.TitleId}=-1";
            DataView.Sort = $"{TitleVersionTable.Defs.Columns.TitleId}, {TitleVersionTable.Defs.Columns.Version} DESC, {TitleVersionTable.Defs.Columns.Revision} ASC";
            Columns.CreateImage<IntegerToImageConverter>("T", TitleVersionTable.Defs.Columns.DocType, "ImageFileType", 20.0);

            Columns.Create<IntegerToCharConverter>("Rev", TitleVersionTable.Defs.Columns.Revision)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.ShortString);
            Columns.Create("Updated", TitleVersionTable.Defs.Columns.Updated).MakeDate();
            Columns.Create("WC", TitleVersionTable.Defs.Columns.WordCount).MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Lang", TitleVersionTable.Defs.Columns.LangId).MakeFixedWidth(FixedWidth.ShortString);
            Columns.Create("File", TitleVersionTable.Defs.Columns.FileName).MakeFlexWidth(1.65);
            Columns.Create("Note", TitleVersionTable.Defs.Columns.Note);

            //Commands.Add("ConvertToRevision", RunConvertToRevisionCommand, CanRunConvertToRevisionCommand);
            Commands.Add("ConvertToVersion", RunConvertToVersionCommand, CanRunConvertToVersionCommand);

            Commands.Add("VersionAddByFile", RunAddVersionByFileCommand);
            Commands.Add("VersionReplace", RunReplaceVersionCommand, (o) => { return SelectedRow != null; });
            Commands.Add("VersionRemove", RunRemoveVersionCommand, (o) => { return SelectedRow != null; });
            Commands.Add("VersionMoveUp", RunMoveUpCommand, CanRunMoveUpCommand);
            Commands.Add("VersionMoveDown", RunMoveDownCommand, CanRunMoveDownCommand);
            Commands.Add("VersionSync", RunSyncCommand, (o) => { return SourceCount > 0 ; });
            Commands.Add("ContextMenuOpening", RunContextMenuOpeningCommand);
            Commands.Add("SaveProperty", RunSavePropertyCommand, CanRunSavePropertyCommand);

            //MenuItems.AddItem("Make this a revision of the version above", Commands["ConvertToRevision"]);
            MenuItems.AddItem("Make this a separate version", Commands["ConvertToVersion"]);
            MenuItems.AddSeparator();

            foreach (DataRow row in DatabaseController.Instance.GetTable<LanguageTable>().Rows)
            {
                string langId = row[LanguageTable.Defs.Columns.Id].ToString();
                string langName = row[LanguageTable.Defs.Columns.Name].ToString();
                string commandId = string.Format("SetLang{0}", langId);
                Commands.Add(commandId, (o) => { SetLanguage(langId); }, CanRunCommandIfRowSelected);
                MenuItems.AddItem(string.Format("Set language to {0} ({1})", langName, langId), Commands[commandId], null, langId);
            }

            HeaderPreface = Strings.HeaderVersions;
            AddViewSourceSortDescriptions();
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
            selectedRowObj = null;
            if (IsSelectedRowAccessible)
            {
                selectedRowObj = new TitleVersionTable.RowObject(SelectedRow);
            }
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
            DataView.RowFilter = $"{TitleVersionTable.Defs.Columns.TitleId}={titleId}";
            DataViewCount = DataView.Count;
            UpdateVersionInfo();
            OnPropertyChanged(nameof(Header));
        }

        /// <summary>
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> to open the selected title version.
        /// </summary>
        /// <param name="item">The <see cref="DataRowView"/> object of the selected row.</param>
        protected override void RunOpenRowCommand(object item)
        {
            if (item is DataRowView view)
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
        private void UpdateVersionInfo()
        {
            verInfo = versionTable.GetVersionInfo(GetOwnerSelectedPrimaryId());
            Debug.WriteLine(verInfo.ToString());
        }

        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.GroupDescriptions.Clear();
            // BUG: If grouped, and the first clicked parent has zero children, all the columns are scrunched together,
            // and clicking on another parent (with children) does not change the columns.
            // If not grouped, still scrunched if the first clicked parent has no children, but subsequent clicks
            // on parents that do have children restore the columns.
            // UPDATE 2018-08-25:
            //   Workaround implemented. By binding the visibility of the data grid to the child count (0=hidden, otherwise visible)
            //   the columns display as they should.
            MainSource.GroupDescriptions.Add(new PropertyGroupDescription(TitleVersionTable.Defs.Columns.Version)); // , new DateToFormattedDateConverter()));
            MainSource.SortDescriptions.Add(new SortDescription(TitleVersionTable.Defs.Columns.Version, ListSortDirection.Descending));
            MainSource.SortDescriptions.Add(new SortDescription(TitleVersionTable.Defs.Columns.Revision, ListSortDirection.Ascending));
        }

        private void RunConvertToVersionCommand(object parm)
        {
            if (selectedRowObj != null)
            {
                versionTable.ConvertToVersion(selectedRowObj);
                AddViewSourceSortDescriptions();
                UpdateVersionInfo();
                OnPropertyChanged(nameof(Header));
            }
        }

        private bool CanRunConvertToVersionCommand(object parm)
        {
            return 
                selectedRowObj != null && 
                verInfo != null &&
                verInfo.GetRevisionCount(selectedRowObj.Version) > 1;
        }

        private void RunAddVersionByFileCommand(object o)
        {
            if (Owner.IsSelectedRowAccessible)
            {
                long titleId = (long)Owner.SelectedRow[TitleTable.Defs.Columns.Id];
                using (var dialog = CommonDialogFactory.Create(Config.Instance.FolderTitleVersion, Strings.CaptionSelectTitleVersionAddByFile))
                {
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        string fileName = Paths.Title.WithoutRoot(dialog.FileName);
                        // TODO - check if fileName already belongs to this title.
                        // If so, don't add it.
                        versionTable.AddVersion(titleId, fileName);
                        OnUpdate();
                    }
                }
            }
        }

        private void RunRemoveVersionCommand(object o)
        {
            if (selectedRowObj != null && Messages.ShowYesNo(Strings.ConfirmationRemoveTitleVersion))
            {
                versionTable.RemoveVersion(selectedRowObj);
                versionTable.Save();
                OnUpdate();
            }
        }

        private void RunReplaceVersionCommand(object o)
        {
            if (selectedRowObj != null)
            {
                using (var dialog = CommonDialogFactory.Create(Config.Instance.FolderTitleVersion, Strings.CaptionSelectTitleVersionReplaceByFile))
                {
                    if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                    {
                        string fileName = Paths.Title.WithoutRoot(dialog.FileName);
                        // TODO - check if fileName already belongs to this title.
                        // If so, don't change to it.
                        selectedRowObj.FileName = fileName;
                        OnPropertyChanged(nameof(VersionFileName));
                    }
                }

            }
        }

        private void RunMoveUpCommand(object o)
        {
            if (selectedRowObj != null)
            {
                versionTable.MoveVersionUp(selectedRowObj);
                AddViewSourceSortDescriptions();
                UpdateVersionInfo();
                OnPropertyChanged(nameof(Header));
            }
        }

        private void RunMoveDownCommand(object o)
        {
            if (selectedRowObj != null)
            {
                versionTable.MoveVersionDown(selectedRowObj);
                AddViewSourceSortDescriptions();
                UpdateVersionInfo();
                OnPropertyChanged(nameof(Header));
            }
        }

        private bool CanRunMoveUpCommand(object o)
        {
            return selectedRowObj != null && verInfo != null && !verInfo.IsLatest(selectedRowObj);
        }

        private bool CanRunMoveDownCommand(object o)
        {
            return selectedRowObj != null && verInfo != null && !verInfo.IsEarliest(selectedRowObj);
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
            if (selectedRowObj != null)
            {
                string fileName = Paths.Title.WithRoot(selectedRowObj.FileName);
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
