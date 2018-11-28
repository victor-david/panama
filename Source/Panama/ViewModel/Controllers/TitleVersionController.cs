using Microsoft.WindowsAPICodePack.Dialogs;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Controls;
using Restless.Tools.OpenXml;
using Restless.Tools.Utility;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
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
        private long currentOwnerTitleId;
        private bool isOpenXml;
        private PropertiesAdapter properties;
        private int dataViewCount;
        private TitleVersionTable versionTable;
        private Database.Tables.TitleVersionController verController;
        private TitleVersionTable.RowObject selectedRowObj;
        private DataGridColumn versionColumn;
        private string toggleGroupText;
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
                if (verController == null) return HeaderPreface;
                if (verController.VersionCount == verController.Versions.Count)
                {
                    return $"{HeaderPreface} ({verController.VersionCount})";
                }
                return $"{HeaderPreface} ({verController.VersionCount}/{verController.Versions.Count})";
            }
        }

        /// <summary>
        /// Gets the text to display on the toggle group button.
        /// </summary>
        public string ToggleGroupText
        {
            get => toggleGroupText;
            private set => SetProperty(ref toggleGroupText, value);
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
            currentOwnerTitleId = -1;
            versionTable = DatabaseController.Instance.GetTable<TitleVersionTable>();
            AssignDataViewFrom(versionTable);
            DataView.RowFilter = $"{TitleVersionTable.Defs.Columns.TitleId}=-1";
            DataView.Sort = $"{TitleVersionTable.Defs.Columns.TitleId}, {TitleVersionTable.Defs.Columns.Version} DESC, {TitleVersionTable.Defs.Columns.Revision} ASC";
            Columns.CreateImage<IntegerToImageConverter>("T", TitleVersionTable.Defs.Columns.DocType, "ImageFileType", 20.0);

            versionColumn = Columns.Create("V", TitleVersionTable.Defs.Columns.Version)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.Standard);
            
            Columns.Create<IntegerToCharConverter>("Rev", TitleVersionTable.Defs.Columns.Revision)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Updated", TitleVersionTable.Defs.Columns.Updated).MakeDate();
            Columns.Create("WC", TitleVersionTable.Defs.Columns.WordCount).MakeFixedWidth(FixedWidth.Standard);
            Columns.Create("Lang", TitleVersionTable.Defs.Columns.LangId).MakeFixedWidth(FixedWidth.ShortString);
            Columns.Create("File", TitleVersionTable.Defs.Columns.FileName).MakeFlexWidth(1.65);
            Columns.Create("Note", TitleVersionTable.Defs.Columns.Note);

            Commands.Add("ConvertToVersion", RunConvertToVersionCommand, CanRunConvertToVersionCommand);
            Commands.Add("VersionAddByFile", RunAddVersionByFileCommand);
            Commands.Add("VersionReplace", RunReplaceVersionCommand, (o) => IsSelectedRowAccessible);
            Commands.Add("VersionRemove", RunRemoveVersionCommand, (o) => IsSelectedRowAccessible);
            Commands.Add("VersionMoveUp", RunMoveUpCommand, CanRunMoveUpCommand);
            Commands.Add("VersionMoveDown", RunMoveDownCommand, CanRunMoveDownCommand);
            Commands.Add("VersionSync", RunSyncCommand, (o) => SourceCount > 0 );
            Commands.Add("ContextMenuOpening", RunContextMenuOpeningCommand);
            Commands.Add("SaveProperty", RunSavePropertyCommand, CanRunSavePropertyCommand);
            Commands.Add("ToggleGroup", RunToggleGroupCommand);
            Commands.Add("SetLanguage", RunSetLanguageCommand, (o) => IsSelectedRowAccessible);

            //MenuItems.AddItem("Make this a revision of the version above", Commands["ConvertToRevision"]);
            MenuItems.AddItem("Make this a separate version", Commands["ConvertToVersion"]);
            MenuItems.AddSeparator();

            foreach (DataRow row in DatabaseController.Instance.GetTable<LanguageTable>().Rows)
            {
                string langId = row[LanguageTable.Defs.Columns.Id].ToString();
                string langName = row[LanguageTable.Defs.Columns.Name].ToString();

                MenuItems.AddItem($"Set language to {langName} ({langId})", Commands["SetLanguage"]).AddCommandParm(langId).AddTag(langId);
            }

            HeaderPreface = Strings.HeaderVersions;
            SetToggleGroupProperties();
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
            selectedRowObj = null;
            currentOwnerTitleId = GetOwnerSelectedPrimaryId();
            DataView.RowFilter = $"{TitleVersionTable.Defs.Columns.TitleId}={currentOwnerTitleId}";
            DataViewCount = DataView.Count;
            verController = versionTable.GetVersionController(currentOwnerTitleId);
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
            if (Config.GroupTitleVersion)
            {
                MainSource.GroupDescriptions.Add(new PropertyGroupDescription(TitleVersionTable.Defs.Columns.Version));
            }
            MainSource.SortDescriptions.Add(new SortDescription(TitleVersionTable.Defs.Columns.Version, ListSortDirection.Descending));
            MainSource.SortDescriptions.Add(new SortDescription(TitleVersionTable.Defs.Columns.Revision, ListSortDirection.Ascending));
        }

        private void RunConvertToVersionCommand(object parm)
        {
            if (CanRunVersionCommand())
            {
                verController.ConvertToVersion(selectedRowObj);
                AddViewSourceSortDescriptions();
                OnPropertyChanged(nameof(Header));
            }
        }

        private bool CanRunConvertToVersionCommand(object parm)
        {
            return
                CanRunVersionCommand() && 
                verController.GetRevisionCount(selectedRowObj.Version) > 1;
        }

        private void RunAddVersionByFileCommand(object o)
        {
            if (verController != null)
            {
                using (var dialog = CommonDialogFactory.Create(Config.Instance.FolderTitleVersion, Strings.CaptionSelectTitleVersionAddByFile))
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

        private void RunRemoveVersionCommand(object o)
        {
            if (CanRunVersionCommand() && Messages.ShowYesNo(Strings.ConfirmationRemoveTitleVersion))
            {
                verController.Remove(selectedRowObj);
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
                        if (CanAddFileToTitle(fileName))
                        {
                            selectedRowObj.FileName = fileName;
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
            var verList = versionTable.GetVersionsWithFile(fileName);
            foreach (var ver in verList)
            {
                if (ver.TitleId == currentOwnerTitleId)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(fileName);
                    sb.AppendLine();
                    sb.Append(Strings.InvalidOpCannotAddVersionFile);
                    Messages.ShowError(sb.ToString());
                    return false;
                }
            }
            return true;
        }

        private void RunMoveUpCommand(object o)
        {
            if (CanRunVersionCommand())
            {
                verController.MoveUp(selectedRowObj);
                AddViewSourceSortDescriptions();
                OnPropertyChanged(nameof(Header));
            }
        }

        private void RunMoveDownCommand(object o)
        {
            if (CanRunVersionCommand())
            {
                verController.MoveDown(selectedRowObj);
                AddViewSourceSortDescriptions();
                OnPropertyChanged(nameof(Header));
            }
        }

        private bool CanRunMoveUpCommand(object o)
        {
            return CanRunVersionCommand() && !verController.IsLatest(selectedRowObj);
        }

        private bool CanRunMoveDownCommand(object o)
        {
            return CanRunVersionCommand() && !verController.IsEarliest(selectedRowObj);
        }

        /// <summary>
        /// Checks basic requirements for running a version command.
        /// </summary>
        /// <returns>
        /// true if selectedRowObject != null and verController != null
        /// and their title ids match.
        /// </returns>
        private bool CanRunVersionCommand()
        {
            return
                selectedRowObj != null &&
                verController != null &&
                selectedRowObj.TitleId == verController.TitleId;
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

        private void RunSetLanguageCommand(object parm)
        {
            if (selectedRowObj != null && parm is string langId)
            {
                selectedRowObj.LanguageId = langId;
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

        private void RunToggleGroupCommand(object parm)
        {
            Config.GroupTitleVersion = !Config.GroupTitleVersion;
            SetToggleGroupProperties();
            AddViewSourceSortDescriptions();
        }

        private void SetToggleGroupProperties()
        {
            ToggleGroupText = Config.GroupTitleVersion ? "Ungroup" : "Group";
            versionColumn.Visibility = (Config.GroupTitleVersion) ? Visibility.Collapsed : Visibility.Visible;
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
