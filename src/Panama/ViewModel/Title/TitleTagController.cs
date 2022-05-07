/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Controls;
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Mvvm;
using System;
using System.Windows.Input;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides a controller that manages the tags that may be assigned to a title.
    /// </summary>
    public class TitleTagController : ObservableObject
    {
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
        /// Gets a boolean value that indicates if there is at least one tag in <see cref="Assigned"/>
        /// </summary>
        public bool HasAssignedTags => Assigned.Count > 0;

        /// <summary>
        /// Gets the <see cref="TitleViewModel"/> that owns this controller
        /// </summary>
        protected TitleViewModel Owner
        {
            get;
        }

        /// <summary>
        /// Gets the collection of available tags.
        /// </summary>
        public TagSelectorItemCollection Available
        {
            get;
        }

        /// <summary>
        /// Gets the collection of currently assigned tags.
        /// </summary>
        public TagSelectorItemCollection Assigned
        {
            get;
        }

        /// <summary>
        /// Gets the command to execute when a tag item is clicked
        /// </summary>
        public ICommand TagItemClickCommand
        {
            get;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleTagController"/> class
        /// </summary>
        public TitleTagController(TitleViewModel owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Available = new TagSelectorItemCollection();
            Assigned = new TagSelectorItemCollection();
            TagItemClickCommand = RelayCommand.Create(RunTagItemClickCommand);
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Refreshes the available tag collection
        /// </summary>
        public void RefreshAvailable()
        {
            Available.Clear();
            foreach (TagRow tagRow in TagTable.EnumerateAll())
            {
                Available.Add(GetTagSelectorItem(tagRow));
            }
        }

        /// <summary>
        /// Enables all entries in <see cref="Available"/> 
        /// and assigns entries to <see cref="Assigned"/>.
        /// </summary>
        public virtual void PopulateAssigned()
        {
            Assigned.Clear();
            Available.EnableAll();
            long titleId = Owner.SelectedTitle?.Id ?? 0;

            foreach (TitleTagRow tagRow in TitleTagTable.EnumerateAll(titleId))
            {
                if (Available.GetItem(tagRow.TagId) is TagSelectorItem item)
                {
                    Assigned.Add(item.Disable().Clone());
                }
            }
            OnPropertyChanged(nameof(HasAssignedTags));
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// Adds a tag to the underlying data
        /// </summary>
        /// <param name="tagParm">The tag selector</param>
        /// <returns>true if added; otherwise, false</returns>
        protected virtual bool AddTag(TagSelectorParm tagParm)
        {
            long titleId = Owner.SelectedTitle?.Id ?? 0;

            if (TitleTagTable.AddIfNotExist(titleId, tagParm.TagSelectorItem.Id))
            {
                Owner.Filters.Tags.Invalidate(tagParm.TagSelectorItem.Id);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes a tag from the underlying data
        /// </summary>
        /// <param name="tagParm">The tag selector</param>
        /// <returns>true if removed; otherwise, false</returns>
        protected virtual bool RemoveTag(TagSelectorParm tagParm)
        {
            long titleId = Owner.SelectedTitle?.Id ?? 0;
            
            if (TitleTagTable.RemoveIfExist(titleId, tagParm.TagSelectorItem.Id))
            {
                Owner.Filters.Tags.Invalidate(tagParm.TagSelectorItem.Id);
                return true;
            }
            return false;
        }
        #endregion

        /************************************************************************/

        #region Private methods
        private void RunTagItemClickCommand(object parm)
        {
            if (parm is TagSelectorParm tagParm)
            {
                if (tagParm.SelectorControlId == AvailableSelectorId)
                {
                    if (AddTag(tagParm))
                    {
                        AddToAssignedList(tagParm.TagSelectorItem);
                    }
                }
                else
                {
                    if (RemoveTag(tagParm))
                    {
                        RemoveFromAssignedList(tagParm.TagSelectorItem);
                    }
                }
                OnPropertyChanged(nameof(HasAssignedTags));
            }
        }

        private void AddToAssignedList(TagSelectorItem item)
        {
            if (Assigned.GetItem(item.Id) == null)
            {
                Assigned.Add(item.Disable().Clone());
            }
        }

        private void RemoveFromAssignedList(TagSelectorItem item)
        {
            Assigned.Remove(item);
            Available.GetItem(item.Id)?.Enable();
        }

        private static TagSelectorItem GetTagSelectorItem(TagRow tagRow)
        {
            return new TagSelectorItem()
            {
                Id = tagRow.Id,
                Content = tagRow.Tag,
                ToolTip = tagRow.Description
            };
        }
        #endregion
    }
}