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
using System.Windows;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for titles management.
    /// </summary>
    public class TitleViewModel : DataGridViewModel<TitleTable>
    {
        #region Private
        private bool isFilterVisible;
        private string previewText;
        private const int PreviewTabIndex = 5;
        private bool autoPreview;
        private bool isOpenXml;
        #endregion

        /************************************************************************/

        #region Properties
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
        /// Gets the controller for the advanced title filters.
        /// </summary>
        public TitleFilterController Filters
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets an enumerable of <see cref="AuthorTable.RowObject"/> items. The UI binds to this list.
        /// </summary>
        public IEnumerable<AuthorTable.RowObject> Authors
        {
            get => DatabaseController.Instance.GetTable<AuthorTable>().EnumerateAuthors();
        }

        /// <summary>
        /// Gets or sets the selected author item.
        /// </summary>
        public AuthorTable.RowObject SelectedAuthor
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a visibility value that determines if the title filter is visible.
        /// </summary>
        public Visibility FilterVisibility
        {
            get => (isFilterVisible) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Gets a string value that displays the written date.
        /// </summary>
        public string WrittenHeader
        {
            get
            {
                string dateStr = Strings.TextNone;
                if (IsSelectedRowAccessible && SelectedRow[TitleTable.Defs.Columns.Written] is DateTime dt)
                {
                    dateStr = dt.ToLocalTime().ToString(Config.Instance.DateFormat);
                }
                return $"{Strings.TextWritten}: {dateStr}";
            }
        }
        /// <summary>
        /// Gets or sets the written date.
        /// </summary>
        public object WrittenDate
        {
            get
            {
                if (IsSelectedRowAccessible)
                {
                    return SelectedRow[TitleTable.Defs.Columns.Written];
                }
                return null;
            }
            set
            {
                if (IsSelectedRowAccessible)
                {
                    if (value != null)
                    {
                        SelectedRow[TitleTable.Defs.Columns.Written] = value;
                    }
                    else
                    {
                        SelectedRow[TitleTable.Defs.Columns.Written] = DBNull.Value;
                    }
                    OnWrittenPropertiesChanged();
                }
            }
        }

        /// <summary>
        /// Sets the selected tab index. This property is bound to the UI, Mode=OneWayToSource.
        /// We use the selected tab item to perform auto preview of the title.
        /// </summary>
        public int SelectedTabIndex
        {
            set
            {
                autoPreview = (value == PreviewTabIndex);
                PrepareForOpenXml();
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
            DisplayName = Strings.CommandTitle;
            Columns.Create("Id", TitleTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.CreateImage<BooleanToImageConverter>("R", TitleTable.Defs.Columns.Ready).AddToolTip(Strings.TooltipTitleReady);
            Columns.CreateImage<BooleanToImageConverter>("Q", TitleTable.Defs.Columns.QuickFlag, "ImageExclamation").AddToolTip(Strings.TooltipTitleQuickFlag);

            Columns.Create("Title", TitleTable.Defs.Columns.Title).MakeFlexWidth(4);

            var col = Columns.Create("Written", TitleTable.Defs.Columns.Written).MakeDate();

            Columns.SetDefaultSort(col, ListSortDirection.Descending);

            Columns.Create("Updated", TitleTable.Defs.Columns.Calculated.LastestVersionDate)
                .MakeDate()
                .AddToolTip(Strings.TooltipTitleUpdated);

            Columns.Create("WC", TitleTable.Defs.Columns.Calculated.LastestVersionWordCount).MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipTitleWordCount);

            Columns.Create("SC", TitleTable.Defs.Columns.Calculated.SubCount).MakeCentered().MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipTitleSubmissionCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("CS", TitleTable.Defs.Columns.Calculated.CurrentSubCount).MakeCentered().MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipTitleCurrentSubmissionCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("VC", TitleTable.Defs.Columns.Calculated.VersionCount).MakeCentered().MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipTitleVersionCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("TC", TitleTable.Defs.Columns.Calculated.TagCount).MakeCentered().MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipTitleTagCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("PC", TitleTable.Defs.Columns.Calculated.PublishedCount).MakeCentered().MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipTitlePublishedCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            AddViewSourceSortDescriptions();

            /* This command is used from this model and from the Filters controller */
            Commands.Add("ClearFilter", (o) => Filters.ClearAll(), (o) => Config.TitleFilter.IsAnyFilterActive);
            Commands.Add("ReadyFilter", (o) => Filters.SetToReady());
            Commands.Add("FlaggedFilter", (o) => Filters.SetToFlagged());
            Commands.Add("SubmittedFilter", (o) => Filters.SetToSubmitted());
            Commands.Add("PublishedFilter", (o) => Filters.SetToPublished());
            Commands.Add("SelfPublishedFilter", (o) => Filters.SetToSelfPublished());
            // TODO
            //Commands.Add("AdvancedFilter", (o) =>
            //{
            //    isFilterVisible = !isFilterVisible;
            //    advFilter.Icon = (isFilterVisible) ? LocalResources.Get("ImageChevronUp") : LocalResources.Get("ImageChevronDown");
            //    OnPropertyChanged(nameof(FilterVisibility));
            //});

            Commands.Add("ExtractTitle", RunExtractTitle, CanRunExtractTitle);
            Commands.Add("ToggleFlag", RunToggleTitleFlagCommand, (p) => IsSelectedRowAccessible);
            Commands.Add("ClearFlags", RunClearTitleFlagsCommand);


            // TODO
            //double minWidth = 80.0;
            //double imgSize = 20.0;
            //advFilter = new VisualCommandViewModel(Strings.CommandFilterAdvanced, Strings.CommandFilterAdvancedTooltip, Commands["AdvancedFilter"], LocalResources.Get("ImageChevronDown"), imgSize, VisualCommandFontSize, 100.0);
            //FilterCommands.Add(advFilter);
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandTitleFilterReady, Strings.CommandTitleFilterReadyTooltip, Commands["ReadyFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandTitleFilterFlagged, Strings.CommandTitleFilterFlaggedTooltip, Commands["FlaggedFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandTitleFilterSubmitted, Strings.CommandTitleFilterSubmittedTooltip, Commands["SubmittedFilter"], null, imgSize, VisualCommandFontSize,  minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandTitleFilterPublished, Strings.CommandTitleFilterPublishedTooltip, Commands["PublishedFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandTitleFilterSelfPublished, Strings.CommandTitleFilterSelfPublishedTooltip, Commands["SelfPublishedFilter"], null, imgSize, VisualCommandFontSize, minWidth));
            //FilterCommands.Add(new VisualCommandViewModel(Strings.CommandClearFilter, Strings.CommandClearFilterTooltip, Commands["ClearFilter"], null, imgSize, VisualCommandFontSize, minWidth));

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandOpenTitleOrDoubleClick, OpenRowCommand).AddImageResource("ImageOpenWordMenu");
            MenuItems.AddItem(Strings.CommandFlagTitle, Commands["ToggleFlag"]).AddImageResource("ImageExclamationMenu");
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandDeleteTitle, DeleteCommand).AddImageResource("ImageDeleteMenu");

            Versions = new TitleVersionController(this);
            Submissions = new TitleSubmissionController(this);
            Published = new TitlePublishedController(this);
            SelfPublished = new TitleSelfPublishedController(this);
            Tags = new TitleTagController(this);

            Filters = new TitleFilterController(this);
            Filters.Apply();
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Called when the selected item on the associated data grid has changed.
        /// </summary>
        protected override void OnSelectedItemChanged()
        {
            base.OnSelectedItemChanged();
            OnWrittenPropertiesChanged();
            Tags.Update();
            Versions.Update();
            Submissions.Update();
            Published.Update();
            SelfPublished.Update();
            PrepareForOpenXml();
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            if (Messages.ShowYesNo(Strings.ConfirmationAddTitle))
            {
                Table.AddDefaultRow();
                Table.Save();
                Filters.ClearAll();
                AddViewSourceSortDescriptions();
                Columns.RestoreDefaultSort();
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
            int childRowCount = SelectedRow.GetChildRows(TitleTable.Defs.Relations.ToSubmission).Length;
            if (childRowCount > 0)
            {
                Messages.ShowError(string.Format(Strings.InvalidOpCannotDeleteTitle, childRowCount));
                return;
            }
            if (Messages.ShowYesNo(Strings.ConfirmationDeleteTitle))
            {
                SelectedRow.Delete();
                Table.Save();
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
        /// <param name="item">The <see cref="DataRowView"/> object of the selected row.</param>
        protected override void RunOpenRowCommand(object item)
        {
            if (SelectedPrimaryKey != null)
            {
                long titleId = (long)SelectedPrimaryKey;
                var verController = DatabaseController.Instance.GetTable<TitleVersionTable>().GetVersionController(titleId);

                if (verController.Versions.Count > 0)
                {
                    OpenHelper.OpenFile(Paths.Title.WithRoot(verController.Versions[0].FileName));
                }
            }
        }

        /// <summary>
        /// Called when this VM is being removed from the workspaces.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            // Fires the filter's OnClosing in order that the filter can remove its event handlers.
            Filters.CloseCommand.Execute(null);
        }
        #endregion

        /************************************************************************/

        #region Private Methods

        private void RunExtractTitle(object o)
        {
            if (SelectedPrimaryKey != null)
            {
                long titleId = (long)SelectedPrimaryKey;
                var verController = DatabaseController.Instance.GetTable<TitleVersionTable>().GetVersionController(titleId);
                if (verController.Versions.Count > 0)
                {
                    Execution.TryCatch(() =>
                        {
                            string fileName = Paths.Title.WithRoot(verController.Versions[0].FileName);
                            var props = OpenXmlDocument.Reader.GetProperties(fileName);
                            string title = props?.Core.Title;
                            if (string.IsNullOrWhiteSpace(title)) title = "(no title)";
                            if (Messages.ShowYesNo(string.Format(Strings.ConfirmationApplyExtractedTitleFormat, title)))
                            {
                                SelectedRow[TitleTable.Defs.Columns.Title] = title;
                                // the grid updates automatcially, but this is needed to update the text box.
                                OnPropertyChanged(nameof(SelectedRow));
                            }
                        });
                }
            }
        }

        private bool CanRunExtractTitle(object o)
        {
            if (SelectedPrimaryKey != null)
            {
                long titleId = (long)SelectedPrimaryKey;
                var verController = DatabaseController.Instance.GetTable<TitleVersionTable>().GetVersionController(titleId);
                if (verController.Versions.Count > 0)
                {
                    return verController.Versions[0].DocType == DocumentTypeTable.Defs.Values.WordOpenXmlFileType;
                }
            }
            return false;
        }

        private void RunToggleTitleFlagCommand(object parm)
        {
            if (IsSelectedRowAccessible)
            {
                var obj = new TitleTable.RowObject(SelectedRow);
                obj.QuickFlag = !obj.QuickFlag;
            }
        }

        private void RunClearTitleFlagsCommand(object parm)
        {
            if (Messages.ShowYesNo(Strings.ConfirmationClearTitleFlags))
            {
                foreach (var title in DatabaseController.Instance.GetTable<TitleTable>().EnumerateTitles())
                {
                    title.QuickFlag = false;
                }
            }
        }

        private void PrepareForOpenXml()
        {
            IsOpenXml = false;
            PreviewText = null;

            if (autoPreview && SelectedPrimaryKey != null)
            {
                long titleId = (long)SelectedPrimaryKey;
                var verController = DatabaseController.Instance.GetTable<TitleVersionTable>().GetVersionController(titleId);
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

        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(TitleTable.Defs.Columns.Written, ListSortDirection.Descending));
        }
        #endregion
    }
}