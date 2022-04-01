/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Controls;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that manages the tags that may be assigned to a title.
    /// </summary>
    public class TitleTagController : BaseController<TitleViewModel, TitleTable>
    {
        #region Private
        private long selectedTitleId;
        #endregion

        /************************************************************************/

        #region Properties
        private TagTable TagTable => DatabaseController.Instance.GetTable<TagTable>();
        private TitleTagTable TitleTagTable => DatabaseController.Instance.GetTable<TitleTagTable>();

        /// <summary>
        /// The id of the tag selector for assigned tags.
        /// </summary>
        public const int AssignedSelectorId = 1000;

        /// <summary>
        /// The id of the tag selector for available tags.
        /// </summary>
        public const int AvailableSelectorId = 1001;

        /// <summary>
        /// Gets a boolean value that indicates if there is at least one tag assigned to the associated title.
        /// </summary>
        public bool HasTags => Assigned.Count > 0;

        /// <summary>
        /// Gets the list of available tags.
        /// </summary>
        public TagSelectorItemCollection Available
        {
            get;
        }

        /// <summary>
        /// Gets the list of currently assigned tags.
        /// </summary>
        public TagSelectorItemCollection Assigned
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleTagController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public TitleTagController(TitleViewModel owner) : base(owner)
        {
            Available = new TagSelectorItemCollection();
            Assigned = new TagSelectorItemCollection();
            Commands.Add("TagItemClick", RunTagSelectorItemClickedCommand);
        }
        #endregion

        /************************************************************************/
        
        #region Public methods
        /// <summary>
        /// Causes the list of available tags to be refreshed
        /// </summary>
        /// <remarks>
        /// This method is called by <see cref="TitleViewModel"/> upon its activation
        /// because the user may have updated / added tags.
        /// </remarks>
        public void RefreshAvailable()
        {
            Available.Clear();
            foreach (TagRow tagRow in TagTable.EnumerateAll())
            {
                Available.Add(GetTagSelectorItem(tagRow));
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
            selectedTitleId = Owner.SelectedTitle?.Id ?? 0;
            Assigned.Clear();
            Available.EnableAll();
            foreach (TitleTagRow tagRow in TitleTagTable.EnumerateAll(selectedTitleId))
            {
                Assigned.Add(GetTagSelectorItem(tagRow));
                Available.GetItem(tagRow.TagId)?.Disable();
            }
            OnPropertyChanged(nameof(HasTags));
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private static TagSelectorItem GetTagSelectorItem(TagRow tagRow)
        {
            return new TagSelectorItem()
            {
                Id = tagRow.Id,
                Content = tagRow.Tag,
                ToolTip = tagRow.Description
            };
        }

        private static TagSelectorItem GetTagSelectorItem(TitleTagRow tagRow)
        {
            return new TagSelectorItem()
            {
                Id = tagRow.TagId,
                Content = tagRow.TagName,
                ToolTip = tagRow.TagDescription,
            };
        }

        private void RunTagSelectorItemClickedCommand(object parm)
        {
            if (parm is TagSelectorParm tagParm)
            {
                if (tagParm.SelectorControlId == AvailableSelectorId)
                {
                    AddTag(tagParm);
                }
                else
                {
                    RemoveTag(tagParm);
                }
                OnPropertyChanged(nameof(HasTags));
            }
        }

        private void AddTag(TagSelectorParm tagParm)
        {
            if (TitleTagTable.AddIfNotExist(selectedTitleId, tagParm.TagSelectorItem.Id))
            {
                if (Assigned.GetItem(tagParm.TagSelectorItem.Id) == null)
                {
                    Assigned.Add(tagParm.TagSelectorItem.Disable().Clone());
                }
            }
        }

        private void RemoveTag(TagSelectorParm tagParm)
        {
            if (TitleTagTable.RemoveIfExist(selectedTitleId, tagParm.TagSelectorItem.Id))
            {
                Assigned.Remove(tagParm.TagSelectorItem);
                Available.GetItem(tagParm.TagSelectorItem.Id)?.Enable();
            }
        }
        #endregion
    }
}