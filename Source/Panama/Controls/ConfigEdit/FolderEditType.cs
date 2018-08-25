﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restless.App.Panama.Controls
{
    /// <summary>
    /// Provide an enumeration that decribes the type of editable folder
    /// </summary>
    public enum FolderEditType
    {
        /// <summary>
        /// The folder edit type is a standard file system folder.
        /// </summary>
        FileSystem,
        /// <summary>
        /// The folder edit type is a MAPI folder.
        /// </summary>
        Mapi
    }
}
