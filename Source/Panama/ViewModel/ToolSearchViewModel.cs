using Restless.App.Panama.Configuration;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Search;
using Restless.Tools.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the search tool.
    /// </summary>
    public class ToolSearchViewModel : DataGridPreviewViewModel
    {
        #region Private
        private string foundHeader;
        private ObservableCollection<WindowsSearchResult> resultsView;
        private List<DataGridColumn> previewColumns;
        private bool isEmptyResultSet;
        private WindowsFileSearch provider;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets a string value for the results header.
        /// </summary>
        public string FoundHeader
        {
            get { return foundHeader; }
            private set
            {
                SetProperty(ref foundHeader, value);
            }
        }

        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        public string SearchText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if the lastest search returned an empty result set.
        /// </summary>
        public bool IsEmptyResultSet
        {
            get { return isEmptyResultSet; }
            private set
            {
                SetProperty(ref isEmptyResultSet, value);
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public ToolSearchViewModel()
        {
            DisplayName = Strings.CommandToolSearch;
            MaxCreatable = 3;
            previewColumns = new List<DataGridColumn>();

            resultsView = new ObservableCollection<WindowsSearchResult>();
            MainSource.Source = resultsView;

            Columns.CreateImage<BooleanToImageConverter>("V", "Extended.IsVersion");
            Columns.Create("Type", WindowsSearchResult.GetBindingReference(SysProps.System.ItemType)).MakeFixedWidth(FixedWidth.ShortString);
            Columns.Create("Size", WindowsSearchResult.GetBindingReference(SysProps.System.Size)).MakeNumeric(null, FixedWidth.LongerNumeric);
            Columns.Create("Modified", WindowsSearchResult.GetBindingReference(SysProps.System.DateModified)).MakeDate()
                .AddToolTip("The modified date according to the file system");

            previewColumns.Add
                (
                    Columns.Create("Created", WindowsSearchResult.GetBindingReference(SysProps.System.Document.DateCreated)).MakeDate()
                    .AddToolTip("The created date according to the document internal properties")
                );
            previewColumns.Add
                (
                    Columns.Create("Saved", WindowsSearchResult.GetBindingReference(SysProps.System.Document.DateSaved)).MakeDate()
                        .AddToolTip("The saved date according to the document internal properties")
                );

            Columns.Create("File", WindowsSearchResult.GetBindingReference(SysProps.System.ItemPathDisplay)).MakeFlexWidth(2.0);

            previewColumns.Add(Columns.Create("Title", WindowsSearchResult.GetBindingReference(SysProps.System.Title)).MakeFlexWidth(1.25));
            previewColumns.Add(Columns.Create("Author", WindowsSearchResult.GetBindingReference(SysProps.System.Author)).MakeFixedWidth(FixedWidth.LongString));
            previewColumns.Add(Columns.Create("Comment", WindowsSearchResult.GetBindingReference(SysProps.System.Comment)));
            previewColumns.Add(Columns.Create("Company", WindowsSearchResult.GetBindingReference(SysProps.System.Company)));

            Commands.Add("Begin", RunSearchCommand);
            Commands.Add("OpenItem", RunOpenItemCommand, (o)=> SelectedItem != null);
            Commands.Add("GoToTitleRecord", RunGoToTitleRecordCommand, CanRunGoToTitleRecordCommand);
            Commands.Add("DeleteItem", RunDeleteItemCommand, CanRunDeleteItemCommand);
            //RawCommands.Add("TogglePreview", (o) => { IsPreviewMode = !IsPreviewMode; });

            MenuItems.AddItem(Strings.CommandOpenItemOrDoubleClick, Commands["OpenItem"], "ImageOpenFileMenu");
            MenuItems.AddItem("Go to title record for this item", Commands["GoToTitleRecord"], "ImageBrowseToUrlMenu");
            MenuItems.AddSeparator();
            MenuItems.AddItem("Delete this item", Commands["DeleteItem"], "ImageDeleteMenu");
            UpdateFoundHeader();
            // init the search provider
            provider = new WindowsFileSearch();
            provider.Scopes.Add(Config.Instance.FolderTitleRoot);
            provider.ExcludedScopes.Add(Config.Instance.FolderExport);
            provider.ExcludedScopes.Add(Config.Instance.FolderSubmissionDocument);
            provider.ExcludedScopes.Add(Config.Instance.FolderSubmissionMessage);
            provider.ExcludedScopes.Add(Config.Instance.FolderSubmissionMessageAttachment);
            IsEmptyResultSet = false;
        }
#pragma warning restore 1591
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called by the ancestor class when a preview of the selected item is needed.
        /// </summary>
        /// <param name="selectedItem">The currently selected grid item.</param>
        protected override void OnPreview(object selectedItem)
        {
            WindowsSearchResult item = selectedItem as WindowsSearchResult;
            if (item != null)
            {
                string path = item.Values[SysProps.System.ItemFolderPathDisplay].ToString();
                string file = item.Values[SysProps.System.FileName].ToString();
                string fileName = Path.Combine(path, file);
                PerformPreview(fileName);
            }
        }

        /// <summary>
        /// Gets the preview mode for the specified item.
        /// </summary>
        /// <param name="selectedItem">The selected grid item</param>
        /// <returns>The preview mode</returns>
        protected override PreviewMode GetPreviewMode(object selectedItem)
        {
            var item = selectedItem as WindowsSearchResult;
            if (item != null)
            {
                string path = item.Values[SysProps.System.ItemFolderPathDisplay].ToString();
                string file = item.Values[SysProps.System.FileName].ToString();
                return DocumentPreviewer.GetPreviewMode(Path.Combine(path, file));
            }
            return PreviewMode.Unsupported;
        }

        /// <summary>
        /// Called by the ancestor class when <see cref="DataGridPreviewViewModel.IsPreviewActive"/> changes.
        /// </summary>
        protected override void OnIsPreviewActiveChanged()
        {
            if (previewColumns != null)
            {
                Visibility visibility = (IsPreviewActive) ? Visibility.Hidden : Visibility.Visible;
                foreach (var col in previewColumns)
                {
                    col.Visibility = visibility;
                }
            }
        }
        #endregion
        
        /************************************************************************/

        #region Private Methods
        private void UpdateFoundHeader()
        {
            FoundHeader = string.Format(Strings.HeaderToolOperationSearchFoundFormat, resultsView.Count);
        }

        private void RunSearchCommand(object o)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                Execution.TryCatch(() =>
                    {
                        resultsView.Clear();
                        var versions = DatabaseController.Instance.GetTable<TitleVersionTable>();
                        var results = provider.GetSearchResults(SearchText);

                        foreach (var result in results)
                        {
                            result.SetItemPathDisplay(Paths.Title.WithoutRoot(result.Values[SysProps.System.ItemPathDisplay].ToString()));
                            result.Extended = new ExtendedSearchResult(versions.GetVersionsWithFile(result.Values[SysProps.System.ItemPathDisplay].ToString()));
                            resultsView.Add(result);
                        }
                        UpdateFoundHeader();
                        IsEmptyResultSet = (results.Count == 0);
                    });
            }
        }

        private void RunOpenItemCommand(object o)
        {
            if (SelectedItem is WindowsSearchResult row)
            {
                OpenHelper.OpenFile(row.Values[SysProps.System.ItemUrl].ToString());
            }
        }

        private void RunGoToTitleRecordCommand(object o)
        {
            if (SelectedItem is WindowsSearchResult row)
            {
                if (row.Extended is ExtendedSearchResult extended && extended.IsVersion)
                {
                    var ws = MainViewModel.SwitchToWorkspace<TitleViewModel>();
                    if (ws != null)
                    {
                        // in case the VM was already open with a filter applied.
                        ws.Filters.ClearAll();

                        // assigning the property applies the filter
                        ws.Config.TitleFilter.Id = extended.Versions[0].TitleId;
                        if (ws.DataView.Count == 1)
                        {
                            /* This method uses a funky work around */
                            ws.SetSelectedItem(ws.DataView[0]);
                            /* Can be assigned directly, but doesn't highlight the row */
                            //ws.SelectedItem = ws.DataView[0];
                        }
                    }
                }
            }
        }

        private bool CanRunGoToTitleRecordCommand(object o)
        {
            if (SelectedItem is WindowsSearchResult row)
            {
                return (row.Extended is ExtendedSearchResult extended && extended.IsVersion);
            }
            return false;
        }


        private void RunDeleteItemCommand(object o)
        {
            if (SelectedItem is WindowsSearchResult row)
            {
                string fileName = Paths.Title.WithRoot(row.Values[SysProps.System.ItemPathDisplay].ToString());
                if (FileOperations.SendToRecycle(fileName))
                {
                    resultsView.Remove(row);
                }
            }
        }

        private bool CanRunDeleteItemCommand(object o)
        {
            if (SelectedItem is WindowsSearchResult row)
            {
                return (row.Extended is ExtendedSearchResult extended && !extended.IsVersion);
            }
            return false;
        }


        #endregion

        #region Private helper class
        private class ExtendedSearchResult
        {
            public List<TitleVersionTable.RowObject> Versions
            {
                get;
            }

            public bool IsVersion
            {
                get => Versions.Count > 0;
            }

            public ExtendedSearchResult(List<TitleVersionTable.RowObject> versions)
            {
                Versions = versions;
            }
        }
        #endregion
    }
}