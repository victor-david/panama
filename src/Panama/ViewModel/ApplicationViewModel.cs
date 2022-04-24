/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Base class for application view models. This class must be interited.
    /// </summary>
    public abstract class ApplicationViewModel : ViewModelBase, INavigator
    {
        #region Public properties
        /// <summary>
        /// Gets the singletom instance of the application information object.
        /// </summary>
        public Core.ApplicationInfo AppInfo => Core.ApplicationInfo.Instance;

        /// <summary>
        /// Gets the singleton instance of the configuration object.
        /// </summary>
        public Core.Config Config => Core.Config.Instance;
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationViewModel"/> class.
        /// </summary>
        protected ApplicationViewModel()
        {
        }
        #endregion
    }
}