/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides options that may be used when cleaning a string with <see cref="StringClean"/>.
    /// </summary>
    [Flags]
    public enum StringCleanOptions
    {
        /// <summary>
        /// No options have been specified
        /// </summary>
        None = 0,
        /// <summary>
        /// Interior white space will compacted, two spaces become one, etc.
        /// </summary>
        TrimInterior = 1,
        /// <summary>
        /// Html will be removed.
        /// </summary>
        RemoveHtml = 2,
        /// <summary>
        /// All options.
        /// </summary>
        All = TrimInterior + RemoveHtml,
    }
}