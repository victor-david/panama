/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Database.Core;
using Restless.Panama.Database.Tables;
using Restless.Toolkit.Controls;
using Restless.Toolkit.Mvvm;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Base class for application view models. This class must be interited.
    /// </summary>
    public abstract class ApplicationViewModel : ViewModelBase, INavigator
    {
        #region
        private bool isOperationInProgress;
        #endregion

        /************************************************************************/

        #region Public properties
        /// <summary>
        /// Gets the singletom instance of the application information object.
        /// </summary>
        public Core.ApplicationInfo AppInfo => Core.ApplicationInfo.Instance;

        /// <summary>
        /// Gets the singleton instance of the configuration object.
        /// </summary>
        public Core.Config Config => Core.Config.Instance;

        /// <summary>
        /// Gets or (from a derived class) sets a boolean value that indicates in an async operation is in progress
        /// </summary>
        public bool IsOperationInProgress
        {
            get => isOperationInProgress;
            protected set => SetProperty(ref isOperationInProgress, value);
        }
        #endregion

        /************************************************************************/

        #region Tables
        /// <summary>
        /// Gets the link table
        /// </summary>
        protected LinkTable LinkTable => DatabaseController.Instance.GetTable<LinkTable>();

        /// <summary>
        /// Gets the orphan exclusion table
        /// </summary>
        protected OrphanExclusionTable OrphanExclusionTable => DatabaseController.Instance.GetTable<OrphanExclusionTable>();

        /// <summary>
        /// Gets the publisher table
        /// </summary>
        protected PublisherTable PublisherTable => DatabaseController.Instance.GetTable<PublisherTable>();

        /// <summary>
        /// Gets the response table
        /// </summary>
        protected ResponseTable ResponseTable => DatabaseController.Instance.GetTable<ResponseTable>();

        /// <summary>
        /// Gets the search table
        /// </summary>
        protected SearchTable SearchTable => DatabaseController.Instance.GetTable<SearchTable>();

        /// <summary>
        /// Gets the submission batch table
        /// </summary>
        protected SubmissionBatchTable SubmissionBatchTable => DatabaseController.Instance.GetTable<SubmissionBatchTable>();

        /// <summary>
        /// Gets the theme table
        /// </summary>
        protected ThemeTable ThemeTable => DatabaseController.Instance.GetTable<ThemeTable>();

        /// <summary>
        /// Gets the title table
        /// </summary>
        protected TitleTable TitleTable => DatabaseController.Instance.GetTable<TitleTable>();

        /// <summary>
        /// Gets the title version table
        /// </summary>
        protected TitleVersionTable TitleVersionTable => DatabaseController.Instance.GetTable<TitleVersionTable>();
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