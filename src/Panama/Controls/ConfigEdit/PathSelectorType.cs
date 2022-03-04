/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
namespace Restless.Panama.Controls
{
    /// <summary>
    /// Provide an enumeration that decribes the type of of a path selector
    /// </summary>
    public enum PathSelectorType
    {
        /// <summary>
        /// The file selector type is a standard file system folder.
        /// </summary>
        FileSystemFolder,

        /// <summary>
        /// The file selector type is a file system file.
        /// </summary>
        FileSystemFile,

        /// <summary>
        /// The file selector type is a MAPI folder.
        /// </summary>
        Mapi
    }
}