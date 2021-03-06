/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Core;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.App.Panama.Resources;
using Restless.Tools.Mvvm;
using Restless.Tools.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Input;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that manages the filter for the list of titles
    /// </summary>
    public class TitleFilterController : TitleController
    {
        #region Private
        private TitleTagTable titleTagTable;
        private SubmissionBatchTable submissionBatchTable;
        private SubmissionTable submissionTable;
        private StringBuilder filter;
        private StringBuilder filterDesc;
        private string wordCountText;
        private bool applyFilterPaused;
        private bool trimNextFilterDesc;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets a short description of the number of records in the data view
        /// </summary>
        public string RecordCountText
        {
            get { return Format.Plural(Owner.DataView.Count, Strings.TextRecord, Strings.TextRecords); }
        }

        /// <summary>
        /// Gets the friendly description of the current filter
        /// </summary>
        public string Description
        {
            get
            {
                if (filterDesc.Length == 0)
                {
                    return "(none)";
                }
                return filterDesc.ToString();
            }
        }

        /// <summary>
        /// Gets a string that describes the state of the word count portion of the filter.
        /// </summary>
        public string WordCountText
        {
            get { return wordCountText; }
            private set
            {
                SetProperty(ref wordCountText, value);
            }
        }

        /// <summary>
        /// Gets the list of available tags. The edit control binds to this.
        /// </summary>
        public TagCommandViewModelCollection Available
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleFilterController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleFilterController(TitleViewModel owner)
            : base(owner)
        {
            Available = new TagCommandViewModelCollection();
            titleTagTable = DatabaseController.Instance.GetTable<TitleTagTable>();
            submissionBatchTable = DatabaseController.Instance.GetTable<SubmissionBatchTable>();
            submissionTable = DatabaseController.Instance.GetTable<SubmissionTable>();

            InitAvailable();
            InitSelected();

            filter = new StringBuilder(512);
            filterDesc = new StringBuilder(512);

            Commands.Add("SelectFolder", RunSelectFolderCommand);
            Commands.Add("ClearFolder", RunClearFolderCommand, CanRunClearFolderCommand);
            Commands.Add("ClearFilterTags", (o) => { ClearTags(); }, (o) => { return Config.TitleFilter.IsTagFilterActive; });
            Config.TitleFilter.Changed += TitleFilterChanged;
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Applies the filter options.
        /// </summary>
        public void Apply()
        {
            if (!applyFilterPaused)
            {
                ApplyPrivate();
            }
        }

        /// <summary>
        /// Sets the filter options to ready only.
        /// </summary>
        public void SetToReady()
        {
            ClearAndApplyAction(()=> Config.TitleFilter.Ready = FilterState.Yes);
        }

        /// <summary>
        /// Sets the filter options to ready only.
        /// </summary>
        public void SetToFlagged()
        {
            ClearAndApplyAction(() => Config.TitleFilter.Flagged = FilterState.Yes);
        }

        /// <summary>
        /// Sets the filter options to submitted only.
        /// </summary>
        public void SetToSubmitted()
        {
            ClearAndApplyAction(() => Config.TitleFilter.Submitted = FilterState.Yes);
        }

        /// <summary>
        /// Sets the filter options to published only.
        /// </summary>
        public void SetToPublished()
        {
            ClearAndApplyAction(() => Config.TitleFilter.Published = FilterState.Yes);
        }

        /// <summary>
        /// Sets the filter options to self published only.
        /// </summary>
        public void SetToSelfPublished()
        {
            ClearAndApplyAction(() => Config.TitleFilter.SelfPublished = FilterState.Yes);
        }

        /// <summary>
        /// Clears the tags portion of the filters.
        /// </summary>
        public void ClearTags()
        {
            Config.Instance.TitleFilter.Tags.Clear();
            foreach (var tagCmd in Available)
            {
                tagCmd.ResetDefaultForeground();
            }
            Config.TitleFilter.Update();
        }

        /// <summary>
        /// Clears all filter options.
        /// </summary>
        public void ClearAll()
        {
            ClearAndApplyAction(null);
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Implemented but not used
        /// </summary>
        protected override void OnUpdate()
        {
        }

        /// <summary>
        /// Called when this controller is closing.
        /// The owner triggers this event by calling the Excecute method of its controller's <see cref="ApplicationViewModel.CloseCommand"/>.
        /// </summary>
        /// <param name="e">The event args</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Config.TitleFilter.Changed -= TitleFilterChanged;
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void ClearAndApplyAction(Action action)
        {
            applyFilterPaused = true;
            Config.Instance.TitleFilter.Reset();
            ClearTags();
            action?.Invoke();
            applyFilterPaused = false;
            Apply();
        }

        /// <summary>
        /// Initializes the list of available tags
        /// </summary>
        private void InitAvailable()
        {
            //tagCache.Clear();
            Available.Clear();
            var tagTable = DatabaseController.Instance.GetTable<TagTable>();
            foreach (DataRow row in tagTable.Rows)
            {
                long tagId = (long)row[TagTable.Defs.Columns.Id];
                ICommand cmd = RelayCommand.Create((o) => { RunToggleTagCommand(tagId); });
                Available.Add(new TagCommandViewModel(tagId, row[TagTable.Defs.Columns.Tag].ToString(), row[TagTable.Defs.Columns.Description].ToString(), cmd));
            }
        }

        /// <summary>
        /// Initializes the list of selected tags.
        /// </summary>
        private void InitSelected()
        {
            List<long> bads = new List<long>();
            //filterTags.AddValuesFromDelimitedString(Owner.Config.TitleFilter.Tags);
            /* Tags may have been deleted since we last saved the list */
            foreach (long tagId in Config.TitleFilter.Tags)
            {
                if (Available.GetItem(tagId) == null)
                {
                    bads.Add(tagId);
                }
            }

            foreach (long tagId in bads)
            {
                Config.TitleFilter.Tags.Remove(tagId);
            }

            foreach (long tagId in Config.TitleFilter.Tags)
            {
                var item = Available.GetItem(tagId);
                if (item != null)
                {
                    item.Highlight();
                }
            }
        }

        private void ApplyPrivate()
        {
            //Debug.WriteLine("Apply title filter");
            filter.Clear();
            filterDesc.Clear();
            var f = Config.TitleFilter;
            if (!string.IsNullOrEmpty(f.Text))
            {
                Append(
                    string.Format("{0} LIKE '%{1}%'", TitleTable.Defs.Columns.Title, f.Text.Replace("'", "''")),
                    string.Format("Title contains {0}", f.Text), 2);
            }

            if (!string.IsNullOrEmpty(f.Folder))

            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(
                    string.Format("{0} LIKE '%{1}%'", TitleTable.Defs.Columns.Calculated.LastestVersionPath, f.Folder.Replace("'", "''")),
                    string.Format("Folder contains {0}", f.Folder), 2);
            }

            if (f.Ready != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(string.Format("{0}={1}", TitleTable.Defs.Columns.Ready, (byte)f.Ready), (f.Ready == FilterState.No ? "not ready" : "ready"));
            }

            if (f.Flagged != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(string.Format("{0}={1}", TitleTable.Defs.Columns.QuickFlag, (byte)f.Flagged), (f.Flagged == FilterState.No ? "not flagged" : "flagged"));
            }

            if (f.Submitted != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");

                if (f.Submitted == FilterState.No)
                {
                    Append(string.Format("ISNULL({0},0) = 0", TitleTable.Defs.Columns.Calculated.CurrentSubCount), "not currently submitted");
                }
                else
                {
                    Append(string.Format("{0} > 0", TitleTable.Defs.Columns.Calculated.CurrentSubCount), "currently submitted");
                }
            }

            if (f.EverSubmitted != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                if (f.EverSubmitted == FilterState.No)
                {
                    Append(string.Format("{0}=0", TitleTable.Defs.Columns.Calculated.SubCount), "not ever submitted");
                }
                else
                {
                    Append(string.Format("{0}>0", TitleTable.Defs.Columns.Calculated.SubCount), "ever submitted");
                }
            }

            if (f.Published != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(string.Format("{0}={1}", TitleTable.Defs.Columns.Calculated.IsPublished, (byte)f.Published), f.Published == FilterState.No ? "not published" : "published");
            }

            if (f.SelfPublished != FilterState.Either)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(string.Format("{0}={1}", TitleTable.Defs.Columns.Calculated.IsSelfPublished, (byte)f.SelfPublished), f.SelfPublished == FilterState.No ? "not self published" : "self published");
            }

            if (f.WordCount != 0)
            {
                int wordCount = Math.Abs(f.WordCount);
                if (filter.Length > 0) Append(" AND ", " and ");
                if (f.WordCount > 0)
                {
                    Append(string.Format("{0}>{1}", TitleTable.Defs.Columns.Calculated.LastestVersionWordCount, wordCount), string.Format("Word count > {0}", wordCount));
                    WordCountText = string.Format("Greater than {0}", wordCount);
                }
                if (f.WordCount < 0)
                {
                    Append(string.Format("{0}<{1} AND {0} <> 0", TitleTable.Defs.Columns.Calculated.LastestVersionWordCount, wordCount), string.Format("Word count < {0}", wordCount));
                    WordCountText = string.Format("Less than {0} (but not zero)", wordCount);
                }
            }
            else
            {
                WordCountText = "(any)";
            }

            if (f.Id != null)
            {
                if (filter.Length > 0) Append(" AND ", " and ");
                Append(string.Format("{0}={1}", TitleTable.Defs.Columns.Id, f.Id), string.Format("Title id equals {0}", f.Id));
            }

            if (f.IsTagFilterActive)
            {
                if (filter.Length > 0)
                {
                    Append(string.Empty, string.Empty, 2);
                }
                Append(string.Empty, "With tags: ");
                bool tagsOnly = true;
                if (filter.Length > 0)
                {
                    Append(" AND (", string.Empty);
                    tagsOnly = false;
                }

                for (int k = 0; k < f.Tags.Count; k++ )
                {
                    long tagId = f.Tags[k];
                    string titleIds = titleTagTable.GetTitleIdsForTag(tagId);
                    if (k > 0)
                    {
                        switch (f.TagCombine)
                        {
                            case TagFilterCombine.Or:
                                Append(" OR ", " or ");
                                break;
                            case TagFilterCombine.And:
                                Append(" AND ", " and ");
                                break;
                            case TagFilterCombine.AndNot:
                                Append(" AND NOT ", " and not ");
                                break;
                        }
                    }
                    var tagItem = Available.GetItem(tagId);
                    string tagName = (tagItem != null) ? tagItem.TagName : "???";
                    Append(string.Format("{0} IN ({1})", TitleTable.Defs.Columns.Id, titleIds), tagName);
                }
                if (!tagsOnly)
                {
                    Append(")", "");
                }
            }

            Owner.DataView.RowFilter = filter.ToString();
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(RecordCountText));
        }

        private void Append(string filterExpression, string filterDescription, int linesToAppend = 0)
        {
            filter.Append(filterExpression);
            if (trimNextFilterDesc)
            {
                filterDescription = filterDescription.TrimStart(' ');
            }
            filterDesc.Append(filterDescription);
            for (int k = 0; k < linesToAppend; k++)
            {
                filterDesc.AppendLine();
            }
            trimNextFilterDesc = linesToAppend > 0;
        }

        private void RunToggleTagCommand(long tagId)
        {
            var item = Available.GetItem(tagId);
            if (item != null)
            {
                if (Config.TitleFilter.Tags.Contains(tagId))
                {
                    Config.TitleFilter.Tags.Remove(tagId);
                    item.ResetDefaultForeground();
                }
                else
                {
                    Config.TitleFilter.Tags.Add(tagId);
                    item.Highlight();
                }
                Config.TitleFilter.Update();
            }
        }

        private void RunSelectFolderCommand(object o)
        {
            using (var diag = CommonDialogFactory.Create(Config.FolderTitleRoot, Strings.CaptionSelectTitleFilterFolder, true))
            {
                if (diag.ShowDialog() == Microsoft.WindowsAPICodePack.Dialogs.CommonFileDialogResult.Ok)
                {
                    Config.TitleFilter.Folder = Paths.Title.WithoutRoot(diag.FileName);
                }
            }
        }

        private void RunClearFolderCommand(object o)
        {
            Config.TitleFilter.Folder = null;
        }

        private bool CanRunClearFolderCommand(object o)
        {
            return !string.IsNullOrEmpty(Config.Instance.TitleFilter.Folder);
        }

        private void TitleFilterChanged(object sender, EventArgs e)
        {
            Apply();
        }
        #endregion
    }
}