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
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.OpenXml;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Threading;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for titles management.
    /// </summary>
    public class TitleViewModel : DataGridViewModel<TitleTable>
    {
        #region Private
        private int selectedEditSection;
        private TitleRow selectedTitle;
        private string previewText;
        private const int SectionPreviewId = 6;
        private bool isOpenXml;
        #endregion

        /************************************************************************/

        #region Properties
        private TitleVersionTable TitleVersionTable => DatabaseController.Instance.GetTable<TitleVersionTable>();

        /// <summary>
        /// Gets or sets the selected edit section
        /// </summary>
        public int SelectedEditSection
        {
            get => selectedEditSection;
            set
            {
                SetProperty(ref selectedEditSection, value);
                PrepareForOpenXml();
            }
        }

        /// <summary>
        /// Gets the currently selected title row
        /// </summary>
        public TitleRow SelectedTitle
        {
            get => selectedTitle;
            private set => SetProperty(ref selectedTitle, value);
        }

        /// <summary>
        /// Gets the controller for the title versions.
        /// </summary>
        public TitleVersionController Versions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the controller for the title submissions.
        /// </summary>
        public TitleSubmissionController Submissions
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the controller for title published records.
        /// </summary>
        public TitlePublishedController Published
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the controller for title published records.
        /// </summary>
        public TitleSelfPublishedController SelfPublished
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the controller for the title tags.
        /// </summary>
        public TitleTagController Tags
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the title filter object.
        /// </summary>
        public TitleRowFilter Filters => Config.TitleFilter;

        /// <summary>
        /// Gets an enumerable of <see cref="AuthorTable.RowObject"/> items. The UI binds to this list.
        /// </summary>
        public IEnumerable<AuthorRow> Authors => DatabaseController.Instance.GetTable<AuthorTable>().EnumerateAuthors();

        /// <summary>
        /// Gets or sets the selected author item.
        /// </summary>
        public AuthorRow SelectedAuthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a string value that displays the written date.
        /// </summary>
        public string WrittenHeader => SelectedTitle?.GetWrittenToLocal(Config.Instance.DateFormat) ?? Strings.TextNone;

        /// <summary>
        /// Gets or sets the written date.
        /// </summary>
        public object WrittenDate
        {
            get => SelectedTitle?.Written;
            set
            {
                SelectedTitle?.SetWrittenDate(value);
                OnWrittenPropertiesChanged();
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
        /// Gets the preview text, the result of reading the latest document version.
        /// </summary>
        public string PreviewText
        {
            get => previewText;
            private set => SetProperty(ref previewText, value);
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleViewModel"/> class.
        /// </summary>
        public TitleViewModel()
        {
            Columns.Create("Id", TitleTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.W042);
            Columns.CreateResource<BooleanToPathConverter>("R", TitleTable.Defs.Columns.Ready, ResourceKeys.Icon.SquareSmallGreenIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipTitleFilterReady);

            Columns.CreateResource<BooleanToPathConverter>("Q", TitleTable.Defs.Columns.QuickFlag, ResourceKeys.Icon.SquareSmallBlueIconKey)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W028)
                .AddToolTip(Strings.ToolTipTitleFilterFlag);

            Columns.Create("Title", TitleTable.Defs.Columns.Title).MakeFlexWidth(4);

            Columns.SetDefaultSort(Columns.Create("Written", TitleTable.Defs.Columns.Written).MakeDate(), ListSortDirection.Descending);

            Columns.Create("Updated", TitleTable.Defs.Columns.Calculated.LastestVersionDate)
                .MakeDate()
                .AddToolTip(Strings.TooltipTitleUpdated);

            Columns.Create("WC", TitleTable.Defs.Columns.Calculated.LastestVersionWordCount).MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitleWordCount);

            Columns.Create("SC", TitleTable.Defs.Columns.Calculated.SubCount).MakeCentered().MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitleSubmissionCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("CS", TitleTable.Defs.Columns.Calculated.CurrentSubCount).MakeCentered().MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitleCurrentSubmissionCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("VC", TitleTable.Defs.Columns.Calculated.VersionCount).MakeCentered().MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitleVersionCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("TC", TitleTable.Defs.Columns.Calculated.TagCount).MakeCentered().MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitleTagCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("PC", TitleTable.Defs.Columns.Calculated.PublishedCount).MakeCentered().MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitlePublishedCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);
 
            Commands.Add("ReadyFilter", p => Filters.SetToReady());
            Commands.Add("FlaggedFilter", p => Filters.SetToFlagged());
            Commands.Add("SubmittedFilter", p => Filters.SetToSubmitted());
            Commands.Add("PublishedFilter", p => Filters.SetToPublished());
            Commands.Add("SelfPublishedFilter", p => Filters.SetToSelfPublished());
            Commands.Add("ExtractTitle", RunExtractTitle, CanRunExtractTitle);
            Commands.Add("ToggleFlag", RunToggleTitleFlagCommand, p => IsSelectedRowAccessible);
            Commands.Add("ClearFlags", RunClearTitleFlagsCommand);

            /* Context menu items */
            MenuItems.AddItem(Strings.MenuItemAddTitle, AddCommand).AddIconResource(ResourceKeys.Icon.PlusIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandOpenTitleOrDoubleClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);
            MenuItems.AddItem(Strings.CommandFlagTitle, Commands["ToggleFlag"]).AddIconResource(ResourceKeys.Icon.ToggleIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandDeleteTitle, DeleteCommand).AddIconResource(ResourceKeys.Icon.XRedIconKey);

            Versions = new TitleVersionController(this);
            Submissions = new TitleSubmissionController(this);
            Published = new TitlePublishedController(this);
            SelfPublished = new TitleSelfPublishedController(this);
            Tags = new TitleTagController(this);

            ListView.IsLiveSorting = true;
            ListView.LiveSortingProperties.Add(TitleTable.Defs.Columns.Written);

            Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                SelectedEditSection = 1;
                Filters.SetListView(ListView);
                Filters.ApplyFilter();
            }));
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called when this view model is activated.
        /// </summary>
        protected override void OnActivated()
        {
            Tags.RefreshAvailable();
            Tags.Update();
        }

        /// <summary>
        /// Called when the selected item on the associated data grid has changed.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            SelectedTitle = TitleRow.Create(SelectedRow);
            OnWrittenPropertiesChanged();
            Tags.Update();
            Versions.Update();
            Submissions.Update();
            Published.Update();
            SelfPublished.Update();
            PrepareForOpenXml();
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return Filters?.OnDataRowFilter(item) ?? false;
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            int value = DataRowCompareDateTime(item2, item1, TitleTable.Defs.Columns.Written);
            if (value == 0)
            {
                value = DataRowCompareString(item1, item2, TitleTable.Defs.Columns.Title);
            }
            return value;
        }

        /// <inheritdoc/>
        protected override void RunClearFilterCommand()
        {
            Filters.ClearAll();
        }

        /// <inheritdoc/>
        protected override bool CanRunClearFilterCommand()
        {
            return Filters.IsAnyFilterActive;
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            if (MessageWindow.ShowYesNo(Strings.ConfirmationAddTitle))
            {
                Table.AddDefaultRow();
                Table.Save();
                Filters.ClearAll();
                Columns.RestoreDefaultSort();
                ForceListViewSort();
            }
        }

        /// <summary>
        /// Called when the framework checks to see if Add command can execute
        /// </summary>
        /// <returns>This method always returns true.</returns>
        protected override bool CanRunAddCommand()
        {
            return true;
        }

        /// <summary>
        /// Runs the delete command to delete a record from the data table
        /// </summary>
        /// <remarks>
        /// Although this method can execute if a row is selected, if the title is participating in one or more
        /// submissions, it cannot be deleted.
        /// </remarks>
        protected override void RunDeleteCommand()
        {
            if (IsSelectedRowAccessible)
            {
                int childRowCount = SelectedRow.GetChildRows(TitleTable.Defs.Relations.ToSubmission).Length;
                if (childRowCount > 0)
                {
                    MessageWindow.ShowError(string.Format(CultureInfo.InvariantCulture, Strings.InvalidOpCannotDeleteTitle, childRowCount));
                    return;
                }

                if (MessageWindow.ShowYesNo(Strings.ConfirmationDeleteTitle))
                {
                    DeleteSelectedRow();
                }
            }
        }

        /// <summary>
        /// Called when the framework checks to see if Delete command can execute
        /// </summary>
        /// <returns>true if a row is accessible; otherwise, false.</returns>
        protected override bool CanRunDeleteCommand()
        {
            return IsSelectedRowAccessible;
        }

        /// <summary>
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> to open the latest version of the selected title.
        /// </summary>
        protected override void RunOpenRowCommand()
        {
            if (SelectedTitle != null)
            {
                Database.Tables.TitleVersionController verController = TitleVersionTable.GetVersionController(SelectedTitle.Id);
                if (verController.Versions.Count > 0)
                {
                    Open.TitleVersionFile(verController.Versions[0].FileName);
                }
            }
        }

        protected override bool CanRunOpenRowCommand()
        {
            return true;
        }

        /// <summary>
        /// Called when this VM is being removed from the workspaces.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            // TODO
            // Fires the filter's OnClosing in order that the filter can remove its event handlers.
            //Filters.CloseCommand.Execute(null);
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void RunExtractTitle(object o)
        {
            if (SelectedTitle != null)
            {
                Database.Tables.TitleVersionController verController = TitleVersionTable.GetVersionController(SelectedTitle.Id);
                if (verController.Versions.Count > 0)
                {
                    Execution.TryCatch(() =>
                        {
                            string fileName = Paths.Title.WithRoot(verController.Versions[0].FileName);
                            PropertiesAdapter props = OpenXmlDocument.Reader.GetProperties(fileName);
                            string title = props?.Core.Title;
                            if (string.IsNullOrWhiteSpace(title))
                            {
                                title = "(no title)";
                            }

                            if (MessageWindow.ShowYesNo(string.Format(CultureInfo.InvariantCulture, Strings.ConfirmationApplyExtractedTitleFormat, title)))
                            {
                                SelectedTitle.Title = title;
                                /* Needed to update the text box */
                                OnPropertyChanged(nameof(SelectedTitle));
                            }
                        });
                }
            }
        }

        private bool CanRunExtractTitle(object o)
        {
            if (SelectedTitle != null)
            {
                Database.Tables.TitleVersionController verController = TitleVersionTable.GetVersionController(SelectedTitle.Id);
                if (verController.Versions.Count > 0)
                {
                    return verController.Versions[0].DocType == DocumentTypeTable.Defs.Values.WordOpenXmlFileType;
                }
            }
            return false;
        }

        private void RunToggleTitleFlagCommand(object parm)
        {
            SelectedTitle?.ToggleQuickFlag();
        }

        private void RunClearTitleFlagsCommand(object parm)
        {
            if (Messages.ShowYesNo(Strings.ConfirmationClearTitleFlags))
            {
                foreach (TitleRow title in Table.EnumerateTitles())
                {
                    title.QuickFlag = false;
                }
            }
        }

        private void PrepareForOpenXml()
        {
            IsOpenXml = false;
            PreviewText = null;

            if (SelectedEditSection == SectionPreviewId && SelectedTitle != null)
            {
                Database.Tables.TitleVersionController verController = TitleVersionTable.GetVersionController(SelectedTitle.Id);
                if (verController.Versions.Count > 0)
                {
                    string fileName = Paths.Title.WithRoot(verController.Versions[0].FileName);
                    long docType = DatabaseController.Instance.GetTable<DocumentTypeTable>().GetDocTypeFromFileName(fileName);
                    if (docType == DocumentTypeTable.Defs.Values.WordOpenXmlFileType)
                    {
                        IsOpenXml = true;
                        Execution.TryCatch(() =>
                        {
                            PreviewText = OpenXmlDocument.Reader.GetText(fileName);
                        }, (ex) => MainWindowViewModel.Instance.CreateNotificationMessage(ex.Message));
                    }
                }
            }
        }

        private void OnWrittenPropertiesChanged()
        {
            OnPropertyChanged(nameof(WrittenHeader));
            OnPropertyChanged(nameof(WrittenDate));
        }
        #endregion
    }
}