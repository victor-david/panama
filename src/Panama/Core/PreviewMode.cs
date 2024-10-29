/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/

namespace Restless.Panama.Core
{
    /// <summary>
    /// Provides enumeration values that describe the possible preview modes.
    /// </summary>
    public enum PreviewMode
    {
        /// <summary>
        /// Preview mode is not set
        /// </summary>
        None,
        /// <summary>
        /// Preview is text.
        /// </summary>
        Text,
        /// <summary>
        /// Preview is an image
        /// </summary>
        Image,
        /// <summary>
        /// The type of preview is unknown or unsupported.
        /// </summary>
        Unsupported
    }
}