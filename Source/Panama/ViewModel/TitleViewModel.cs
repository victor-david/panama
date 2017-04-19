﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Restless.App.Panama.Collections;
using Restless.App.Panama.Configuration;
using Restless.App.Panama.Controls;
using Restless.App.Panama.Converters;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.OpenXml;
using Restless.Tools.Search;
using Restless.Tools.Utility;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for titles management.
    /// </summary>
    public class TitleViewModel : DataGridViewModel<TitleTable>
    {
        #region Private
        private bool filterIsVisible;
        private string previewText;
        private const int PreviewTabIndex = 4;
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
        /// Gets a visibility value that determines if the title filter is visible.
        /// </summary>
        public Visibility TitleFilterVisibility
        {
            get
            {
                return (filterIsVisible) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets a string value that displays the written date.
        /// </summary>
        public string WrittenHeader
        {
            get
            {
                string dateStr = Strings.TextNone;
                if (SelectedRow != null && SelectedRow[TitleTable.Defs.Columns.Written] != DBNull.Value)
                {
                    dateStr = ((DateTime)SelectedRow[TitleTable.Defs.Columns.Written]).ToString(Config.Instance.DateFormat);
                }
                return String.Format("{0}: {1}", Strings.TextWritten, dateStr);
            }
        }

        /// <summary>
        /// Gets or sets the written date.
        /// </summary>
        public object WrittenDate
        {
            get
            {
                if (SelectedRow != null)
                {
                    return SelectedRow[TitleTable.Defs.Columns.Written];
                }
                return null;
            }
            set
            {
                if (SelectedRow != null)
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
        /// Gets the response date display name. Used to bind to the edit view
        /// Can't bind directly DisplayDate="{Binding SelectedRow[response]}"
        /// because when there isn't yet a response date, the calendar shows January, 0001
        /// </summary>
        public DateTime WrittenDisplayDate
        {
            get
            {
                DateTime displayDate = DateTime.UtcNow;
                if (SelectedRow != null && SelectedRow[TitleTable.Defs.Columns.Written] != DBNull.Value)
                {
                    displayDate = (DateTime)SelectedRow[TitleTable.Defs.Columns.Written];
                }
                return displayDate;
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
            get { return isOpenXml; }
            private set
            {
                isOpenXml = value;
                OnPropertyChanged("IsOpenXml");
            }
        }

        /// <summary>
        /// Gets the preview text, the result of reading the latest document version.
        /// </summary>
        public string PreviewText
        {
            get { return previewText; }
            private set
            {
                previewText = value;
                OnPropertyChanged("PreviewText");
            }
        }
        #endregion

        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public TitleViewModel()
        {
            DisplayName = Strings.CommandTitle;
            MaxCreatable = 1;
            Columns.Create("Id", TitleTable.Defs.Columns.Id).MakeFixedWidth(FixedWidth.Standard);
            Columns.CreateImage<BooleanToImageConverter>("R", TitleTable.Defs.Columns.Ready);
            Columns.Create("Title", TitleTable.Defs.Columns.Title).MakeFlexWidth(4);
            Columns.SetDefaultSort(Columns.Create("Written", TitleTable.Defs.Columns.Written).MakeDate(), ListSortDirection.Descending);

            Columns.Create("Updated", TitleTable.Defs.Columns.Calculated.LastestVersionDate).MakeDate()
                .AddToolTip(Strings.TooltipTitleUpdated);

            Columns.Create("WC", TitleTable.Defs.Columns.Calculated.LastestVersionWordCount).MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipTitleWordCount);

            Columns.Create("SC", TitleTable.Defs.Columns.Calculated.SubCount).MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipTitleSubmissionCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("CS", TitleTable.Defs.Columns.Calculated.CurrentSubCount).MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipTitleCurrentSubmissionCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("VC", TitleTable.Defs.Columns.Calculated.VersionCount).MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipTitleVersionCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("TC", TitleTable.Defs.Columns.Calculated.TagCount).MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipTitleTagCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            Columns.Create("PC", TitleTable.Defs.Columns.Calculated.PublishedCount).MakeFixedWidth(FixedWidth.Standard)
                .AddToolTip(Strings.TooltipTitlePublishedCount)
                .AddSort(null, TitleTable.Defs.Columns.Title, DataGridColumnSortBehavior.AlwaysAscending);

            AddViewSourceSortDescriptions();
            RawCommands.Add("TitleFilter", (o) =>
                {
                    filterIsVisible = !filterIsVisible;
                    OnPropertyChanged("TitleFilterVisibility");
                });

            /* This command is used from this model and from the Filters controller */
            RawCommands.Add("ClearFilter", (o) => { Filters.ClearAll(); }, (o) => { return Config.TitleFilter.IsAnyFilterActive; });
            RawCommands.Add("ExtractTitle", RunExtractTitle, CanRunExtractTitle);

            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandAddTitle, Strings.CommandAddTitleTooltip, AddCommand, ResourceHelper.Get("ImageAdd"), VisualCommandImageSize, VisualCommandFontSize));
            VisualCommands.Add(new VisualCommandViewModel(Strings.CommandClearFilter, Strings.CommandClearFilterTooltip, RawCommands["ClearFilter"], ResourceHelper.Get("ImageFilter"), VisualCommandImageSize, VisualCommandFontSize));

            /* Context menu items */
            MenuItems.AddItem(Strings.CommandOpenTitleOrDoubleClick, OpenRowCommand, "ImageOpenWordMenu");
            MenuItems.AddSeparator();
            MenuItems.AddItem(Strings.CommandDeleteTitle, DeleteCommand, "ImageDeleteMenu");

            Versions = new TitleVersionController(this);
            Submissions = new TitleSubmissionController(this);
            Published = new TitlePublishedController(this);
            Tags = new TitleTagController(this);

            //FilterPrompt = Strings.FilterPromptTitle;
            Filters = new TitleFilterController(this);
            Filters.Apply();
        }
        #pragma warning restore 1591
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
            PrepareForOpenXml();
        }

        /// <summary>
        /// Runs the add command to add a new record to the data table
        /// </summary>
        protected override void RunAddCommand()
        {
            Table.AddDefaultRow();
            Table.Save();
            FilterText = null;
            AddViewSourceSortDescriptions();
            Columns.RestoreDefaultSort();
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
                Messages.ShowError(String.Format(Strings.InvalidOpCannotDeleteTitle, childRowCount));
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
        /// <returns>true if a row is selected; otherwise, false.</returns>
        protected override bool CanRunDeleteCommand()
        {
            return CanRunCommandIfRowSelected(null);
        }

        /// <summary>
        /// Runs the <see cref="DataGridViewModel{T}.OpenRowCommand"/> to open the latest version of the selected title.
        /// </summary>
        /// <param name="item">The <see cref="DataRowView"/> object of the selected row.</param>
        protected override void RunOpenRowCommand(object item)
        {
            if (SelectedPrimaryKey != null)
            {
                Int64 titleId = (Int64)SelectedPrimaryKey;
                DataRow row = DatabaseController.Instance.GetTable<TitleVersionTable>().GetLastVersion(titleId);
                if (row != null)
                {
                    string fileName = Paths.Title.WithRoot(row[TitleVersionTable.Defs.Columns.FileName].ToString());
                    OpenHelper.OpenFile(fileName);
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
                Int64 titleId = (Int64)SelectedPrimaryKey;
                DataRow version = DatabaseController.Instance.GetTable<TitleVersionTable>().GetLastVersion(titleId);
                if (version != null)
                {
                    Execution.TryCatch(() =>
                        {
                            string fileName = Paths.Title.WithRoot(version[TitleVersionTable.Defs.Columns.FileName].ToString());
                            var props = OpenXmlDocument.Reader.GetProperties(fileName);
                            string title = (props != null) ? props.Core.Title : null;
                            if (String.IsNullOrWhiteSpace(title)) title = "(no title)";
                            if (Messages.ShowYesNo(String.Format(Strings.ConfirmationApplyExtractedTitleFormat, title)))
                            {
                                SelectedRow[TitleTable.Defs.Columns.Title] = title;
                                // the grid updates automatcially, but this is needed to update the text box.
                                OnPropertyChanged("SelectedRow");
                            }
                        });
                }
            }
        }

        private bool CanRunExtractTitle(object o)
        {
            if (SelectedPrimaryKey != null)
            {
                Int64 titleId = (Int64)SelectedPrimaryKey;
                DataRow version = DatabaseController.Instance.GetTable<TitleVersionTable>().GetLastVersion(titleId);
                if (version != null)
                {
                    return (Int64)version[TitleVersionTable.Defs.Columns.DocType] == DocumentTypeTable.Defs.Values.WordOpenXmlFileType;
                }
            }
            return false;
        }

        private void PrepareForOpenXml()
        {
            IsOpenXml = false;
            PreviewText = null;

            if (autoPreview && SelectedPrimaryKey != null)
            {
                Int64 titleId = (Int64)SelectedPrimaryKey;
                DataRow row = DatabaseController.Instance.GetTable<TitleVersionTable>().GetLastVersion(titleId);
                if (row != null)
                {
                    string fileName = Paths.Title.WithRoot(row[TitleVersionTable.Defs.Columns.FileName].ToString());
                    Int64 docType = DatabaseController.Instance.GetTable<DocumentTypeTable>().GetDocTypeFromFileName(fileName);
                    if (docType == DocumentTypeTable.Defs.Values.WordOpenXmlFileType)
                    {
                        IsOpenXml = true;
                        Execution.TryCatch(() =>
                        {
                            PreviewText = OpenXmlDocument.Reader.GetText(fileName);
                        }, (ex) => { MainViewModel.CreateNotificationMessage(ex.Message); });
                    }
                }
            }
        }

        private void OnWrittenPropertiesChanged()
        {
            OnPropertyChanged("WrittenHeader");
            OnPropertyChanged("WrittenDate");
            OnPropertyChanged("WrittenDisplayDate");
        }

        private void AddViewSourceSortDescriptions()
        {
            MainSource.SortDescriptions.Clear();
            MainSource.SortDescriptions.Add(new SortDescription(TitleTable.Defs.Columns.Written, ListSortDirection.Descending));
        }
        #endregion
    }
}