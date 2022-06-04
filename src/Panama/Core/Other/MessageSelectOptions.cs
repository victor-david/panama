/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/

namespace Restless.Panama.Core
{
    /// <summary>
    /// Represents options that are used to control the behavior of the MessageSelectWindowViewModel class
    /// </summary>
    public class MessageSelectOptions
    {
        /// <summary>
        /// Gets the message selection mode.
        /// </summary>
        public MessageSelectMode Mode
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the scope for the message selection
        /// </summary>
        public string Scope
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSelectOptions"/> class.
        /// </summary>
        /// <param name="mode">The message selection mode</param>
        /// <param name="scope">The scope. May be a file or a mapi scope.</param>
        public MessageSelectOptions(MessageSelectMode mode, string scope)
        {
            Mode = mode;
            Scope = scope;
        }
    }
}