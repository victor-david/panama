/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides enumeration values that describe how multiple tag selections in a title filter are combined.
    /// </summary>
    public enum TagFilterCombine
    {
        /// <summary>
        /// Any of the tags. Selections are combined with OR.
        /// </summary>
        Any,
        /// <summary>
        /// All of the tags. Selections are combined with AND.
        /// </summary>
        All,
    }
}