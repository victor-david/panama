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

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Provides enumeration values that describe the types of submission documents that may be created.
    /// </summary>
    public enum SubmissionDocumentCreateType
    {
        /// <summary>
        /// No selection
        /// </summary>
        None,
        /// <summary>
        /// Create a document
        /// </summary>
        CreateDocX,
        /// <summary>
        /// Create a placeholder entry
        /// </summary>
        CreatePlaceholder,
    }

}