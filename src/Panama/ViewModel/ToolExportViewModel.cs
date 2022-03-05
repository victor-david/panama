/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Resources;
using System.ComponentModel;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the title export tool.
    /// </summary>
    public class ToolExportViewModel : ApplicationViewModel
    {
        #region Private
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the controller object for the export operation
        /// </summary>
        public ToolExportTitleController Export
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolExportViewModel"/> class.
        /// </summary>
        public ToolExportViewModel()
        {
            DisplayName = Strings.CommandToolExport;
            MaxCreatable = 1;
            Commands.Add("Begin", (o) =>
            {
                Export.Run();
            });
            Export = new ToolExportTitleController(this);
        }
        #endregion

        /************************************************************************/

        #region Protected Methods
        /// <summary>
        /// Raises the Closing event.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            SetCancelIfTasksInProgress(e);
            base.OnClosing(e);
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        #endregion
    }
}