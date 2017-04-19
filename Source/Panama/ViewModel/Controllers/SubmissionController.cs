﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Restless.Tools.Database.SQLite;
using System.ComponentModel;
using Restless.App.Panama.Database.Tables;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Base class for submission batch supplemental controllers.
    /// Currently, this class provides no additional functionality
    /// </summary>
    public abstract class SubmissionController : TitleLatestVersionController<SubmissionViewModel, SubmissionBatchTable>
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Public properties
        #endregion

        /************************************************************************/

        #region Protected properties
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SubmissionController"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        protected SubmissionController(SubmissionViewModel owner)
            : base(owner)
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        #endregion

        /************************************************************************/

        #region Protected methods
        #endregion

        /************************************************************************/

        #region Private methods
        #endregion

    }
}
