/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
#if DOCX
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.OfficeAutomation;
using Restless.Tools.Utility;

namespace Restless.App.Panama.Tools
{
    /// <summary>
    /// Represents the event argumenets that are sent with the <see cref="DocumentConverter.ItemCompleted"/> event
    /// </summary>
    public class ConversionCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the conversion candidate for the event.
        /// </summary>
        public DocumentConversionCandidate Item
        {
            get;
            private set;
        }
        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ConversionCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="item">The conversion item.</param>
        public ConversionCompletedEventArgs(DocumentConversionCandidate item)
        {
            Validations.ValidateNull(item, "ConversionCompletedEventArgs.Item");
            Item = item;
        }
        #endregion
    }
}
#endif