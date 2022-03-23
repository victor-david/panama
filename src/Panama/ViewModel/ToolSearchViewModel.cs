/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Panama.Tools;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.Utility;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SysProps = Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the search tool.
    /// </summary>
    public class ToolSearchViewModel : DataGridViewModel<SearchTable>
    {
        #region Private
        private SearchRow selectedSearch;
        private bool haveResults;
        private readonly WindowsFileSearch provider;
        private TitleVersionTable TitleVersionTable => DatabaseController.Instance.GetTable<TitleVersionTable>();
        private SearchTable SearchTable => DatabaseController.Instance.GetTable<SearchTable>();
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets or sets the search text.
        /// </summary>
        public string SearchText
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a boolean value that indicates if the lastest search contains results
        /// </summary>
        public bool HaveResults
        {
            get => haveResults;
            private set => SetProperty(ref haveResults, value);
        }

        /// <summary>
        /// Gets the selected search row
        /// </summary>
        public SearchRow SelectedSearch
        {
            get => selectedSearch;
            private set => SetProperty(ref selectedSearch, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolSearchViewModel"/> class.
        /// </summary>
        public ToolSearchViewModel()
        {
            Columns.CreateResource<BooleanToPathConverter>("V", SearchTable.Defs.Columns.IsVersion, ResourceKeys.Icon.SquareSmallGreenIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028);
            Columns.Create("Type", SearchTable.Defs.Columns.Type).MakeFixedWidth(FixedWidth.W048);
            Columns.Create("Size", SearchTable.Defs.Columns.Size).MakeNumeric(null, FixedWidth.W076);
            Columns.Create("Created", SearchTable.Defs.Columns.Created).MakeDate();
            Columns.Create("Modified", SearchTable.Defs.Columns.Modified).MakeDate();
            Columns.Create("File", SearchTable.Defs.Columns.File).MakeFlexWidth(2.0);
            Columns.Create("Title", SearchTable.Defs.Columns.Title).MakeFlexWidth(1.25);
            Columns.Create("Author", SearchTable.Defs.Columns.Author).MakeFixedWidth(FixedWidth.W180);
            Columns.Create("Company", SearchTable.Defs.Columns.Company);

            Commands.Add("Begin", RunSearchCommand);
            Commands.Add("GoToTitleRecord", RunGoToTitleRecordCommand, CanRunGoToTitleRecordCommand);
            Commands.Add("DeleteItem", RunDeleteItemCommand, CanRunDeleteItemCommand);
            //RawCommands.Add("TogglePreview", (o) => { IsPreviewMode = !IsPreviewMode; });

            MenuItems.AddItem(Strings.CommandOpenItemOrDoubleClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);
            //MenuItems.AddItem("Go to title record for this item", Commands["GoToTitleRecord"]).AddImageResource("ImageBrowseToUrlMenu");
            MenuItems.AddSeparator();
            MenuItems.AddItem("Delete this item", Commands["DeleteItem"]).AddIconResource(ResourceKeys.Icon.XRedIconKey);

            // init the search provider
            provider = new WindowsFileSearch();
            provider.Scopes.Add(Config.Instance.FolderTitleRoot);
            provider.ExcludedScopes.Add(Config.Instance.FolderExport);
            provider.ExcludedScopes.Add(Config.Instance.FolderSubmissionDocument);
            provider.ExcludedScopes.Add(Config.Instance.FolderSubmissionMessage);
            provider.ExcludedScopes.Add(Config.Instance.FolderSubmissionMessageAttachment);
            HaveResults = true;
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedSearch = SearchRow.Create(SelectedRow);
        }

        protected override void RunOpenRowCommand()
        {
            if (SelectedSearch != null)
            {
                Open.TitleVersionFile(SelectedSearch.File);
            }
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void RunSearchCommand(object parm)
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                Execution.TryCatch(() =>
                    {
                        WindowsSearchResultCollection results = provider.GetSearchResults(SearchText);

                        foreach (WindowsSearchResult result in results)
                        {
                            string pathDisplay = Paths.Title.WithoutRoot(result.Values.GetValue<string>(SysProps.System.ItemPathDisplay));
                            result.SetItemPathDisplay(pathDisplay);
                            bool versionExists = TitleVersionTable.VersionWithFileExists(pathDisplay);
                            SearchTable.Add(result.ToSearchTableItem(versionExists));
                        }
                        HaveResults = results.Count > 0;
                        ListView.Refresh();
                    });
            }
        }

        private void RunGoToTitleRecordCommand(object parm)
        {
            if (SelectedSearch != null)
            {
                //if (row.Extended is ExtendedSearchResult extended && extended.IsVersion)
                //{
                //    var ws = MainWindowViewModel.Instance.SwitchToWorkspace<TitleViewModel>();
                //    if (ws != null)
                //    {
                //        ws.Config.TitleFilter.SetIdFilter(extended.Versions[0].TitleId);
                //        if (ws.MainView.Count == 1)
                //        {
                //            /* This method uses a funky work around */
                //            // TODO
                //            // ws.SetSelectedItem(ws.MainView[0]);
                //            /* Can be assigned directly, but doesn't highlight the row */
                //            //ws.SelectedItem = ws.DataView[0];
                //        }
                //    }
                //}
            }
        }

        private bool CanRunGoToTitleRecordCommand(object o)
        {
            //if (SelectedItem is WindowsSearchResult row)
            //{
            //    return row.Extended is ExtendedSearchResult extended && extended.IsVersion;
            //}
            return false;
        }


        private void RunDeleteItemCommand(object parm)
        {
            if (CanRunDeleteItemCommand(parm))
            {
                // TODO
                //string fileName = Paths.Title.WithRoot(row.Values[SysProps.System.ItemPathDisplay].ToString());
                //if (FileOperations.SendToRecycle(fileName))
                //{
                //    resultsView.Remove(row);
                //}
            }
        }

        private bool CanRunDeleteItemCommand(object parm)
        {
            return !SelectedSearch?.IsVersion ?? false;
        }
        #endregion
    }
}