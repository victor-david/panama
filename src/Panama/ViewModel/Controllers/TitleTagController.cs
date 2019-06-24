/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Core;
using Restless.App.Panama.Database;
using Restless.App.Panama.Database.Tables;
using Restless.Tools.Mvvm;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that manages the tags that may be assigned to a title.
    /// </summary>
    public class TitleTagController : TitleController
    {
        #region Private
        private TitleTagTable titleTagTable;
        private TagCacheDictionary tagCache;
        #endregion

        /************************************************************************/
        
        #region Public properties
        /// <summary>
        /// Gets a boolean value that indicates if there are no tags assigned to the associated title.
        /// </summary>
        public bool HasZeroTags
        {
            get { return Current.Count == 0; }
        }

        /// <summary>
        /// Gets the list of currently assigned tags. The edit control binds to this.
        /// </summary>
        public ObservableCollection<TagCommandViewModel> Current
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the list of available tags. The edit control binds to this.
        /// </summary>
        public ObservableCollection<TagCommandViewModel> Available
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleTagController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleTagController(TitleViewModel owner)
            : base(owner)
        {
            Current = new ObservableCollection<TagCommandViewModel>();
            Available = new ObservableCollection<TagCommandViewModel>();

            titleTagTable = DatabaseController.Instance.GetTable<TitleTagTable>();
            tagCache = new TagCacheDictionary();
            RefreshAvailable();
        }
        #endregion

        /************************************************************************/
        
        #region Public methods

        /// <summary>
        /// Causes the list of available tags to be refreshed
        /// </summary>
        public void RefreshAvailable()
        {
            tagCache.Clear();
            Available.Clear();
            var tagTable = DatabaseController.Instance.GetTable<TagTable>();
            foreach (DataRow row in tagTable.Rows)
            {
                long tagId = (long)row[TagTable.Defs.Columns.Id];
                ICommand cmd = RelayCommand.Create((o) => { RunTagAddCommand(tagId); }, (o) => CanRunTagAddCommand(tagId));
                Available.Add(new TagCommandViewModel(tagId, row[TagTable.Defs.Columns.Tag].ToString(), row[TagTable.Defs.Columns.Description].ToString(), cmd));
                tagCache.Add(row);
            }
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Called by the <see cref=" ControllerBase{VM,T}.Owner"/> of this controller
        /// in order to update the controller values.
        /// </summary>
        protected override void OnUpdate()
        {
            long titleId = GetOwnerSelectedPrimaryId();
            Current.Clear();
            titleTagTable.DefaultView.RowFilter = string.Format("{0}={1}", TitleTagTable.Defs.Columns.TitleId, titleId);
            titleTagTable.DefaultView.Sort = TitleTagTable.Defs.Columns.Joined.TagName;
            foreach (DataRowView dataRowView in titleTagTable.DefaultView)
            {
                long tagId = (long)dataRowView[TitleTagTable.Defs.Columns.TagId];
                ICommand cmd = RelayCommand.Create((o) => { RunTagRemoveCommand(tagId); }, (o) => CanRunTagRemoveCommand(tagId));
                Current.Add(new TagCommandViewModel(tagId, tagCache[tagId].Name, tagCache[tagId].Description, cmd));
            }
            OnPropertyChanged(nameof(HasZeroTags));
        }
        #endregion

        /************************************************************************/

        #region Private methods

        private void RunTagAddCommand(long tagId)
        {
            long titleId = GetOwnerSelectedPrimaryId();
            if (titleId != long.MinValue)
            {
                titleTagTable.Add(tagId, titleId);
                titleTagTable.Save();
                OnUpdate();
            }
        }

        private bool CanRunTagAddCommand(long tagId)
        {
            long titleId = GetOwnerSelectedPrimaryId();
            return !titleTagTable.TagExists(tagId, titleId);
        }

        private void RunTagRemoveCommand(long tagId)
        {
            long titleId = GetOwnerSelectedPrimaryId();
            if (titleId != long.MinValue)
            {
                titleTagTable.Remove(tagId, titleId);
                titleTagTable.Save();
                OnUpdate();
                if (Config.TitleFilter.IsTagFilterActive)
                {
                    Config.TitleFilter.Update();
                }
            }
        }

        private bool CanRunTagRemoveCommand(long tagId)
        {
            return true;
        }
        #endregion
    }
}