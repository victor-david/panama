/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/

namespace Restless.Panama.Core
{
    /// <summary>
    /// Helper class for tag assignment
    /// </summary>
    public class TagCache
    {
        #region Public properties
        /// <summary>
        /// Gets the tag id
        /// </summary>
        public long Id
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tag name
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the tag description
        /// </summary>
        public string Description
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TagCache"/> class.
        /// </summary>
        /// <param name="id">The tag id.</param>
        /// <param name="name">The tag name.</param>
        /// <param name="description">The tag description.</param>
        public TagCache(long id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
        #endregion
    }
}