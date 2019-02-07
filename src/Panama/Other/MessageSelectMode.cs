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
using Restless.Tools.Utility;

namespace Restless.App.Panama
{
    /// <summary>
    /// Represents the message selection mode
    /// </summary>
    public enum MessageSelectMode
    {
        /// <summary>
        /// Select message or messages.
        /// </summary>
        Message,
        /// <summary>
        /// Select folder
        /// </summary>
        Folder
    }
}