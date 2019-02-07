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

namespace Restless.App.Panama.Configuration
{
    /// <summary>
    /// Represents the various options that may be applied to the creation of a submission document.
    /// </summary>
    public class SubmissionDocumentOptions
    {
        /// <summary>
        /// Gets or sets the company name to be inserted into a new submission document.
        /// </summary>
        public string Company
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the text to be inserted into a new submission document.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets opr sets the header text to be inserted into a new submission document.
        /// </summary>
        public string Header
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates if page numbers should be inserted into the header.
        /// </summary>
        public bool HeaderPageNumbers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the footer text to be inserted into a new submission document.
        /// </summary>
        public string Footer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a boolean value that indicates if page numbers should be inserted into the footer.
        /// </summary>
        public bool FooterPageNumbers
        {
            get;
            set;
        }


        /************************************************************************/

        #region Constructor
        #pragma warning disable 1591
        public SubmissionDocumentOptions()
        {
        }
        #pragma warning restore 1591
        #endregion




    }
}