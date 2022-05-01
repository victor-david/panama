/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Controls;
using Restless.Panama.Core;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Panama.Resources;
using Restless.Panama.View;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Core.OpenXml;
using Restless.Toolkit.Core.Utility;
using Restless.Toolkit.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Windows.Threading;
using TableColumns = Restless.Panama.Database.Tables.TitleTable.Defs.Columns;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for titles management.
    /// </summary>
    public class TitleViewModel : DataRowViewModel<TitleTable>
    {
        #region Private
        private int selectedEditSection;
        private TitleRow selectedTitle;
        private PreviewMode previewMode;
        private string previewText;
        private const int SectionPreviewId = 6;
        #endregion

        /************************************************************************/

        #region Properties
        /// <inheritdoc/>
        public override bool AddCommandEnabled => true;

        /// <inheritdoc/>
        public override bool DeleteCommandEnabled => IsSelectedRowAccessible;

        /// <inheritdoc/>
        public override bool ClearFilterCommandEnabled => Filters.IsAnyFilterActive;

        /// <summary>
        /// Gets or sets the selected edit section
        /// </summary>
        public int SelectedEditSection
        {
            get => selectedEditSection;
            set
            {
                SetProperty(ref selectedEditSection, value);
                PrepareDocumentPreview();
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
        /// Gets the preview mode
        /// </summary>
        public PreviewMode PreviewMode
        {
            get => previewMode;
            private set => SetProperty(ref previewMode, value);
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
            Columns.Create("Id", TableColumns.Id)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042);

            Columns.Add(CreateFlagsColumn("Flags", GetFlagGridColumns())
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W076)
                .AddToolTip(TitleFlagsToolTip.Create(this))
                );

            Columns.Create("Title", TableColumns.Title).MakeFlexWidth(4);

            Columns.Create("Written", TableColumns.Written)
                .MakeDate()
                .MakeInitialSortDescending();

            Columns.Create("Updated", TableColumns.Calculated.LastestVersionDate)
                .MakeDate()
                .AddToolTip(Strings.TooltipTitleUpdated);

            Columns.Create("WC", TableColumns.Calculated.LastestVersionWordCount)
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitleWordCount)
                .SetSelectorName("Word Count");

            Columns.Create("SC", TableColumns.Calculated.SubCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitleSubmissionCount)
                .AddSort(null, TableColumns.Title, DataGridColumnSortBehavior.AlwaysAscending)
                .SetSelectorName("Total Submission Count");

            Columns.Create("CS", TableColumns.Calculated.CurrentSubCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitleCurrentSubmissionCount)
                .AddSort(null, TableColumns.Title, DataGridColumnSortBehavior.AlwaysAscending)
                .SetSelectorName("Current Submission Count");

            Columns.Create("VC", TableColumns.Calculated.VersionCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitleVersionCount)
                .AddSort(null, TableColumns.Title, DataGridColumnSortBehavior.AlwaysAscending)
                .SetSelectorName("Version Count");

            Columns.Create("TC", TableColumns.Calculated.TagCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitleTagCount)
                .AddSort(null, TableColumns.Title, DataGridColumnSortBehavior.AlwaysAscending)
                .SetSelectorName("Tag Count");

            Columns.Create("PC", TableColumns.Calculated.PublishedCount)
                .MakeCentered()
                .MakeFixedWidth(FixedWidth.W042)
                .AddToolTip(Strings.TooltipTitlePublishedCount)
                .AddSort(null, TableColumns.Title, DataGridColumnSortBehavior.AlwaysAscending)
                .SetSelectorName("Published Count");

            Columns.RestoreColumnState(Config.TitleGridColumnState);

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
            MenuItems.AddItem(Strings.MenuItemOpenTitleOrDoubleClick, OpenRowCommand).AddIconResource(ResourceKeys.Icon.ChevronRightIconKey);
            MenuItems.AddItem(Strings.MenuItemFlagTitle, Commands["ToggleFlag"]).AddIconResource(ResourceKeys.Icon.ToggleIconKey);
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.MenuItemDeleteTitle, DeleteCommand).AddIconResource(ResourceKeys.Icon.XRedIconKey);

            Versions = new TitleVersionController(this);
            Submissions = new TitleSubmissionController(this);
            Published = new TitlePublishedController(this);
            SelfPublished = new TitleSelfPublishedController(this);
            Tags = new TitleTagController(this);

            ListView.IsLiveSorting = true;
            ListView.LiveSortingProperties.Add(TableColumns.Written);

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
            base.OnActivated();
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
            SelectedTitle?.SetDateFormat(Config.DateFormat);
            Tags.Update();
            Versions.Update();
            Submissions.Update();
            Published.Update();
            SelfPublished.Update();
            PrepareDocumentPreview();
        }

        /// <inheritdoc/>
        protected override bool OnDataRowFilter(DataRow item)
        {
            return Filters?.OnDataRowFilter(item) ?? false;
        }

        /// <inheritdoc/>
        protected override int OnDataRowCompare(DataRow item1, DataRow item2)
        {
            int value = DataRowCompareDateTime(item2, item1, TableColumns.Written);
            if (value == 0)
            {
                value = DataRowCompareString(item1, item2, TableColumns.Title);
            }
            return value;
        }

        /// <inheritdoc/>
        protected override void RunClearFilterCommand()
        {
            Filters.ClearAll();
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
                ForceListViewSort();
            }
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
        /// Runs the <see cref="DataRowViewModel{T}.OpenRowCommand"/> to open the latest version of the selected title.
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

        /// <inheritdoc/>
        protected override void OnSave()
        {
            Config.TitleGridColumnState = Columns.GetColumnState();
        }

        /// <inheritdoc/>
        protected override void OnClosing()
        {
            base.OnClosing();
            SignalSave();
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void RunExtractTitle(object parm)
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

        private bool CanRunExtractTitle(object parm)
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

        private void PrepareDocumentPreview()
        {
            PreviewText = null;
            PreviewMode = PreviewMode.Unsupported;

            if (SelectedEditSection == SectionPreviewId && SelectedTitle != null)
            {
                Database.Tables.TitleVersionController verController = TitleVersionTable.GetVersionController(SelectedTitle.Id);
                if (verController.Versions.Count > 0)
                {
                    string fileName = Paths.Title.WithRoot(verController.Versions[0].FileName);
                    if (DocumentPreviewer.GetPreviewMode(fileName) == PreviewMode.Text)
                    {
                        PreviewMode = PreviewMode.Text;
                        PreviewText = DocumentPreviewer.GetText(fileName);
                    }
                }
            }
        }

        private FlagGridColumnCollection GetFlagGridColumns()
        {
            return new FlagGridColumnCollection()
            {
                { TableColumns.Ready, Config.Colors.TitleReady.ColorBrush },
                { TableColumns.QuickFlag, Config.Colors.TitleFlagged.ColorBrush },
                { TableColumns.Calculated.IsPublished, Config.Colors.TitlePublished.ColorBrush },
                { TableColumns.Calculated.IsSelfPublished, Config.Colors.TitleSelfPublished.ColorBrush },
                { TableColumns.Calculated.IsSubmitted, Config.Colors.TitleSubmitted.ColorBrush }
            };
        }
        #endregion
    }
}