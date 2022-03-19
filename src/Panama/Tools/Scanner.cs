/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Toolkit.Mvvm;
using System;
using System.Threading.Tasks;

namespace Restless.Panama.Tools
{
    /// <summary>
    /// Represents the base class for a tool that scans files. This class must be inherited.
    /// </summary>
    public abstract class Scanner
    {
        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="Scanner"/> class.
        /// </summary>
        protected Scanner()
        {
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// Asynchronously executes the scanner operation.
        /// </summary>
        /// <returns>Task</returns>
        public async Task<FileScanResult> ExecuteAsync()
        {
            return await Task.Run(() => ExecuteTask());
        }
        #endregion

        /************************************************************************/

        #region Protected methods
        /// <summary>
        /// When implemented in a derived class, executes the scan operation.
        /// This method is called from within <see cref="ExecuteAsync"/>
        /// </summary>
        protected abstract FileScanResult ExecuteTask();
        #endregion
    }
}