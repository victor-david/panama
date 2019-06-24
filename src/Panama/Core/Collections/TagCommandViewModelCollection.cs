/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.ViewModel;
using System.Collections.ObjectModel;

namespace Restless.App.Panama.Core
{
    /// <summary>
    /// An observable collection of TagCommandViewModel objects
    /// </summary>
    public class TagCommandViewModelCollection : ObservableCollection<TagCommandViewModel>
    {

        /// <summary>
        /// Gets the TagCommandViewModel with the specified id
        /// </summary>
        /// <param name="tagId">The tag id</param>
        /// <returns>The TagCommandViewModel with the specified id, or null if not found.</returns>
        public TagCommandViewModel GetItem(long tagId)
        {
            foreach (TagCommandViewModel vm in this)
            {
                if (vm.TagId == tagId)
                {
                    return vm;
                }
            }
            return null;
        }
    }
}