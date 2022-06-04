/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the view model logic for the <see cref="View.AboutWindow"/>.
    /// </summary>
    public class AboutWindowViewModel : WindowViewModel
    {

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutWindowViewModel"/> class.
        /// </summary>
        public AboutWindowViewModel()
        {
            DisplayName = $"About {AppInfo.Title}";
        }
        #endregion
    }
}