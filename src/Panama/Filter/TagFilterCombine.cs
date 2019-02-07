/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Restless.App.Panama.Filter
{
    /// <summary>
    /// Provides enumeration values that describe how multiple tag selections in a title filter are combined.
    /// </summary>
    public enum TagFilterCombine
    {
        /// <summary>
        /// Multiple tag selection are combined with OR.
        /// </summary>
        Or,
        /// <summary>
        /// Multiple tag selection are combined with AND.
        /// </summary>
        And,
        /// <summary>
        /// Multiple tag selection are combined with AND NOT.
        /// </summary>
        AndNot,

    }
}