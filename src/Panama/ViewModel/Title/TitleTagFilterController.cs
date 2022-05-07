/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Controls;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the ability to assign and remove tags from the title filter
    /// </summary>
    public class TitleTagFilterController : TitleTagController
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TitleTagFilterController"/> class
        /// </summary>
        /// <param name="owner">The owner</param>
        public TitleTagFilterController(TitleViewModel owner) : base(owner)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <inheritdoc/>
        public override void PopulateAssigned()
        {
            Assigned.Clear();
            Available.EnableAll();
            foreach (long tagId in Owner.Filters.Tags)
            {
                if (Available.GetItem(tagId) is TagSelectorItem item)
                {
                    Assigned.Add(item.Disable().Clone());
                }
            }

            OnPropertyChanged(nameof(HasAssignedTags));
        }

        /// <summary>
        /// Clears all entries in <see cref="Assigned"/> and
        /// enables all entries in <see cref="Available"/>
        /// </summary>
        /// <remarks>
        /// This method adjusts entries in the two lists;
        /// it does not change any underlying filter data
        /// </remarks>
        public void ClearAssigned()
        {
            Assigned.Clear();
            Available.EnableAll();
            OnPropertyChanged(nameof(HasAssignedTags));
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <inheritdoc/>
        protected override bool AddTag(TagSelectorParm tagParm)
        {
            return Owner.Filters.Tags.Add(tagParm.TagSelectorItem.Id);
        }

        /// <inheritdoc/>
        protected override bool RemoveTag(TagSelectorParm tagParm)
        {
            return Owner.Filters.Tags.Remove(tagParm.TagSelectorItem.Id);
        }
        #endregion
    }
}