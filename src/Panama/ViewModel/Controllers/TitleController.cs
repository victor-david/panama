/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.App.Panama.Database.Tables;

namespace Restless.App.Panama.ViewModel
{
    /// <summary>
    /// Base class for title supplemental controllers.
    /// </summary>
    public abstract class TitleController : ControllerBase<TitleViewModel, TitleTable>
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
        /// Initializes a new instance of the <see cref="TitleController"/> class.
        /// </summary>
        /// <param name="owner">The owner of this controller</param>
        protected TitleController(TitleViewModel owner)
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