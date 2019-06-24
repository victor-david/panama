/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
namespace Restless.App.Panama.Core
{
    /// <summary>
    /// Provides the enumeration values that describe the state of a filter.
    /// </summary>
    public enum FilterState
    {
        /// <summary>
        /// No. The value compared to the filter state must be false
        /// </summary>
        No = 0,
        /// <summary>
        /// Yes. The value compared to the filter state must be true.
        /// </summary>
        Yes = 1,
        /// <summary>
        /// Either. The filter is not used.
        /// </summary>
        Either = 2
    }
}