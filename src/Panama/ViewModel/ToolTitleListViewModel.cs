/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Resources;
using Restless.Toolkit.Core.Utility;
using System;
using System.ComponentModel;
using System.IO;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Provides the logic that is used for the orphan finder tool.
    /// </summary>
    public class ToolTitleListViewModel : ApplicationViewModel // DataGridViewModelBase
    {
        #region Private
        private string text;
        #endregion

        /************************************************************************/

        #region Properties
        /// <summary>
        /// Gets the text of the title list document.
        /// </summary>
        public string Text
        {
            get { return text; }
            set
            {
                SetProperty(ref text, value);
            }
        }
        /// <summary>
        /// Gets the controller object for the title list operation.
        /// </summary>
        public ToolTitleListController Creator
        {
            get;
            private set;
        }
        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolTitleListViewModel"/> class.
        /// </summary>
        public ToolTitleListViewModel()
        {
            DisplayName = Strings.CommandToolTitleList;
            Creator = new ToolTitleListController(this);
            Creator.Scanner.Completed += ScannerCompleted;
            Commands.Add("Begin", (o) => Creator.Run());
            Commands.Add("OpenFile", (o) => OpenHelper.OpenFile(Creator.TitleListFileName));
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
            if (!e.Cancel)
            {
                Creator.Scanner.Completed -= ScannerCompleted;
            }
        }
        #endregion

        /************************************************************************/

        #region Private Methods
        private void ScannerCompleted(object sender, EventArgs e)
        {
            Text = File.ReadAllText(Creator.TitleListFileName);
        }
        #endregion
    }
}