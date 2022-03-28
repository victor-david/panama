/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/

using Restless.Panama.Database.Tables;

namespace Restless.Panama.ViewModel
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